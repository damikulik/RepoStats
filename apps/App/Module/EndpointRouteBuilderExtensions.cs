using RepoStats.Application;

namespace RepoStats.AppHost.Module;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointConventionBuilder MapRepoStatsEndpoints(this IEndpointRouteBuilder app)
        => app.MapGet(
            "letter-occurences",
            async (IRepoStatisticsService service, HttpContext http, CancellationToken token)
            =>
            {
                var result = await service.GetLetterOccurences(token);

                if (result.IsError)
                {
                    if (result.FirstError.Metadata is Dictionary<string, object> metadata
                        && metadata.TryGetValue("RetryAfter", out object? retryDelay)
                        && retryDelay is int delay)
                    {
                        http.Response.Headers.RetryAfter = $"{delay}";
                    }

                    return Results.Problem(detail: result.FirstError.Description, statusCode: result.FirstError.NumericType);
                }

                return Results.Ok(result.Value);
            })
        .WithTags("stats")
        .WithName("GetLetterOccurences")
        .RequireRateLimiting("fixed")
        .RequireRateLimiting("basic");
}
