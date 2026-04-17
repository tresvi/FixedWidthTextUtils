using FixedWidthTextUtils.Attributes;
using System;

namespace Ordinal_Example_NF_4_8
{
    /// <summary>
    /// Mapeo por orden de propiedades (modo ordinal): cada atributo declara solo el largo del campo.
    /// Modelo de referencia para modo ordinal (solo largos de campo).
    /// </summary>
    internal class ClienteOrdinal
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
