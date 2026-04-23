using FixedWidthTextUtils.Exceptions;
using System;
using System.Reflection;

namespace FixedWidthTextUtils.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class BooleanFieldAttribute : FieldAttribute
    {
        public string TextForTrue { get; set; }
        public string TextForFalse { get; set; }

        public BooleanFieldAttribute(int startPosition, int endPosition, string textForTrue, string textForFalse)
            : base(startPosition, endPosition)
        {
            this.TextForTrue = textForTrue;
            this.TextForFalse = textForFalse;
        }

        public BooleanFieldAttribute(int fieldLength, string textForTrue, string textForFalse)
            : base(fieldLength)
        {
            this.TextForTrue = textForTrue;
            this.TextForFalse = textForFalse;
        }


        public override bool ValidateFieldDefinition(PropertyInfo property, object originObject, out string errorMessage)
        {
            if (this.Length != this.TextForTrue.Length)
            {
                errorMessage = $"La longitud definida en el parametro \"{nameof(TextForTrue)}\" del attribute ({this.TextForTrue.Length} " +
                    $"caracteres) debe coincidir con la longitud definida para este campo ({this.Length} caracteres)";
                return false;
            }

            if (this.TextForTrue == this.TextForFalse)
            {
                errorMessage = $"El valor del parametro \"{nameof(TextForTrue)}\" no puede coincidir con el valor del parametro \"{nameof(TextForFalse)}\"";
                return false;
            }

            //Se permite que el false sea el caracter empty ". esto es para dar flexibilidad en la definicionde los false
            if (this.Length != this.TextForFalse.Length && this.TextForFalse != "")
            {
                errorMessage = $"La longitud definida en el parametro \"{nameof(TextForFalse)}\" del attribute ({this.TextForFalse.Length} " +
                    $"caracteres) debe coincidir con la longitud del campo definido ({this.Length} caracteres)";
                return false;
            }

            errorMessage = "";
            return true;
        }


        public override object Parse(PropertyInfo property, object targetObject, string rawFieldContent)
        {
            return Parse(property, targetObject, (rawFieldContent ?? string.Empty).AsSpan());
        }


        public override object Parse(PropertyInfo property, object targetObject, ReadOnlySpan<char> rawFieldContent)
        {
            if (property.PropertyType != typeof(bool))
                throw new ParseFieldException($"La propiedad de asignacion \"{targetObject.GetType().Name}.{property.Name}\" no es del tipo bool");

            if (this.TextForFalse == "")
            {
                return rawFieldContent.SequenceEqual(this.TextForTrue.AsSpan());
            }

            if (rawFieldContent.SequenceEqual(this.TextForTrue.AsSpan()))
                return true;
            if (rawFieldContent.SequenceEqual(this.TextForFalse.AsSpan()))
                return false;

            throw new ParseFieldException($"El valor \"{rawFieldContent.ToString()}\" no puede ser reconocido como un booleano válido para ser asignado a " +
                $"la property \"{targetObject.GetType().Name}.{property.Name}\". Verifique que el dato coincida con los valores definidos para la property");
        }


        public override string ToText(PropertyInfo property, object originObject)
        {
            if (property.PropertyType != typeof(bool) && property.PropertyType != typeof(bool?))
                throw new SerializeFieldException($"La propiedad para la serializacion \"{originObject.GetType().Name}.{property.Name}\" no es del tipo bool");

            bool value = (bool)property.GetValue(originObject);

            if (value)
                return this.TextForTrue;
            else 
                return this.TextForFalse;
        }


        public override void WriteTo(PropertyInfo property, object originObject, Span<char> destination)
        {
            if (property.PropertyType != typeof(bool) && property.PropertyType != typeof(bool?))
                throw new SerializeFieldException($"La propiedad para la serializacion \"{originObject.GetType().Name}.{property.Name}\" no es del tipo bool");

            object raw = property.GetValue(originObject);
            bool value = raw != null && (bool)raw;

            string text = value ? this.TextForTrue : this.TextForFalse;
            // Si TextForFalse == "" tratamos al falso como espacios para no dejar el slice sin escribir.
            if (text.Length == 0)
            {
                destination.Fill(' ');
                return;
            }

            int n = Math.Min(text.Length, destination.Length);
            text.AsSpan(0, n).CopyTo(destination);
            if (n < destination.Length)
                destination.Slice(n).Fill(' ');
        }
    }
}
