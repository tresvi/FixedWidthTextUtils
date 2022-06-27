﻿using System;

namespace FixedFlatFileUtils.Exceptions
{
    public class ParseFieldException : FixedWidthTextException
    {
        public ParseFieldException() { }
        public ParseFieldException(string message) : base(message) { }
        public ParseFieldException(string message, Exception inner) : base(message, inner) { }
    }
}
