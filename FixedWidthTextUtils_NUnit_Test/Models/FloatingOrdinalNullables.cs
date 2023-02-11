using FixedWidthTextUtils.Attributes;


namespace FixedWidthTextUtils_NUnit_Test.Models
{
    internal class FloatingOrdinalNullables
    {
        [NullableFloatingField(4, 2, true, "    ")]
        public float? FloatNull { get; set; }
        [NullableFloatingField(4, 2, true, "    ")]
        public float? FloatNotNull { get; set; }

        [NullableFloatingField(4, 2, true, "    ")]
        public double? DoubleNull { get; set; }
        [NullableFloatingField(4, 2, true, "    ")]
        public double? DoubleNotNull { get; set; }

        [NullableFloatingField(4, 2, true, "    ")]
        public decimal? DecimalNull { get; set; }
        [NullableFloatingField(4, 2, true, "    ")]
        public decimal? DecimalNotNull { get; set; }
    }
}
