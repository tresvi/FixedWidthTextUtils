using FixedWidthTextUtils.Exceptions;
using System;
using System.Reflection;

namespace FixedWidthTextUtils.Attributes
{

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class FloatingFieldAttribute : FieldAttribute
    {
        internal int DecimalPositions { get; set; }
        internal bool FillLeftWithZero { get; set; }

        public FloatingFieldAttribute(int startPosition, int endPosition, int decimalPositions, bool fillLeftWithZeros) 
            : base(startPosition, endPosition)
        {
            if (decimalPositions < 0)
                throw new ArgumentOutOfRangeException($"El valor de {nameof(decimalPositions)} debe ser un valor mayor o igual a 0" );

            DecimalPositions = decimalPositions;
            FillLeftWithZero = fillLeftWithZeros;
        }

        public FloatingFieldAttribute(int fieldLength, int decimalPositions, bool fillLeftWithZeros) 
            : base(fieldLength)
        {
            if (decimalPositions < 0)
                throw new ArgumentOutOfRangeException($"El valor de {nameof(decimalPositions)} debe ser un valor mayor o igual a 0");

            DecimalPositions = decimalPositions;
            FillLeftWithZero = fillLeftWithZeros;
        }


        public override bool ValidateFieldDefinition(PropertyInfo property, object originObject, out string errorMesage)
        {
            errorMesage = "";
            return true;
        }


        public override object Parse(PropertyInfo property, object targetObject, string rawFieldContent)
        {
            int divisorDecimal = (int)Math.Pow(10, this.DecimalPositions);

            if (!long.TryParse(rawFieldContent, out long valorEntero))
                throw new ParseFieldException($"El valor {rawFieldContent} no puede ser reconocido como numerico");

            object result;

            if (property.PropertyType == typeof(float) || property.PropertyType == typeof(float?))
            {
                result = (float)valorEntero / divisorDecimal;
            }
            else if (property.PropertyType == typeof(double) || property.PropertyType == typeof(double?))
            {
                result = (double)valorEntero / divisorDecimal;
            }
            else if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?))
            {
                result = (decimal)valorEntero / divisorDecimal;
            }
            else
            {
                throw new ParseFieldException($"La property {targetObject.GetType().Name}.{property.Name} es del tipo " +
                    $" {property.PropertyType.Name} el cual no es un destino soportado para un número de punto flotante");
            }

            return result;
        }


        public override string ToText(PropertyInfo property, object originObject)
        {
            long integerPart, decimalPart;
            int decimalDivider = (int)Math.Pow(10, this.DecimalPositions);

            if (property.PropertyType == typeof(float) || property.PropertyType == typeof(float?) || property.PropertyType == typeof(Single))
            {
                float valorTemp = (float)property.GetValue(originObject);
                integerPart = (long)Math.Truncate(valorTemp);
                decimalPart = (long)(Math.Round(valorTemp * decimalDivider) - integerPart * decimalDivider);
            }
            else if (property.PropertyType == typeof(double) || property.PropertyType == typeof(double?))
            {
                double valorTemp = (double)property.GetValue(originObject);
                integerPart = (long)Math.Truncate(valorTemp);
                decimalPart = (long)(Math.Round(valorTemp * decimalDivider) - integerPart * decimalDivider);
            }
            else if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?))
            {
                decimal valorTemp = (decimal)property.GetValue(originObject);
                integerPart = (long)Math.Truncate(valorTemp);
                decimalPart = (long)(valorTemp * decimalDivider) - integerPart * decimalDivider;
            }
            else
            {
                throw new SerializeFieldException($"La propiedad \"{originObject.GetType().Name}.{property.Name}\" es" +
                    $" del tipo {property.PropertyType.Name} y no es aceptada para serializar un numero de punto flotante");
            }

            decimalPart = Math.Abs(decimalPart);

            string integerPartText;
            string decimalPartText = Math.Abs(decimalPart).ToString().PadRight((int)this.DecimalPositions, '0');

            string mascara;

            if (this.FillLeftWithZero)
            {
                if (integerPart < 0)
                    mascara = $"D{this.Length - this.DecimalPositions - 1}";
                else
                    mascara = $"D{this.Length - this.DecimalPositions}";

                integerPartText = integerPart.ToString(mascara);
            }
            else
            {
                integerPartText = integerPart.ToString();
            }

            string serializedField = integerPartText + decimalPartText;
            serializedField = serializedField.PadLeft(this.Length, ' ');
            return serializedField;
        }

    }
}
