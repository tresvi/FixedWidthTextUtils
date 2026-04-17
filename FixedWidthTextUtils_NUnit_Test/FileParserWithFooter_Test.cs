using FixedWidthTextUtils;
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
    internal class FileParserWithFooter_Test
    {
        [TestCase(@".\..\..\..\TestFilesWithFooter\3ClientesOK_FooterOK.txt", 3, new long[] { })]
        public void ParseFile_InputClientsOkFooterOk(string filePath, int expectedTotalLinesOK, long[] expectedNumbersOfFailedLines)
        {
            FileParserWithFooter<Footer_Client> fileConvert = new(filePath);
            Footer_Client footer_client;
            List<Client_Simple> clientes = fileConvert.Parse<Client_Simple>(false, out footer_client);

            Assert.AreEqual(expectedTotalLinesOK, clientes.Count);
            Assert.AreEqual(expectedNumbersOfFailedLines.Length, fileConvert.InvalidLines.Count);
            Assert.AreEqual(clientes.Sum(x => x.Id), footer_client.Total);
        }


        [TestCase(@".\..\..\..\TestFilesWithFooter\3ClientesOK_FooterNoOK.txt")]
        public void ParseFile_InputClientsOKFooterNoOK(string filePath)
        {
            FileParserWithFooter<Footer_Client> fileConvert = new(filePath);
            Footer_Client footer_client;
            Assert.That(() =>
                fileConvert.Parse<Client_Simple>(false, out footer_client),
                Throws.InstanceOf<ParseFieldException>().With.Message.Contains("no puede ser reconocido")
            );
        }


        [TestCase(@".\..\..\..\TestFilesWithFooter\3ClientesOK_SinFooter.txt")]
        public void ParseFile_InputClientsOKWithoutFooter(string filePath)
        {
            FileParserWithFooter<Footer_Client> fileConvert = new(filePath);
            Footer_Client footer_client;
            Assert.That(() =>
                fileConvert.Parse<Client_Simple>(false, out footer_client),
                Throws.InstanceOf<ParseFieldException>().With.Message.Contains("no puede ser reconocido")
            );
        }


        [TestCase(@".\..\..\..\TestFilesWithFooter\ArchivoVacio.txt")]
        public void ParseFile_EmptyFile(string filePath)
        {
            FileParserWithFooter<Footer_Client> fileConvert = new(filePath);
            Footer_Client footer_client;
            Assert.That(() =>
                fileConvert.Parse<Client_Simple>(false, out footer_client),
                Throws.InstanceOf<ParseFieldException>().With.Message.Contains("La linea a parsear es EMPTY")
            );
        }


        [TestCase(@".\..\..\..\TestFilesWithFooter\0Clientes_SoloFooter.txt")]
        public void ParseFile_InputOnlyFooter(string filePath)
        {
            FileParserWithFooter<Footer_Client> fileConvert = new(filePath);
            Footer_Client footer_client;
            List<Client_Simple> clientes = fileConvert.Parse<Client_Simple>(false, out footer_client);

            Assert.AreEqual(clientes.Sum(x => x.Id), 0);
        }

        [TestCase(@".\..\..\..\TestFilesWithFooter\3ClientesOK_FooterOK.txt")]
        public void ToFlatFile_ClosedLoopAgainstParseOK(string filePath)
        {
            const string OUTPUT_FILE = "TempOutput.txt";
            FileParserWithFooter<Footer_Client> fileConvert = new(filePath);
            Footer_Client footer_client;
            List<Client_Simple> clientes = fileConvert.Parse<Client_Simple>(false, out footer_client);
            fileConvert.ToFlatFile(clientes, footer_client, OUTPUT_FILE);

            bool fileComparison = File.ReadLines(filePath).SequenceEqual(File.ReadLines(OUTPUT_FILE));
            Assert.IsTrue(fileComparison);
            File.Delete(OUTPUT_FILE);
        }

        [TestCase]
        public void ParseFile_InputNonExistentFile_ThrowsIOException()
        {
            string filePath = new Guid().ToString() + ".txt";
            FileParserWithFooter<Footer_Client> fileConvert = new(filePath);
            Footer_Client footer_client;
            Assert.That(() =>
                fileConvert.Parse<Client_Simple>(false, out footer_client),
                Throws.InstanceOf<IOException>()
            );
        }

        [Test]
        public void Footer_property_get_and_set_round_trips_value()
        {
            var fileConvert = new FileParserWithFooter<Footer_Client>("unused-path-for-this-test");
            var footer = new Footer_Client { Total = 12345 };
            fileConvert.Footer = footer;
            Assert.That(fileConvert.Footer.Total, Is.EqualTo(12345));
        }

        [Test]
        public void ToFlatFile_when_output_path_is_directory_wraps_access_failure_in_SerializeFieldException()
        {
            string dir = Path.Combine(Path.GetTempPath(), "fw_footer_dir_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            try
            {
                var fileConvert = new FileParserWithFooter<Footer_Client>("unused-path-for-this-test");
                var footer = new Footer_Client();
                var ex = Assert.Throws<SerializeFieldException>(() =>
                    fileConvert.ToFlatFile(new List<Client_Simple>(), footer, dir));
                Assert.That(ex!.Message, Does.Contain("Error generar archivo de texto"));
                Assert.That(ex.InnerException, Is.Not.Null);
                Assert.That(
                    ex.InnerException is IOException || ex.InnerException is UnauthorizedAccessException,
                    Is.True,
                    $"Se esperaba IOException o UnauthorizedAccessException como causa interna; recibido {ex.InnerException?.GetType().Name}.");
            }
            finally
            {
                try { Directory.Delete(dir); } catch { /* ignore */ }
            }
        }

        [Test]
        public void ToFlatFile_when_entity_serialize_fails_rethrows_SerializeFieldException()
        {
            string outPath = Path.Combine(Path.GetTempPath(), "fw_footer_ser_" + Guid.NewGuid().ToString("N") + ".txt");
            try
            {
                var fileConvert = new FileParserWithFooter<Footer_Client>("unused-path-for-this-test");
                var footer = new Footer_Client();
                Assert.That(() => fileConvert.ToFlatFile(
                        new List<Model_IntegerTooWideForField> { new Model_IntegerTooWideForField() },
                        footer,
                        outPath),
                    Throws.InstanceOf<SerializeFieldException>().With.Message.Contains("serialización del entero"));
            }
            finally
            {
                if (File.Exists(outPath))
                    File.Delete(outPath);
            }
        }

        [Test]
        public void ToFlatFile_when_output_path_empty_wraps_in_SerializeFieldException()
        {
            var fileConvert = new FileParserWithFooter<Footer_Client>("unused-path-for-this-test");
            var footer = new Footer_Client();
            var ex = Assert.Throws<SerializeFieldException>(() =>
                fileConvert.ToFlatFile(new List<Client_Simple>(), footer, string.Empty));
            Assert.That(ex!.Message, Does.Contain("Error generar archivo de texto"));
            Assert.That(ex.InnerException, Is.Not.Null);
        }


        [Test]
        public void ToFlatFile_WithFooter_UsesParserEncoding_IncludingUtf8BomWhenConfigured()
        {
            string src = Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\TestFilesWithFooter\3ClientesOK_FooterOK.txt");
            Assume.That(File.Exists(src), "Falta TestFilesWithFooter/3ClientesOK_FooterOK.txt");

            var encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
            string outPath = Path.Combine(Path.GetTempPath(), "FixedWidthTextUtils_footer_enc_" + Guid.NewGuid().ToString("N") + ".txt");

            try
            {
                var fileConvert = new FileParserWithFooter<Footer_Client>(src, encoding);
                Footer_Client footer;
                List<Client_Simple> clientes = fileConvert.Parse<Client_Simple>(false, out footer);
                fileConvert.ToFlatFile(clientes, footer, outPath);

                byte[] bytes = File.ReadAllBytes(outPath);
                Assert.That(bytes.Length, Is.GreaterThanOrEqualTo(3));
                Assert.AreEqual(0xEF, bytes[0]);
                Assert.AreEqual(0xBB, bytes[1]);
                Assert.AreEqual(0xBF, bytes[2]);
            }
            finally
            {
                if (File.Exists(outPath))
                    File.Delete(outPath);
            }
        }
    }
}
