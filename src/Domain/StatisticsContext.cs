namespace RepoStats.Domain;

public record StatisticsContext(string Owner, string Repository, IReadOnlyList<string> Languages);
