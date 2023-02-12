using FixedWidthTextUtils.Attributes;
using FixedWidthTextUtils.Exceptions;
using System;
using System.Reflection;
using System.Text;

namespace FixedWidthTextUtils
{
    public static class LineParser
    {
        //TODO: Agregar soporte clases con properties con los tipos conocidos, pero nullables. Hoy en dia solo soporta nullable de string. Si se toma un nulo, se puede serializar con el fillerchar correspondiente
        //TODO: Agregar Cache estatico para cada tipo de objeto, memorizando el LineLength y el FillerChar por cada tipo de objeto usado
        //TODO: Agregar cache estatico de Attributtes por Propiedad por Objeto, para que no tenga que recorrer todos los attributes de objetos que ya conoce.
        //TODO: Agregar algun control o indicador de que hay campos cuya definicion se solapa.
        //TODO: Para los stringFieldAttibute se valida la start position y la endPosition, en los demas no.
        //TODO: Usar {nameof(fieldLength)} en donde se mencionen nombres de properties

        public static bool TryParse<T>(string input, out T result) where T : new()
        {
            try
            {
                result = Parse<T>(input);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
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
                    if (fieldAttrib.IsOrdinalMode)
                    {
                        fieldAttrib.StartPosition = ordinalModePositionCounter;
                        ordinalModePositionCounter += fieldAttrib.Length;
                        fieldAttrib.EndPosition = ordinalModePositionCounter;

                        if (fieldAttrib.EndPosition > maxLineLength)
                            throw new SerializeFieldException($"El largo de la linea declarado en el atributo Stringeable de la clase (de {maxLineLength} caracteres) es insuficiente " +
                                $"para contener la serializacion de la propiedad {property.Name}. Extienda el tamano de linea o revise la definicion de la propiedad.");
                    }
                    else
                    {
                        ordinalModePositionCounter = fieldAttrib.EndPosition;

                        if (fieldAttrib.EndPosition >= maxLineLength)
                            throw new SerializeFieldException($"El largo de la linea declarado en el atributo Stringeable de la clase (de {maxLineLength} caracteres) es insuficiente " +
                                $"para contener la serializacion de la propiedad {property.Name}. Extienda el tamano de linea o revise la definicion de la propiedad.");
                    }

                    string serializedField = fieldAttrib.ToPlainText(property, value);
                    outputLine = Utils.ReplaceAt(outputLine, fieldAttrib.StartPosition, serializedField);
                }
            }

            return outputLine.ToString();
        }


        public static T Parse<T>(string input) where T : new()
        {
            if (String.IsNullOrEmpty(input)) return new T();
            T targetObject = new T();
            
            int ordinalModePositionCounter = 0;

            PropertyInfo[] properties = targetObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                foreach (FieldAttribute fieldAttrib in property.GetCustomAttributes(typeof(FieldAttribute), true))
                {
                    if (fieldAttrib.IsOrdinalMode)
                    {
                        fieldAttrib.StartPosition = ordinalModePositionCounter;
                        fieldAttrib.EndPosition = ordinalModePositionCounter + fieldAttrib.Length - 1;
                        ordinalModePositionCounter += fieldAttrib.Length;
                    }

                    if (fieldAttrib.StartPosition > input.Length - 1)
                        throw new ParseFieldException($"La definicion de la propiedad {property.Name} posee un StartPosition " +
                            $"({fieldAttrib.StartPosition}) que excede el largo de la linea de entrada de {input.Length} caracteres)");

                    if (fieldAttrib.EndPosition > input.Length - 1)
                        throw new ParseFieldException($"La definicion de la propiedad {property.Name} posee un EndPosition " +
                            $"({fieldAttrib.EndPosition}) que excede el largo de la linea de entrada ({input.Length} caracteres)");

                    string rawFieldContent = input.Substring(fieldAttrib.StartPosition, Math.Min((fieldAttrib.EndPosition - fieldAttrib.StartPosition + 1), input.Length - fieldAttrib.StartPosition));

                    object result = fieldAttrib.Parse(property, targetObject, rawFieldContent);
                    property.SetValue(targetObject, result);
                }
            }
            return targetObject;
        }


    }
}
