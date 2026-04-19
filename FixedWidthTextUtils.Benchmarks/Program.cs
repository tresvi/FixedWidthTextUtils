using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace FixedWidthTextUtils.Benchmarks
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // Config con MemoryDiagnoser y exporters utiles para versionar resultados.
            IConfig config = ManualConfig
                .Create(DefaultConfig.Instance)
                .AddDiagnoser(MemoryDiagnoser.Default)
                .AddExporter(MarkdownExporter.GitHub)
                .AddExporter(CsvExporter.Default)
                .AddLogger(ConsoleLogger.Default)
                .WithSummaryStyle(SummaryStyle.Default.WithMaxParameterColumnWidth(40));

            // Permite filtrar por CLI:
            //   dotnet run -c Release -- --filter *LineParser*
            //   dotnet run -c Release -- --list flat
            BenchmarkSwitcher
                .FromAssembly(typeof(Program).Assembly)
                .Run(args, config);
        }
    }
}
