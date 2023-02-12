﻿using System;
using System.Reflection;

namespace FixedWidthTextUtils.Attributes
{
    public abstract class FieldAttribute : Attribute
    {
        internal int StartPosition { get; set; }
        internal int EndPosition { get; set; }
        //internal int FieldLength { get; set; }
        internal bool IsOrdinalMode { get; set; }

        internal int Length { get; set; }

        public FieldAttribute(int startPosition, int endPosition)
        {
            if (startPosition < 0)
                throw new ArgumentException(nameof(startPosition), "StartPosition debe ser >= 0");

            if (endPosition < startPosition)
                throw new ArgumentException(nameof(EndPosition), "EndPosition debe ser >= a StartPosition");

            StartPosition = startPosition;
            EndPosition = endPosition;
            Length = EndPosition - StartPosition + 1;
            IsOrdinalMode = false;
        }

        public FieldAttribute(int fieldLength)
        {
            Length = fieldLength;
            //FieldLength = fieldLength;
            IsOrdinalMode = true;
        }

        internal abstract object Parse(PropertyInfo property, object targetObject, string rawFieldContent);
        internal abstract string ToPlainText(PropertyInfo property, object originObject);

    }
}
