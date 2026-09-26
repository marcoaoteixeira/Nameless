// QueryOperationDocumentTransformer.cs
//
// PROBLEM
// -------
// On .NET 10, Microsoft.AspNetCore.OpenApi's document generator silently skips
// endpoints mapped to the HTTP QUERY method: its internal method-mapping code only
// knows how to convert ApiDescription.HttpMethod values for the classic verbs
// (GET/POST/PUT/DELETE/PATCH/HEAD/OPTIONS/TRACE) into OpenApiOperation entries.
// Native support lands in .NET 11 (dotnet/aspnetcore#65714), gated behind OpenAPI 3.2.
//
// KEY INSIGHT
// -----------
// The *binding* side of the pipeline is already method-agnostic. ASP.NET Core's
// EndpointMetadataApiDescriptionProvider builds a full ApiDescription -- parameters,
// binding sources, CLR types, request/response formats -- for ANY verb string found
// in the endpoint's HttpMethodMetadata, QUERY included. That ApiDescription is exactly
// what's available on OpenApiDocumentTransformerContext.DescriptionGroups.
//
// So instead of hand-rolling parameter/schema extraction from RouteEndpoint metadata,
// this transformer reuses those already-correct ApiDescription objects and does the
// last-mile ApiDescription -> OpenApiOperation conversion that the built-in generator
// currently declines to do for QUERY.
//
// USAGE
// -----
//   builder.Services.AddOpenApi(options =>
//   {
//       options.AddDocumentTransformer<QueryOperationDocumentTransformer>();
//   });
//
// VERIFIED AGAINST
// ----------------
// Microsoft.AspNetCore.OpenApi 10.0.x / Microsoft.OpenApi 2.0.0, compiled against
// net10.0 with `dotnet build` (not hand-guessed) to confirm every type/property/
// method used below actually exists with this shape.
//
// LIMITATIONS
// -----------
// - On OpenAPI 3.0/3.1 documents (the .NET 10 default), "query" is not a spec-legal
//   PathItem property. This transformer writes it directly into PathItem.Operations
//   anyway, because that's what renders as a testable operation in tools like Scalar
//   today. This is intentionally NOT strict-spec-compliant for 3.0/3.1. Set
//   AlsoEmitSpecCompliantExtension = true for an additional x-oai-additionalOperations
//   mirror matching the shape the official .NET 11 fix uses -- useful for downstream
//   tooling that specifically reads that extension, though most UIs (Scalar included,
//   as of this writing) won't render it as an interactive operation.
// - [FromServices]/DI-bound and other non-API-surface parameters are excluded,
//   matching built-in generator behavior.

using System.Text.Json;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.OpenApi;

namespace Nameless.Web.OpenApi;

