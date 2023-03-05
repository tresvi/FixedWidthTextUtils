using FixedWidthTextUtils.Exceptions;
using System.Reflection;
using System;

namespace FixedWidthTextUtils.Attributes
{
    public sealed class NullableBooleanFieldAttribute : BooleanFieldAttribute
    {
        //TODO: Validar que el textoForNull, TextForFalse y TextForTrue, tengan la longitud del campo recortado.
        //TODO: Validar que el textoForNull tengan la longitud del campo recortado para todas las clases.

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


        public override bool ValidateFieldDefinition(PropertyInfo property, object originObject, out string errorMesage)
        {
            if (this.Length != this.TextForNull.Length)
            {
                errorMesage = $"La longitud definida en el parametro \"{nameof(TextForNull)}\" del attribute ({this.TextForNull.Length} " +
                    $"caracteres) debe coincidir con la longitud definida para este campo ({this.Length} caracteres)";
                return false;
            }

            if (this.TextForNull == this.TextForTrue)
            {
                errorMesage = $"El valor del parametro \"{nameof(TextForNull)}\" no puede coincidir con el valor del parametro \"{nameof(TextForTrue)}\"";
                return false;
            }

            if (this.TextForNull == this.TextForFalse)
            {
                errorMesage = $"El valor del parametro \"{nameof(TextForNull)}\" no puede coincidir con el valor del parametro \"{nameof(TextForFalse)}\"";
                return false;
            }

            return base.ValidateFieldDefinition(property, originObject, out errorMesage);
        }


        public override object Parse(PropertyInfo property, object targetObject, string rawFieldContent)
        {
            if (property.PropertyType != typeof(bool?))
                throw new ParseFieldException($"La propiedad de asignacion \"{targetObject.GetType().Name}" +
                    $".{property.Name}\" no es del tipo bool nullable");

            bool? value;
            if (this.TextForFalse == "")
            {
                value = rawFieldContent == this.TextForTrue;
            }
            else
            {
                if (rawFieldContent == this.TextForTrue)
                    value = true;
                else if (rawFieldContent == this.TextForFalse)
                    value = false;
                else if (rawFieldContent == this.TextForNull)
                    value = null;
                else
                { 
                    throw new ParseFieldException($"El valor \"{rawFieldContent}\" no puede ser reconocido como un bool " +
                        $" nullable válido para ser asignado a la property \"{targetObject.GetType().Name}.{property.Name}\"." +
                        $" Verifique que el dato coincida con los valores definidos para la property");
                }
            }

            return value;
        }


        public override string ToText(PropertyInfo property, object originObject)
        {
            if (property.GetValue(originObject) == null) return this.TextForNull;

            return base.ToText(property, originObject);

        }

    }
}
