using FixedWidthTextUtils;
using FixedWidthTextUtils.Exceptions;
using FixedWidthTextUtils_NUnit_Test.Models;
using NUnit.Framework;
using System;

namespace FixedWidthTextUtils_NUnit_Test
{
    [TestFixture]
    internal class LineParser_Nullables_Test
    {

        //TODO: Agregar test de ida y vuelta

        [TestCase("SINO  HELLO    1234        30122023")]
        public void Parse_NullableBoolean_OK(string inputLine)
        {
            //arrange

            //act
            OrdinalNullables parsedObject = LineParser.Parse<OrdinalNullables>(inputLine);

            //assert
            Assert.AreEqual(true, parsedObject.BooleanTrue);
            Assert.AreEqual(false, parsedObject.BooleanFalse);
            Assert.AreEqual(null, parsedObject.BooleanNull);
            Assert.AreEqual("HELLO", parsedObject.SimpleString);
            Assert.AreEqual(null, parsedObject.IntegerNull);
            Assert.AreEqual(1234, parsedObject.IntegerNotNull);
            Assert.AreEqual(null , parsedObject.DateTimeNull);
            Assert.AreEqual(new DateTime(2023, 12, 30), parsedObject.DateTimeNotNull);
        }


        [TestCase("SINOX HELLO")]
        public void Parse_NullableBoolean_ThrowExceptions(string inputLine)
        {
            //arrange

            //act
            //assert
            Assert.That(() =>
                LineParser.Parse<OrdinalToFail>(inputLine),
                Throws.InstanceOf<ParseFieldException>().With.Message.Contains("La propiedad de asignacion")
            );

            Assert.That(() =>
                LineParser.Parse<OrdinalNullables>(inputLine),
                Throws.InstanceOf<ParseFieldException>().With.Message.Contains("no puede ser reconocido")
            );
        }


        [TestCase("  01  02  03  04  05  06  07  08  ")]
        public void Parse_IntegerNullables_OK(string inputLine)
        {
            //arrange

            //act
            IntegerOrdinalNullables parsedObject = LineParser.Parse<IntegerOrdinalNullables>(inputLine);

            //assert
            Assert.AreEqual(null, parsedObject.ByteNull);
            Assert.AreEqual(1, parsedObject.ByteNotNull);

            Assert.AreEqual(null, parsedObject.SByteNull);
            Assert.AreEqual(2, parsedObject.SByteNotNull);

            Assert.AreEqual(null, parsedObject.ShortNull);
            Assert.AreEqual(3, parsedObject.ShortNotNull);

            Assert.AreEqual(null, parsedObject.UShortNull);
            Assert.AreEqual(4, parsedObject.UShortNotNull);

            Assert.AreEqual(null, parsedObject.IntNull);
            Assert.AreEqual(5, parsedObject.IntNotNull);

            Assert.AreEqual(null, parsedObject.UIntNull);
            Assert.AreEqual(6, parsedObject.UIntNotNull);

            Assert.AreEqual(null, parsedObject.LongNull);
            Assert.AreEqual(7, parsedObject.LongNotNull);

            Assert.AreEqual(null, parsedObject.ULongNull);
            Assert.AreEqual(8, parsedObject.ULongNotNull);
        }


        [TestCase("    0123    0234    0345    XXXXX")]
        public void Parse_FloatingNullables_OK(string inputLine)
        {
            //arrange
            const double tolerance = 0.01;

            //act
            FloatingOrdinalNullables parsedObject = LineParser.Parse<FloatingOrdinalNullables>(inputLine);

            //assert
            Assert.AreEqual(null, parsedObject.FloatNull);
            Assert.That(1.23d, Is.EqualTo(parsedObject.FloatNotNull).Within(tolerance));

            Assert.AreEqual(null, parsedObject.DoubleNull);
            Assert.That(2.34d, Is.EqualTo(parsedObject.DoubleNotNull).Within(tolerance));

            Assert.AreEqual(null, parsedObject.DecimalNull);
            Assert.That(3.45d, Is.EqualTo(parsedObject.DecimalNotNull).Within(tolerance));
        }


        [TestCase(1, "SINO  HELLO    1234        30122023")]
        [TestCase(2, "  01  02  03  04  05  06  07  08")]
        public void ToTextLine_InputString_ReturnString(int testCase, string inputLine)
        {
            //arrange
            string outputLine = "";

            //act
            if (testCase == 1)
            {
                OrdinalNullables ordinalNullable = LineParser.Parse<OrdinalNullables>(inputLine);
                outputLine = LineParser.ToTextLine(ordinalNullable);
            }
            else if (testCase == 2)
            {
                IntegerOrdinalNullables integreOrdinalNullable = LineParser.Parse<IntegerOrdinalNullables>(inputLine);
                outputLine = LineParser.ToTextLine(integreOrdinalNullable);
            }
            else if (testCase == 3)
            { 
                FloatingOrdinalNullables floatinOrdinalNullable = LineParser.Parse<FloatingOrdinalNullables>(inputLine);
                outputLine = LineParser.ToTextLine(floatinOrdinalNullable);
            }

            //assert
            Assert.AreEqual(inputLine, outputLine);
        }
    }
}
