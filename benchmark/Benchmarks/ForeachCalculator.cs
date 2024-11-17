using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Text;

using RepoStats.Domain;

namespace RepoStats.Benchmarks;

internal sealed class ForeachCalculator(StatisticsContext sourceCodeContext)
{
    private FrozenDictionary<char, int>? _stats;
    private DateTimeOffset _lastUpdate = DateTimeOffset.MinValue;

    public Task<CharacterOccurencesStatistics?> GetCurrentStatistics()
    {
        CharacterOccurencesStatistics? snapshot = default;
        if (_stats is FrozenDictionary<char, int> stats)
        {
            snapshot = new CharacterOccurencesStatistics(_lastUpdate, stats);
        }

        return Task.FromResult(snapshot);
    }

    public async Task CalculateLetterStats(ISourceCodeRepository repository, ISystemContext context, CancellationToken token)
    {
        IReadOnlyList<RepositoryResource> searchResult = await repository.Search(sourceCodeContext, token);

        Dictionary<char, int> stats = new();
        Decoder decoder = context.Encoding.GetDecoder();
        char[] charBuffer = new char[1];

        foreach (RepositoryResource resource in searchResult)
        {
            var file = await repository.Fetch(sourceCodeContext, resource, token);
            var sequence = file.Content;

            for (int i = 0; i < sequence.Length; i++)
            {
                int charCount = decoder.GetChars(sequence.Slice(i, 1).ToArray(), 0, 1, charBuffer, 0);

                if (charCount > 0)
                {
                    var candidate = charBuffer[0];
                    if (char.IsLetter(candidate) || char.IsDigit(candidate))
                    {
                        stats.TryGetValue(candidate, out int occs);
                        stats[candidate] = ++occs;
                    }
                }
            }
        }

        _stats = stats.ToFrozenDictionary();
        _lastUpdate = context.TimeProvider.GetUtcNow();
    }
}
