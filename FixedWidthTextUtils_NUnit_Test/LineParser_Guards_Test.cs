using FixedWidthTextUtils;
using FixedWidthTextUtils.Attributes;
using FixedWidthTextUtils.Exceptions;
using FixedWidthTextUtils_NUnit_Test.Models;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace FixedWidthTextUtils_NUnit_Test
{
    [TestFixture]
    internal class LineParser_Guards_Test
    {
        [Test]
        public void TryParse_InvalidLine_ReturnsFalse_AndDefaultResult()
        {
            bool ok = LineParser.TryParse<Client_Simple>("x", out Client_Simple result);

            Assert.IsFalse(ok);
            Assert.IsNull(result);
        }

        [Test]
        public void TryParse_ValidFirstLineFromFixture_ReturnsTrue()
        {
            string path = Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\TestFiles\3ClientesOK.txt");
            Assume.That(File.Exists(path), "Falta el archivo de prueba en TestFiles.");
            string line = File.ReadLines(path).First();

            bool ok = LineParser.TryParse<Client_Simple>(line, out Client_Simple result);

            Assert.IsTrue(ok);
            Assert.IsNotNull(result);
            Assert.AreNotEqual(0L, result.Id);
        }

        [Test]
        public void GetInitializedLine_MixedOrdinalAndPositional_ThrowsSerializeFieldException()
        {
            var model = new Model_MixedOrdinalPositional { OrdinalPart = 1, PositionalPart = 2 };

            Assert.Throws<SerializeFieldException>(() => LineParser.ToTextLine(model));
        }

        [Test]
        public void ToTextLine_IntegerWiderThanField_ThrowsSerializeFieldException()
        {
            var model = new Model_IntegerTooWideForField();

            Assert.Throws<SerializeFieldException>(() => LineParser.ToTextLine(model));
        }

        [Test]
        public void Parse_IntegerFieldOnNullableInt_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                LineParser.Parse<Model_IntegerFieldOnNullableInt>("123"));
        }

        [Test]
        public void Parse_IntegerFieldOnUnsupportedType_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                LineParser.Parse<Model_IntegerFieldOnDecimal>("123"));
        }

        [Test]
        public void Parse_IntegerField_TrimsPaddedSpaces()
        {
            string line = "   42"; // Length 5 field 0..4

            var parsed = LineParser.Parse<Model_IntegerPaddedField>(line);

            Assert.AreEqual(42, parsed.Value);
        }

        [Test]
        public void Parse_FloatingField_TrimsPaddedSpaces()
        {
            // Length 5, 1 decimal: raw long  123 -> 12.3f
            string line = "  123";

            var parsed = LineParser.Parse<Model_FloatingPaddedField>(line);

            Assert.AreEqual(12.3f, parsed.Value, 0.0001f);
        }

        [Test]
        public void ParseOrdinal_SecondLineStillCorrect_AfterParsingFirstLine()
        {
            string path = Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\TestFiles\3ClientesOK.txt");
            Assume.That(File.Exists(path));
            string[] lines = File.ReadAllLines(path);
            Assume.That(lines.Length, Is.GreaterThanOrEqualTo(2));

            Client_OnlyOrdinal first = LineParser.Parse<Client_OnlyOrdinal>(lines[0]);
            Client_OnlyOrdinal second = LineParser.Parse<Client_OnlyOrdinal>(lines[1]);

            Assert.AreEqual(123456789L, first.Id);
            Assert.AreEqual(222222222L, second.Id);
        }

        [Test]
        public void TryParse_InvalidIntegerNullableDefinition_ReturnsFalse_DoesNotThrow()
        {
            bool ok = LineParser.TryParse<Model_IntegerFieldOnNullableInt>("123", out Model_IntegerFieldOnNullableInt result);

            Assert.IsFalse(ok);
            Assert.IsNull(result);
        }

        [Test]
        public void TryParse_InvalidIntegerOnDecimalDefinition_ReturnsFalse_DoesNotThrow()
        {
            bool ok = LineParser.TryParse<Model_IntegerFieldOnDecimal>("12", out Model_IntegerFieldOnDecimal result);

            Assert.IsFalse(ok);
            Assert.IsNull(result);
        }

        [Test]
        public void Parse_EmptyInput_ThrowsParseFieldException()
        {
            Assert.Throws<ParseFieldException>(() => LineParser.Parse<Client_Simple>(string.Empty));
        }

        [Test]
        public void TryParse_NullInput_ReturnsFalse_DoesNotThrow()
        {
            bool ok = LineParser.TryParse<Client_Simple>(null, out Client_Simple result);

            Assert.IsFalse(ok);
            Assert.IsNull(result);
        }

        [Test]
        public void ToTextLine_IntegerNegativeWithZeroFill_HasExactFieldLength()
        {
            var model = new Model_IntNegativeZeroPad();
            string line = LineParser.ToTextLine(model);
            Assert.AreEqual(5, line.Length);
            Assert.AreEqual("-0005", line);
        }

        [Test]
        public void ToTextLine_IntegerSpacePad_HasExactFieldLength()
        {
            var model = new Model_IntSpacePadNoZero();
            string line = LineParser.ToTextLine(model);
            Assert.AreEqual(3, line.Length);
            Assert.AreEqual(" 42", line);
        }

        [Test]
        public void ToTextLine_IntegerExactWidth_NoPaddingNeeded()
        {
            var model = new Model_IntExactWidthFiveDigits();
            string line = LineParser.ToTextLine(model);
            Assert.AreEqual(5, line.Length);
            Assert.AreEqual("12345", line);
        }

        [Test]
        public void ToTextLine_StringField_HasExactDeclaredLength()
        {
            var model = new Model_StringFixedWidth();
            string line = LineParser.ToTextLine(model);
            Assert.AreEqual(4, line.Length);
            Assert.AreEqual("AB  ", line);
        }

        [Test]
        public void ToTextLine_FloatingField_HasExactDeclaredLength()
        {
            var model = new Model_FloatingFixedWidth();
            string line = LineParser.ToTextLine(model);
            Assert.AreEqual(5, line.Length);
        }

        [Test]
        public void Parse_BooleanFieldDefinition_TextForTrueLengthMismatch_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => LineParser.Parse<Model_BooleanTrueLengthMismatch>("SI "));
        }

        [Test]
        public void Parse_BooleanFieldDefinition_TextForTrueEqualsFalse_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => LineParser.Parse<Model_BooleanTrueEqualsFalse>("OK"));
        }

        [Test]
        public void Parse_DateTimeFieldDefinition_FormatLengthMismatch_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => LineParser.Parse<Model_DateTimeFormatLengthMismatch>("20220101"));
        }
    }
}
