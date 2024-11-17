using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Octokit;

using Polly;
using Polly.Retry;

using RepoStats.Domain;

namespace RepoStats.GitHubLoader;

public static class GitHubRepositoryRegistrator
{
    public static IServiceCollection AddGibHubSourceCodeRepository(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        string sectionPath = typeof(GitHubRepositoryRegistrator).Namespace?.Replace(".", ":") ?? throw new InvalidOperationException();

        var config = configuration.GetSection(sectionPath).Get<GitHubConfig>()
            ?? throw new InvalidOperationException("Can't bind the configuration for GitHub source code repository.");

        if (string.IsNullOrEmpty(config.SecurityKey) || string.IsNullOrEmpty(config.AppName))
        {
            throw new InvalidOperationException("GitHub configuration must have non-empty Security Key and App Name.");
        }

        services.AddResiliencePipeline(nameof(GitHubRepository), builder
            => builder
                .AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    BackoffType = DelayBackoffType.Exponential,
                })
                .AddTimeout(TimeSpan.FromSeconds(1)));

        services.AddSingleton<IGitHubClient>(new GitHubClient(new ProductHeaderValue(config.AppName))
        {
            Credentials = new Credentials(config.SecurityKey),
        });
        services.AddTransient<ISourceCodeRepository, GitHubRepository>(sp =>
        {
            var scope = sp.CreateScope();
            return ActivatorUtilities.CreateInstance<GitHubRepository>(sp, scope);
        });
        services.AddSingleton<Func<ISourceCodeRepository>>(sp => () => sp.GetRequiredService<ISourceCodeRepository>());

        return services;
    }
}
