using FixedFlatFileUtils.Exceptions;
using System;
using System.Globalization;
using System.Reflection;

namespace FixedFlatFileUtils.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class DateTimeFieldAttribute : FieldAttribute
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


        internal override void Parse(PropertyInfo property, object targetObject, string rawFieldContent)
        {
            if (property.PropertyType != typeof(DateTime))
                throw new ParseFieldException($"La propiedad de asignacion {property.Name} no es del tipo DateTime");

            rawFieldContent = rawFieldContent.Trim();

            bool parseOK = DateTime.TryParseExact(rawFieldContent, this.Format.Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaTemp);

            if (!parseOK)
                throw new ParseFieldException($"El valor \"{rawFieldContent}\" no puede ser interpretado como fecha según el formato \"{this.Format}\"  de la propiedad {property.Name}");

            property.SetValue(targetObject, fechaTemp);
        }


        internal override string ToPlainText(PropertyInfo property, object originObject)
        {
            if (property.PropertyType != typeof(DateTime))
                throw new SerializeFieldException($"La propiedad para la serializacion {property.Name} no es del tipo DateTime");

            DateTime DateTemp = (DateTime)property.GetValue(originObject);

            string outputText = DateTemp.ToString(this.Format);
            outputText = this.LeftPadding ? outputText.PadLeft(this.Length) : outputText.PadRight(this.Length);
            return outputText;
        }
    }
}
