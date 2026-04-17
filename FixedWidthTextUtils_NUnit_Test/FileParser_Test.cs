using FixedWidthTextUtils;
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
    internal class FileParser_Test
    {

        [TestCase(@".\..\..\..\TestFiles\3ClientesOK.txt", 3, new long[] {})]
        [TestCase(@".\..\..\..\TestFiles\4Clientes_3roConError.txt", 3, new long[] { 3 })]
        public void ParseFile_InputIgnoreWrongLines(string filePath, int expectedTotalLinesOK, long[] expectedNumbersOfFailedLines)
        {
            FileParser fileConvert = new(filePath);
            List<Client_Simple> clientes = fileConvert.Parse<Client_Simple>(true);

            Assert.AreEqual(expectedTotalLinesOK, clientes.Count);
            Assert.AreEqual(expectedNumbersOfFailedLines.Length, fileConvert.InvalidLines.Count);

            long[] numbersOfFailedLines = fileConvert.InvalidLines.Select(x => x.Number).ToArray();
            Assert.That(numbersOfFailedLines, Is.EquivalentTo(expectedNumbersOfFailedLines));
        }


        [TestCase(@".\..\..\..\TestFiles\3ClientesOK.txt", 3, new long[] { })]
        [TestCase(@".\..\..\..\TestFiles\4Clientes_3roConError.txt", 3, new long[] { 3 })]
        public void ParseFile_WithClientOrdinal_InputIgnoreWrongLines(string filePath, int expectedTotalLinesOK, long[] expectedNumbersOfFailedLines)
        {
            FileParser fileConvert = new(filePath);
            List<Client_OnlyOrdinal> clientes = fileConvert.Parse<Client_OnlyOrdinal>(true);

            Assert.AreEqual(expectedTotalLinesOK, clientes.Count);
            Assert.AreEqual(expectedNumbersOfFailedLines.Length, fileConvert.InvalidLines.Count);

            long[] numbersOfFailedLines = fileConvert.InvalidLines.Select(x => x.Number).ToArray();
            Assert.That(numbersOfFailedLines, Is.EquivalentTo(expectedNumbersOfFailedLines));
        }


        [TestCase(@".\..\..\..\TestFiles\3ClientesOK.txt", 3, new long[] { })]
        [TestCase(@".\..\..\..\TestFiles\4Clientes_3roConError.txt", 3, new long[] { 3 })]
        public void ParseFile_WithClientPosAndOrdinal_InputIgnoreWrongLines(string filePath, int expectedTotalLinesOK, long[] expectedNumbersOfFailedLines)
        {
            FileParser fileConvert = new(filePath);
            List<Client_PositionalAndOrdinal> clientes = fileConvert.Parse<Client_PositionalAndOrdinal>(true);

            Assert.AreEqual(expectedTotalLinesOK, clientes.Count);
            Assert.AreEqual(expectedNumbersOfFailedLines.Length, fileConvert.InvalidLines.Count);

            long[] numbersOfFailedLines = fileConvert.InvalidLines.Select(x => x.Number).ToArray();
            Assert.That(numbersOfFailedLines, Is.EquivalentTo(expectedNumbersOfFailedLines));
        }


        [TestCase(@".\..\..\..\TestFiles\3ClientesOK.txt")]
        public void ToFlatFile_ClosedLoopAgainstParseOK(string filePath)
        {
            const string OUTPUT_FILE = "TempOutput.txt";
            FileParser fileConvert = new(filePath);
            List<Client_Simple> clientes = fileConvert.Parse<Client_Simple>(false);
            fileConvert.ToFlatFile(clientes, OUTPUT_FILE);

            bool fileComparison = File.ReadLines(filePath).SequenceEqual(File.ReadLines(OUTPUT_FILE));
            Assert.IsTrue(fileComparison);
            File.Delete(OUTPUT_FILE);
        }


        [TestCase]
        public void ParseFile_InputNonExistentFile_ThrowsIOException()
        {
            string filePath = new Guid().ToString() + ".txt";
            FileParser fileConvert = new(filePath);
            Assert.That(() =>
                fileConvert.Parse<Client_Simple>(false),
                Throws.InstanceOf<IOException>()
            );
        }


        [Test]
        public void ToFlatFile_WritesUsingParserEncoding_IncludingUtf8BomWhenConfigured()
        {
            string src = Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\TestFiles\3ClientesOK.txt");
            Assume.That(File.Exists(src), "Falta TestFiles/3ClientesOK.txt");

            var encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
            string outPath = Path.Combine(Path.GetTempPath(), "FixedWidthTextUtils_enc_" + Guid.NewGuid().ToString("N") + ".txt");

            try
            {
                var fileConvert = new FileParser(src, encoding);
                List<Client_Simple> clientes = fileConvert.Parse<Client_Simple>(false);
                fileConvert.ToFlatFile(clientes, outPath);

                byte[] bytes = File.ReadAllBytes(outPath);
                Assert.That(bytes.Length, Is.GreaterThanOrEqualTo(3), "Archivo vacío o demasiado corto.");
                Assert.AreEqual(0xEF, bytes[0]);
                Assert.AreEqual(0xBB, bytes[1]);
                Assert.AreEqual(0xBF, bytes[2], "ToFlatFile debe usar Encoding del FileParser (UTF-8 con BOM en esta prueba).");
            }
            finally
            {
                if (File.Exists(outPath))
                    File.Delete(outPath);
            }
        }

    }
}
