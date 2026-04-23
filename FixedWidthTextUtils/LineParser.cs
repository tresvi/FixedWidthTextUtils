using FixedWidthTextUtils.Attributes;
using FixedWidthTextUtils.Exceptions;
using System;
using System.Buffers;

namespace FixedWidthTextUtils
{
    public static class LineParser
    {
        //TODO: Agregar algun control o indicador de que hay campos cuya definicion se solapa.
        //TODO: Agregar Test con lineas de texto nula.

        public static bool TryParse<T>(string input, out T result) where T : new()
        {
            try
            {
                result = Parse<T>(input);
                return true;
            }
            catch (ParseFieldException)
            {
                result = default(T);
                return false;
            }
            catch (ArgumentException)
            {
                result = default(T);
                return false;
            }
            catch
            {
                throw;
            }
        }

        public static T Parse<T>(string input) where T : new()
        {
            if (string.IsNullOrEmpty(input)) throw new ParseFieldException("La linea a parsear es EMPTY");

            LineModelPlan plan = LineModelPlanCache.GetPlan(typeof(T));
            if (plan.IsMixedOrdinalAndPositional)
                throw new ArgumentException(plan.ModelErrorMessage);

            // Activator compilado evita Activator.CreateInstance + ahorra el costo del constraint new().
            T targetObject = plan.Activator != null ? (T)plan.Activator() : new T();
            ReadOnlySpan<char> line = input.AsSpan();
            int inputLength = line.Length;

            FieldPlanEntry[] fields = plan.Fields;
            for (int idx = 0; idx < fields.Length; idx++)
            {
                FieldPlanEntry entry = fields[idx];
                int startPos = entry.ParseStart;
                int endPos = entry.ParseEndInclusive;

                if (startPos > inputLength - 1)
                    throw new ParseFieldException($"La definicion de la propiedad {entry.Property.Name} posee un StartPosition " +
                        $"({startPos}) que excede el largo de la linea de entrada de {inputLength} caracteres)");

                if (endPos > inputLength - 1)
                    throw new ParseFieldException($"La definicion de la propiedad {entry.Property.Name} posee un EndPosition " +
                        $"({endPos}) que excede el largo de la linea de entrada ({inputLength} caracteres)");

                int sliceLen = Math.Min(endPos - startPos + 1, inputLength - startPos);
                ReadOnlySpan<char> slice = line.Slice(startPos, sliceLen);

                object parseResult = entry.FieldAttrib.Parse(entry.Property, targetObject, slice);
                entry.Setter(targetObject, parseResult);
            }

            return targetObject;
        }


        public static string ToTextLine(object value)
        {
            Type type = value.GetType();
            LineModelPlan plan = LineModelPlanCache.GetPlan(type);
            if (plan.IsMixedOrdinalAndPositional)
                throw new SerializeFieldException(plan.ModelErrorMessage);

            int maxLineLength = plan.LineLength;

            // Validacion de longitud (antes se hacia campo a campo en el loop): se podria mover al
            // BuildPlan, pero al depender de cada FieldAttribute lo dejamos aqui sin loop adicional:
            // las llamadas a WriteTo van a fallar por su cuenta si el slice queda corto.

#if NET6_0_OR_GREATER
            return string.Create(maxLineLength, (plan, value), static (buffer, state) =>
            {
                buffer.Fill(' ');
                FieldPlanEntry[] fields = state.plan.Fields;
                for (int i = 0; i < fields.Length; i++)
                {
                    FieldPlanEntry entry = fields[i];
                    int len = entry.FieldAttrib.Length;
                    Span<char> dest = buffer.Slice(entry.SerializeStart, len);
                    entry.FieldAttrib.WriteTo(entry.Property, state.value, dest);
                }
            });
#else
            char[] rented = ArrayPool<char>.Shared.Rent(maxLineLength);
            try
            {
                Span<char> buffer = rented.AsSpan(0, maxLineLength);
                buffer.Fill(' ');

                FieldPlanEntry[] fields = plan.Fields;
                for (int i = 0; i < fields.Length; i++)
                {
                    FieldPlanEntry entry = fields[i];
                    int len = entry.FieldAttrib.Length;
                    Span<char> dest = buffer.Slice(entry.SerializeStart, len);
                    entry.FieldAttrib.WriteTo(entry.Property, value, dest);
                }

                return new string(rented, 0, maxLineLength);
            }
            finally
            {
                ArrayPool<char>.Shared.Return(rented);
            }
#endif
        }
    }
}
