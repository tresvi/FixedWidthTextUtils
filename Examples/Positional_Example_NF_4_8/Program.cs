using FixedWidthTextUtils;
using System;
using System.IO;
using System.Reflection;

namespace Positional_Example_NF_4_8
{
    /// <summary>
    /// Ejemplo en modo posicional: línea de 103 caracteres (prefijo del fixture de pruebas).
    /// </summary>
    internal static class Program
    {
        private static void Main()
        {
            Console.WriteLine("Positional_Example_NF_4_8 — modo posicional (start/end por campo)");
            Console.WriteLine();

            string samplePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SampleLine103.txt");
            if (!File.Exists(samplePath))
            {
                Console.WriteLine($"No se encontró {samplePath}. Copie SampleLine103.txt junto al ejecutable.");
                Console.ReadKey();
                return;
            }

            string inputLine = File.ReadAllText(samplePath).TrimEnd('\r', '\n');
            Console.WriteLine($"Línea de entrada ({inputLine.Length} caracteres) desde SampleLine103.txt");
            Console.WriteLine();

            try
            {
                Cliente parsed = LineParser.Parse<Cliente>(inputLine);
                PrintObject(parsed);

                string roundTrip = LineParser.ToTextLine(parsed);
                Console.WriteLine();
                Console.WriteLine("Round-trip ToTextLine (primeros 120 caracteres):");
                string preview = roundTrip.Length <= 120 ? roundTrip : roundTrip.Substring(0, 120) + "...";
                Console.WriteLine(preview);
                Console.WriteLine($"Longitud salida: {roundTrip.Length}");
                Console.WriteLine($"Coincide con entrada: {string.Equals(inputLine, roundTrip, StringComparison.Ordinal)}");
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
