using FixedWidthTextUtils.Attributes;
using System;
using System.Reflection;
using System.Text;

namespace FixedWidthTextUtils_NUnit_Test.Models
{

    internal class Client_PositionalAndOrdinal
    {
        [IntegerField(9, true)]
        public long Id { get; set; }

        [StringField(1)]
        public string? Code { get; set; }

        [StringField(20, StringFieldAttribute.TrimMode.Trim)]
        public string? Name { get; set; }

        [StringField(20)]
        public string? Street { get; set; }

        [IntegerField(5, true)]
        public int HouseNumber { get; set; }

        [StringField(5, StringFieldAttribute.TrimMode.TrimStart, true)]
        public string PostCode { get; set; } = default!;

        [StringField(14, StringFieldAttribute.TrimMode.TrimEnd)]
        public string? City { get; set; }

        [StringField(15, StringFieldAttribute.TrimMode.Trim)]
        public string? Country { get; set; }

        [DateTimeField(8, "yyyyMMdd")]
        public DateTime BirthDate { get; set; }

        [IntegerField(97, 99, true)]
        public int HeigthInCentimeters { get; set; }

        [FloatingField(100, 102, 1, true)]
        public float WeightFloat { get; set; }

        [StringField(103, 109)]
        public string? Filler { get; set; }


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
