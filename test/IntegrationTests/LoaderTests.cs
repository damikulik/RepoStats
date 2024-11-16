using System.Text;

using RepoStats.Domain;
using RepoStats.GitHubLoader;

using Shouldly;

using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests;

public class LoaderTests(GitHubFixture fixture, ITestOutputHelper helper) : IClassFixture<GitHubFixture>, IDisposable
{
    private bool _disposedValue;

    [Fact]
    public async Task CanSearchRepoFiles()
    {
        GitHubRepository repository = new(fixture.Client, this);
        IReadOnlyList<RepositoryResource> files = await repository.Search(CancellationToken.None);

        files.ShouldNotBeNull();
        files.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task CanGetRepoContent()
    {
        GitHubRepository repository = new(fixture.Client, this);
        IReadOnlyList<RepositoryResource> files = await repository.Search(CancellationToken.None);
        files.ShouldNotBeNull();
        files.ShouldNotBeEmpty();

        var file = files[0];
        RepositoryResourceContent content = await repository.Fetch(file, CancellationToken.None);

        content.ShouldNotBeNull();
        content.Reference.ShouldBe(file.Reference);
        content.Content.Length.ShouldBePositive();

        helper.WriteLine($"Read {Encoding.UTF8.GetString(content.Content.Span)}");

        helper.WriteLine($"Found {files.Count}");
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            _disposedValue = true;
        }
    }
}
