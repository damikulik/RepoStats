using RepoStats.Domain;

using Shouldly;

namespace UnitTests;

public class CalculatorTests(DomainFixture fixture) : IClassFixture<DomainFixture>
{
    [Fact]
    public async Task ReturnsNullWhenNoStatistics()
    {
        CharacterOccurencesStatisticsCalculator subject = new(new StatisticsContext("test", "test", new HashSet<string> { "test" }));

        CharacterOccurencesStatistics? result = await subject.GetCurrentStatistics();

        result.ShouldBeNull();
    }

    [Fact]
    public async Task ReturnsResultWhenStatisticsCalculated()
    {
        CharacterOccurencesStatisticsCalculator subject = new(new StatisticsContext("test", "test", new HashSet<string> { "test" }));

        await subject.CalculateLetterStats(fixture.Repository, fixture.Context, CancellationToken.None);

        CharacterOccurencesStatistics? result = await subject.GetCurrentStatistics();

        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task CalculatesStatistics()
    {
        CharacterOccurencesStatisticsCalculator subject = new(new StatisticsContext("test", "test", new HashSet<string> { "test" }));

        var task = subject.CalculateLetterStats(fixture.Repository, fixture.Context, CancellationToken.None);
        await task;

        task.Status.ShouldBe(TaskStatus.RanToCompletion);
    }

    [Fact]
    public async Task CalculatorIsCorrect()
    {
        CharacterOccurencesStatisticsCalculator subject = new(new StatisticsContext("test", "test", new HashSet<string> { "test" }));

        await subject.CalculateLetterStats(fixture.SmallRepository, fixture.Context, CancellationToken.None);

        var stats = await subject.GetCurrentStatistics();

        stats.ShouldNotBeNull();
        stats.Occurences.ShouldNotBeNull();
        stats.Occurences['a'].ShouldBe(6);
        stats.Occurences['b'].ShouldBe(9);
        stats.Occurences['c'].ShouldBe(12);
        stats.Occurences.TryGetValue('\\', out int occs).ShouldBeFalse();
    }
}
