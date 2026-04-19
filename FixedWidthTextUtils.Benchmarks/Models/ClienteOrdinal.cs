using FixedWidthTextUtils.Attributes;
using System;

namespace FixedWidthTextUtils.Benchmarks.Models
{
    /// <summary>
    /// Modelo ordinal equivalente al usado en Ordinal_Example_NF_4_8 / ParseBenchmark_Example_NF_4_8.
    /// </summary>
    public class ClienteOrdinal
    {
        [StringField(1)]
        public string Code { get; set; }

        [StringField(4, StringFieldAttribute.TrimMode.Trim)]
        public string Name { get; set; }

        [StringField(5, StringFieldAttribute.TrimMode.Trim)]
        public string LastName { get; set; }

        [BooleanField(3, "YES", "NO ")]
        public bool Enable { get; set; }

        [IntegerField(4, true)]
        public int ZipCode { get; set; }

        [DateTimeField(8, "yyyyMMdd")]
        public DateTime BirthDate { get; set; }

        [FloatingField(4, 2, true)]
        public float Weight { get; set; }
    }
}
