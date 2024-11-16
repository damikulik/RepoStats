using RepoStats.Application;
using RepoStats.GitHubLoader;

namespace RepoStats.AppHost.Module;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepoStats(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddRepoStatsApplication(configuration)
            .AddGibHubSourceCodeRepository(configuration);

        services.AddHostedService<StatisticsCalculatorHost>();

        return services;
    }
}
