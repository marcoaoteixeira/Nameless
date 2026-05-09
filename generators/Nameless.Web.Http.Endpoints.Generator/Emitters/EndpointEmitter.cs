using System.Text;
using Nameless.Web.Http.Endpoints.Generator.Models;

namespace Nameless.Web.Http.Endpoints.Generator.Emitters;

internal static class EndpointEmitter {
    internal static void Emit(StringBuilder sb, EndpointModel endpoint, string builderVar) {
        var endpointVar = $"_ep_{endpoint.ClassName}";

        var routeTemplate = EscapeStringLiteral(endpoint.Metadata.RouteTemplate);
        sb.AppendLine(
            $"{builderVar}.Map{endpoint.Metadata.HttpMethod}(\"{routeTemplate}\", static async ("
        );

        // Forward all HandleAsync parameters as-is
        var parameters = endpoint.Parameters
                                 .Select(BuildParameterDeclaration)
                                 .ToList();

        // Append the endpoint class itself, resolved from DI
        parameters.Add($"[{FQN.FROM_SERVICES}] global::{endpoint.FullClassName} {endpointVar}");
        sb.AppendLine(string.Join(", ", parameters));

        sb.AppendLine(") =>");

        // Build the endpoint handle method call arguments (exclude the endpoint var)
        var callArgs = string.Join(", ", endpoint.Parameters.Select(static parameter => parameter.Name));
        sb.Append($"await {endpointVar}.HandleAsync({callArgs}).ConfigureAwait(false))");

        // Chained metadata calls
        foreach (var version in endpoint.Versions) {
            sb.Append($".MapToApiVersion(new {FQN.API_VERSION}({version.Major}, {version.Minor}))");
        }

        foreach (var produces in endpoint.Produces) {
            string? contentType = null;

            switch (produces.Kind) {
                case ProducesKind.Response:
                    contentType = produces.ContentType ?? "application/json";
                    sb.Append($".Produces<{produces.FullTypeName}>(statusCode: {produces.StatusCode}, contentType: \"{contentType}\")");
                    break;
                case ProducesKind.Problem:
                    contentType = produces.ContentType ?? "application/problem+json";
                    sb.Append($".ProducesProblem(statusCode: {produces.StatusCode}, contentType: \"{contentType}\")");
                    break;
                case ProducesKind.ValidationProblem:
                    contentType = produces.ContentType ?? "application/problem+json";
                    sb.AppendLine($".ProducesValidationProblem(statusCode: {produces.StatusCode}, contentType: \"{contentType}\")");
                    break;
            }
        }

        if (endpoint.AcceptsTypeName is not null) {
            sb.Append($".Accepts<{endpoint.AcceptsTypeName}>(");

            if (endpoint.AcceptsContentType is not null) {
                sb.Append($"\"{EscapeStringLiteral(endpoint.AcceptsContentType)}\"");
            }

            sb.Append(')');
        }

        foreach (var filter in endpoint.FilterTypeNames) {
            sb.Append($".AddEndpointFilter<{filter}>()");
        }

        if (endpoint.RequiresAuthorization) {
            sb.Append(".RequireAuthorization(");

            if (endpoint.AuthorizationPolicy is not null) {
                sb.Append($"\"{EscapeStringLiteral(endpoint.AuthorizationPolicy)}\"");
            }

            sb.Append(')');
        }

        if (endpoint.AllowAnonymous) {
            sb.AppendLine(".AllowAnonymous()");
        }

        if (endpoint.CorsPolicy is not null) {
            sb.Append($".RequireCors(\"{EscapeStringLiteral(endpoint.CorsPolicy)}\")");
        }

        if (endpoint.RateLimitingPolicy is not null) {
            sb.Append($".RequireRateLimiting(\"{EscapeStringLiteral(endpoint.RateLimitingPolicy)}\")");
        }

        if (endpoint.OutputCachePolicy is not null) {
            sb.Append($".CacheOutput(\"{EscapeStringLiteral(endpoint.OutputCachePolicy)}\")");
        }

        if (endpoint.RequestTimeoutPolicy is not null) {
            sb.Append($".WithRequestTimeout(\"{EscapeStringLiteral(endpoint.RequestTimeoutPolicy)}\")");
        }

        if (endpoint.DisableHttpMetrics) {
            sb.AppendLine(".DisableHttpMetrics()");
        }

        if (endpoint.UseAntiforgery) {
            sb.AppendLine($".WithMetadata(new {FQN.REQUIRE_ANTIFORGERY_TOKEN_ATTRIBUTE}())");
        }

        if (endpoint.Summary is not null) {
            sb.Append($".WithSummary(\"{EscapeStringLiteral(endpoint.Summary)}\")");
        }

        if (endpoint.Description is not null) {
            sb.Append($".WithDescription(\"{EscapeStringLiteral(endpoint.Description)}\")");
        }

        var endpointDisplayName = endpoint.Metadata.EndpointName ?? endpoint.ClassName;
        sb.Append($".WithName(\"{EscapeStringLiteral(endpointDisplayName)}\")");

        if (!endpoint.Metadata.Tags.IsEmpty) {
            var tagLiterals = string.Join(
                separator: ", ",
                values: endpoint.Metadata.Tags.Select(static tag => $"\"{EscapeStringLiteral(tag)}\"")
            );
            sb.Append($".WithTags({tagLiterals})");
        }

        sb.AppendLine(";");
    }

    private static string BuildParameterDeclaration(ParameterModel param) {
        var bindingName = EscapeStringLiteral(param.BindingName);
        var attrPrefix = param.BindingKind switch {
            ParameterBindingKind.FromBody => $"[{FQN.FROM_BODY}] ",

            ParameterBindingKind.FromRoute => string.IsNullOrWhiteSpace(bindingName)
                    ? $"[{FQN.FROM_ROUTE}] "
                    : $"[{FQN.FROM_ROUTE}(Name = \"{bindingName}\")] ",

            ParameterBindingKind.FromQuery => string.IsNullOrWhiteSpace(bindingName)
                    ? $"[{FQN.FROM_QUERY}] "
                    : $"[{FQN.FROM_QUERY}(Name = \"{bindingName}\")] ",

            ParameterBindingKind.FromHeader => string.IsNullOrWhiteSpace(bindingName)
                    ? $"[{FQN.FROM_HEADER}] "
                    : $"[{FQN.FROM_HEADER}(Name = \"{bindingName}\")] ",

            ParameterBindingKind.AsParameters => $"[{FQN.AS_PARAMETERS}] ",

            _ => string.Empty
        };

        return $"{attrPrefix}{param.FullTypeName} {param.Name}";
    }
}
