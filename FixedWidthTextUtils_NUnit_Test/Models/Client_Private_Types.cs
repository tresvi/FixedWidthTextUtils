using FixedWidthTextUtils.Attributes;
using System;

namespace FixedWidthTextUtils_NUnit_Test.Models
{
 
    internal class Client_With_Priv_Method
    {
#pragma warning disable IDE0051

        [IntegerField(0, 8, true)]
        private long Id { get; set; }


        [StringField(9, 9, StringFieldAttribute.TrimMode.Trim)]
        private string? Code { get; set; }


        [DateTimeField(10, 17, "yyyyMMdd")]
        internal DateTime Date { get; set; }


        [FloatingField(18, 20, 1 , true)]
        internal double FloatingField { get; set; }

    }
}
