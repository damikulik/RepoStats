using System.Buffers;
using System.Collections.Frozen;

using RepoStats.Domain;

namespace RepoStats.Benchmarks;

internal sealed class MapReduceCalculator(StatisticsContext sourceCodeContext)
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

        List<Dictionary<char, int>> maps = [];

        foreach (RepositoryResource resource in searchResult)
        {
            var content = await repository.Fetch(sourceCodeContext, resource, token);

            var map = context.Encoding.GetString(content.ToArray())
                .GroupBy(p => p)
                .Where(p => char.IsLetter(p.Key) || char.IsDigit(p.Key))
                .ToDictionary(p => p.Key, p => p.Count());

            maps.Add(map);
        }

        _stats = maps.Aggregate(new Dictionary<char, int>(), (state, current) =>
        {
            foreach (var item in current)
            {
                state.TryGetValue(item.Key, out int occs);
                state[item.Key] = occs + item.Value;
            }

            return state;
        }).ToFrozenDictionary();
        _lastUpdate = context.TimeProvider.GetUtcNow();
    }
}
