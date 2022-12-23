using FixedWidthTextUtils;
using FixedWidthTextUtils.Attributes;
using System;
using System.Reflection;

namespace Ejemplo_NF_4._8
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string inputLine = "123456789ANed Flanders        WhateverStreet      04566 2222Brussels      Belgium        1981122918091991991991920211229101112    2021013010111220210131101112    -0000123451111111111-92222222222299999999999990001234525  123   123456729121981";

            try
            {
                Cliente parsedClient = LineParser.Parse<Cliente>(inputLine);
                PrintObject(parsedClient);
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
