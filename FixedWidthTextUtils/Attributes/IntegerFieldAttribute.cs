﻿using FixedWidthTextUtils.Exceptions;
using System;
using System.Reflection;

namespace FixedWidthTextUtils.Attributes
{

    /// <summary>
    /// Atributo de campo para enteros: int, uint, long y ulong
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class IntegerFieldAttribute : FieldAttribute
    {
        internal bool FillLeftWithZero { get; set; }

        public IntegerFieldAttribute(int startPosition, int endPosition, bool fillLeftWithZero = true) 
            : base(startPosition, endPosition)
        {
            FillLeftWithZero = fillLeftWithZero;
        }

        public IntegerFieldAttribute(int fieldLength, bool fillLeftWithZero = true) 
            : base(fieldLength)
        {
            FillLeftWithZero = fillLeftWithZero;
        }


        public override bool ValidateFieldDefinition(PropertyInfo property, object originObject, out string errorMesage)
        {
            errorMesage = "";
            return true;
        }


        public override object Parse(PropertyInfo property, object targetObject, string rawFieldContent)
        {
            //Reviso si la asignacion se hace a alguna property de algun tipo entero
            string parseErrorMessage = $"El valor \"{rawFieldContent}\" no puede ser reconocido como un entero válido del tipo {property.PropertyType.Name} " +
                $"para la property {targetObject.GetType().Name}.{property.Name}. Verifique que el dato sea numérico y este dentro del rango del tipo correspondiente";

            if (property.PropertyType == typeof(byte) || property.PropertyType == typeof(byte?))
            {
                if (!byte.TryParse(rawFieldContent, out byte parsedValue))
                    throw new ParseFieldException(parseErrorMessage);

                return parsedValue;
            }
            else if (property.PropertyType == typeof(sbyte) || property.PropertyType == typeof(sbyte?))
            {
                if (!sbyte.TryParse(rawFieldContent, out sbyte parsedValue))
                    throw new ParseFieldException(parseErrorMessage);

                return parsedValue;
            }
            else if (property.PropertyType == typeof(short) || property.PropertyType == typeof(short?))
            {
                if (!short.TryParse(rawFieldContent, out short parsedValue))
                    throw new ParseFieldException(parseErrorMessage);

                return parsedValue;
            }
            else if (property.PropertyType == typeof(ushort) || property.PropertyType == typeof(ushort?))
            {
                if (!ushort.TryParse(rawFieldContent, out ushort parsedValue))
                    throw new ParseFieldException(parseErrorMessage);

                return parsedValue;
            }
            else if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
            {
                if (!int.TryParse(rawFieldContent, out int parsedValue))
                    throw new ParseFieldException(parseErrorMessage);

                return parsedValue;
            }
            else if (property.PropertyType == typeof(uint) || property.PropertyType == typeof(uint?))
            {
                if (!uint.TryParse(rawFieldContent, out uint parsedValue))
                    throw new ParseFieldException(parseErrorMessage);

                return parsedValue;
            }
            else if (property.PropertyType == typeof(long) || property.PropertyType == typeof(long?))
            {
                if (!long.TryParse(rawFieldContent, out long parsedValue))
                    throw new ParseFieldException(parseErrorMessage);

                return parsedValue;
            }
            else if (property.PropertyType == typeof(ulong) || property.PropertyType == typeof(ulong?))
            {
                if (!ulong.TryParse(rawFieldContent, out ulong parsedValue))
                    throw new ParseFieldException(parseErrorMessage);

                return parsedValue;
            }
            else
            {
                throw new ParseFieldException($"La property {property.Name} es de tipo {property.PropertyType.Name} el cual no es aceptado como tipo numérico entero");
            }
        }



        public override string ToText(PropertyInfo property, object originObject)
        {
            //IntegerFieldAttribute integerAttribute = (IntegerFieldAttribute) fieldAttribute;
            string outputText = property.GetValue(originObject).ToString().Trim();

            if (this.FillLeftWithZero)
            {
                if (outputText.StartsWith("-"))
                {
                    outputText = outputText.TrimStart('-');
                    outputText = "-" + outputText.PadLeft(this.Length - 1, '0');
                }
                else
                {
                    outputText = outputText.PadLeft(this.Length, '0');
                }
            }
            else
            {
                outputText = outputText.PadLeft(this.Length, ' ');
            }

            return outputText;
        }
    }
}
