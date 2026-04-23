using FixedWidthTextUtils;
using System;
using System.Diagnostics;

namespace ParseBenchmark
{
    /// <summary>
    /// Mide el tiempo de parsear la misma línea 1.000.000 de veces (posicional vs ordinal),
    /// usando las mismas entradas que los ejemplos Positional_Example y Ordinal_Example.
    /// </summary>
    internal static class Program
    {
        private const string SamplePositionalLine = "123456789ANed Flanders        WhateverStreet      04566 2222Brussels      Belgium        19811229180919";
        private const string SampleOrdinalLine = "AJuanPerezYES6109202211191234";
        private const int Iterations = 1_000_000;

        private static void Main()
        {
            Console.WriteLine("ParseBenchmark_Example_NF_4_8");
            Console.WriteLine($"Iteraciones: {Iterations:N0} (misma línea en cada una)");
            Console.WriteLine();

            // warmup para estabilizar JIT antes de medir.
            for (int i = 0; i < 500; i++)
            {
                LineParser.Parse<Cliente>(SamplePositionalLine);
                LineParser.Parse<ClienteOrdinal>(SampleOrdinalLine);
            }

            long checksumPos = RunPositionalBenchmark();
            long checksumOrd = RunOrdinalBenchmark();

            Console.WriteLine();
            Console.WriteLine($"Checksum posicional (evita dead-code): {checksumPos}");
            Console.WriteLine($"Checksum ordinal (evita dead-code): {checksumOrd}");
            Console.WriteLine();
            if (Environment.UserInteractive && !Console.IsInputRedirected)
            {
                Console.WriteLine("Pulse una tecla para salir...");
                Console.ReadKey();
            }
        }

        private static long RunPositionalBenchmark()
        {
            Console.WriteLine("--- Modo posicional ---");
            Console.WriteLine($"Línea ({SamplePositionalLine.Length} caracteres), primeros 40: {SamplePositionalLine.Substring(0, Math.Min(40, SamplePositionalLine.Length))}...");

            var sw = Stopwatch.StartNew();
            long sum = 0;
            for (int i = 0; i < Iterations; i++)
            {
                Cliente c = LineParser.Parse<Cliente>(SamplePositionalLine);
                sum += c.Id;
                sum += c.HouseNumber;
            }

            sw.Stop();
            PrintTiming(sw.Elapsed, Iterations);
            return sum;
        }

        private static long RunOrdinalBenchmark()
        {
            Console.WriteLine();
            Console.WriteLine("--- Modo ordinal ---");
            Console.WriteLine($"Línea ({SampleOrdinalLine.Length} caracteres): {SampleOrdinalLine}");

            var sw = Stopwatch.StartNew();
            long sum = 0;
            for (int i = 0; i < Iterations; i++)
            {
                ClienteOrdinal c = LineParser.Parse<ClienteOrdinal>(SampleOrdinalLine);
                sum += c.ZipCode;
                sum += c.Enable ? 1 : 0;
            }

            sw.Stop();
            PrintTiming(sw.Elapsed, Iterations);
            return sum;
        }

        private static void PrintTiming(TimeSpan elapsed, int iterations)
        {
            double totalMs = elapsed.TotalMilliseconds;
            double perOpUs = (totalMs * 1000.0) / iterations;
            Console.WriteLine($"Tiempo total: {totalMs:N2} ms");
            Console.WriteLine($"Por parseo: {perOpUs:N3} µs ({totalMs / iterations:N6} ms)");
            Console.WriteLine($"Parseos por segundo: {iterations / elapsed.TotalSeconds:N0}");
        }
    }
}
