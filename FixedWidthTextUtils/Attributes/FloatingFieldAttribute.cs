using FixedFlatFileUtils.Exceptions;
using System;
using System.Reflection;

namespace FixedFlatFileUtils.Attributes
{

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class FloatingFieldAttribute : FieldAttribute
    {
        internal int DecimalPositions { get; set; }
        internal bool FillLeftWithZero { get; set; }

        public FloatingFieldAttribute(int startPosition, int endPosition, int decimalPositions, bool fillLeftWithZeros) : base(startPosition, endPosition)
        {
            if (decimalPositions < 0)
                throw new ArgumentOutOfRangeException($"El valor de {nameof(decimalPositions)} debe ser un valor mayor o igual a 0" );

            DecimalPositions = decimalPositions;
            FillLeftWithZero = fillLeftWithZeros;
        }

        internal override void Parse(PropertyInfo property, object targetObject, string rawFieldContent)
        {
            int divisorDecimal = (int)Math.Pow(10, this.DecimalPositions);

            if (!long.TryParse(rawFieldContent, out long valorEntero))
                throw new ParseFieldException($"El valor {rawFieldContent} no puede ser reconocido como numerico");

            if (property.PropertyType == typeof(float))
            {
                float valorTemp = (float)valorEntero / divisorDecimal;
                property.SetValue(targetObject, valorTemp);
            }
            else if (property.PropertyType == typeof(double))
            {
                double valorTemp = (double)valorEntero / divisorDecimal;
                property.SetValue(targetObject, valorTemp);
            }
            else if (property.PropertyType == typeof(decimal))
            {
                decimal valorTemp = (decimal)valorEntero / divisorDecimal;
                property.SetValue(targetObject, valorTemp);
            }
            else
            {
                throw new ParseFieldException($"La property {property.Name} es de tipo {property.PropertyType.Name} el cual no es soportado como un número de punto flotante");
            }
        }


        internal override string ToPlainText(PropertyInfo property, object originObject)
        {
            long integerPart, decimalPart;
            int decimalDivider = (int)Math.Pow(10, this.DecimalPositions);

            if (property.PropertyType == typeof(float) || property.PropertyType == typeof(Single))
            {
                float valorTemp = (float)property.GetValue(originObject);
                integerPart = (long)Math.Truncate(valorTemp);
                decimalPart = (long)(Math.Round(valorTemp * decimalDivider) - integerPart * decimalDivider);
            }
            else if (property.PropertyType == typeof(double))
            {
                double valorTemp = (double)property.GetValue(originObject);
                integerPart = (long)Math.Truncate(valorTemp);
                decimalPart = (long)(Math.Round(valorTemp * decimalDivider) - integerPart * decimalDivider);
            }
            else if (property.PropertyType == typeof(decimal))
            {
                decimal valorTemp = (decimal)property.GetValue(originObject);
                integerPart = (long)Math.Truncate(valorTemp);
                decimalPart = (long)(valorTemp * decimalDivider) - integerPart * decimalDivider;
            }
            else
            {
                throw new SerializeFieldException($"La propiedad {property.Name} es del tipo {property.PropertyType.Name} y no es aceptada para serializar");
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
