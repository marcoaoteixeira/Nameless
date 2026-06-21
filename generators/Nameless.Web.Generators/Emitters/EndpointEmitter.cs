using Nameless.Web.Generators.Infrastructure;
using Nameless.Web.Generators.Models;

namespace Nameless.Web.Generators.Emitters;

public sealed class EndpointEmitter : Emitter<EndpointModel> {
    public static EndpointEmitter Instance { get; } = new();

    static EndpointEmitter() { }

    private EndpointEmitter() { }

    protected override void EmitFileContent(CodeWriter cw, EndpointModel model) {
        cw.WriteLine($"namespace {model.Class.Namespace};");

        cw.WriteLine();
        using (cw.Block($"{model.Class.Accessibility} partial class {model.Class.Name} {{")) {
            EmitRegister(cw, model);

            cw.WriteLine();

            EmitMap(cw, model);
        }
    }

    private static void EmitRegister(CodeWriter cw, EndpointModel model) {
        using (cw.Block($"{model.Class.Accessibility} static void Register(IServiceCollection services) {{")) {
            cw.WriteLine($"services.TryAddTransient<global::{model.Class.FullName}>();");
        }
    }

    private static void EmitMap(CodeWriter cw, EndpointModel model) {
        const string BuilderArgName = "builder";
        const string EndpointVarName = "_ep_";

        using (cw.Block($"{model.Class.Accessibility} static void Map(IEndpointRouteBuilder {BuilderArgName}) {{")) {
            var routeTemplate = !string.IsNullOrWhiteSpace(model.Arguments.Route)
                ? $"\"{EscapeStringLiteral(model.Arguments.Route)}\""
                : "\"/\"";

            cw.WriteLine($"{BuilderArgName}.Map{model.Arguments.HttpVerb}({routeTemplate}, static async (");
            cw.Indent().Indent();

            var parameters = model.Handler.Parameters.Select(CreateParameterDeclaration).ToList();
            foreach (var parameter in parameters) {
                cw.WriteLine($"{parameter}, ");
            }

            cw.WriteLine($"[FromServices] global::{model.Class.FullName} {EndpointVarName}");
            cw.Dedent();

            var callArgs = string.Join(", ", model.Handler.Parameters.Select(static parameter => parameter.Name));
            cw.WriteLine($") => await {EndpointVarName}.{model.Handler.Method.Name}({callArgs}).ConfigureAwait(false)");
            cw.Dedent();

            cw.Write(")");

            if (model.Conventions.Count > 0) {
                cw.WriteLine();

                var allButLast = model.Conventions.Take(model.Conventions.Count - 1);
                foreach (var convention in allButLast) {
                    cw.WriteLine(convention);
                }

                var last = model.Conventions.LastOrDefault();
                cw.WriteLine(last, closing: ";");
            } else { cw.Write(";"); }
        }
    }

    private static string CreateParameterDeclaration(EndpointHandlerParameterModel model) {
        var bindingName = EscapeStringLiteral(model.BindingName);
        var attrPrefix = model.BindingKind switch {
            ParameterBindingKind.AsParameters => "[AsParameters] ",

            ParameterBindingKind.FromBody => "[FromBody] ",

            ParameterBindingKind.FromForm => "[FromForm] ",

            ParameterBindingKind.FromHeader => string.IsNullOrWhiteSpace(bindingName)
                    ? "[FromHeader] "
                    : $"[FromHeader(Name = \"{bindingName}\")] ",

            ParameterBindingKind.FromQuery => string.IsNullOrWhiteSpace(bindingName)
                    ? "[FromQuery] "
                    : $"[FromQuery(Name = \"{bindingName}\")] ",

            ParameterBindingKind.FromRoute => string.IsNullOrWhiteSpace(bindingName)
                    ? "[FromRoute] "
                    : $"[FromRoute(Name = \"{bindingName}\")] ",

            _ => string.Empty
        };

        return $"{attrPrefix}{model.Type} {model.Name}";
    }
}
