using Microsoft.Extensions.Configuration;

using Moq;

using Octokit;

using Polly;
using Polly.Registry;
using Polly.Retry;

using RepoStats.Domain;

namespace RepoStats.GitHubLoader.IntegrationTests;

public class GitHubFixture
{
    internal IGitHubClient Client { get; }

    internal StatisticsContext Statistics { get; } = new("lodash", "lodash", new HashSet<string> { "javascript" });

    internal ResiliencePipelineProvider<string> ResilienceMechanism { get; }

    public GitHubFixture()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddUserSecrets<GitHubFixture>()
            .Build();

        var resilienceMock = new Mock<ResiliencePipelineProvider<string>>();
        resilienceMock
            .Setup(p => p.GetPipeline(It.IsAny<string>()))
            .Returns(new ResiliencePipelineBuilder()
                .AddRetry(new RetryStrategyOptions
                {
                    OnRetry = arg => ValueTask.CompletedTask,
                })
                .AddTimeout(TimeSpan.FromSeconds(1))
                .Build());

        ResilienceMechanism = resilienceMock.Object;

        Client = new GitHubClient(new ProductHeaderValue("RepoStats"))
        {
            Credentials = new Credentials(configuration.GetValue<string>("SecurityKey") ?? throw new InvalidOperationException()),
        };
    }
}
