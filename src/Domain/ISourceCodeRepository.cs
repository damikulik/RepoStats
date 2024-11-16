namespace RepoStats.Domain;

public interface ISourceCodeRepository : IDisposable
{
    Task<IReadOnlyList<RepositoryResource>> Search(CancellationToken token);

    Task<RepositoryResourceContent> Fetch(RepositoryResource resource, CancellationToken token);
}
