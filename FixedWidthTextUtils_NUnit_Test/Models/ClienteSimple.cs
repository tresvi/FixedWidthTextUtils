using FixedWidthTextUtils.Attributes;
using System;
using System.Reflection;
using System.Text;

namespace FixedWidthTextUtils_NUnit_Test.Models
{

    [StringeableClass(110, ' ')]
    internal class ClienteSimple
    {
        [IntegerField(0, 8, true)]
        public long Id { get; set; }

        [StringField(9, 9)]
        public string Code { get; set; }

        [StringField(10, 29, StringFieldAttribute.TrimMode.Trim)]
        public string Name { get; set; }

        [StringField(30, 49)]
        public string Street { get; set; }

        [IntegerField(50, 54, true)]
        public int HouseNumber { get; set; }

        [StringField(55, 59, StringFieldAttribute.TrimMode.TrimStart, true)]
        public string PostCode { get; set; }

        [StringField(60, 73, StringFieldAttribute.TrimMode.TrimEnd)]
        public string City { get; set; }

        [StringField(74, 88, StringFieldAttribute.TrimMode.Trim)]
        public string Country { get; set; }

        [DateTimeField(89, 96, "yyyyMMdd")]
        public DateTime BirthDate { get; set; }

        [IntegerField(97, 99, true)]
        public int HeigthInCentimeters { get; set; }

        [FloatingField(100, 102, 1, true)]
        public float WeightFloat { get; set; }


        public float PropertyParaNoSerailizar { get; set; }
        public static DateTime PropertyComputada { get { return DateTime.Now; } }


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
