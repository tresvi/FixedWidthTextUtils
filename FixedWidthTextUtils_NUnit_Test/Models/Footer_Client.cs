using FixedWidthTextUtils.Attributes;

namespace FixedWidthTextUtils_NUnit_Test.Models
{
    internal class Footer_Client
    {
        [StringField(0, 1)]
        public string? RegisterType { get; set; }

        [IntegerField(2, 6)]
        public int NroRegistros { get; set; }

        [StringField(7, 7)]
        public string? ControlCharacter { get; set; }

        [IntegerField(8, 20)]
        public int Total { get; set; }
    }
}
