using System.Text;
using Microsoft.CodeAnalysis;
using Nameless.Web.Generators.Infrastructure;
using Nameless.Web.Generators.Models;

namespace Nameless.Web.Generators.Emitters;

public sealed class RegistrationEmitter : Emitter<EndpointGroupModelCollection> {
    private const string SERVICES_ARG = "services";
    private const string BUILDER_ARG = "builder";
    private const string CONFIG_API_VERSIONING_ARG = "configureApiVersioning";
    private const string CONFIG_API_EXPLORER_ARG = "configureApiExplorer";

    public static RegistrationEmitter Instance { get; } = new();

    static RegistrationEmitter() { }

    private RegistrationEmitter() { }

    public void Emit(SourceProductionContext context, DiagnosticAwareResult<EndpointGroupModelCollection> input) {
        var (groups, diagnostics) = (input.Model ?? [], input.Diagnostics);

        foreach (var diagnostic in diagnostics) {
            context.ReportDiagnostic(diagnostic.ToDiagnostic());
        }

        if (groups.Count == 0) { return; }

        EmitAutoEndpointFiles(context, groups);

        var @class = Emit(groups);

        context.AddSource(@class.HintName, @class.SourceCode);
    }

    protected override void EmitFileContent(CodeWriter cw, EndpointGroupModelCollection model) {
        cw.WriteLine();
        cw.WriteLine($"namespace {Project.Namespaces.Root};");

        cw.WriteLine();
        using (cw.Block($"public static class {Project.RegistrationClassName} {{")) {
            EmitRegisterEndpointsMethod(cw, model);
            EmitMapEndpointsMethod(cw, model);
        }
    }

    private static void EmitAutoEndpointFiles(SourceProductionContext context, EndpointGroupModelCollection model)
    {
        // Emit endpoint partial classes
        foreach (var grouping in model) {
            foreach (var endpoint in grouping.Endpoints) {
                var endpointClass = EndpointEmitter.Instance.Emit(endpoint);
                context.AddSource(endpointClass.HintName, endpointClass.SourceCode);
            }
            
            // Emit endpoint group partial class
            var endpointGroupClass = EndpointGroupEmitter.Instance.Emit(grouping);
            context.AddSource(endpointGroupClass.HintName, endpointGroupClass.SourceCode);
        }
    }

    private static void EmitRegisterEndpointsMethod(CodeWriter cw, EndpointGroupModelCollection model) {
        var opening = new StringBuilder();

        cw.WriteLine();

        opening.Append($"public static IServiceCollection {Project.RegisterMethodName}(");
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

            EmitApiVersioningConfiguration(cw);

            cw.WriteLine($"return {SERVICES_ARG};");
        }

        EmitDefaultApiVersioningConfiguration(cw);
        EmitDefaultApiExplorerConfiguration(cw);
    }

    private static void EmitApiVersioningConfiguration(CodeWriter cw) {
        cw.WriteLine();

        using (cw.Block(SERVICES_ARG, closing: string.Empty)) {
            cw.WriteLine($".AddApiVersioning({CONFIG_API_VERSIONING_ARG} ?? DefaultApiVersioningConfiguration)");
            cw.WriteLine($".AddApiExplorer({CONFIG_API_EXPLORER_ARG} ?? DefaultApiExplorerConfiguration);");
        }
    }

    private static void EmitDefaultApiVersioningConfiguration(CodeWriter cw) {
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

    private static void EmitDefaultApiExplorerConfiguration(CodeWriter cw) {
        cw.WriteLine();

        using (cw.Block("private static void DefaultApiExplorerConfiguration(ApiExplorerOptions options) {")) {
            cw.WriteLine("options.GroupNameFormat = \"'v'VVV\";");
            cw.WriteLine("options.SubstituteApiVersionInUrl = false;");
        }
    }

    private static void EmitMapEndpointsMethod(CodeWriter cw, EndpointGroupModelCollection model) {
        cw.WriteLine();

        using (cw.Block($"public static IEndpointRouteBuilder {Project.MapMethodName}(this IEndpointRouteBuilder {BUILDER_ARG}) {{")) {
            foreach (var endpointGroup in model) {
                EmitEndpointGroupMapping(cw, endpointGroup);
            }

            cw.WriteLine($"return {BUILDER_ARG};");
        }
    }

    private static void EmitEndpointGroupMapping(CodeWriter cw, EndpointGroupModel endpointGroup) {
        var groupVar = $"_g_{Sanitize(endpointGroup.Class.FullName)}";
        cw.WriteLine($"var {groupVar} = global::{endpointGroup.Class.FullName}.Create({BUILDER_ARG});");

        foreach (var endpoint in endpointGroup.Endpoints) {
            cw.WriteLine($"global::{endpoint.Class.FullName}.Map({groupVar});");
        }

        cw.WriteLine();
    }
}
