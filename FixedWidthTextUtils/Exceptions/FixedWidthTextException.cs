using System;
using System.Collections.Generic;
using System.Text;

namespace FixedFlatFileUtils.Exceptions
{
    public abstract class FixedWidthTextException: SystemException
    {
        public FixedWidthTextException() { }
        public FixedWidthTextException(string message) : base(message) { }
        public FixedWidthTextException(string message, Exception inner) : base(message, inner) { }
    }
}
