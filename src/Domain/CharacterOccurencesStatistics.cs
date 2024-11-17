using System.Collections.Frozen;

namespace RepoStats.Domain;

public record CharacterOccurencesStatistics(DateTimeOffset LastUpdated, FrozenDictionary<char, int> Occurences);
