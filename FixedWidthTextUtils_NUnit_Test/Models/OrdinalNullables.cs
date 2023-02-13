using FixedWidthTextUtils.Attributes;
using System;

namespace FixedWidthTextUtils_NUnit_Test.Models
{
    internal class OrdinalNullables
    {
        [NullableBooleanField(2, "SI", "NO", "  ")]
        public bool? BooleanTrue { get; set; }

        [NullableBooleanField(2, "SI", "NO", "  ")]
        public bool? BooleanFalse { get; set; }

        [NullableBooleanField(2, "SI", "NO", "  ")]
        public bool? BooleanNull { get; set; }

        [StringField(5)]
        public string? SimpleString { get; set; }

        [NullableIntegerField(4, true, "    ")]
        public long? IntegerNull { get; set; }

        [NullableIntegerField(4, true, "    ")]
        public long? IntegerNotNull { get; set; }

        [NullableDateTimeField(8, "ddMMyyyy", "        ")]
        public DateTime? DateTimeNull { get; set; }

        [NullableDateTimeField(8, "ddMMyyyy", "        ")]
        public DateTime? DateTimeNotNull { get; set; }

    }
}
