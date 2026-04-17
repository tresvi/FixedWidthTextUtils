using FixedWidthTextUtils;
using FixedWidthTextUtils.Attributes;
using FixedWidthTextUtils.Exceptions;
using FixedWidthTextUtils_NUnit_Test.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FixedWidthTextUtils_NUnit_Test
{
    [TestFixture]
    internal class Coverage_FileParser_and_StringTests
    {
        [Test]
        public void FileParser_Parse_ignoreWrongLines_records_InvalidLines_on_bad_row()
        {
            string path = Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\TestFiles\4Clientes_3roConError.txt");
            Assume.That(File.Exists(path));

            var fp = new FileParser(path);
            List<Client_Simple> ok = fp.Parse<Client_Simple>(ignoreWrongLines: true);

            Assert.That(ok.Count, Is.EqualTo(3));
            Assert.That(fp.InvalidLines.Count, Is.EqualTo(1));
            Assert.That(fp.InvalidLines[0].Number, Is.EqualTo(3));
        }

        [Test]
        public void FileParser_Parse_empty_path_throws_wrapped_access_exception()
        {
            var fp = new FileParser(string.Empty);
            Exception? ex = Assert.Throws<Exception>(() => fp.Parse<Client_Simple>(false));
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.Message, Does.Contain("Error al acceder al archivo"));
            Assert.That(ex.InnerException, Is.Not.Null);
        }

        [Test]
        public void FileParser_ToFlatFile_when_line_serialize_throws_wraps_in_SerializeFieldException()
        {
            string path = Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\TestFiles\3ClientesOK.txt");
            Assume.That(File.Exists(path));
            string outPath = Path.Combine(Path.GetTempPath(), "fw_bad_ser_" + Guid.NewGuid().ToString("N") + ".txt");
            try
            {
                var fp = new FileParser(path);
                var badList = new List<Model_IntegerTooWideForField> { new Model_IntegerTooWideForField() };

                var ex = Assert.Throws<SerializeFieldException>(() => fp.ToFlatFile(badList, outPath));
                Assert.That(ex, Is.Not.Null);
                Assert.That(ex!.Message, Does.Contain("serialización del entero"));
            }
            finally
            {
                if (File.Exists(outPath))
                    File.Delete(outPath);
            }
        }

        [Test]
        public void FileParserWithFooter_Parse_empty_path_throws_wrapped_exception()
        {
            var fp = new FileParserWithFooter<Footer_Client>(string.Empty);
            Assert.Throws<Exception>(() => fp.Parse<Client_Simple>(false, out _));
        }

        [Test]
        public void Parse_BooleanField_OnBoolNullableProperty_ThrowsParseFieldException()
        {
            Assert.Throws<ParseFieldException>(() =>
                LineParser.Parse<Model_BooleanOnBoolNullable>("SI"));
        }

        [Test]
        public void Parse_StringField_TrimMode_NoTrim()
        {
            var m = LineParser.Parse<Model_StringTrimNo>("  AB");
            Assert.AreEqual("  AB", m.S);
        }

        [Test]
        public void Parse_StringField_TrimMode_Trim()
        {
            var m = LineParser.Parse<Model_StringTrimAll>("  AB  ");
            Assert.AreEqual("AB", m.S);
        }

        [Test]
        public void Parse_StringField_TrimMode_TrimStart()
        {
            var m = LineParser.Parse<Model_StringTrimStart>("  AB");
            Assert.AreEqual("AB", m.S);
        }

        [Test]
        public void Parse_StringField_TrimMode_TrimEnd()
        {
            var m = LineParser.Parse<Model_StringTrimEnd>("AB  ");
            Assert.AreEqual("AB", m.S);
        }

        [Test]
        public void ToTextLine_StringField_LeftPadding_true()
        {
            var m = new Model_StringLeftPad { S = "XY" };
            string line = LineParser.ToTextLine(m);
            Assert.AreEqual(4, line.Length);
            Assert.AreEqual("  XY", line);
        }

        [Test]
        public void ToTextLine_StringField_on_non_string_property_ThrowsSerializeFieldException()
        {
            var m = new Model_StringOnInt { X = 1 };
            Assert.Throws<SerializeFieldException>(() => LineParser.ToTextLine(m));
        }
    }
}
