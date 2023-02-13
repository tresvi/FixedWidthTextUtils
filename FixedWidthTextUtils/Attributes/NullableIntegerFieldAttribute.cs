using FixedWidthTextUtils.Exceptions;
using System.Reflection;

namespace FixedWidthTextUtils.Attributes
{
    public sealed class NullableIntegerFieldAttribute : IntegerFieldAttribute
    {
        private string TextForNull { get; set; }

        public NullableIntegerFieldAttribute(int startPosition, int endPosition, bool fillLeftWithZero, string textForNull) 
            : base(startPosition, endPosition, fillLeftWithZero)
        {
            this.TextForNull = textForNull;
        }


        public NullableIntegerFieldAttribute(int fieldLength, bool fillLeftWithZero, string textForNull) 
            : base(fieldLength, fillLeftWithZero)
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
            bool isValidType = property.PropertyType == typeof(byte?)
                || property.PropertyType == typeof(sbyte?)
                || property.PropertyType == typeof(short?)
                || property.PropertyType == typeof(ushort?)
                || property.PropertyType == typeof(int?)
                || property.PropertyType == typeof(uint?)
                || property.PropertyType == typeof(long?)
                || property.PropertyType == typeof(ulong?);

            if (!isValidType)
                throw new ParseFieldException($"La property {targetObject.GetType().Name}.{property.Name} es de tipo " +
                    $"{property.PropertyType.Name} el cual no es un destino soportado para un número de Entero");

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
