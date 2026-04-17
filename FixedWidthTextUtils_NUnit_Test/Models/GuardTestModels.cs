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

    /// <summary>TextForFalse vacío: todo lo distinto de TextForTrue se interpreta como false.</summary>
    internal class Model_BooleanTextForFalseEmpty
    {
        [BooleanField(3, "YES", "")]
        public bool Flag { get; set; }
    }

    /// <summary>BooleanField en propiedad no bool (Parse y ToText fallan).</summary>
    internal class Model_BooleanOnIntProperty
    {
        [BooleanField(2, "SI", "NO")]
        public int NotBool { get; set; }
    }

    internal class Model_BooleanNative
    {
        [BooleanField(2, "SI", "NO")]
        public bool Flag { get; set; } = true;
    }

    internal class Model_NullableStringOrdinal
    {
        [NullableStringField(4, "~~~~", StringFieldAttribute.TrimMode.NoTrim)]
        public string? Text { get; set; }
    }

    internal class Model_NullableStringOnIntProperty
    {
        [NullableStringField(2, "~~")]
        public int? Bad { get; set; }
    }

    internal class Model_NullableFloatOnIntNullable
    {
        [NullableFloatingField(5, 1, "     ")]
        public int? Value { get; set; }
    }

    internal class Model_NullableFloatTextForNullLengthMismatch
    {
        [NullableFloatingField(5, 1, "XX")]
        public float? Value { get; set; }
    }

    internal class Model_NullableDateTimeTextForNullLengthMismatch
    {
        [NullableDateTimeField(8, "yyyyMMdd", "XX")]
        public DateTime? When { get; set; }
    }

    internal class Model_NullableDateTimeOnNonNullableDateTime
    {
        [NullableDateTimeField(8, "yyyyMMdd", "xxxxxxxx")]
        public DateTime When { get; set; }
    }

    internal class Model_NullableBooleanOneCharTriple
    {
        [NullableBooleanField(1, "Y", "N", "?")]
        public bool? Flag { get; set; }
    }

    internal class Model_NullableBooleanTextForNullLengthMismatch
    {
        [NullableBooleanField(1, "Y", "N", "??")]
        public bool? Flag { get; set; }
    }

    internal class Model_BooleanOnBoolNullable
    {
        [BooleanField(2, "SI", "NO")]
        public bool? Flag { get; set; }
    }

    internal class Model_StringTrimNo
    {
        [StringField(4, StringFieldAttribute.TrimMode.NoTrim)]
        public string S { get; set; } = "";
    }

    internal class Model_StringTrimAll
    {
        [StringField(4, StringFieldAttribute.TrimMode.Trim)]
        public string S { get; set; } = "";
    }

    internal class Model_StringTrimStart
    {
        [StringField(4, StringFieldAttribute.TrimMode.TrimStart)]
        public string S { get; set; } = "";
    }

    internal class Model_StringTrimEnd
    {
        [StringField(4, StringFieldAttribute.TrimMode.TrimEnd)]
        public string S { get; set; } = "";
    }

    internal class Model_StringLeftPad
    {
        [StringField(4, StringFieldAttribute.TrimMode.NoTrim, true)]
        public string S { get; set; } = "";
    }

    internal class Model_StringOnInt
    {
        [StringField(2)]
        public int X { get; set; }
    }

    internal class Model_DateTimeSingle
    {
        [DateTimeField(8, "yyyyMMdd")]
        public DateTime When { get; set; }
    }

    internal class Model_BooleanNativeOrdinal
    {
        [BooleanField(2, "SI", "NO")]
        public bool Flag { get; set; }
    }

    internal class Model_FloatingDouble
    {
        [FloatingField(5, 2, true)]
        public double Value { get; set; } = 12.34;
    }

    internal class Model_FloatingDecimal
    {
        [FloatingField(5, 2, false)]
        public decimal Value { get; set; } = 99.99m;
    }

    /// <summary>TextForNull más corto que el ancho del campo entero nullable.</summary>
    internal class Model_NullableIntegerTextForNullLengthMismatch
    {
        [NullableIntegerField(3, "XX")]
        public int? Value { get; set; }
    }

    /// <summary>TextForFalse más corto que el ancho del campo (y no vacío): ValidateFieldDefinition.</summary>
    internal class Model_BooleanFalseLengthMismatch
    {
        [BooleanField(3, "YES", "N")]
        public bool Flag { get; set; }
    }

    /// <summary>BooleanField ctor posicional (start, end).</summary>
    internal class Model_BooleanPositional
    {
        [BooleanField(0, 1, "SI", "NO")]
        public bool Flag { get; set; }
    }

    /// <summary>NullableStringField ctor posicional (start, end).</summary>
    internal class Model_NullableStringPositional
    {
        [NullableStringField(0, 3, "NULL", StringFieldAttribute.TrimMode.TrimEnd, false)]
        public string? Text { get; set; }
    }

    /// <summary>NullableFloatingField ctor posicional (start, end).</summary>
    internal class Model_NullableFloatingPositional
    {
        [NullableFloatingField(0, 4, 2, "-----", true)]
        public double? Value { get; set; }
    }

    internal class Model_IntegerField_Byte { [IntegerField(3, true)] public byte V { get; set; } }
    internal class Model_IntegerField_SByte { [IntegerField(3, true)] public sbyte V { get; set; } }
    internal class Model_IntegerField_UShort { [IntegerField(3, true)] public ushort V { get; set; } }
    internal class Model_IntegerField_UInt { [IntegerField(5, true)] public uint V { get; set; } }
    internal class Model_IntegerField_ULong { [IntegerField(5, true)] public ulong V { get; set; } }
    internal class Model_IntegerField_Long { [IntegerField(8, true)] public long V { get; set; } }
}
