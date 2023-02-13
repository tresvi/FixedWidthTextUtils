using FixedWidthTextUtils.Exceptions;
using System;
using System.Globalization;
using System.Reflection;

namespace FixedWidthTextUtils.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class DateTimeFieldAttribute : FieldAttribute
    {
        public string Format { get; set; }
        public bool PadToRight { get; set; }
        public bool LeftPadding { get; set; }


        public DateTimeFieldAttribute(int startPosition, int endPosition, string format, bool leftPadding = false) : base(startPosition, endPosition)
        {
            if (string.IsNullOrWhiteSpace(format))
                throw new ArgumentException(nameof(format), " format no puede ser un valor nulo");      //!! Revisar de como cambiarlo por una excpecion propia

            Format = format;
            LeftPadding = leftPadding;
        }

        public DateTimeFieldAttribute(int fieldLength, string format, bool leftPadding = false) : base(fieldLength)
        {
            if (string.IsNullOrWhiteSpace(format))
                throw new ArgumentException(nameof(format), " format no puede ser un valor nulo");      //!! Revisar de como cambiarlo por una excpecion propia

            Format = format;
            LeftPadding = leftPadding;
        }


        public override bool ValidateFieldDefinition(PropertyInfo property, object originObject, out string errorMesage)
        {
            if (this.Length != this.Format.Length)
            {
                errorMesage = $"La longitud definida en el parametro \"{nameof(Format)}\" del attribute ({this.Format.Length} " +
                    $"caracteres) debe coincidir con la longitud definida para este campo ({this.Length} caracteres)";
                return false;
            }

            errorMesage = "";
            return true;
        }


        public override object Parse(PropertyInfo property, object targetObject, string rawFieldContent)
        {
            bool isValidType = property.PropertyType == typeof(DateTime)
                            || property.PropertyType == typeof(DateTime?);

            if (!isValidType)
                throw new ParseFieldException($"La property {targetObject.GetType().Name}.{property.Name} es de tipo " +
                    $"{property.PropertyType.Name} el cual no es un destino soportado para un {typeof(DateTime?).Name}");

            rawFieldContent = rawFieldContent.Trim();

            bool parseOK = DateTime.TryParseExact(rawFieldContent, this.Format.Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaTemp);

            if (!parseOK)
                throw new ParseFieldException($"El valor \"{rawFieldContent}\" no puede ser interpretado como fecha según el formato \"{this.Format}\"  de la propiedad {property.Name}");

            return fechaTemp;
        }


        public override string ToText(PropertyInfo property, object originObject)
        {
            if (property.PropertyType != typeof(DateTime) && property.PropertyType != typeof(DateTime?))
                throw new SerializeFieldException($"La propiedad para la serializacion \"{originObject.GetType().Name}.{property.Name}\" no es del tipo DateTime");

            DateTime DateTemp = (DateTime)property.GetValue(originObject);

            string outputText = DateTemp.ToString(this.Format);
            outputText = this.LeftPadding ? outputText.PadLeft(this.Length) : outputText.PadRight(this.Length);
            return outputText;
        }
    }
}
