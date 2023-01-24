using FixedWidthTextUtils.Exceptions;
using System;
using System.Reflection;

namespace FixedWidthTextUtils.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class StringFieldAttribute : FieldAttribute
    {
        public enum TrimMode { NoTrim, Trim, TrimStart, TrimEnd };
        public TrimMode TrimInputMode { get; set; }
        public bool LeftPadding { get; set; }


        public StringFieldAttribute(int startPosition, int endPosition, TrimMode trimInputMode = TrimMode.NoTrim, bool leftPadding = false) : base(startPosition, endPosition)
        {
            if (startPosition < 0)
                throw new ArgumentException(nameof(startPosition), "StartPosition debe ser >= 0");

            if (endPosition < startPosition)
                throw new ArgumentException(nameof(endPosition), "EndPosition debe ser >= a StartPosition");

            TrimInputMode = trimInputMode;
            LeftPadding = leftPadding;
        }

        public StringFieldAttribute(int fieldLength, TrimMode trimInputMode = TrimMode.Trim, bool leftPadding = false) : base(fieldLength)
        {
            if (fieldLength < 1)
                throw new ArgumentException(nameof(fieldLength), $"{nameof(fieldLength)} debe ser mayor a 1");

            TrimInputMode = trimInputMode;
            LeftPadding = leftPadding;
        }


        internal override void Parse(PropertyInfo property, object targetObject, string rawFieldContent)
        {
            if (property.PropertyType != typeof(String) || property.PropertyType != typeof(string))
                throw new ParseFieldException($"La propiedad de asignacion {property.Name} no es del tipo string");

            switch (this.TrimInputMode)
            {
                case TrimMode.Trim:
                    rawFieldContent = rawFieldContent.Trim();
                    break;
                case TrimMode.TrimStart:
                    rawFieldContent = rawFieldContent.TrimStart();
                    break;
                case TrimMode.TrimEnd:
                    rawFieldContent = rawFieldContent.TrimEnd();
                    break;
            }

            property.SetValue(targetObject, rawFieldContent);
        }


        internal override string ToPlainText(PropertyInfo property, object originObject)
        {
            if (property.PropertyType != typeof(String) || property.PropertyType != typeof(string))
                throw new SerializeFieldException($"La propiedad para la serializacion {property.Name} no es del tipo string");

            string outputText = (property.GetValue(originObject) ?? "").ToString();
            outputText = this.LeftPadding ? outputText.PadLeft(this.Length) : outputText.PadRight(this.Length);
            return outputText;
        }
    }
}
