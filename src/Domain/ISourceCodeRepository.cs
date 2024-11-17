namespace RepoStats.Domain;

public interface ISourceCodeRepository : IDisposable
{
    Task<IReadOnlyList<RepositoryResource>> Search(StatisticsContext context, CancellationToken token);

    Task<RepositoryResourceContent> Fetch(StatisticsContext context, RepositoryResource resource, CancellationToken token);
}
