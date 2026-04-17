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
    }
}
