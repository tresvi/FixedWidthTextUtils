using FixedWidthTextUtils.Exceptions;
using System;
using System.Reflection;

namespace FixedWidthTextUtils.Attributes
{
    public sealed class NullableBooleanFieldAttribute : BooleanFieldAttribute
    {
        private string TextForNull { get; set; }

        public NullableBooleanFieldAttribute(int startPosition, int endPosition, string textForTrue, string textForFalse, string textForNull) 
            : base(startPosition, endPosition, textForTrue, textForFalse)
        {
            TextForNull = textForNull;
        }

        public NullableBooleanFieldAttribute(int fieldLength, string textForTrue, string textForFalse, string textForNull) 
            : base(fieldLength, textForTrue, textForFalse)
        {
            TextForNull = textForNull;
        }


        public override bool ValidateFieldDefinition(PropertyInfo property, object originObject, out string errorMessage)
        {
            if (this.Length != this.TextForNull.Length)
            {
                errorMessage = $"La longitud definida en el parametro \"{nameof(TextForNull)}\" del attribute ({this.TextForNull.Length} " +
                    $"caracteres) debe coincidir con la longitud definida para este campo ({this.Length} caracteres)";
                return false;
            }

            return base.ValidateFieldDefinition(property, originObject, out errorMessage);
        }


        public override object Parse(PropertyInfo property, object targetObject, string rawFieldContent)
        {
            return Parse(property, targetObject, (rawFieldContent ?? string.Empty).AsSpan());
        }


        public override object Parse(PropertyInfo property, object targetObject, ReadOnlySpan<char> rawFieldContent)
        {
            if (property.PropertyType != typeof(bool?))
                throw new ParseFieldException($"La propiedad de asignacion \"{targetObject.GetType().Name}" +
                    $".{property.Name}\" no es del tipo bool nullable");

            bool? value;
            if (this.TextForFalse == "")
            {
                value = rawFieldContent.SequenceEqual(this.TextForTrue.AsSpan());
            }
            else
            {
                if (rawFieldContent.SequenceEqual(this.TextForTrue.AsSpan()))
                    value = true;
                else if (rawFieldContent.SequenceEqual(this.TextForFalse.AsSpan()))
                    value = false;
                else if (rawFieldContent.SequenceEqual(this.TextForNull.AsSpan()))
                    value = null;
                else
                    throw new ParseFieldException($"El valor \"{rawFieldContent.ToString()}\" no puede ser reconocido como un bool " +
                        $" nullable válido para ser asignado a la property \"{targetObject.GetType().Name}.{property.Name}\"." +
                        $" Verifique que el dato coincida con los valores definidos para la property");
            }

            return value;
        }


        public override string ToText(PropertyInfo property, object originObject)
        {
            if (property.GetValue(originObject) == null) return this.TextForNull;

            return base.ToText(property, originObject);
        }


        public override void WriteTo(PropertyInfo property, object originObject, Span<char> destination)
        {
            if (property.GetValue(originObject) == null)
            {
                this.TextForNull.AsSpan().CopyTo(destination);
                return;
            }
            base.WriteTo(property, originObject, destination);
        }

    }
}
