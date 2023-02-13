using FixedWidthTextUtils.Exceptions;
using System.Reflection;

namespace FixedWidthTextUtils.Attributes
{
    public sealed class NullableFloatingFieldAttribute : FloatingFieldAttribute
    {
        private string TextForNull { get; set; }

        public NullableFloatingFieldAttribute(int startPosition, int endPosition, int decimalPositions, string textForNull, bool fillLeftWithZeros = true)
            : base(startPosition, endPosition, decimalPositions, fillLeftWithZeros)
        {
            this.TextForNull = textForNull;
        }


        public NullableFloatingFieldAttribute(int fieldLength, int decimalPositions, string textForNull, bool fillLeftWithZeros = true)
            : base(fieldLength, decimalPositions, fillLeftWithZeros)
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
            bool isValidType = property.PropertyType == typeof(float?) 
                            || property.PropertyType == typeof(double?) 
                            || property.PropertyType == typeof(decimal?);
            
            if (!isValidType) 
                throw new ParseFieldException($"La property {targetObject.GetType().Name}.{property.Name} es de tipo " +
                    $"{property.PropertyType.Name} el cual no es un destino soportado para un número de punto flotante Nullable");

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