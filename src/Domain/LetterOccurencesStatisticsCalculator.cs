using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Text;

namespace RepoStats.Domain;

public sealed class LetterOccurencesStatisticsCalculator
{
    private FrozenDictionary<char, int>? _stats;
    private DateTimeOffset _lastUpdate = DateTimeOffset.MinValue;

    public Task<LetterOccurencesStatistics?> GetCurrentStatistics()
    {
        LetterOccurencesStatistics? snapshot = default;
        if (_stats is FrozenDictionary<char, int> stats)
        {
            snapshot = new LetterOccurencesStatistics(_lastUpdate, stats);
        }

        return Task.FromResult(snapshot);
    }

    public async Task CalculateLetterStats(ISourceCodeRepository repository, ISystemContext context, CancellationToken token)
    {
        IReadOnlyList<RepositoryResource> searchResult = await repository.Search(token);

        ConcurrentDictionary<char, int> stats = new();

        await Parallel.ForEachAsync(searchResult, async (RepositoryResource resource, CancellationToken token) =>
        {
            var file = await repository.Fetch(resource, token);

            Decoder decoder = context.Decoder;
            char[] charBuffer = new char[1];
            var sequence = file.Content;

            for (int i = 0; i < sequence.Length; i++)
            {
                int charCount = decoder.GetChars(sequence.Slice(i, 1).ToArray(), 0, 1, charBuffer, 0);

                if (charCount > 0)
                {
                    var candidate = charBuffer[0];
                    if (char.IsLetter(candidate) || char.IsDigit(candidate))
                    {
                        stats.AddOrUpdate(charBuffer[0], 1, (_, occurences) => occurences + 1);
                    }
                }

                token.ThrowIfCancellationRequested();
            }
        });

        _stats = stats.ToFrozenDictionary();
        _lastUpdate = context.TimeProvider.GetUtcNow();
    }
}
