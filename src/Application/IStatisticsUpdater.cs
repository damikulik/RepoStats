namespace RepoStats.Application;

public interface IStatisticsUpdater
{
    Task Process(CancellationToken stoppingToken);
}
