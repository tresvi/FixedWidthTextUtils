using FixedWidthTextUtils.Attributes;
using FixedWidthTextUtils.Exceptions;
using System;
using System.Reflection;
using System.Text;

namespace FixedWidthTextUtils
{
    public static class LineParser
    {
        //TODO: Agregar cache estatico con el nombre de las clases que ya se validaron, para no validarlas 2 veces en la misma instancia.
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
            T targetObject = new T();

            int ordinalModePositionCounter = 0;

            PropertyInfo[] properties = targetObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                foreach (FieldAttribute fieldAttrib in property.GetCustomAttributes(typeof(FieldAttribute), true))
                {
                    int startPos;
                    int endPos;
                    if (fieldAttrib.IsOrdinalMode)
                    {
                        startPos = ordinalModePositionCounter;
                        endPos = ordinalModePositionCounter + fieldAttrib.Length - 1;
                        ordinalModePositionCounter += fieldAttrib.Length;
                    }
                    else
                    {
                        startPos = fieldAttrib.StartPosition;
                        endPos = fieldAttrib.EndPosition;
                    }

                    if (startPos > input.Length - 1)
                        throw new ParseFieldException($"La definicion de la propiedad {property.Name} posee un StartPosition " +
                            $"({startPos}) que excede el largo de la linea de entrada de {input.Length} caracteres)");

                    if (endPos > input.Length - 1)
                        throw new ParseFieldException($"La definicion de la propiedad {property.Name} posee un EndPosition " +
                            $"({endPos}) que excede el largo de la linea de entrada ({input.Length} caracteres)");

                    if (!fieldAttrib.ValidateFieldDefinition(property, targetObject, out string errorMessage))
                        throw new ArgumentException($"Error de definicion de campo en la property {property.Name} de la clase {targetObject.GetType().Name}. Detalles: {errorMessage}");

                    string rawFieldContent = input.Substring(startPos, Math.Min((endPos - startPos + 1), input.Length - startPos));

                    object parseResult = fieldAttrib.Parse(property, targetObject, rawFieldContent);
                    property.SetValue(targetObject, parseResult);
                }
            }
            return targetObject;
        }


        public static string ToTextLine(object value)
        {
            string initializedLine = Utils.GetInitializedLine(value);
            int maxLineLength = initializedLine.Length;
            StringBuilder outputLine = new StringBuilder(initializedLine);
            int ordinalModePositionCounter = 0;

            PropertyInfo[] properties = value.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                foreach (FieldAttribute fieldAttrib in property.GetCustomAttributes(typeof(FieldAttribute), true))
                {
                    int startPos;
                    if (fieldAttrib.IsOrdinalMode)
                    {
                        startPos = ordinalModePositionCounter;
                        ordinalModePositionCounter += fieldAttrib.Length;
                        int exclusiveEnd = ordinalModePositionCounter;
                        if (exclusiveEnd > maxLineLength)
                            throw new SerializeFieldException($"El largo de la linea declarado en el atributo Stringeable de la clase (de {maxLineLength} caracteres) es insuficiente " +
                                $"para contener la serializacion de la propiedad {property.Name} de la clase {value.GetType().Name}. Extienda el tamano de linea o revise la definicion de la propiedad.");
                    }
                    else
                    {
                        ordinalModePositionCounter = fieldAttrib.EndPosition;

                        if (fieldAttrib.EndPosition >= maxLineLength)
                            throw new SerializeFieldException($"El largo de la linea declarado en el atributo Stringeable de la clase (de {maxLineLength} caracteres) es insuficiente " +
                                $"para contener la serializacion de la propiedad {property.Name}. Extienda el tamano de linea o revise la definicion de la propiedad.");

                        startPos = fieldAttrib.StartPosition;
                    }

                    if (!fieldAttrib.ValidateFieldDefinition(property, value, out string errorMessage))
                        throw new ArgumentException($"Error de definicion de campo en la property {property.Name} de la clase {value.GetType().Name}. Detalles: {errorMessage}");

                    string serializedField = fieldAttrib.ToText(property, value);
                    outputLine = Utils.ReplaceAt(outputLine, startPos, serializedField);
                }
            }

            return outputLine.ToString();
        }


    }
}
