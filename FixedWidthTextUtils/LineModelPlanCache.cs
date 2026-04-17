using FixedWidthTextUtils.Attributes;
using FixedWidthTextUtils.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace FixedWidthTextUtils
{
    internal sealed class FieldPlanEntry
    {
        public FieldPlanEntry(PropertyInfo property, FieldAttribute fieldAttrib, int parseStart, int parseEndInclusive, int serializeStart)
        {
            Property = property;
            FieldAttrib = fieldAttrib;
            ParseStart = parseStart;
            ParseEndInclusive = parseEndInclusive;
            SerializeStart = serializeStart;
        }

        public PropertyInfo Property { get; }
        public FieldAttribute FieldAttrib { get; }
        public int ParseStart { get; }
        public int ParseEndInclusive { get; }
        public int SerializeStart { get; }
    }

    internal sealed class LineModelPlan
    {
        public LineModelPlan(int lineLength, IReadOnlyList<FieldPlanEntry> fields)
        {
            LineLength = lineLength;
            Fields = fields;
        }

        public int LineLength { get; }
        public IReadOnlyList<FieldPlanEntry> Fields { get; }
    }

    /// <summary>
    /// Plan de mapeo propiedad/campo por tipo: se construye una sola vez y se reutiliza (sin repetir reflexión por línea).
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
                throw new SerializeFieldException(
                    $"La clase {type.Name} mezcla campos en modo ordinal y posicional. " +
                    "Use solo un modo por clase, o convierta todos los campos al mismo modo.");
            }

            // Ningún ValidateFieldDefinition del proyecto usa originObject; null evita instanciar T (p. ej. ctor interno en otro ensamblado).
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

                entries.Add(new FieldPlanEntry(property, fieldAttrib, parseStart, parseEndInclusive, serializeStart));
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

            return new LineModelPlan(lineLength, entries);
        }
    }
}
