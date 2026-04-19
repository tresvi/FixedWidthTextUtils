using BenchmarkDotNet.Attributes;
using FixedWidthTextUtils;
using FixedWidthTextUtils.Benchmarks.Models;

namespace FixedWidthTextUtils.Benchmarks.Benchmarks
{
    /// <summary>
    /// Mide el costo de parsear una sola línea (hot path tipico de cada Parse).
    /// </summary>
    [MemoryDiagnoser]
    [HideColumns("Job", "Error", "StdDev", "RatioSD")]
    public class LineParserBenchmarks
    {
        private const string SamplePositionalLine =
            "123456789ANed Flanders        WhateverStreet      04566 2222Brussels      Belgium        19811229180919";

        private const string SampleOrdinalLine = "AJuanPerezYES6109202211191234";

        [GlobalSetup]
        public void Setup()
        {
            // Forzamos la construcción del plan en el cache para no medir el costo
            // del primer Parse (que incluye la reflexión de BuildPlan).
            LineParser.Parse<Cliente>(SamplePositionalLine);
            LineParser.Parse<ClienteOrdinal>(SampleOrdinalLine);
        }

        [Benchmark(Baseline = true, Description = "Parse<Cliente> (posicional, 11 campos)")]
        public Cliente Parse_Positional()
        {
            return LineParser.Parse<Cliente>(SamplePositionalLine);
        }

        [Benchmark(Description = "Parse<ClienteOrdinal> (ordinal, 7 campos)")]
        public ClienteOrdinal Parse_Ordinal()
        {
            return LineParser.Parse<ClienteOrdinal>(SampleOrdinalLine);
        }

        [Benchmark(Description = "TryParse<Cliente> (camino no-throw)")]
        public Cliente TryParse_Positional_Ok()
        {
            LineParser.TryParse(SamplePositionalLine, out Cliente result);
            return result;
        }
    }
}
