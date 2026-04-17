using FixedWidthTextUtils;
using System;
using System.Reflection;

namespace Ordinal_Example_NF_4_8
{
    /// <summary>
    /// Ejemplo en modo ordinal: línea fija construida por la suma de anchos de campo (29 caracteres).
    /// </summary>
    internal static class Program
    {
        private const string SampleOrdinalLine = "AJuanPerezYES6109202211191234";

        private static void Main()
        {
            Console.WriteLine("Ordinal_Example_NF_4_8 — modo ordinal (solo largos de campo)");
            Console.WriteLine($"Línea de entrada ({SampleOrdinalLine.Length} caracteres): {SampleOrdinalLine}");
            Console.WriteLine();

            try
            {
                ClienteOrdinal parsed = LineParser.Parse<ClienteOrdinal>(SampleOrdinalLine);
                PrintObject(parsed);

                string roundTrip = LineParser.ToTextLine(parsed);
                Console.WriteLine();
                Console.WriteLine("Round-trip ToTextLine:");
                Console.WriteLine(roundTrip);
                Console.WriteLine($"Coincide con entrada: {string.Equals(SampleOrdinalLine, roundTrip, StringComparison.Ordinal)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("Pulse una tecla para salir...");
            Console.ReadKey();
        }

        private static void PrintObject(object objectToPrint)
        {
            foreach (PropertyInfo property in objectToPrint.GetType().GetProperties())
            {
                Console.WriteLine($"  {property.Name}: {property.GetValue(objectToPrint)}");
            }
        }
    }
}
