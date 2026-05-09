using Microsoft.CodeAnalysis;

namespace Nameless.Web.Http.Endpoints.Generator.Diagnostics;

internal static class DiagnosticDescriptors {
    private const string CATEGORY = "EndpointsGenerator";

    internal static readonly DiagnosticDescriptor MissingEndpointExecutionHandle = new(
        id: "ENDPOINTS002",
        title: $"Missing '{ENDPOINT_EXECUTION_HANDLE_NAME}' method",
        messageFormat: $"Endpoint class '{{0}}' has no '{ENDPOINT_EXECUTION_HANDLE_NAME}' method",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor InvalidVersionString = new(
        id: "ENDPOINTS003",
        title: "Invalid version string",
        messageFormat: "Version string '{0}' on '{1}' is not a valid version (expected formats: '1', '1.0', '1.0.0')",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    // ENDPOINTS004 — reserved.

    internal static readonly DiagnosticDescriptor EndpointExecutionHandleNotAccessible = new(
        id: "ENDPOINTS005",
        title: $"'{ENDPOINT_EXECUTION_HANDLE_NAME}' method not accessible",
        messageFormat: $"Method '{ENDPOINT_EXECUTION_HANDLE_NAME}' on '{{0}}' is not public or internal and will not be accessible from generated code",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor ConflictingAuthAttributes = new(
        id: "ENDPOINTS006",
        title: "Conflicting authorization attributes",
        messageFormat: "Endpoint class '{0}' has both [AllowAnonymous] and [Authorize]; [AllowAnonymous] takes precedence",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor EndpointGroupNotFound = new(
        id: "ENDPOINTS007",
        title: "Group marker class not decorated with [GroupAttribute]",
        messageFormat: "Endpoint '{0}' references group type '{1}' which is not decorated with [GroupAttribute]",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor EndpointVersionNotInGroupVersionSet = new(
        id: "ENDPOINTS008",
        title: "Endpoint version not in group version set",
        messageFormat: "Endpoint '{0}' declares [Version(\"{1}\")] but that version is not in group '{2}' Versions set [{3}]",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor GroupMarkerEmptyName = new(
        id: "ENDPOINTS009",
        title: "Group marker has empty Name",
        messageFormat: "Group marker class '{0}' has an empty or whitespace Name. The Name must be a non-empty string.",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
