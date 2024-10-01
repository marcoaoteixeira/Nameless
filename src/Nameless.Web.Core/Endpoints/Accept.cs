namespace Nameless.Web.Endpoints;
public sealed record Accept {
    public Type RequestType { get; init; } = typeof(void);
    public string ContentType { get; init; } = string.Empty;
    public string[] AdditionalContentTypes { get; init; } = [];
    public bool IsOptional { get; init; }

    public static Accept Accepts<TRequest>(string contentType, params string[] additionalContentTypes)
        => new() {
            RequestType = typeof(TRequest),
            ContentType = Prevent.Argument.NullOrWhiteSpace(contentType),
            AdditionalContentTypes = additionalContentTypes,
            IsOptional = false
        };

    public static Accept Accepts<TRequest>(string contentType, bool isOptional, params string[] additionalContentTypes)
        => new() {
            RequestType = typeof(TRequest),
            ContentType = Prevent.Argument.NullOrWhiteSpace(contentType),
            AdditionalContentTypes = additionalContentTypes,
            IsOptional = isOptional
        };
}
