using FixedWidthTextUtils.Attributes;
using System;
using System.Reflection;
using System.Text;

namespace FixedWidthTextUtils_NUnit_Test.Models
{

    [StringeableClass(241, ' ')]
    internal class Cliente
    {
        [IntegerField(0, 8, true)]
        public long Id { get; set; }

        [StringField(9, 9)]
        public string? Code { get; set; }

        [StringField(10, 29, StringFieldAttribute.TrimMode.Trim)]
        public string? Name { get; set; }

        [StringField(30, 49)]
        public string? Street { get; set; }

        [IntegerField(50, 54, true)]
        public int HouseNumber { get; set; }

        [StringField(55, 59, StringFieldAttribute.TrimMode.TrimStart, true)]
        public string PostCode { get; set; } = default!;

        [StringField(60, 73, StringFieldAttribute.TrimMode.TrimEnd)]
        public string? City { get; set; }

        [StringField(74, 88, StringFieldAttribute.TrimMode.Trim)]
        public string? Country { get; set; }

        [DateTimeField(89, 96, "yyyyMMdd")]
        public DateTime BirthDate { get; set; }

        [IntegerField(97, 99, true)]
        public int HeigthInCentimeters { get; set; }

        [FloatingField(100, 102, 1, true)]
        public float WeightFloat { get; set; }

        [FloatingField(103, 105, 1, true )]
        public Single WeightSingle { get; set; }

        [FloatingField(106, 108, 1, true)]
        public Double WeightDouble { get; set; }

        [FloatingField(109, 111, 1, true)]
        public Decimal WeightDecimal { get; set; }
        
        [DateTimeField(112, 125, "yyyyMMddHHmmss")]
        public DateTime AnotherDateTime { get; set; }

        [DateTimeField(126, 143, "    yyyyMMddHHmmss", true)]
        public DateTime PadRightDateTime { get; set; }

        [DateTimeField(144, 161, "yyyyMMddHHmmss    ", true)]
        public DateTime PadLeftDateTime { get; set; }

        [IntegerField(162, 171, true)]
        public int CampoIntNegativo { get; set; }

        [IntegerField(172, 181, true)]
        public uint CampoUInt { get; set; }

        [IntegerField(182, 194, true)]
        public long CampoLong { get; set; }

        [IntegerField(195, 207, true)]
        public ulong CampoULong { get; set; }
        
        [FloatingField(208, 217, 2, true)]
        public float OtroCampoFlotante { get; set; }

        [IntegerField(218, 222, false)]
        public int CampoIntegerConEspacios { get; set; }

        [FloatingField(223, 232, 2, false)]
        public float CampoFlotanteConEspacios { get; set; }

        [DateTimeField(233, 240, "ddMMyyyy")]
        public DateTime OtroDateTime { get; set; }

        public float PropertyParaNoSerailizar { get; set; }
        public static DateTime PropertyComputada{ get { return DateTime.Now; } }


        public override string ToString()
        {
            StringBuilder sb = new();

            foreach (PropertyInfo property in this.GetType().GetProperties())
            {
                sb.AppendLine($"{property.Name} : {property.GetValue(this)}");
            }
            return sb.ToString();
        }
    }
}
