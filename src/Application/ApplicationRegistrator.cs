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

        services.AddSingleton(config);
        services.AddSingleton<ISystemContext, SystemContext>();

        services.AddSingleton<ApplicationService>();
        services.AddSingleton<IStatisticsUpdater>(sp => sp.GetRequiredService<ApplicationService>());
        services.AddSingleton<IRepoStatisticsService>(sp => sp.GetRequiredService<ApplicationService>());

        return services;
    }
}
