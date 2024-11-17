using System.Collections.Concurrent;

using Octokit;

using Polly.Registry;

using RepoStats.Domain;

namespace RepoStats.GitHubLoader;

internal class GitHubRepository(IGitHubClient client, ResiliencePipelineProvider<string> pipelineProvider, IDisposable lifetimeReference) : ISourceCodeRepository
{
    private bool _disposedValue;

    public async Task<IReadOnlyList<RepositoryResource>> Search(StatisticsContext context, CancellationToken token)
    {
        var results = new ConcurrentBag<SearchCodeResult>();

        await Parallel.ForEachAsync(context.Languages, async (configLanguage, token) =>
        {
            if (!Enum.TryParse(configLanguage, true, out Language searchLanguage))
            {
                return;
            }

            bool more = true;
            int page = 1;
            int size = 50;

            var pipeline = pipelineProvider.GetPipeline(nameof(GitHubRepository));

            while (more)
            {
                var request = new SearchCodeRequest()
                {
                    Page = page,
                    PerPage = size,
                    Language = searchLanguage,
                    In = [CodeInQualifier.Path],
                };

                request.Repos.Add(context.Owner, context.Repository);

                SearchCodeResult matches =
                    await pipeline.ExecuteAsync(async _ => await client.Search.SearchCode(request), token);

                results.Add(matches);
                more = matches.TotalCount > (page * size);
            }
        });

        return results.SelectMany(p => p.Items)
            .DistinctBy(p => p.Url)
            .Select(s => new RepositoryResource(s.Name, s.Path, s.Sha))
            .ToList();
    }

    public async Task<RepositoryResourceContent> Fetch(StatisticsContext context, RepositoryResource resource, CancellationToken token)
    {
        var pipeline = pipelineProvider.GetPipeline(nameof(GitHubRepository));
        var content = await pipeline
            .ExecuteAsync(async _ => await client.Repository.Content.GetRawContent(context.Owner, context.Repository, resource.Path), token);

        return new(resource.Reference, content);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                lifetimeReference.Dispose();
            }

            _disposedValue = true;
        }
    }
}
