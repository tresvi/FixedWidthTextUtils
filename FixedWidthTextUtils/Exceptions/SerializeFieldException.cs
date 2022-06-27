using System;

namespace FixedWidthTextUtils.Exceptions
{
    public class SerializeFieldException : FixedWidthTextException
    {
        public SerializeFieldException() { }
        public SerializeFieldException(string message) : base(message) { }
        public SerializeFieldException(string message, Exception inner) : base(message, inner) { }
    }
}
