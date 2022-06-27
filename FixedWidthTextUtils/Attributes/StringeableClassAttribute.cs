using System;


namespace FixedFlatFileUtils.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class StringeableClassAttribute : Attribute
    {
        public int RegisterLineLength { get; set; }
        public char FillerChar { get; set; } = ' ';

        public StringeableClassAttribute(int registerLineLength, char fillerChar)
        {
            RegisterLineLength = registerLineLength;
            FillerChar = fillerChar;
        }

    }
}
