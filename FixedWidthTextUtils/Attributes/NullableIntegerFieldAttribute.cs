using FixedWidthTextUtils.Exceptions;
using System.Reflection;

namespace FixedWidthTextUtils.Attributes
{
    public class NullableIntegerFieldAttribute : IntegerFieldAttribute
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


        internal override object Parse(PropertyInfo property, object targetObject, string rawFieldContent)
        {
            bool typeAllowed = property.PropertyType == typeof(byte?)
                || property.PropertyType == typeof(sbyte?)
                || property.PropertyType == typeof(short?)
                || property.PropertyType == typeof(ushort?)
                || property.PropertyType == typeof(int?)
                || property.PropertyType == typeof(uint?)
                || property.PropertyType == typeof(long?)
                || property.PropertyType == typeof(ulong?);

            if (!typeAllowed)
                throw new ParseFieldException($"La property {property.Name} es de tipo {property.PropertyType.Name} " +
                    $"el cual no es un destino soportado para un número de punto flotante Nullable");

            if (rawFieldContent == this.TextForNull)
                return null;
            else
                return base.Parse(property, targetObject, rawFieldContent);
        }


        internal override string ToPlainText(PropertyInfo property, object originObject)
        {
            if (property.GetValue(originObject) == null)
                return this.TextForNull;

            return base.ToPlainText(property, originObject);
        }

    }
}
