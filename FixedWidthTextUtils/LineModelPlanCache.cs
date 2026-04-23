using FixedWidthTextUtils.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace FixedWidthTextUtils
{
    internal sealed class FieldPlanEntry
    {
        public FieldPlanEntry(
            PropertyInfo property,
            FieldAttribute fieldAttrib,
            int parseStart,
            int parseEndInclusive,
            int serializeStart,
            Action<object, object> setter,
            Func<object, object> getter)
        {
            Property = property;
            FieldAttrib = fieldAttrib;
            ParseStart = parseStart;
            ParseEndInclusive = parseEndInclusive;
            SerializeStart = serializeStart;
            Setter = setter;
            Getter = getter;
        }

        public PropertyInfo Property { get; }
        public FieldAttribute FieldAttrib { get; }
        public int ParseStart { get; }
        public int ParseEndInclusive { get; }
        public int SerializeStart { get; }

        /// <summary>Setter compilado: equivalente a property.SetValue(target, value) sin reflexion en el hot path.</summary>
        public Action<object, object> Setter { get; }
        /// <summary>Getter compilado: equivalente a property.GetValue(origin) sin reflexion en el hot path.</summary>
        public Func<object, object> Getter { get; }
    }

    internal sealed class LineModelPlan
    {
        public LineModelPlan(
            int lineLength,
            FieldPlanEntry[] fields,
            bool isMixedOrdinalAndPositional,
            string modelErrorMessage,
            Func<object> activator)
        {
            LineLength = lineLength;
            Fields = fields;
            IsMixedOrdinalAndPositional = isMixedOrdinalAndPositional;
            ModelErrorMessage = modelErrorMessage;
            Activator = activator;
        }

        public int LineLength { get; }
        // Array para iteracion sin enumerador (mejor inlining/JIT que IReadOnlyList<T>).
        public FieldPlanEntry[] Fields { get; }
        public bool IsMixedOrdinalAndPositional { get; }
        public string ModelErrorMessage { get; }
        /// <summary>Constructor compilado del modelo, equivale a <c>new T()</c> sin <see cref="Activator.CreateInstance"/>.</summary>
        public Func<object> Activator { get; }
    }

    /// <summary>
    /// Plan de mapeo propiedad/campo por tipo: se construye una sola vez y se reutiliza (sin repetir reflexion por linea).
    /// Ademas, en la construccion del plan compilamos delegates Setter/Getter por propiedad para evitar el costo de
    /// <see cref="PropertyInfo.SetValue(object, object)"/> y <see cref="PropertyInfo.GetValue(object)"/> en el hot path.
    /// </summary>
    internal static class LineModelPlanCache
    {
        private static readonly ConcurrentDictionary<Type, LineModelPlan> Plans = new ConcurrentDictionary<Type, LineModelPlan>();

        public static LineModelPlan GetPlan(Type type)
        {
            return Plans.GetOrAdd(type, BuildPlan);
        }

        private static LineModelPlan BuildPlan(Type type)
        {
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            var tuples = new List<(PropertyInfo Property, FieldAttribute FieldAttrib)>();
            bool hasOrdinal = false;
            bool hasPositional = false;

            foreach (PropertyInfo property in properties)
            {
                foreach (FieldAttribute fieldAttrib in property.GetCustomAttributes(typeof(FieldAttribute), true))
                {
                    tuples.Add((property, fieldAttrib));
                    if (fieldAttrib.IsOrdinalMode)
                        hasOrdinal = true;
                    else
                        hasPositional = true;
                }
            }

            if (hasOrdinal && hasPositional)
            {
                string mixedError = $"La clase {type.Name} mezcla campos en modo ordinal y posicional. " +
                    "Use solo un modo por clase, o convierta todos los campos al mismo modo.";
                return new LineModelPlan(0, Array.Empty<FieldPlanEntry>(), true, mixedError, BuildActivator(type));
            }

            // Ningun ValidateFieldDefinition del proyecto usa originObject; null evita instanciar T.
            object probe = null;

            int ordinalParseCursor = 0;
            int ordinalSerializeCursor = 0;
            var entries = new List<FieldPlanEntry>(tuples.Count);

            foreach ((PropertyInfo property, FieldAttribute fieldAttrib) in tuples)
            {
                int parseStart;
                int parseEndInclusive;
                int serializeStart;

                if (fieldAttrib.IsOrdinalMode)
                {
                    parseStart = ordinalParseCursor;
                    parseEndInclusive = ordinalParseCursor + fieldAttrib.Length - 1;
                    ordinalParseCursor += fieldAttrib.Length;

                    serializeStart = ordinalSerializeCursor;
                    ordinalSerializeCursor += fieldAttrib.Length;
                }
                else
                {
                    parseStart = fieldAttrib.StartPosition;
                    parseEndInclusive = fieldAttrib.EndPosition;
                    serializeStart = fieldAttrib.StartPosition;
                }

                if (!fieldAttrib.ValidateFieldDefinition(property, probe, out string errorMessage))
                {
                    throw new ArgumentException(
                        $"Error de definicion de campo en la property {property.Name} de la clase {type.Name}. Detalles: {errorMessage}");
                }

                // Hook para que el atributo pre-calcule datos derivados de PropertyInfo
                // (por ejemplo el tipo entero subyacente). Se ejecuta una vez por entry.
                fieldAttrib.Bind(property);

                Action<object, object> setter = BuildSetter(property);
                Func<object, object> getter = BuildGetter(property);

                entries.Add(new FieldPlanEntry(property, fieldAttrib, parseStart, parseEndInclusive, serializeStart, setter, getter));
            }

            int lineLength;
            if (hasOrdinal)
            {
                lineLength = ordinalParseCursor;
            }
            else
            {
                int maxEndPosition = 0;
                foreach ((_, FieldAttribute fieldAttrib) in tuples)
                {
                    if (fieldAttrib.EndPosition > maxEndPosition)
                        maxEndPosition = fieldAttrib.EndPosition;
                }
                lineLength = maxEndPosition + 1;
            }

            return new LineModelPlan(lineLength, entries.ToArray(), false, null, BuildActivator(type));
        }


        /// <summary>
        /// Construye un setter equivalente a <c>(object t, object v) =&gt; ((TDecl)t).Property = (TProp)v;</c>.
        /// Si la property pertenece a un value type o no tiene setter, cae a <see cref="PropertyInfo.SetValue(object, object)"/>.
        /// </summary>
        private static Action<object, object> BuildSetter(PropertyInfo property)
        {
            MethodInfo setMethod = property.GetSetMethod(true);
            if (setMethod == null || property.DeclaringType == null || property.DeclaringType.IsValueType)
            {
                return (target, value) => property.SetValue(target, value);
            }

            try
            {
                ParameterExpression targetParam = Expression.Parameter(typeof(object), "t");
                ParameterExpression valueParam = Expression.Parameter(typeof(object), "v");

                UnaryExpression typedTarget = Expression.Convert(targetParam, property.DeclaringType);
                UnaryExpression typedValue = Expression.Convert(valueParam, property.PropertyType);

                MethodCallExpression callSet = Expression.Call(typedTarget, setMethod, typedValue);
                return Expression.Lambda<Action<object, object>>(callSet, targetParam, valueParam).Compile();
            }
            catch
            {
                // Cualquier escenario raro de reflexion: fallback seguro.
                return (target, value) => property.SetValue(target, value);
            }
        }


        /// <summary>
        /// Construye un getter equivalente a <c>(object o) =&gt; (object)((TDecl)o).Property;</c>.
        /// </summary>
        private static Func<object, object> BuildGetter(PropertyInfo property)
        {
            MethodInfo getMethod = property.GetGetMethod(true);
            if (getMethod == null || property.DeclaringType == null || property.DeclaringType.IsValueType)
            {
                return target => property.GetValue(target);
            }

            try
            {
                ParameterExpression targetParam = Expression.Parameter(typeof(object), "o");

                UnaryExpression typedTarget = Expression.Convert(targetParam, property.DeclaringType);
                MethodCallExpression callGet = Expression.Call(typedTarget, getMethod);
                UnaryExpression boxed = Expression.Convert(callGet, typeof(object));

                return Expression.Lambda<Func<object, object>>(boxed, targetParam).Compile();
            }
            catch
            {
                return target => property.GetValue(target);
            }
        }


        /// <summary>
        /// Equivalente a <c>() =&gt; (object) new T()</c> compilado, evita el overhead de
        /// <see cref="System.Activator.CreateInstance(Type)"/> en cada Parse.
        /// </summary>
        private static Func<object> BuildActivator(Type type)
        {
            ConstructorInfo ctor = type.GetConstructor(Type.EmptyTypes);
            if (ctor == null)
            {
                // Sin ctor publico sin parametros: no podemos compilar; el caller usara new T() (que el JIT resuelve).
                return null;
            }

            try
            {
                NewExpression newExpr = Expression.New(ctor);
                UnaryExpression boxed = Expression.Convert(newExpr, typeof(object));
                return Expression.Lambda<Func<object>>(boxed).Compile();
            }
            catch
            {
                return null;
            }
        }
    }
}
