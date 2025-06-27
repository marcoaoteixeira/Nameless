using Microsoft.AspNetCore.Http;

namespace Nameless.Web.Endpoints;

/// <summary>
/// Represents a minimal endpoint interface.
/// </summary>
public interface IEndpoint {
    /// <summary>
    /// Configures the endpoint with the specified descriptor.
    /// </summary>
    /// <param name="descriptor">The endpoint descriptor.</param>
    void Configure(IEndpointDescriptor descriptor);
}

public abstract class EndpointBase : IEndpoint {
    /// <inheritdoc />
    public abstract void Configure(IEndpointDescriptor descriptor);

    public static IResult Ok(object? data = null) {
        return TypedResults.Ok(data);
    }

    public static Task<IResult> OkAsync(object? data = null) {
        return Task.FromResult(Ok(data));
    }

    public static IResult BadRequest(object? error = null) {
        return TypedResults.BadRequest(error);
    }

    public static Task<IResult> BadRequestAsync(object? error = null) {
        return Task.FromResult(BadRequest(error));
    }

    public static IResult Problem(string? detail = null,
                                  string? instance = null,
                                  int? statusCode = null,
                                  string? title = null,
                                  string? type = null,
                                  IEnumerable<KeyValuePair<string, object?>>? extensions = null) {
        return TypedResults.Problem(detail, instance, statusCode, title, type, extensions);
    }

    public static Task<IResult> ProblemAsync(string? detail = null,
                                             string? instance = null,
                                             int? statusCode = null,
                                             string? title = null,
                                             string? type = null,
                                             IEnumerable<KeyValuePair<string, object?>>? extensions = null) {
        return Task.FromResult(Problem(detail, instance, statusCode, title, type, extensions));
    }

    public static IResult ValidationProblem(IEnumerable<KeyValuePair<string, string[]>> errors,
                                            string? detail = null,
                                            string? instance = null,
                                            string? title = null,
                                            string? type = null,
                                            IEnumerable<KeyValuePair<string, object?>>? extensions = null) {
        return TypedResults.ValidationProblem(errors, detail, instance, title, type, extensions);
    }

    public static Task<IResult> ValidationProblemAsync(IEnumerable<KeyValuePair<string, string[]>> errors,
                                                       string? detail = null,
                                                       string? instance = null,
                                                       string? title = null,
                                                       string? type = null,
                                                       IEnumerable<KeyValuePair<string, object?>>? extensions = null) {
        return Task.FromResult(ValidationProblem(errors, detail, instance, title, type, extensions));
    }

    public static IResult NotFound(object? value = null) {
        return TypedResults.NotFound(value);
    }

    public static Task<IResult> NotFoundAsync(object? value = null) {
        return Task.FromResult(NotFound(value));
    }
}