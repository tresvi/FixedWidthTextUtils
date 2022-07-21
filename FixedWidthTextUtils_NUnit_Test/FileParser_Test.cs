using FixedWidthTextUtils;
using FixedWidthTextUtils_NUnit_Test.Models;
using NUnit.Framework;
using System.Collections.Generic;
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
            List<ClienteSimple> clientes = fileConvert.Parse<ClienteSimple>(true);

            Assert.AreEqual(expectedTotalLinesOK, clientes.Count);
            Assert.AreEqual(expectedNumbersOfFailedLines.Length, fileConvert.InvalidLines.Count);

            long[] numbersOfFailedLines = fileConvert.InvalidLines.Select(x => x.Number).ToArray();
            Assert.That(numbersOfFailedLines, Is.EquivalentTo(expectedNumbersOfFailedLines));
            
        }

    }
}
