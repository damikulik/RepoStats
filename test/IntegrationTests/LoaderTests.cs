using System.Buffers;
using System.Text;

using RepoStats.Domain;

using Shouldly;

using Xunit.Abstractions;

namespace RepoStats.GitHubLoader.IntegrationTests;

public class LoaderTests(GitHubFixture fixture, ITestOutputHelper helper) : IClassFixture<GitHubFixture>, IDisposable
{
    private bool _disposedValue;

    [Fact]
    public async Task CanSearchRepoFiles()
    {
        GitHubRepository repository = new(fixture.Client, fixture.ResilienceMechanism, this);
        IReadOnlyList<RepositoryResource> files = await repository.Search(fixture.Statistics, CancellationToken.None);

        files.ShouldNotBeNull();
        files.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task CanGetRepoContent()
    {
        GitHubRepository repository = new(fixture.Client, fixture.ResilienceMechanism, this);
        IReadOnlyList<RepositoryResource> files = await repository.Search(fixture.Statistics, CancellationToken.None);
        files.ShouldNotBeNull();
        files.ShouldNotBeEmpty();

        var file = files[0];
        var content = await repository.Fetch(fixture.Statistics, file, CancellationToken.None);

        content.Length.ShouldBePositive();

        helper.WriteLine($"Read {Encoding.UTF8.GetString(content.ToArray())}");

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
