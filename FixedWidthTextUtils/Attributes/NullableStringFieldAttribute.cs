using FixedWidthTextUtils.Exceptions;
using System.Reflection;

namespace FixedWidthTextUtils.Attributes
{
    internal sealed class NullableStringFieldAttribute : StringFieldAttribute
    {
        private string TextForNull { get; set; }

        public NullableStringFieldAttribute(int fieldLength, string textForNull, TrimMode trimInputMode = TrimMode.TrimEnd, bool leftPadding = false) 
            : base(fieldLength, trimInputMode, leftPadding)
        {
            this.TextForNull = textForNull;
        }


        public NullableStringFieldAttribute(int startPosition, int endPosition, string textForNull, TrimMode trimInputMode = TrimMode.TrimEnd, bool leftPadding = false) 
            : base(startPosition, endPosition, trimInputMode, leftPadding)
        {
            this.TextForNull = textForNull;
        }


        public override object Parse(PropertyInfo property, object targetObject, string rawFieldContent)
        {
            if (property.PropertyType != typeof(string))
                throw new ParseFieldException($"La propiedad de asignacion \"{targetObject.GetType().Name}" +
                    $".{property.Name}\" no es del tipo bool nullable");

            return base.Parse(property, targetObject, rawFieldContent);
        }


        public override string ToText(PropertyInfo property, object originObject)
        {
            if (property.GetValue(originObject) == null) return this.TextForNull;

            return base.ToText(property, originObject);

        }
    }
}
