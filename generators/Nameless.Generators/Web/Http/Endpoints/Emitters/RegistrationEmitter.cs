using System.Text;
using Microsoft.CodeAnalysis;
using Nameless.Generators.Shared.Infrastructure;
using Nameless.Generators.Web.Http.Endpoints.Models;

namespace Nameless.Generators.Web.Http.Endpoints.Emitters;

public sealed class RegistrationEmitter : Emitter<EndpointGroupModelCollection> {
    private const string SERVICES_ARG = "services";
    private const string BUILDER_ARG = "builder";
    private const string CONFIG_API_VERSIONING_ARG = "configureApiVersioning";
    private const string CONFIG_API_EXPLORER_ARG = "configureApiExplorer";

    public static RegistrationEmitter Instance { get; } = new();

    static RegistrationEmitter() { }

    private RegistrationEmitter() { }

    public override void Emit(SourceProductionContext context, DiagnosticAwareResult<EndpointGroupModelCollection> result, bool prettify) {
        base.Emit(context, result, prettify);

        EmitAutoEndpointFiles(
            context,
            result.Model ?? []
        );
    }

    protected override void WriteFileContent(CodeWriter cw, EndpointGroupModelCollection model) {
        cw.WriteLine();
        cw.WriteLine($"namespace {model.Class.Namespace};");

        cw.WriteLine();
        using (cw.Block($"public static class {model.Class.Name} {{")) {
            WriteRegisterHandler(cw, model);
            WriteMapHandler(cw, model);
        }
    }

    private static void EmitAutoEndpointFiles(SourceProductionContext context, EndpointGroupModelCollection model)
    {
        // Emit endpoint partial classes
        foreach (var grouping in model) {
            foreach (var endpoint in grouping.Endpoints) {
                EndpointEmitter.Instance.Emit(context, endpoint);
            }
            
            // Emit endpoint group partial class
            EndpointGroupEmitter.Instance.Emit(context, grouping);
        }
    }

    private static void WriteRegisterHandler(CodeWriter cw, EndpointGroupModelCollection model) {
        var opening = new StringBuilder();

        cw.WriteLine();

        opening.Append($"public static IServiceCollection {Project.RegisterHandlerName}(");
        opening.Append($"this IServiceCollection {SERVICES_ARG}, ");
        opening.Append($"Action<ApiVersioningOptions>? {CONFIG_API_VERSIONING_ARG} = null, ");
        opening.Append($"Action<ApiExplorerOptions>? {CONFIG_API_EXPLORER_ARG} = null");
        opening.Append(") {");

        using (cw.Block(opening.ToString())) {
            foreach (var group in model) {
                foreach (var endpoint in group.Endpoints) {
                    cw.WriteLine($"global::{endpoint.Class.FullName}.Register({SERVICES_ARG});");
                }
            }

            WriteApiVersioningConfigurationInstructions(cw);

            cw.WriteLine($"return {SERVICES_ARG};");
        }

        WriteDefaultApiVersioningConfigurationInstructions(cw);
        WriteDefaultApiExplorerConfigurationInstructions(cw);
    }

    private static void WriteApiVersioningConfigurationInstructions(CodeWriter cw) {
        cw.WriteLine();

        using (cw.Block(SERVICES_ARG, closing: string.Empty)) {
            cw.WriteLine($".AddApiVersioning({CONFIG_API_VERSIONING_ARG} ?? DefaultApiVersioningConfiguration)");
            cw.WriteLine($".AddApiExplorer({CONFIG_API_EXPLORER_ARG} ?? DefaultApiExplorerConfiguration);");
        }
    }

    private static void WriteDefaultApiVersioningConfigurationInstructions(CodeWriter cw) {
        cw.WriteLine();

        using (cw.Block("private static void DefaultApiVersioningConfiguration(ApiVersioningOptions options) {")) {
            cw.WriteLine("options.ReportApiVersions = true;");
            cw.WriteLine("options.AssumeDefaultVersionWhenUnspecified = true;");
            cw.WriteLine("options.DefaultApiVersion = new ApiVersion(majorVersion: 1);");

            using (cw.Block("options.ApiVersionReader = ApiVersionReader.Combine(", closing: ");")) {
                cw.WriteLine("new UrlSegmentApiVersionReader(),");
                cw.WriteLine("new HeaderApiVersionReader(\"api-version\")");
            }
        }
    }

    private static void WriteDefaultApiExplorerConfigurationInstructions(CodeWriter cw) {
        cw.WriteLine();

        using (cw.Block("private static void DefaultApiExplorerConfiguration(ApiExplorerOptions options) {")) {
            cw.WriteLine("options.GroupNameFormat = \"'v'VVV\";");
            cw.WriteLine("options.SubstituteApiVersionInUrl = false;");
        }
    }

    private static void WriteMapHandler(CodeWriter cw, EndpointGroupModelCollection model) {
        cw.WriteLine();

        using (cw.Block($"public static IEndpointRouteBuilder {Project.MapMethodName}(this IEndpointRouteBuilder {BUILDER_ARG}) {{")) {
            foreach (var endpointGroup in model) {
                WriteEndpointGroupMapInstructions(cw, endpointGroup);
            }

            cw.WriteLine($"return {BUILDER_ARG};");
        }
    }

    private static void WriteEndpointGroupMapInstructions(CodeWriter cw, EndpointGroupModel endpointGroup) {
        var groupVar = $"_g_{Sanitize(endpointGroup.Class.FullName)}";
        cw.WriteLine($"var {groupVar} = global::{endpointGroup.Class.FullName}.Create({BUILDER_ARG});");

        foreach (var endpoint in endpointGroup.Endpoints) {
            cw.WriteLine($"global::{endpoint.Class.FullName}.Map({groupVar});");
        }

        cw.WriteLine();
    }
}
