using Microsoft.CodeAnalysis;

namespace Nameless.Web.Http.Endpoints.Generator.Diagnostics;

internal static class DiagnosticDescriptors {
    private const string CATEGORY = "AutoEndpointsGenerator";

    internal static readonly DiagnosticDescriptor WrongTargetSymbol = new(
        id: "AEP001",
        title: "Endpoint attribute is applied to wrong target type",
        messageFormat: "Type '{0}' is not a valid type for endpoints",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor MissingEndpointExecutionHandler = new(
        id: "AEP002",
        title: $"Missing '{ENDPOINT_EXECUTION_HANDLER_NAME}' method",
        messageFormat: $"Endpoint class '{{0}}' has no '{ENDPOINT_EXECUTION_HANDLER_NAME}' method",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor InvalidVersionString = new(
        id: "AEP003",
        title: "Invalid version string",
        messageFormat: "Version string '{0}' on '{1}' is not a valid version (expected formats: '1', '1.0', '1.0.0')",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    // AEP004 — reserved.

    internal static readonly DiagnosticDescriptor EndpointExecutionHandlerNotAccessible = new(
        id: "AEP005",
        title: $"'{ENDPOINT_EXECUTION_HANDLER_NAME}' method not accessible",
        messageFormat: $"Method '{ENDPOINT_EXECUTION_HANDLER_NAME}' on '{{0}}' is not public or internal and will not be accessible from generated code",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor ConflictingAuthAttributes = new(
        id: "AEP006",
        title: "Conflicting authorization attributes",
        messageFormat: "'{0}' has both [AllowAnonymous] and [Authorize]; [AllowAnonymous] takes precedence",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor EndpointGroupNotFound = new(
        id: "AEP007",
        title: $"Group marker class not decorated with {ClassNames.ENDPOINT_GROUPING_ATTR}",
        messageFormat: $"Endpoint '{{0}}' references group type '{{1}}' which is not decorated with {ClassNames.ENDPOINT_GROUPING_ATTR}",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor EndpointVersionNotInGroupVersionSet = new(
        id: "AEP008",
        title: "Endpoint version not in group version set",
        messageFormat: "Endpoint '{0}' declares [Version(\"{1}\")] but that version is not in group '{2}' Versions set [{3}]",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor GroupMarkerEmptyName = new(
        id: "AEP009",
        title: "Group marker class has empty Name",
        messageFormat: "Group marker class '{0}' has an empty or whitespace Name. The Name must be a non-empty string.",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor ClassMustBePartial = new(
        id: "AEP010",
        title: "Endpoint or group marker class must be partial",
        messageFormat: "Class '{0}' must be declared as 'partial' for the endpoint generator to emit the companion partial class",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor ClassMustBePublicOrInternal = new(
        id: "AEP011",
        title: "Endpoint or group marker class must be public or internal",
        messageFormat: "Class '{0}' is not public or internal and will not be accessible from generated code",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor MissingEndpointAttribute = new(
        id: "AEP999",
        title: $"Type is missing {ClassNames.ENDPOINT_ATTR_WITH_ARG} annotation",
        messageFormat: $"Type '{{0}}' must be annotated with {ClassNames.ENDPOINT_ATTR_WITH_ARG}",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
