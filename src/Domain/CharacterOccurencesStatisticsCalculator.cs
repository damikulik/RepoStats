using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Text;

namespace RepoStats.Domain;

public sealed class CharacterOccurencesStatisticsCalculator(StatisticsContext sourceCodeContext)
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

        ConcurrentDictionary<char, int> stats = new();
        Decoder decoder = context.Encoding.GetDecoder();

        await Parallel.ForEachAsync(searchResult, async (RepositoryResource resource, CancellationToken token) =>
        {
            var file = await repository.Fetch(sourceCodeContext, resource, token);

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
