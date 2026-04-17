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

    internal class Model_IntNegativeZeroPad
    {
        [IntegerField(0, 4, true)]
        public int Value { get; set; } = -5;
    }

    internal class Model_IntSpacePadNoZero
    {
        [IntegerField(0, 2, false)]
        public int Value { get; set; } = 42;
    }

    internal class Model_IntExactWidthFiveDigits
    {
        [IntegerField(0, 4, true)]
        public int Value { get; set; } = 12345;
    }

    internal class Model_StringFixedWidth
    {
        [StringField(0, 3)]
        public string Text { get; set; } = "AB";
    }

    internal class Model_FloatingFixedWidth
    {
        [FloatingField(0, 4, 1, true)]
        public float Value { get; set; } = 12.3f;
    }

    /// <summary>TextForTrue más corto que el campo: ValidateFieldDefinition falla.</summary>
    internal class Model_BooleanTrueLengthMismatch
    {
        [BooleanField(3, "SI", "NO")]
        public bool Flag { get; set; }
    }

    /// <summary>TextForTrue igual a TextForFalse.</summary>
    internal class Model_BooleanTrueEqualsFalse
    {
        [BooleanField(2, "OK", "OK")]
        public bool Flag { get; set; }
    }

    /// <summary>Formato más largo que el ancho del campo.</summary>
    internal class Model_DateTimeFormatLengthMismatch
    {
        [DateTimeField(0, 6, "yyyyMMdd")]
        public DateTime When { get; set; }
    }
}
