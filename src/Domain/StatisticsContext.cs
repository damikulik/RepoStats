namespace RepoStats.Domain;

public record StatisticsContext(string Owner, string Repository, IReadOnlySet<string> Languages);
