using FixedWidthTextUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PruebaTransaccion
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //const string inputLine = "123456789ANed Flanders        WhateverStreet      04566 2222Brussels      Belgium        1981122918091991991991920211229101112    2021013010111220210131101112    -0000123451111111111-92222222222299999999999990001234525  123   123456729121981";
            //const string inputPersonaFisica = "1234567890123456789012345678901234567890123456789012345CCCC020292666447DU29266644ORIPALAVECINO                              RAUL                                    MSAR19811229EMA0085SEXCNN200012292001122188899967785AA1234567BANCO NACION             N2017122920161112CALLE 103                     361   ---XXXXY01879MMMTTTTTTTTTTTTTTTFFFFFFFFFFFFFFFBERAZATEGUI         ARraulpalav@gmail.com                                                   20251020ARUYCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCPPPPPPPPPPMARIA PEREZ                                                 SIUY-00000000000560TTTTTTTTTTTTTTTTTTTTTTTTT1234OBLIUYS000000000000333DOWNTOWN                      EVERGREEN                     NNNNNNTTTTTPPPDepaSpringfield         Boston              USA123456789";
            const string inputPersonaJuridica = "1234567890123456789012345678901234567890123456789012345CCCC120292666447MACDONALDS                                                  FFF11198112291981123100742912EXCSS202208172023081712320010101Juan Perez               00012367784AA123456719811229Calle 103                        361---2doB101879AAA42757411       43047641       Berazategui         ARpepegarcia@gmail.com                                                  SSUSCasilla Post                  Cod PostalINSC123456789012345UYFATCA  OCDE   112345678901234512345678901234567890123451234UY1234512345202301211234S123456789012345Palermo                       Calle Falsa                   123456Tor11ro-2doBSpringfield         Boston              USABCDEFGHIJ";
            try
            {
                Request_CU0504_T109 personaJuridica = LineParser.Parse<Request_CU0504_T109>(inputPersonaJuridica);
                string textoPersonaJuridica = LineParser.ToTextLine(personaJuridica);

                Console.WriteLine($"{inputPersonaJuridica.Length} {textoPersonaJuridica.Length}");

                if (inputPersonaJuridica == textoPersonaJuridica)
                    Console.WriteLine($"InputLine y la clase serializada, son identicas!!!\n");
                else
                    Console.WriteLine($"ERROR: InputLine y la clase serializada, DIFIEREN\n");

                Console.WriteLine(textoPersonaJuridica);
                Console.WriteLine(inputPersonaJuridica);

                PrintObject(personaJuridica);
                return;

                /*
                Request_CU0504_T109 personaJuridica = new Request_CU0504_T109();
                string textoPersonaJuridica = LineParser.ToTextLine(inputPersonaJuridica);
                Console.WriteLine(textoPersonaJuridica);


                Request_CU0504_T109 personaJuridica2 = LineParser.Parse<Request_CU0504_T109>(textoPersonaJuridica);
                string textoPersonaJuridica2 = LineParser.ToTextLine(personaJuridica2);
                if (textoPersonaJuridica == textoPersonaJuridica2) 
                    Console.WriteLine("iguales");
                else
                    Console.WriteLine("Distintos");
                Console.WriteLine(textoPersonaJuridica2);

                PrintObject(personaJuridica);
                return;
                */

                /*
                Request_CU0503_T109 personaFisica = LineParser.Parse<Request_CU0503_T109>(inputPersonaFisica);
                string textoPersonaFisica = LineParser.ToTextLine(personaFisica);
                Console.WriteLine($"Tamano InpuLine: {inputLine.Length} \nTamano cadena serializada: {textoPersonaFisica.Length}");
                
                if (inputPersonaFisica.TrimEnd() == textoPersonaFisica.TrimEnd())
                    Console.WriteLine("InputLine y la clase serializada, son identicas!!!\n");
                else
                    Console.WriteLine("ERROR: inputPersonaFisica y la clase serializada, DIFIEREN\n");

                PrintObject(personaFisica);
                return;
                */
                /*
                Request_CU0503_T109 personaFisica = new Request_CU0503_T109();
                string textoPersonaFisica = LineParser.ToTextLine(personaFisica);
                Request_CU0503_T109 personaFisica2 = LineParser.Parse<Request_CU0503_T109>(inputLine);
                PrintObject(personaFisica);
                return;
                */

            }
            catch (Exception exception)
            {
                Console.WriteLine($"Parsing error: {exception.Message}");
            }

            Console.WriteLine("\npress any key to exit...");
            Console.ReadKey();
        }


        static void PrintObject(object objectToPrint)
        {
            foreach (PropertyInfo property in objectToPrint.GetType().GetProperties())
            {
                Console.WriteLine($"{property.Name}: {property.GetValue(objectToPrint)}");
            }
        }

    }
}
