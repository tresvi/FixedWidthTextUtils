using FixedWidthTextUtils.Attributes;
using FixedWidthTextUtils.Exceptions;
using System;
using System.Reflection;
using System.Text;

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
            if (String.IsNullOrEmpty(input)) throw new ParseFieldException("La linea a parsear es EMPTY");

            LineModelPlan plan = LineModelPlanCache.GetPlan(typeof(T));
            T targetObject = new T();
            int inputLength = input.Length;

            foreach (FieldPlanEntry entry in plan.Fields)
            {
                PropertyInfo property = entry.Property;
                FieldAttribute fieldAttrib = entry.FieldAttrib;
                int startPos = entry.ParseStart;
                int endPos = entry.ParseEndInclusive;

                if (startPos > inputLength - 1)
                    throw new ParseFieldException($"La definicion de la propiedad {property.Name} posee un StartPosition " +
                        $"({startPos}) que excede el largo de la linea de entrada de {inputLength} caracteres)");

                if (endPos > inputLength - 1)
                    throw new ParseFieldException($"La definicion de la propiedad {property.Name} posee un EndPosition " +
                        $"({endPos}) que excede el largo de la linea de entrada ({inputLength} caracteres)");

                string rawFieldContent = input.Substring(startPos, Math.Min((endPos - startPos + 1), inputLength - startPos));

                object parseResult = fieldAttrib.Parse(property, targetObject, rawFieldContent);
                property.SetValue(targetObject, parseResult);
            }

            return targetObject;
        }


        public static string ToTextLine(object value)
        {
            Type type = value.GetType();
            LineModelPlan plan = LineModelPlanCache.GetPlan(type);
            int maxLineLength = plan.LineLength;
            string initializedLine = new string(' ', maxLineLength);
            StringBuilder outputLine = new StringBuilder(initializedLine);

            foreach (FieldPlanEntry entry in plan.Fields)
            {
                PropertyInfo property = entry.Property;
                FieldAttribute fieldAttrib = entry.FieldAttrib;
                int startPos = entry.SerializeStart;

                if (fieldAttrib.IsOrdinalMode)
                {
                    int exclusiveEnd = startPos + fieldAttrib.Length;
                    if (exclusiveEnd > maxLineLength)
                        throw new SerializeFieldException($"El largo de la linea declarado en el atributo Stringeable de la clase (de {maxLineLength} caracteres) es insuficiente " +
                            $"para contener la serializacion de la propiedad {property.Name} de la clase {type.Name}. Extienda el tamano de linea o revise la definicion de la propiedad.");
                }
                else
                {
                    if (fieldAttrib.EndPosition >= maxLineLength)
                        throw new SerializeFieldException($"El largo de la linea declarado en el atributo Stringeable de la clase (de {maxLineLength} caracteres) es insuficiente " +
                            $"para contener la serializacion de la propiedad {property.Name}. Extienda el tamano de linea o revise la definicion de la propiedad.");
                }

                string serializedField = fieldAttrib.ToText(property, value);
                outputLine = Utils.ReplaceAt(outputLine, startPos, serializedField);
            }

            return outputLine.ToString();
        }


    }
}
