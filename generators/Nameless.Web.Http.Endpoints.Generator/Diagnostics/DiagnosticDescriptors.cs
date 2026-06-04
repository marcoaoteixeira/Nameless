using Microsoft.CodeAnalysis;

namespace Nameless.Web.Http.Endpoints.Generator.Diagnostics;

internal static class DiagnosticDescriptors {
    private const string CATEGORY = "AutoEndpointsGenerator";

    #region Common diagnostics

    internal static readonly DiagnosticDescriptor InvalidContextTargetSymbol = new(
        id: "AEP001",
        title: "Type Symbol",
        messageFormat: "Symbol '{0}' is not a valid named type symbol",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor ClassTypeModifierMustBePartial = new(
        id: "AEP002",
        title: "Class type modifier must be partial",
        messageFormat: "Class '{0}' must be declared as 'partial' for the endpoint generator to properly emit the companion class",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor ClassTypeModifierMustNotBeAbstract = new(
        id: "AEP003",
        title: "Class must not be abstract",
        messageFormat: "Class '{0}' must not be declared as 'abstract' since it needs to be instantiated eventually",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor ClassAccessorModifierMustBePublicOrInternal = new(
        id: "AEP004",
        title: "Class accessor modifier must be internal or internal",
        messageFormat: "Class '{0}' must be declared as 'internal' or 'public' for the endpoint generated companion class be accessible to other components",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor ConflictingEndpointVsEndpointGroupAttributes = new(
        id: "AEP005",
        title: "Conflicting endpoint/group attributes",
        messageFormat: $"Class '{{0}}' has both '{Project.Classes.Names.EndpointAttribute}' and '{Project.Classes.Names.EndpointGroupAttribute}'; Class must be marked with only one of them",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor ConflictingAllowAnonymousVsUseAuthorizationAttributes = new(
        id: "AEP006",
        title: "Conflicting authorization attributes",
        messageFormat: "Class '{0}' has both [AllowAnonymous] and [UseAuthorization]; [AllowAnonymous] takes precedence",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor ConflictingDisableCookieRedirectVsAllowCookieRedirectAttributes = new(
        id: "AEP007",
        title: "Conflicting cookie redirect attributes",
        messageFormat: "Class '{0}' has both [DisableCookieRedirect] and [AllowCookieRedirect]; [AllowCookieRedirect] takes precedence",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor ConflictingDisableCorsVsUseCorsAttributes = new(
        id: "AEP008",
        title: "Conflicting CORS attributes",
        messageFormat: "Class '{0}' has both [DisableCors] and [UseCors]; [DisableCors] takes precedence",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor ConflictingDisableOutputCacheVsUseOutputCacheAttributes = new(
        id: "AEP009",
        title: "Conflicting output cache attributes",
        messageFormat: "Class '{0}' has both [DisableOutputCache] and [UseOutputCache]; [DisableOutputCache] takes precedence and places a policy preventing any cache logic",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor ConflictingDisableRateLimitingVsUseRateLimitingAttributes = new(
        id: "AEP010",
        title: "Conflicting rate limiting attributes",
        messageFormat: "Class '{0}' has both [DisableRateLimiting] and [UseRateLimiting]; [DisableRateLimiting] takes precedence and places a policy preventing any rate limiting logic",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor ConflictingDisableRequestTimeoutVsUseRequestTimeoutAttributes = new(
        id: "AEP011",
        title: "Conflicting request timeout attributes",
        messageFormat: "Class '{0}' has both [DisableRequestTimeout] and [UseRequestTimeout]; [DisableRequestTimeout] takes precedence and places a policy preventing any request timeout logic",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    #endregion

    #region Endpoint diagnostics

    internal static readonly DiagnosticDescriptor ClassMissingEndpointAttribute = new(
        id: "AEP101",
        title: "Class missing endpoint attribute",
        messageFormat: $"Class '{{0}}' must be marked with '{Project.Classes.Names.EndpointAttribute}' to be considered an endpoint",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor ClassMustDeclareHandlerMethod = new(
        id: "AEP102",
        title: "Class must declare handler method",
        messageFormat: $"Endpoint class '{{0}}' must declare handler '{EndpointClass.HandlerMethodName}'",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor ClassHandlerMethodMustBePublicOrInternal = new(
        id: "AEP103",
        title: "Class handler method must be public or internal",
        messageFormat: "Endpoint class '{0}' handler must be declared as 'public' or 'internal' so it can be accessed by execution engine",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor EndpointAttributeVersionArgumentIsInvalid = new(
        id: "AEP104",
        title: "Endpoint attribute argument 'Version' has an invalid value",
        messageFormat: "Endpoint attribute in class '{0}' declares Version argument with value '{1}', which is a invalid version value (expected formats: '1', '1.0', '1.0-alpha')",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor EndpointGroupNotFound = new(
        id: "AEP105",
        title: "Endpoint attribute declare missing Endpoint group class",
        messageFormat: $"Endpoint attribute in class '{{0}}' references group '{{1}}' which was not found; The endpoint group class may be missing '{Project.Classes.Names.EndpointGroupAttribute}'",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    #endregion

    #region Endpoint Grouping diagnostics

    internal static readonly DiagnosticDescriptor ClassMissingEndpointGroupAttribute = new(
        id: "AEP201",
        title: "Class missing endpoint group attribute",
        messageFormat: $"Class '{{0}}' must be marked with '{Project.Classes.Names.EndpointGroupAttribute}' to be considered an endpoint group",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor EndpointGroupAttributeNameArgumentIsEmpty = new(
        id: "AEP203",
        title: "Endpoint group attribute argument 'name' is empty",
        messageFormat: "Endpoint group class '{0}' has an empty or only whitespace 'name' argument. The 'name' argument must be a non-empty string.",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor EndpointGroupAttributePrefixArgumentIsEmpty = new(
        id: "AEP202",
        title: "Endpoint group attribute argument 'prefix' is empty",
        messageFormat: "Endpoint group class '{0}' has an empty or only whitespace 'prefix'. The 'prefix' argument must be a non-empty string.",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    #endregion    
}
