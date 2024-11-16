using ErrorOr;

namespace RepoStats.Application;

public interface IRepoStatisticsService
{
    Task<ErrorOr<CharacterOccurencesView>> GetLetterOccurences(CancellationToken token);
}
