using System;

namespace FixedFlatFileUtils.Exceptions
{
    public class NonStringeableClassException : FixedWidthTextException
    {
        public NonStringeableClassException() { }

        public NonStringeableClassException(string message) : base(message){}

        public NonStringeableClassException(string message, Exception inner) : base(message, inner){}
    }
}
