using System.Buffers;

namespace RepoStats.Domain;

public interface ISourceCodeRepository : IDisposable
{
    Task<IReadOnlyList<RepositoryResource>> Search(StatisticsContext context, CancellationToken token);

    Task<ReadOnlySequence<byte>> Fetch(StatisticsContext context, RepositoryResource resource, CancellationToken token);
}
