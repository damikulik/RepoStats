using System.Collections.Immutable;
using System.Diagnostics;

using ErrorOr;

using Microsoft.Extensions.Logging;

using RepoStats.Domain;

namespace RepoStats.Application;

internal class ApplicationService(RepoStatsConfig config, ISystemContext systemContext, Func<ISourceCodeRepository> repositoryCreator, ILogger<ApplicationService> logger)
    : IRepoStatisticsService, IStatisticsUpdater
{
    private readonly LetterOccurencesStatisticsCalculator _calculator = new();

    public async Task Process(CancellationToken stoppingToken)
    {
        PeriodicTimer timer = new(TimeSpan.FromSeconds(config.Frequency));
        while (!stoppingToken.IsCancellationRequested)
        {
            var mark = Stopwatch.GetTimestamp();

            try
            {
                using ISourceCodeRepository repository = repositoryCreator();
                await _calculator.CalculateLetterStats(repository, systemContext, stoppingToken);

                logger.LogInformation("Completed {Operation} in {Elapsed}ms", nameof(_calculator.CalculateLetterStats), Stopwatch.GetElapsedTime(mark));
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Unable to complete {Operation}", nameof(_calculator.CalculateLetterStats));
            }

            await timer.WaitForNextTickAsync(stoppingToken);
        }
    }

    public async Task<ErrorOr<CharacterOccurencesView>> GetLetterOccurences(CancellationToken token)
    {
        LetterOccurencesStatistics? statistics = await _calculator.GetCurrentStatistics();

        if (statistics is null)
        {
            return Error.Custom(503, "Service Unavailable", "Not warmed up yet.", new() { { "RetryAfter", 15 } });
        }

        return new CharacterOccurencesView(
            statistics.LastUpdated.UtcDateTime,
            statistics.Occurences
                .OrderByDescending(p => p.Value)
                .ToDictionary(p => p.Key, p => p.Value));
    }
}
