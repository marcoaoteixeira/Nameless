using Microsoft.CodeAnalysis;

namespace Nameless.Web.Http.Endpoints.Generator.Diagnostics;

public static class DiagnosticDescriptors {
    private const string CATEGORY = "AutoEndpointsGenerator";

    #region Common diagnostics

    public static readonly DiagnosticDescriptor InvalidContextTargetSymbol = new(
        id: "AEP001",
        title: "Type Symbol",
        messageFormat: "Symbol '{0}' is not a valid named type symbol",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ClassTypeModifierMustBePartial = new(
        id: "AEP002",
        title: "Class must be partial",
        messageFormat: "Class '{0}' must be declared as 'partial' for the endpoint generator to properly emit the companion class",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ClassAccessorModifierMustBePublicOrInternal = new(
        id: "AEP003",
        title: "Class must be public or internal",
        messageFormat: "Class '{0}' must be declared as 'internal' or 'public' for the endpoint generator to properly emit the companion class registration methods",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ClassTypeModifierMustNotBeAbstract = new(
        id: "AEP004",
        title: "Class must not be abstract",
        messageFormat: "Class '{0}' must not be declared as 'abstract' since it needs to be instantiated eventually",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ConflictingAllowAnonymousAndUseAuthorizationAttributes = new(
        id: "AEP005",
        title: "Conflicting authorization attributes",
        messageFormat: "'{0}' has both [AllowAnonymous] and [UseAuthorization]; [AllowAnonymous] takes precedence",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ConflictingAllowAndDisableCookieRedirectAttributes = new(
        id: "AEP006",
        title: "Conflicting cookie redirect attributes",
        messageFormat: "'{0}' has both [AllowCookieRedirect] and [DisableCookieRedirect]; [AllowCookieRedirect] takes precedence",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ConflictingDisableAndUseCorsAttributes = new(
        id: "AEP007",
        title: "Conflicting CORS attributes",
        messageFormat: "'{0}' has both [DisableCors] and [UseCors]; [DisableCors] takes precedence",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ConflictingDisableAndUseOutputCacheAttributes = new(
        id: "AEP008",
        title: "Conflicting output cache attributes",
        messageFormat: "'{0}' has both [DisableOutputCache] and [UseOutputCache]; [DisableOutputCache] takes precedence and places a policy preventing any cache logic",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ConflictingDisableAndUseRateLimitingAttributes = new(
        id: "AEP009",
        title: "Conflicting rate limiting attributes",
        messageFormat: "'{0}' has both [DisableRateLimiting] and [UseRateLimiting]; [DisableRateLimiting] takes precedence and places a policy preventing any rate limiting logic",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ConflictingDisableAndUseRequestTimeoutAttributes = new(
        id: "AEP010",
        title: "Conflicting request timeout attributes",
        messageFormat: "'{0}' has both [DisableRequestTimeout] and [UseRequestTimeout]; [DisableRequestTimeout] takes precedence and places a policy preventing any request timeout logic",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    #endregion

    #region Endpoint diagnostics

    public static readonly DiagnosticDescriptor EndpointMissingEndpointAttribute = new(
        id: "AEP101",
        title: "Endpoint class missing endpoint attribute",
        messageFormat: $"Endpoint class '{{0}}' must be annotated with attribute '{ClassNames.EndpointAttribute}'",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor EndpointHasMisplacedEndpointGroupAttribute = new(
        id: "AEP102",
        title: "Endpoint class has misplaced endpoint group attribute",
        messageFormat: $"Endpoint class '{{0}}' must not be annotated with attribute '{ClassNames.EndpointGroupAttribute}'",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor EndpointMissingHandlerMethod = new(
        id: "AEP103",
        title: "Endpoint class missing handler",
        messageFormat: $"Endpoint class '{{0}}' must declare handler '{EndpointClass.HandlerMethodName}'",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor EndpointHandlerMethodMustBePublic = new(
        id: "AEP104",
        title: "Endpoint class handler must be public",
        messageFormat: "Endpoint class '{0}' handler must be declared as 'public' to be accessible for execution and enable efficiently testing",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor EndpointAttributeInvalidVersion = new(
        id: "AEP105",
        title: "Endpoint attribute declares invalid version",
        messageFormat: "Endpoint attribute in class '{0}' declares Version as '{1}' which is a invalid version value (expected formats: '1', '1.0', '1.0-alpha')",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    #endregion

    #region Endpoint Grouping diagnostics

    public static readonly DiagnosticDescriptor EndpointGroupClassEmptyName = new(
        id: "AEP201",
        title: "Endpoint group class has empty 'Name'",
        messageFormat: "Endpoint group class '{0}' has an empty or whitespace 'Name'. The 'Name' must be a non-empty string.",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor EndpointGroupClassEmptyPrefix = new(
        id: "AEP202",
        title: "Endpoint group class has empty 'Prefix'",
        messageFormat: "Endpoint group class '{0}' has an empty or whitespace 'Prefix'. The 'Prefix' must be a non-empty string.",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor EndpointGroupNotFound = new(
        id: "AEP203",
        title: $"Endpoint group class not decorated with {ClassNames.EndpointGroupAttribute}",
        messageFormat: $"Endpoint '{{0}}' references group type '{{1}}' which is not decorated with {ClassNames.EndpointGroupAttribute}",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MisplacedEndpointAttribute = new(
        id: "AEP204",
        title: $"Endpoint Group is annotated with '[{ClassNames.EndpointAttribute}]'",
        messageFormat: $"Endpoint Group class '{{0}}' is annotated with '[{ClassNames.EndpointAttribute}]', remove the attribute as it is an endpoint group class",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MissingEndpointGroupAttribute = new(
        id: "AEP205",
        title: $"Class is missing {ClassNames.EndpointGroupAttribute} annotation",
        messageFormat: $"Type '{{0}}' must be annotated with {ClassNames.EndpointGroupAttribute}",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    #endregion    
}
