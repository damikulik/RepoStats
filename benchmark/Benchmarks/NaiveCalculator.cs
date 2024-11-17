using System.Buffers;
using System.Collections.Frozen;

using RepoStats.Domain;

namespace RepoStats.Benchmarks;

internal sealed class NaiveCalculator(StatisticsContext sourceCodeContext)
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

        List<string> contents = new List<string>();
        foreach (var item in searchResult)
        {
            var content = await repository.Fetch(sourceCodeContext, item, token);
            contents.Add(context.Encoding.GetString(content.ToArray()));
        }

        List<Dictionary<char, int>> maps = [];

        foreach (string payload in contents)
        {
            var map = payload
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
