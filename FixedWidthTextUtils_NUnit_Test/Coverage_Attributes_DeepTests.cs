using FixedWidthTextUtils;
using FixedWidthTextUtils.Attributes;
using FixedWidthTextUtils.Exceptions;
using FixedWidthTextUtils_NUnit_Test.Models;
using NUnit.Framework;
using System;

namespace FixedWidthTextUtils_NUnit_Test
{
    [TestFixture]
    internal class Coverage_Attributes_DeepTests
    {
        [Test]
        public void Parse_BooleanField_TextForFalseEmpty_TrueWhenRawEqualsTextForTrue()
        {
            var parsed = LineParser.Parse<Model_BooleanTextForFalseEmpty>("YES");
            Assert.IsTrue(parsed.Flag);
        }

        [Test]
        public void Parse_BooleanField_TextForFalseEmpty_FalseWhenRawNotEqualsTextForTrue()
        {
            var parsed = LineParser.Parse<Model_BooleanTextForFalseEmpty>("NOX");
            Assert.IsFalse(parsed.Flag);
        }

        [Test]
        public void Parse_BooleanField_OnNonBoolProperty_ThrowsParseFieldException()
        {
            Assert.Throws<ParseFieldException>(() =>
                LineParser.Parse<Model_BooleanOnIntProperty>("SI"));
        }

        [Test]
        public void ToTextLine_BooleanField_OnNonBoolProperty_ThrowsSerializeFieldException()
        {
            var model = new Model_BooleanOnIntProperty { NotBool = 1 };
            Assert.Throws<SerializeFieldException>(() => LineParser.ToTextLine(model));
        }

        [Test]
        public void ToTextLine_BooleanField_NativeBool_serializes_TextForTrue_and_TextForFalse()
        {
            var trueModel = new Model_BooleanNative { Flag = true };
            var falseModel = new Model_BooleanNative { Flag = false };

            Assert.AreEqual("SI", LineParser.ToTextLine(trueModel));
            Assert.AreEqual("NO", LineParser.ToTextLine(falseModel));
        }

