using System;

namespace FixedWidthTextUtils.Exceptions
{
    public class DateFormatException: FixedWidthTextException
    {
        public DateFormatException() { }
        public DateFormatException(string message) : base(message) { }
        public DateFormatException(string message, Exception inner) : base(message, inner) { }
    }
}
