using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BenchmarkDotNet.Attributes;
using FixedWidthTextUtils;
using FixedWidthTextUtils.Benchmarks.Models;

namespace FixedWidthTextUtils.Benchmarks.Benchmarks
{
    /// <summary>
    /// Mide la lectura/escritura completa de archivos de texto plano (FileParser).
    /// Genera un archivo temporal con N líneas en GlobalSetup y lo borra al finalizar.
    /// </summary>
    [MemoryDiagnoser]
    [HideColumns("Job", "Error", "StdDev", "RatioSD")]
    public class FileParserBenchmarks
    {
        private const string SamplePositionalLine =
            "123456789ANed Flanders        WhateverStreet      04566 2222Brussels      Belgium        19811229180919";

        private string _inputFile;
        private string _outputFile;
        private FileParser _parser;
        private List<Cliente> _entitiesToWrite;

        /// <summary>
        /// Cantidad de líneas del archivo de entrada. Usamos varios tamaños para ver
        /// como escala el parsing (lineal en N, pero queremos detectar bottlenecks de I/O).
        /// </summary>
        [Params(1_000, 10_000, 100_000)]
        public int LineCount { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "FixedWidthTextUtils.Benchmarks");
            Directory.CreateDirectory(tempDir);

            _inputFile = Path.Combine(tempDir, $"input_{LineCount}.txt");
            _outputFile = Path.Combine(tempDir, $"output_{LineCount}.txt");

            using (var sw = new StreamWriter(_inputFile, false, Encoding.UTF8))
            {
                for (int i = 0; i < LineCount; i++)
                    sw.WriteLine(SamplePositionalLine);
            }

            _parser = new FileParser(_inputFile, Encoding.UTF8);

            // Pre-warm del LineModelPlanCache para no contaminar la primera medición.
            LineParser.Parse<Cliente>(SamplePositionalLine);

            _entitiesToWrite = _parser.Parse<Cliente>(ignoreWrongLines: false);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            try { if (File.Exists(_inputFile)) File.Delete(_inputFile); } catch { }
            try { if (File.Exists(_outputFile)) File.Delete(_outputFile); } catch { }
        }

        [Benchmark(Description = "FileParser.Parse<Cliente>")]
        public int Parse_File()
        {
            List<Cliente> result = _parser.Parse<Cliente>(ignoreWrongLines: false);
            return result.Count;
        }

        [Benchmark(Description = "FileParser.ToFlatFile<Cliente>")]
        public void ToFlatFile()
        {
            _parser.ToFlatFile(_entitiesToWrite, _outputFile);
        }
    }
}
