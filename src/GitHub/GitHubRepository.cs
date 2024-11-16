using Octokit;

using RepoStats.Domain;

namespace RepoStats.GitHubLoader;

internal class GitHubRepository(IGitHubClient client, IDisposable lifetimeReference) : ISourceCodeRepository
{
    private bool _disposedValue;

    public async Task<IReadOnlyList<RepositoryResource>> Search(CancellationToken token)
    {
        var request = new SearchCodeRequest()
        {
            In = [CodeInQualifier.Path],
            Language = Language.JavaScript,
        };

        request.Repos.Add("lodash", "lodash");

        SearchCodeResult contentsJs = await client.Search.SearchCode(request);

        request.Language = Language.TypeScript;
        SearchCodeResult contentsTs = await client.Search.SearchCode(request);

        return contentsJs.Items
            .Concat(contentsTs.Items)
            .Select(c => new RepositoryResource(c.Name, c.Path, c.Sha))
            .ToList();
    }

    public async Task<RepositoryResourceContent> Fetch(RepositoryResource resource, CancellationToken token)
    {
        var content = await client.Repository.Content.GetRawContent("lodash", "lodash", resource.Path);

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
