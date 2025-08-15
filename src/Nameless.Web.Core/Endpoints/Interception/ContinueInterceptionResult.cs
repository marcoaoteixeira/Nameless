using Microsoft.AspNetCore.Http;

namespace Nameless.Web.Endpoints.Interception;

public static class InterceptorResult {
    public static IResult Continue() {
        return ContinueInterceptionResult.Instance;
    }
}

/// <summary>
///     Represents a proceed with interception result.
/// </summary>
public sealed class ContinueInterceptionResult : IResult {
    public static IResult Instance { get; } = new ContinueInterceptionResult();

    // Explicit static constructor to tell the C# compiler
    // not to mark type as beforefieldinit
    static ContinueInterceptionResult() {

    }

    private ContinueInterceptionResult() {

    }

    /// <inheritdoc />
    public Task ExecuteAsync(HttpContext httpContext) {
        return Task.CompletedTask;
    }
}
