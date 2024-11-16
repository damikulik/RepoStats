using Octokit;

namespace RepoStats.GitHubLoader;

internal record GitHubConfig(string SecurityKey, string AppName);
