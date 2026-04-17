using FixedWidthTextUtils.Attributes;
using System;

namespace FixedWidthTextUtils_NUnit_Test.Models
{
    /// <summary>Ordinal + posicional en la misma clase: GetLineLength / serialización deben rechazarlo.</summary>
    internal class Model_MixedOrdinalPositional
    {
        [IntegerField(2, true)]
        public int OrdinalPart { get; set; }

        [IntegerField(10, 12, true)]
        public int PositionalPart { get; set; }
    }

    /// <summary>Valor entero demasiado largo para el ancho del campo.</summary>
    internal class Model_IntegerTooWideForField
    {
        [IntegerField(0, 1, true)]
        public int Value { get; set; } = 99999;
    }

    /// <summary>int? con IntegerFieldAttribute (debe usar NullableIntegerFieldAttribute).</summary>
    internal class Model_IntegerFieldOnNullableInt
    {
        [IntegerField(0, 2, true)]
        public int? Value { get; set; }
    }

    /// <summary>IntegerField sobre propiedad no entera.</summary>
    internal class Model_IntegerFieldOnDecimal
    {
        [IntegerField(0, 2, true)]
        public decimal Value { get; set; }
    }

    /// <summary>Campo entero con espacios laterales en el texto fijo.</summary>
    internal class Model_IntegerPaddedField
    {
        [IntegerField(0, 4, false)]
        public int Value { get; set; }
    }

    /// <summary>Campo numérico implícito con espacios.</summary>
    internal class Model_FloatingPaddedField
    {
        [FloatingField(0, 4, 1, false)]
        public float Value { get; set; }
    }
}
