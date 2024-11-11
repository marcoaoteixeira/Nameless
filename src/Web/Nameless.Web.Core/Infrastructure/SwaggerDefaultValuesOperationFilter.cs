using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nameless.Web.Infrastructure;

public sealed class SwaggerDefaultValuesOperationFilter : IOperationFilter {
    private static readonly string[] ExcludeFromFilter = [Constants.API_VERSION_HEADER_KEY];

    public void Apply(OpenApiOperation operation, OperationFilterContext context) {
        const string defaultResponseKey = "default";

        var apiDescription = context.ApiDescription;

        operation.Deprecated |= apiDescription.IsDeprecated();

        foreach (var responseType in context.ApiDescription.SupportedResponseTypes) {
            var responseKey = responseType.IsDefaultResponse
                ? defaultResponseKey
                : responseType.StatusCode.ToString();

            var response = operation.Responses[responseKey];

            foreach (var contentType in response.Content.Keys) {
                if (responseType.ApiResponseFormats
                                .All(apiResponseFormat
                                         => apiResponseFormat.MediaType != contentType)) {
                    response.Content.Remove(contentType);
                }
            }
        }

        if (operation.Parameters is null) { return; }

        // Exclude list of parameters
        operation.Parameters = operation.Parameters
                                        .Where(parameter => !ExcludeFromFilter.Contains(parameter.Name))
                                        .ToList();

        foreach (var parameter in operation.Parameters) {
            var description = apiDescription.ParameterDescriptions
                                            .First(parameterDescription
                                                       => parameterDescription.Name == parameter.Name);
            
            parameter.Description ??= description.ModelMetadata
                                                 .Description;

            if (parameter.Schema.Default is null &&
                description.DefaultValue is not null &&
                description.DefaultValue is not DBNull &&
                description.ModelMetadata is { } modelMetadata) {
                // REF: https://github.com/Microsoft/aspnet-api-versioning/issues/429#issuecomment-605402330
                var json = JsonSerializer.Serialize(description.DefaultValue, modelMetadata.ModelType);
                parameter.Schema.Default = OpenApiAnyFactory.CreateFromJson(json);
            }

            parameter.Required |= description.IsRequired;
        }
    }
}