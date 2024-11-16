using System.Collections.Frozen;

namespace RepoStats.Domain;

public record LetterOccurencesStatistics(DateTimeOffset LastUpdated, FrozenDictionary<char, int> Occurences);
