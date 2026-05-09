using System.Text;
using Nameless.Web.Http.Endpoints.Generator.Models;

namespace Nameless.Web.Http.Endpoints.Generator.Emitters;

internal static class GroupEmitter {
    internal static void Emit(StringBuilder sb, GroupModel group) {
        var isGrouped = !string.IsNullOrEmpty(group.Name);
        var builderVar = isGrouped ? $"_group_{Sanitize(group.Name)}" : SELF_REF;

        if (isGrouped) {
            EmitVersionSet(sb, group);
            EmitMapGroup(sb, group, builderVar);
        }

        foreach (var endpoint in group.Endpoints) {
            EndpointEmitter.Emit(sb, endpoint, builderVar);
        }
    }

    private static void EmitVersionSet(StringBuilder sb, GroupModel group) {
        if (group.Versions.IsEmpty) { return; }

        var versionSetVar = $"_version_set_{Sanitize(group.Name)}";
        sb.AppendLine($"var {versionSetVar} = {SELF_REF}.NewApiVersionSet()");

        foreach (var version in group.Versions) {
            var method = version.Deprecated ? "HasDeprecatedApiVersion" : "HasApiVersion";
            sb.AppendLine(
                $".{method}(new {FQN.API_VERSION}({version.Major}, {version.Minor}))"
            );
        }

        sb.AppendLine(".ReportApiVersions().Build();");
    }

    private static void EmitMapGroup(StringBuilder sb, GroupModel group, string builderVar) {
        var versionSetVar = !group.Versions.IsEmpty
            ? $"_version_set_{Sanitize(group.Name)}"
            : null;

        sb.Append(
            $"var {builderVar} = {SELF_REF}.MapGroup(\"{EscapeStringLiteral(group.Prefix)}\")"
        );

        if (versionSetVar is not null) {
            sb.Append($".WithApiVersionSet({versionSetVar})");
        }

        sb.AppendLine(";");
    }

    private static string Sanitize(string name) {
        return new string(
            [.. name.Select(static @char => char.IsLetterOrDigit(@char) ? @char : '_')]
        );
    }
}
