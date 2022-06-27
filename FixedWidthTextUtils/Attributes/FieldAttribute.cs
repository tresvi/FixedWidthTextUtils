using System;
using System.Reflection;

namespace FixedFlatFileUtils.Attributes
{
    public abstract class FieldAttribute : Attribute
    {
        internal int StartPosition { get; set; }
        internal int EndPosition { get; set; }
        internal int Length
        {
            get { return EndPosition - StartPosition + 1; }
        }

        public FieldAttribute(int startPosition, int endPosition)
        {
            if (startPosition < 0)
                throw new ArgumentException(nameof(startPosition), "StartPosition debe ser >= 0");

            if (endPosition < startPosition)
                throw new ArgumentException(nameof(EndPosition), "EndPosition debe ser >= a StartPosition");

            StartPosition = startPosition;
            EndPosition = endPosition;
        }

        internal abstract void Parse(PropertyInfo property, object targetObject, string rawFieldContent);
        internal abstract string ToPlainText(PropertyInfo property, object originObject);

    }
}
