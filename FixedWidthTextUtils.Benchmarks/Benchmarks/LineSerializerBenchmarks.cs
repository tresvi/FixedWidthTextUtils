using System;
using BenchmarkDotNet.Attributes;
using FixedWidthTextUtils;
using FixedWidthTextUtils.Benchmarks.Models;

namespace FixedWidthTextUtils.Benchmarks.Benchmarks
{
    /// <summary>
    /// Mide el costo de serializar una entidad a línea de texto (ToTextLine).
    /// </summary>
    [MemoryDiagnoser]
    [HideColumns("Job", "Error", "StdDev", "RatioSD")]
    public class LineSerializerBenchmarks
    {
        private Cliente _clientePositional;
        private ClienteOrdinal _clienteOrdinal;

        [GlobalSetup]
        public void Setup()
        {
            _clientePositional = new Cliente
            {
                Id = 123456789,
                Code = "A",
                Name = "Ned Flanders",
                Street = "WhateverStreet",
                HouseNumber = 4566,
                PostCode = " 2222",
                City = "Brussels",
                Country = "Belgium",
                BirthDate = new DateTime(1981, 12, 29),
                HeigthInCentimeters = 180,
                WeightFloat = 91.9f,
            };

            _clienteOrdinal = new ClienteOrdinal
            {
                Code = "A",
                Name = "Juan",
                LastName = "Perez",
                Enable = true,
                ZipCode = 6109,
                BirthDate = new DateTime(2022, 11, 19),
                Weight = 12.34f,
            };

            // Pre-warm del LineModelPlanCache para no medir el costo del primer plan.
            LineParser.ToTextLine(_clientePositional);
            LineParser.ToTextLine(_clienteOrdinal);
        }

        [Benchmark(Baseline = true, Description = "ToTextLine(Cliente) (posicional, 11 campos)")]
        public string ToTextLine_Positional()
        {
            return LineParser.ToTextLine(_clientePositional);
        }

        [Benchmark(Description = "ToTextLine(ClienteOrdinal) (ordinal, 7 campos)")]
        public string ToTextLine_Ordinal()
        {
            return LineParser.ToTextLine(_clienteOrdinal);
        }
    }
}
