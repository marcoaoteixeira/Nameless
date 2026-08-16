using Nameless.Generators.Infrastructure;
using Nameless.Generators.Web.Http.Endpoints.Conventions;
using Nameless.Generators.Web.Http.Endpoints.Models;

namespace Nameless.Generators.Web.Http.Endpoints.Emitters;

public sealed class EndpointGroupEmitter : Emitter<EndpointGroupModel> {
    private const string BUILDER_ARG_NAME = "builder";

    public static EndpointGroupEmitter Instance { get; } = new();

    static EndpointGroupEmitter() { }

    private EndpointGroupEmitter() { }

    protected override void EmitFileContent(CodeWriter cw, EndpointGroupModel model) {
        cw.WriteLine($"namespace {model.Class.Namespace};");

        cw.WriteLine();
        using (cw.Block($"{model.Class.Accessibility} partial class {model.Class.Name} {{")) {
            EmitCreate(cw, model);
        }
    }

    private static void EmitCreate(CodeWriter cw, EndpointGroupModel model) {
        using (cw.Block($"{model.Class.Accessibility} static IEndpointRouteBuilder Create(IEndpointRouteBuilder {BUILDER_ARG_NAME}) {{")) {
            EmitVersionSetBlock(cw, model);

            var result = EmitGroupDefinitionBlock(cw, model);
            
            cw.WriteLine($"return {result};");
        }
    }

    private static void EmitVersionSetBlock(CodeWriter cw, EndpointGroupModel model) {
        const string VersionSetVarName = "vs";

        if (model.ReportVersions.Length <= 0) { return; }

        using (cw.Block($"var {VersionSetVarName} = {BUILDER_ARG_NAME}.NewApiVersionSet()", closing: string.Empty)) {
            foreach (var version in model.ReportVersions) {
                var major = $"majorVersion: {version.Major}";
                var minor = $"minorVersion: {(version.Minor is not null ? version.Minor.ToString() : "null")}";
                var status = $"status: {(!string.IsNullOrWhiteSpace(version.Status) ? $"\"{EscapeStringLiteral(version.Status)}\"" : "null")}";

                cw.WriteLine($".HasApiVersion(new ApiVersion({major}, {minor}, {status}))");
            }

            cw.WriteLine(".ReportApiVersions()");
            cw.WriteLine(".Build();");
        }

        model.Conventions.Add(new Convention(
            call: $".WithApiVersionSet({VersionSetVarName})"
        ));
    }

    private static string EmitGroupDefinitionBlock(CodeWriter cw, EndpointGroupModel model) {
        const string GroupVarName = "result";

        var prefix = !string.IsNullOrWhiteSpace(model.Arguments.Prefix)
            ? $"\"{EscapeStringLiteral(model.Arguments.Prefix)}\""
            : "string.Empty";

        using (cw.Block($"var {GroupVarName} = {BUILDER_ARG_NAME}.MapGroup({prefix})", closing: string.Empty)) {
            if (model.Conventions.Count > 0) {
                var allButLast = model.Conventions.Take(model.Conventions.Count - 1);
                foreach (var convention in allButLast) {
                    cw.WriteLine(convention);
                }

                var last = model.Conventions.LastOrDefault();
                cw.WriteLine(last, closing: ";");
            }
            else { cw.Write(";"); }
        }

        return GroupVarName;
    }
}
