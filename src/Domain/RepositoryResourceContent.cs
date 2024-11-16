using System;

namespace RepoStats.Domain;

public record RepositoryResourceContent(string Reference, ReadOnlyMemory<byte> Content);
