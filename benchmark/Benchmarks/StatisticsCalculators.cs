using System.Globalization;

using BenchmarkDotNet.Attributes;

using RepoStats.Application;
using RepoStats.Domain;

namespace RepoStats.Benchmarks;

[MemoryDiagnoser]
[ThreadingDiagnoser]
[MarkdownExporterAttribute.GitHub]
public class StatisticsCalculators
{
    private Dictionary<string, int> _textInfos = [];

    [Params(1, 100)]
    public int RepositoryFiles { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _textInfos = RepositoryFiles switch
        {
            1 => new Dictionary<string, int> { { "big-file", 10000 } },
            _ => Enumerable
                .Range(0, RepositoryFiles)
                .ToDictionary(_ => Guid.NewGuid().ToString()[0..4], _ => Random.Shared.Next(50, 200)),
        };
    }

    [Benchmark(Baseline = true)]
    public async Task Current()
    {
        var repository = new BenchmarkRepository(_textInfos);
        var context = new SystemContext(new RepoStatsConfig(1, "utf-8", CultureInfo.GetCultureInfo("en")));
        CharacterOccurencesStatisticsCalculator calculator = new(new StatisticsContext("benchmark", "benchmark", new HashSet<string> { "b" }));

        await calculator.CalculateLetterStats(repository, context, CancellationToken.None);
    }

    [Benchmark]
    public async Task Direct()
    {
        var repository = new BenchmarkRepository(_textInfos);
        var context = new SystemContext(new RepoStatsConfig(1, "utf-8", CultureInfo.GetCultureInfo("en")));
        DirectCalculator calculator = new(new StatisticsContext("benchmark", "benchmark", new HashSet<string> { "b" }));

        await calculator.CalculateLetterStats(repository, context, CancellationToken.None);
    }

    [Benchmark]
    public async Task MapReduce()
    {
        var repository = new BenchmarkRepository(_textInfos);
        var context = new SystemContext(new RepoStatsConfig(1, "utf-8", CultureInfo.GetCultureInfo("en")));
        MapReduceCalculator calculator = new(new StatisticsContext("benchmark", "benchmark", new HashSet<string> { "b" }));

        await calculator.CalculateLetterStats(repository, context, CancellationToken.None);
    }

    [Benchmark]
    public async Task Naive()
    {
        var repository = new BenchmarkRepository(_textInfos);
        var context = new SystemContext(new RepoStatsConfig(1, "utf-8", CultureInfo.GetCultureInfo("en")));
        NaiveCalculator calculator = new(new StatisticsContext("benchmark", "benchmark", new HashSet<string> { "b" }));

        await calculator.CalculateLetterStats(repository, context, CancellationToken.None);
    }
}
