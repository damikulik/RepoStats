namespace RepoStats.Application;

internal class StatisticsCalculatorHost(IStatisticsUpdater updater) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken) => updater.Process(stoppingToken);
}
