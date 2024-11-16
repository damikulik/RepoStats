using Microsoft.Extensions.Configuration;

using Octokit;

namespace IntegrationTests
{
    public class GitHubFixture
    {
        internal IGitHubClient Client { get; }

        public GitHubFixture()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddUserSecrets<GitHubFixture>()
                .Build();

            Client = new GitHubClient(new ProductHeaderValue("RepoStats"))
            {
                Credentials = new Credentials(configuration.GetValue<string>("SecurityKey") ?? throw new InvalidOperationException()),
            };
        }
    }
}
