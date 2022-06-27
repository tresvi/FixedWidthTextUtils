using FixedWidthTextUtils.Attributes;
using System;

namespace FixedWidthTextUtils_NUnit_Test.Models
{
    public class Client_NonStringeable
    {
        [IntegerField(0, 8, true)]
        public long Id { get; set; }


        [StringField(9, 9, StringFieldAttribute.TrimMode.Trim)]
        public string? Code { get; set; }


        [DateTimeField(10, 17, "yyyyMMdd")]
        public DateTime Date { get; set; }


        [FloatingField(18, 20, 1, true)]
        public double FloatingField { get; set; }
    }
}