public sealed class QueryOperationDocumentTransformer : IOpenApiDocumentTransformer {
    /// <summary>
    ///     If true, also mirrors each QUERY operation into an
    ///     "x-oai-additionalOperations" extension on the PathItem, matching
    ///     the shape the official .NET 11 fix uses for pre-3.2 documents.
    ///     Off by default -- see LIMITATIONS above.
    /// </summary>
    public bool AlsoEmitSpecCompliantExtension { get; set; }

    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken) {
        var queryDescriptions = context.DescriptionGroups
            .SelectMany(group => group.Items)
            .Where(description => string.Equals(description.HttpMethod, HttpMethod.Query.Method, StringComparison.OrdinalIgnoreCase));

        // Seed from whatever the built-in generator already registered for
        // the other verbs in this same document build, so we don't emit
        // duplicate tag entries.
        var knownTagNames = new HashSet<string>(
            collection: ExtractKnownTags(document),
            comparer: StringComparer.Ordinal
        );

        foreach (var description in queryDescriptions) {
            var path = NormalizePath(description.RelativePath);

            if (!document.Paths.TryGetValue(path, out var pathItem)) {
                // A freshly constructed OpenApiPathItem does NOT initialize
                // Operations -- it's null until something sets it. Must be
                // given a dictionary up front.
                pathItem = new OpenApiPathItem { Operations = [] };
                document.Paths.Add(path, pathItem);
            }
            else if (pathItem is OpenApiPathItem { Operations: null } existingConcrete) {
                // Defensive: an existing entry (e.g. contributed by another
                // transformer) could theoretically still have a null
                // Operations dictionary.
                existingConcrete.Operations = [];
            }

            var operation = await BuildOperationAsync(description, path, document, knownTagNames, context, cancellationToken);

            // Operations has no setter on the IOpenApiPathItem interface, but
            // the Dictionary instance it returns is mutable once initialized
            // above.
            pathItem.Operations?[HttpMethod.Query] = operation;

            if (AlsoEmitSpecCompliantExtension) {
                AddAdditionalOperationsExtension(pathItem, operation);
            }
        }
    }

    private static IEnumerable<string> ExtractKnownTags(OpenApiDocument document) {
        return document.Tags is not null
            ? document.Tags.Where(tag => tag.Name is not null).Select(tag => tag.Name).OfType<string>()
            : [];
    }

    private static async Task<OpenApiOperation> BuildOperationAsync(ApiDescription description, string path, OpenApiDocument document, HashSet<string> knownTagNames, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken) {
        var operation = new OpenApiOperation {
            OperationId = BuildOperationId(path),
            Summary = TryGetEndpointMetadata<IEndpointSummaryMetadata>(description)?.Summary ?? $"{HttpMethod.Query.Method} {path}",
            Description = TryGetEndpointMetadata<IEndpointDescriptionMetadata>(description)?.Description
                ?? "Safe, idempotent query operation using the HTTP QUERY method.",
            Tags = BuildTags(description, path, document, knownTagNames),
        };

        await AddParametersAsync(operation, description, context, cancellationToken);
        await AddRequestBodyAsync(operation, description, context, cancellationToken);
        await AddResponsesAsync(operation, description, context, cancellationToken);

        return operation;
    }

    private static async Task AddParametersAsync(OpenApiOperation operation, ApiDescription description, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken) {
        foreach (var parameter in description.ParameterDescriptions) {
            // Body goes into RequestBody, not Parameters.
            // Service/special-bound values (e.g. [FromServices],
            // CancellationToken, HttpContext) aren't part of the public
            // API surface -- the built-in generator skips these too.
            if (parameter.Source == BindingSource.Body ||
                parameter.Source == BindingSource.Services ||
                parameter.Source == BindingSource.Special) { continue; }

            var location = parameter.Source.Id switch {
                nameof(ParameterLocation.Query) => ParameterLocation.Query,
                nameof(ParameterLocation.Path) => ParameterLocation.Path,
                nameof(ParameterLocation.Header) => ParameterLocation.Header,
                _ => ParameterLocation.Query,
            };

            var schema = await context.GetOrCreateSchemaAsync(parameter.Type, parameter, cancellationToken);

            operation.Parameters ??= [];
            operation.Parameters.Add(new OpenApiParameter {
                Name = parameter.Name,
                In = location,
                Required = location == ParameterLocation.Path || parameter.IsRequired,
                Schema = schema,
            });
        }
    }

    private static async Task AddRequestBodyAsync(OpenApiOperation operation, ApiDescription description, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken) {
        var bodyParameter = description.ParameterDescriptions.FirstOrDefault(parameter => parameter.Source == BindingSource.Body);
        if (bodyParameter is null) { return; }

        var schema = await context.GetOrCreateSchemaAsync(bodyParameter.Type, bodyParameter, cancellationToken);

        var mediaTypes = description.SupportedRequestFormats
            .Select(format => format.MediaType)
            .Where(mediaType => !string.IsNullOrEmpty(mediaType))
            .Distinct()
            .DefaultIfEmpty("application/json");

        operation.RequestBody = new OpenApiRequestBody {
            Required = true,
            Content = mediaTypes.ToDictionary(
                keySelector: mediaType => mediaType,
                elementSelector: _ => new OpenApiMediaType { Schema = schema }
            ),
        };
    }

    private static async Task AddResponsesAsync(OpenApiOperation operation, ApiDescription description, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken) {
        operation.Responses = [];

        foreach (var responseType in description.SupportedResponseTypes) {
            var statusCodeKey = responseType.IsDefaultResponse
                ? "default"
                : responseType.StatusCode.ToString();

            var response = new OpenApiResponse {
                Description = ReasonPhrases.GetReasonPhrase(responseType.StatusCode) is { Length: > 0 } phrase ? phrase : "Response",
            };

            if (responseType.Type is not null && responseType.Type != typeof(void)) {
                var schema = await context.GetOrCreateSchemaAsync(responseType.Type, null, cancellationToken);

                var mediaTypes = responseType.ApiResponseFormats
                    .Select(format => format.MediaType)
                    .Where(mediaType => !string.IsNullOrEmpty(mediaType))
                    .Distinct()
                    .DefaultIfEmpty("application/json");

                response.Content = mediaTypes.ToDictionary(
                    keySelector: mediaType => mediaType,
                    elementSelector: _ => new OpenApiMediaType { Schema = schema }
                );
            }

            operation.Responses[statusCodeKey] = response;
        }

        if (operation.Responses.Count == 0) {
            operation.Responses["200"] = new OpenApiResponse { Description = "Success" };
        }
    }

    /// <summary>
    ///     Mirrors the operation into an x-oai-additionalOperations
    ///     extension, matching the fallback shape ASP.NET Core itself uses
    ///     for QUERY on pre-3.2 documents. Only invoked when
    ///     <see cref="AlsoEmitSpecCompliantExtension"/> is true.
    /// </summary>
    private static void AddAdditionalOperationsExtension(IOpenApiPathItem pathItem, OpenApiOperation operation) {
        // Serialize just enough of the operation to a JSON tree. This mirrors
        // the shape without depending on internal ASP.NET Core serialization
        // helpers.
        var summary = new {
            query = new {
                operationId = operation.OperationId,
                summary = operation.Summary,
                description = operation.Description,
            }
        };

        var node = JsonSerializer.SerializeToNode(summary);

        if (node is null) { return; }

        if (pathItem is not OpenApiPathItem concretePathItem) { return; }

        concretePathItem.Extensions ??= new Dictionary<string, IOpenApiExtension>();
        concretePathItem.Extensions["x-oai-additionalOperations"] = new JsonNodeExtension(node);
    }

    private static string NormalizePath(string? relativePath) {
        var path = $"/{(relativePath ?? string.Empty).TrimStart('/')}";

        // Strip route constraints like {id:int} -> {id}, since OpenAPI path
        // templates don't carry ASP.NET Core routing constraint syntax.
        return RegexCache.RouteParameterPattern().Replace(path, "{$1}");
    }

    private static string BuildOperationId(string path) {
        var slug = RegexCache.OpenApiOperationIdSlugPattern().Replace(path.Trim('/'), "_").Trim('_');

        return string.IsNullOrEmpty(slug) ? "Query_Root" : $"Query_{slug}";
    }

    // OpenApiTag/OpenApiTagReference have no Equals/GetHashCode overrides
    // (confirmed via reflection against the real Microsoft.OpenApi 2.0.0
    // assembly), so document.Tags can't self-dedupe by name -- we track known
    // tag names ourselves. This set MUST be scoped to a single TransformAsync
    // call (passed down, not a static/instance field): the document is
    // rebuilt on every request in dev, and can be rebuilt per named document
    // in general, so caching across calls would cause tags to go missing from
    // any document built after the first one.
    private static HashSet<OpenApiTagReference> BuildTags(ApiDescription description, string path, OpenApiDocument document, HashSet<string> knownTagNames) {
        var tagNames = TryGetEndpointMetadata<ITagsMetadata>(description)?.Tags.ToList();

        if (tagNames is null || tagNames.Count == 0) {
            var firstSegment = path.Trim('/').Split('/').FirstOrDefault();
            tagNames = string.IsNullOrEmpty(firstSegment) ? [] : [firstSegment];
        }

        document.Tags ??= new HashSet<OpenApiTag>();

        var result = new HashSet<OpenApiTagReference>();

        foreach (var tagName in tagNames) {
            if (knownTagNames.Add(tagName)) {
                document.Tags.Add(new OpenApiTag { Name = tagName });
            }

            result.Add(new OpenApiTagReference(tagName, document));
        }

        return result;
    }

    private static T? TryGetEndpointMetadata<T>(ApiDescription description) where T : class {
        return description.ActionDescriptor.EndpointMetadata.OfType<T>().LastOrDefault();
    }
}
