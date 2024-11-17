using System.Buffers;
using System.Text;

using Bogus.DataSets;

using RepoStats.Domain;

namespace RepoStats.Benchmarks;

internal class BenchmarkRepository(IDictionary<string, int> repoInfo) : ISourceCodeRepository
{
    private readonly Lorem _lorem = new("en");

    private bool _disposedValue;

    public async Task<IReadOnlyList<RepositoryResource>> Search(StatisticsContext context, CancellationToken token)
    {
        await Task.Delay(1, token);
        return repoInfo
            .Select(c => new RepositoryResource(c.Key, c.Key, Guid.NewGuid().ToString("N")[0..8]))
            .ToList();
    }

    public async Task<ReadOnlySequence<byte>> Fetch(StatisticsContext context, RepositoryResource resource, CancellationToken token)
    {
        await Task.Delay(Random.Shared.Next(20, 60), token);
        return new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(_lorem.Lines(lineCount: repoInfo[resource.Path])));
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // nothing to do here
            }

            _disposedValue = true;
        }
    }
}
