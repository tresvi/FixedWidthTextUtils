using FixedWidthTextUtils.Exceptions;
using System;
using System.Reflection;

namespace FixedWidthTextUtils.Attributes
{
    public sealed class NullableDateTimeFieldAttribute : DateTimeFieldAttribute
    {
        private string TextForNull { get; set; }

        public NullableDateTimeFieldAttribute(int startPosition, int endPosition, string format, string textForNull, bool leftPadding = false)
            : base(startPosition, endPosition, format, leftPadding)
        {
            this.TextForNull = textForNull;
        }


        public NullableDateTimeFieldAttribute(int fieldLength, string format, string textForNull, bool leftPadding = false) 
            : base(fieldLength, format, leftPadding)
        {
            this.TextForNull = textForNull;
        }


        public override bool ValidateFieldDefinition(PropertyInfo property, object originObject, out string errorMesage)
        {
            if (this.Length != this.TextForNull.Length)
            {
                errorMesage = $"La longitud definida en el parametro \"{nameof(TextForNull)}\" del attribute ({this.TextForNull.Length} " +
                    $"caracteres) debe coincidir con la longitud definida para este campo ({this.Length} caracteres)";
                return false;
            }

            return base.ValidateFieldDefinition(property, originObject, out errorMesage);
        }

        public override object Parse(PropertyInfo property, object targetObject, string rawFieldContent)
        {
            if (property.PropertyType != typeof(DateTime?))
                throw new ParseFieldException($"La propiedad de asignacion \"{targetObject.GetType().Name}.{property.Name}\", " +
                    $"es del tipo {property.PropertyType.Name} pero se esperaba un {typeof(DateTime?).Name}");

            if (rawFieldContent == this.TextForNull)
                return null;
            else
                return base.Parse(property, targetObject, rawFieldContent);
        }


        public override string ToText(PropertyInfo property, object originObject)
        {
            if (property.GetValue(originObject) == null) return this.TextForNull;

            return base.ToText(property, originObject);
        }

    }
}
