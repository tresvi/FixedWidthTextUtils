using FixedWidthTextUtils;
using FixedWidthTextUtils_NUnit_Test.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
    }
}
