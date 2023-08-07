using FixedWidthTextUtils;
using FixedWidthTextUtils.Exceptions;
using FixedWidthTextUtils_NUnit_Test.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
    }
}
