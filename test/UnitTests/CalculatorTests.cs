using RepoStats.Domain;

using Shouldly;

namespace UnitTests;

public class CalculatorTests(DomainFixture fixture) : IClassFixture<DomainFixture>
{
    [Fact]
    public async Task ReturnsNullWhenNoStatistics()
    {
        CharacterOccurencesStatisticsCalculator subject = new(new StatisticsContext("test", "test", ["test"]));

        CharacterOccurencesStatistics? result = await subject.GetCurrentStatistics();

        result.ShouldBeNull();
    }

    [Fact]
    public async Task ReturnsResultWhenStatisticsCalculated()
    {
        CharacterOccurencesStatisticsCalculator subject = new(new StatisticsContext("test", "test", ["test"]));

        await subject.CalculateLetterStats(fixture.Repository, fixture.Context, CancellationToken.None);

        CharacterOccurencesStatistics? result = await subject.GetCurrentStatistics();

        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task CalculatesStatistics()
    {
        CharacterOccurencesStatisticsCalculator subject = new(new StatisticsContext("test", "test", ["test"]));

        var task = subject.CalculateLetterStats(fixture.Repository, fixture.Context, CancellationToken.None);
        await task;

        task.Status.ShouldBe(TaskStatus.RanToCompletion);
    }
}
