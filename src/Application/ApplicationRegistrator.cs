using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using RepoStats.Domain;

namespace RepoStats.Application;

public static class ApplicationRegistrator
{
    public static IServiceCollection AddRepoStatsApplication(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        string sectionPath = typeof(ApplicationRegistrator).Namespace?.Replace(".", ":") ?? throw new InvalidOperationException();

        var config = configuration.GetSection(sectionPath).Get<RepoStatsConfig>()
            ?? throw new InvalidOperationException("Can't bind the configuration for RepoStats application.");

        var context = configuration.GetSection($"RepoStats:Statistics:CharacterOccurences").Get<StatisticsContext>()
            ?? throw new InvalidOperationException("Can't bind the Statistics Context.");

        services.AddSingleton(config);
        services.AddSingleton(context);
        services.AddSingleton<ISystemContext, SystemContext>();

        services.AddSingleton<ApplicationService>();
        services.AddSingleton<IStatisticsUpdater>(sp => sp.GetRequiredService<ApplicationService>());
        services.AddSingleton<IRepoStatisticsService>(sp => sp.GetRequiredService<ApplicationService>());

        return services;
    }
}
