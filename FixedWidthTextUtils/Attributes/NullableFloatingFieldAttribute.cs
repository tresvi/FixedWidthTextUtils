using FixedWidthTextUtils.Exceptions;
using System.Reflection;

namespace FixedWidthTextUtils.Attributes
{
    public class NullableFloatingFieldAttribute : FloatingFieldAttribute
    {
        private string TextForNull { get; set; }

        public NullableFloatingFieldAttribute(int startPosition, int endPosition, int decimalPositions, bool fillLeftWithZeros, string textForNull)
            : base(startPosition, endPosition, decimalPositions, fillLeftWithZeros)
        {
            this.TextForNull = textForNull;
        }


        public NullableFloatingFieldAttribute(int fieldLength, int decimalPositions, bool fillLeftWithZeros, string textForNull)
            : base(fieldLength, decimalPositions, fillLeftWithZeros)
        {
            this.TextForNull = textForNull;
        }


        internal override object Parse(PropertyInfo property, object targetObject, string rawFieldContent)
        {
            bool typeAllowed = property.PropertyType == typeof(float?) 
                            || property.PropertyType == typeof(double?) 
                            || property.PropertyType == typeof(decimal?);
            
            if (!typeAllowed) 
                throw new ParseFieldException($"La property {targetObject.GetType().Name}.{property.Name} es de tipo {property.PropertyType.Name} " +
                    $"el cual no es un destino soportado como un número de punto flotante Nullable");

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