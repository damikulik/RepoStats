using System.Globalization;
using System.Text;

using Bogus.DataSets;

using Microsoft.Extensions.Time.Testing;

using Moq;

using RepoStats.Domain;

namespace UnitTests;

public class DomainFixture
{
    public ISystemContext Context { get; internal set; }

    public ISourceCodeRepository Repository { get; internal set; }

    public StatisticsContext StatsContext { get; } = new("test", "test", ["test"]);

    public DomainFixture()
    {
        var systemMock = new Mock<ISystemContext>();
        systemMock.SetupGet(p => p.Culture).Returns(CultureInfo.InvariantCulture);
        systemMock.SetupGet(p => p.Decoder).Returns(Encoding.UTF8.GetDecoder());
        systemMock.SetupGet(p => p.TimeProvider).Returns(new FakeTimeProvider());

        Context = systemMock.Object;

        byte[] content = Encoding.UTF8.GetBytes(new Lorem("en").Sentences(20));

        var repoMock = new Mock<ISourceCodeRepository>();
        repoMock.Setup(p => p.Fetch(It.IsAny<StatisticsContext>(), It.IsAny<RepositoryResource>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RepositoryResourceContent("test", content));
        repoMock.Setup(p => p.Search(It.IsAny<StatisticsContext>(), CancellationToken.None))
            .ReturnsAsync([new RepositoryResource("test", "test", "test")]);

        Repository = repoMock.Object;
    }
}
