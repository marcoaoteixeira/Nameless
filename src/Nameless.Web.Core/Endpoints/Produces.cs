using System.Net;

namespace Nameless.Web.Endpoints;

public sealed record Produces {
    public HttpStatusCode StatusCode { get; init; } = HttpStatusCode.OK;
    
    public Type ResponseType { get; init; } = typeof(void);
    
    public string ContentType { get; init; } = string.Empty;
    
    public string[] AdditionalContentTypes { get; init; } = [];
    
    public ProducesType Type { get; init; }

    public static Produces Result<TType>(HttpStatusCode statusCode = HttpStatusCode.OK,
                                         string contentType = "application/json")
        => new() {
            ResponseType = typeof(TType),
            StatusCode = statusCode,
            ContentType = Prevent.Argument.Null(contentType)
        };

    public static Produces Result(Type responseType,
                                  HttpStatusCode statusCode = HttpStatusCode.OK,
                                  string contentType = "application/json")
        => new() {
            ResponseType = Prevent.Argument.Null(responseType),
            StatusCode = statusCode,
            ContentType = Prevent.Argument.Null(contentType)
        };

    public static Produces Problem(HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
                                 string contentType = "application/problem+json")
        => new() {
            StatusCode = statusCode,
            ContentType = Prevent.Argument.Null(contentType)
        };

    public static Produces ValidationProblem(HttpStatusCode statusCode = HttpStatusCode.BadRequest,
                                             string contentType = "application/problem+json")
        => new() {
            StatusCode = statusCode,
            ContentType = Prevent.Argument.Null(contentType)
        };
}