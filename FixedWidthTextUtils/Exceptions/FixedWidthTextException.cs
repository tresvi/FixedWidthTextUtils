using System;

namespace FixedWidthTextUtils.Exceptions
{
    public abstract class FixedWidthTextException: ApplicationException
    {
        public FixedWidthTextException() { }
        public FixedWidthTextException(string message) : base(message) { }
        public FixedWidthTextException(string message, Exception inner) : base(message, inner) { }
    }
}
