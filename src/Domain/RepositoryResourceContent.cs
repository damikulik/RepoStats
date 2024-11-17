using System.Buffers;

namespace RepoStats.Domain;

public record RepositoryResourceContent(string Reference, ReadOnlySequence<byte> Content);
