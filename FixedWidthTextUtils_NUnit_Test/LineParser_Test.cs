using NUnit.Framework;
using FixedWidthTextUtils.Attributes;
using FixedWidthTextUtils_NUnit_Test.Models;
using System;
using System.Reflection;
using FixedWidthTextUtils;

namespace FixedWidthTextUtils_NUnit
{
    [TestFixture]
    public class LineParser_Test
    {
        private static Cliente cliente1 = new();
        private const string inputLine1 = "123456789ANed Flanders        WhateverStreet      04566 2222Brussels      Belgium        1981122918091991991991920211229101112    2021013010111220210131101112    -0000123451111111111-92222222222299999999999990001234525  123   123456729121981";
        
        private Cliente cliente2 = new();
        private const string inputLine2 = "222222222AHomero Simpson      WhateverStreet      00117 2222Brussels      Belgium        1981122918091191191191120211229191112    2021013019111220210131191112    -0000123451111111111-9222222222229999999999999-001234525 -123  -123456731121981";

        [SetUp]
        public void Setup()
        {
            cliente1 = new();
            cliente1.Id = 123456789;
            cliente1.Code = "A";
            cliente1.Name = "Ned Flanders";
            cliente1.Street = "WhateverStreet      ";
            cliente1.HouseNumber = 4566;
            cliente1.PostCode = "2222";
            cliente1.City = "Brussels";
            cliente1.Country = "Belgium";
            cliente1.BirthDate = new DateTime(1981, 12, 29);
            cliente1.HeigthInCentimeters = 180;
            cliente1.WeightFloat = 91.9f;
            cliente1.WeightSingle = 91.9f;
            cliente1.WeightDouble = 91.9d;
            cliente1.WeightDecimal = 91.9M;
            cliente1.AnotherDateTime = new DateTime(2021, 12, 29, 10, 11, 12);
            cliente1.PadRightDateTime = new DateTime(2021, 01, 30, 10, 11, 12);
            cliente1.PadLeftDateTime = new DateTime(2021, 01, 31, 10, 11, 12);
            cliente1.CampoIntNegativo = -12345;
            cliente1.CampoUInt = 1111111111;
            cliente1.CampoLong = -922222222222;
            cliente1.CampoULong = 9999999999999;
            cliente1.OtroCampoFlotante = 12345.25f;
            cliente1.CampoIntegerConEspacios = 123;
            cliente1.CampoFlotanteConEspacios = 12345.67f;
            cliente1.OtroDateTime = new DateTime(1981, 12, 29);

            cliente2 = new();
            cliente2.Id = 222222222;
            cliente2.Code = "A";
            cliente2.Name = "Homero Simpson";
            cliente2.Street = "WhateverStreet      ";
            cliente2.HouseNumber = 117;
            cliente2.PostCode = "2222";
            cliente2.City = "Brussels";
            cliente2.Country = "Belgium";
            cliente2.BirthDate = new DateTime(1981, 12, 29);
            cliente2.HeigthInCentimeters = 180;
            cliente2.WeightFloat = 91.1f;
            cliente2.WeightSingle = 91.1f;
            cliente2.WeightDouble = 91.1d;
            cliente2.WeightDecimal = 91.1M;
            cliente2.AnotherDateTime = new DateTime(2021, 12, 29, 19, 11, 12);
            cliente2.PadRightDateTime = new DateTime(2021, 01, 30, 19, 11, 12);
            cliente2.PadLeftDateTime = new DateTime(2021, 01, 31, 19, 11, 12);
            cliente2.CampoIntNegativo = -12345;
            cliente2.CampoUInt = 1111111111;
            cliente2.CampoLong = -922222222222;
            cliente2.CampoULong = 9999999999999;
            cliente2.OtroCampoFlotante = -12345.25f;
            cliente2.CampoIntegerConEspacios = -123;
            cliente2.CampoFlotanteConEspacios = -12345.67f;
            cliente2.OtroDateTime = new DateTime(1981, 12, 31);
        }


        [TestCase(inputLine1, 1)]
        [TestCase(inputLine2, 2)]
        public void Parse_InputString_ReturnCliente(string inputLine, int NroCliente)
        {
            //arrrange
            Cliente clienteEsperado;

            if (NroCliente == 1)
                clienteEsperado = cliente1;
            else if (NroCliente == 2)
                clienteEsperado = cliente2;
            else
                throw new Exception("Nro de Cliente desconocido 1");

            //act
            Cliente clienteParseado = LineParser.Parse<Cliente>(inputLine);

            //assert. Recorre automaticamente todas las propiedades parseables de la clase
            Assert.Multiple(() =>
            {
                foreach (PropertyInfo property in clienteEsperado.GetType().GetProperties())
                {
                    foreach (Attribute attribute in property.GetCustomAttributes(true))
                    {
                        if (!(attribute is FieldAttribute)) continue;
                        Assert.AreEqual(property.GetValue(clienteEsperado), property.GetValue(clienteParseado));
                    }
                }
            });
        }


        [TestCase(inputLine1)]
        [TestCase(inputLine2)]
        public void ToFlatLine2_InputString_ReturnString(string inputLine)
        {
            //arrrange

            //act
            Cliente clienteParseado = LineParser.Parse<Cliente>(inputLine);
            string outputLine = LineParser.ToFlatLine(clienteParseado);

            //asssert
            Assert.AreEqual(inputLine, outputLine);
        }


        [TestCase("012345678A20221229023")]
        public void ToFlatLine2_InputString_AssignInInternalAndPrivMethods(string inputLine)
        {
            //arrrange

            //act
            Client_With_Priv_Method clienteParseado = LineParser.Parse<Client_With_Priv_Method>(inputLine);
            string outputLine = LineParser.ToFlatLine(clienteParseado);

            //asssert
            Assert.AreEqual(inputLine, outputLine);
        }


        //TODO: Metodos de prueba de:
        //Agregar properties  char, uchar y verlo fallar
        //-TryParse
        //-Todas las exceptiones custom de Parse().
        //-Todas las exceptiones custom de ToFlatLine2().
    }
}
