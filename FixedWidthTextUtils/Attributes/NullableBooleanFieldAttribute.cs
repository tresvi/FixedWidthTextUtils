using FixedWidthTextUtils.Exceptions;
using System.Reflection;
using System;

namespace FixedWidthTextUtils.Attributes
{
    public class NullableBooleanFieldAttribute : BooleanFieldAttribute
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

        internal override object Parse(PropertyInfo property, object targetObject, string rawFieldContent)
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


        internal override string ToPlainText(PropertyInfo property, object originObject)
        {
            if (property.PropertyType != typeof(bool?))
                throw new SerializeFieldException($"La propiedad para la serializacion \"{property.Name}\" no es del tipo Nullable<bool>");

            bool? value = (bool?)property.GetValue(originObject);

            if (!value.HasValue) 
                return this.TextForNull;
            else if (value.Value == true)
                return this.TextForTrue;
            else
                return this.TextForFalse;
        }

    }
}
