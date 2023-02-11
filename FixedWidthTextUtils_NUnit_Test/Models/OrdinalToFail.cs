using FixedWidthTextUtils.Attributes;

namespace FixedWidthTextUtils_NUnit_Test.Models
{
    internal class OrdinalToFail
    {
        [NullableBooleanField(2, "SI", "NO", "  ")]
        public bool Boolean { get; set; }
    }
}
