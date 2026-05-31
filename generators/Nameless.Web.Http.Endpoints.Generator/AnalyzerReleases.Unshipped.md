; Unshipped analyzer releases
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
AEP001 | AutoEndpointsGenerator | Error | InvalidContextTargetSymbol
AEP002 | AutoEndpointsGenerator | Error | ClassTypeModifierMustBePartial
AEP003 | AutoEndpointsGenerator | Error | ClassAccessorModifierMustBePublicOrInternal
AEP004 | AutoEndpointsGenerator | Error | ClassTypeModifierMustNotBeAbstract
AEP005 | AutoEndpointsGenerator | Warning | ConflictingAllowAnonymousAndUseAuthorizationAttributes
AEP006 | AutoEndpointsGenerator | Warning | ConflictingAllowAndDisableCookieRedirectAttributes
AEP007 | AutoEndpointsGenerator | Warning | ConflictingDisableAndUseCorsAttributes
AEP008 | AutoEndpointsGenerator | Warning | ConflictingDisableAndUseOutputCacheAttributes
AEP009 | AutoEndpointsGenerator | Warning | ConflictingDisableAndUseRateLimitingAttributes
AEP010 | AutoEndpointsGenerator | Warning | ConflictingDisableAndUseRequestTimeoutAttributes

AEP101 | AutoEndpointsGenerator | Error | EndpointMissingEndpointAttribute
AEP102 | AutoEndpointsGenerator | Error | EndpointHasMisplacedEndpointGroupAttribute
AEP103 | AutoEndpointsGenerator | Error | EndpointMissingHandlerMethod
AEP104 | AutoEndpointsGenerator | Error | EndpointHandlerMethodMustBePublic
AEP105 | AutoEndpointsGenerator | Error | EndpointAttributeInvalidVersion

AEP201 | AutoEndpointsGenerator | Error | EndpointGroupClassEmptyName
AEP202 | AutoEndpointsGenerator | Error | EndpointGroupClassEmptyPrefix
AEP203 | AutoEndpointsGenerator | Error | EndpointGroupNotFound