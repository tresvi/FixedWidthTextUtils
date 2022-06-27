using FixedWidthTextUtils;
using FixedWidthTextUtils.Exceptions;
using FixedWidthTextUtils_NUnit_Test.Models;
using FixedWidthTextUtils_NUnit_Test.Models;
using NUnit.Framework;
using System;

namespace FixedWidthTextUtils_NUnit_Test
{
    [TestFixture]
    internal class RegisterUtiliy_Exceptions_Test
    {
        //TODO: Agregar excepciones de numericos fuera de rango. Sobre todo los destinos de tipos enteros.

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Client_Throws_NonStringeable()
        {
            string inputLine = "012345678A20221229023";
            Client_NonStringeable parsedClient = RegisterConvert.Parse<Client_NonStringeable>(inputLine);
            FileConvert fileConvert = new(@".\..\..\..\TestFiles\3ClientesOK.txt");

            Assert.Multiple(() =>
            {
                Assert.Throws<NonStringeableClassException>(() => RegisterConvert.ToFlatLine(parsedClient));
                Assert.Throws<NonStringeableClassException>(() => fileConvert.ParseFile<Client_NonStringeable>(true));
            });
        }

        [Test]
        public void Client_Throws_NonStringeable2()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<ParseFieldException>(() => RegisterConvert.Parse<Client_With_Err_DateTime_Prop>("012320221229"));
                ParseFieldException? parseFieldException = Assert.Throws<ParseFieldException>(() => RegisterConvert.Parse<Client_With_Priv_Method>("012345678A202212290r3"));
                Assert.That(parseFieldException?.Message, Does.Contain("numerico"));

                parseFieldException = Assert.Throws<ParseFieldException>(() => RegisterConvert.Parse<Client_With_Priv_Method>("0a2345678A20221229023"));
                Assert.That(parseFieldException?.Message, Does.Contain("entero"));

                parseFieldException = Assert.Throws<ParseFieldException>(() => RegisterConvert.Parse<Client_With_Priv_Method>("012345678A202A1229023"));
                Assert.That(parseFieldException?.Message, Does.Contain("fecha"));

            });
        }

    }
}
