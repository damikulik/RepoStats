namespace RepoStats.Application;

public record CharacterOccurencesView(DateTime RefreshedAt, IDictionary<char, int> Occurences);