        [Test]
        public void FieldAttribute_InvalidStartPosition_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new IntegerFieldAttribute(-1, 5, true));
        }

        [Test]
        public void FieldAttribute_EndBeforeStart_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new IntegerFieldAttribute(5, 3, true));
        }

        [Test]
        public void FloatingFieldAttribute_NegativeDecimalPositions_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new FloatingFieldAttribute(5, -1, true));
        }

        [Test]
        public void Parse_NullableStringField_TextForNull_raw_is_returned_by_base_Parse()
        {
            // NullableStringFieldAttribute.Parse no compara TextForNull; solo ToText devuelve TextForNull cuando la propiedad es null.
            var parsed = LineParser.Parse<Model_NullableStringOrdinal>("~~~~");
            Assert.AreEqual("~~~~", parsed.Text);
        }

        [Test]
        public void Parse_NullableStringField_nonNull_returns_trimmed_value()
        {
            var parsed = LineParser.Parse<Model_NullableStringOrdinal>("abcd");
            Assert.AreEqual("abcd", parsed.Text);
        }

        [Test]
        public void Parse_NullableStringField_OnNonStringProperty_ThrowsParseFieldException()
        {
            Assert.Throws<ParseFieldException>(() =>
                LineParser.Parse<Model_NullableStringOnIntProperty>("12"));
        }

        [Test]
        public void Parse_NullableFloatingField_OnUnsupportedNullableType_ThrowsParseFieldException()
        {
            Assert.Throws<ParseFieldException>(() =>
                LineParser.Parse<Model_NullableFloatOnIntNullable>("12345"));
        }

        [Test]
        public void Parse_NullableFloatingField_TextForNullLengthMismatch_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                LineParser.Parse<Model_NullableFloatTextForNullLengthMismatch>("12345"));
        }

        [Test]
        public void Parse_NullableDateTimeField_TextForNullLengthMismatch_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                LineParser.Parse<Model_NullableDateTimeTextForNullLengthMismatch>("20220101"));
        }

        [Test]
        public void Parse_NullableDateTimeField_OnNonNullableDateTime_ThrowsParseFieldException()
        {
            Assert.Throws<ParseFieldException>(() =>
                LineParser.Parse<Model_NullableDateTimeOnNonNullableDateTime>("20220101"));
        }

        [Test]
        public void Parse_NullableBooleanField_Y_N_and_null_marker()
        {
            Assert.IsTrue(LineParser.Parse<Model_NullableBooleanOneCharTriple>("Y").Flag!.Value);
            Assert.IsFalse(LineParser.Parse<Model_NullableBooleanOneCharTriple>("N").Flag!.Value);
            Assert.IsNull(LineParser.Parse<Model_NullableBooleanOneCharTriple>("?").Flag);
        }

        [Test]
        public void Parse_NullableBooleanField_invalid_raw_ThrowsParseFieldException()
        {
            Assert.Throws<ParseFieldException>(() =>
                LineParser.Parse<Model_NullableBooleanOneCharTriple>("X"));
        }

        [Test]
        public void Parse_NullableBooleanField_TextForNullLengthMismatch_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                LineParser.Parse<Model_NullableBooleanTextForNullLengthMismatch>("Y"));
        }

        [Test]
        public void ToTextLine_NullableBooleanField_null_emits_TextForNull()
        {
            var model = new Model_NullableBooleanOneCharTriple { Flag = null };
            string line = LineParser.ToTextLine(model);
            Assert.AreEqual(1, line.Length);
            Assert.AreEqual("?", line);
        }

        [Test]
        public void ToTextLine_NullableStringField_null_emits_TextForNull()
        {
            var model = new Model_NullableStringOrdinal { Text = null };
            string line = LineParser.ToTextLine(model);
            Assert.AreEqual("~~~~", line);
        }

        [Test]
        public void Parse_BooleanField_unrecognized_token_ThrowsParseFieldException()
        {
            Assert.Throws<ParseFieldException>(() =>
                LineParser.Parse<Model_BooleanNativeOrdinal>("XX"));
        }

        [Test]
        public void Parse_DateTimeField_invalid_format_ThrowsParseFieldException()
        {
            Assert.Throws<ParseFieldException>(() =>
                LineParser.Parse<Model_DateTimeSingle>("abcdefgh"));
        }

        [Test]
        public void RoundTrip_FloatingField_double_ToText_and_Parse()
        {
            var model = new Model_FloatingDouble { Value = 12.34 };
            string line = LineParser.ToTextLine(model);
            Assert.AreEqual(5, line.Length);
            var parsed = LineParser.Parse<Model_FloatingDouble>(line);
            Assert.AreEqual(12.34, parsed.Value, 0.001);
        }

        [Test]
        public void RoundTrip_FloatingField_decimal_ToText_and_Parse()
        {
            var model = new Model_FloatingDecimal { Value = 99.99m };
            string line = LineParser.ToTextLine(model);
            Assert.AreEqual(5, line.Length);
            var parsed = LineParser.Parse<Model_FloatingDecimal>(line);
            Assert.AreEqual(99.99m, parsed.Value);
        }

        [Test]
        public void Parse_BooleanField_SI_maps_true_and_NO_maps_false()
        {
            Assert.IsTrue(LineParser.Parse<Model_BooleanNative>("SI").Flag);
            Assert.IsFalse(LineParser.Parse<Model_BooleanNative>("NO").Flag);
        }

        [Test]
        public void Parse_BooleanField_positional_ctor_SI_NO_roundtrip()
        {
            Assert.IsTrue(LineParser.Parse<Model_BooleanPositional>("SI").Flag);
            Assert.IsFalse(LineParser.Parse<Model_BooleanPositional>("NO").Flag);
            Assert.AreEqual("SI", LineParser.ToTextLine(new Model_BooleanPositional { Flag = true }));
            Assert.AreEqual("NO", LineParser.ToTextLine(new Model_BooleanPositional { Flag = false }));
        }

        [Test]
        public void Parse_BooleanField_TextForFalse_length_mismatch_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                LineParser.Parse<Model_BooleanFalseLengthMismatch>("YES"));
        }

        [Test]
        public void Parse_NullableIntegerField_TextForNull_length_mismatch_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                LineParser.Parse<Model_NullableIntegerTextForNullLengthMismatch>("123"));
        }

        [Test]
        public void ToTextLine_NullableStringField_non_null_uses_base_ToText_padding()
        {
            var model = new Model_NullableStringOrdinal { Text = "ab" };
            string line = LineParser.ToTextLine(model);
            Assert.AreEqual(4, line.Length);
            Assert.AreEqual("ab  ", line);
        }

        [Test]
        public void Parse_and_ToTextLine_NullableStringField_positional_ctor()
        {
            var parsed = LineParser.Parse<Model_NullableStringPositional>("abcd");
            Assert.AreEqual("abcd", parsed.Text);
            string line = LineParser.ToTextLine(new Model_NullableStringPositional { Text = "xy" });
            Assert.AreEqual("xy  ", line);
        }

        [Test]
        public void RoundTrip_NullableFloatingField_positional_ctor_null_marker_and_value()
        {
            string nullLine = LineParser.ToTextLine(new Model_NullableFloatingPositional { Value = null });
            Assert.AreEqual("-----", nullLine);
            Assert.IsNull(LineParser.Parse<Model_NullableFloatingPositional>(nullLine).Value);

            var withValue = new Model_NullableFloatingPositional { Value = 12.34 };
            string valueLine = LineParser.ToTextLine(withValue);
            var roundTrip = LineParser.Parse<Model_NullableFloatingPositional>(valueLine);
            Assert.AreEqual(12.34, roundTrip.Value!.Value, 0.001);
        }
    }
}
