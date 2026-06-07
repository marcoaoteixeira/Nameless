; Unshipped analyzer releases
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
AEP001 | AutoEndpointsGenerator | Error | InvalidContextTargetSymbol
AEP002 | AutoEndpointsGenerator | Error | ClassTypeModifierMustBePartial
AEP003 | AutoEndpointsGenerator | Error | ClassTypeModifierMustNotBeAbstract
AEP004 | AutoEndpointsGenerator | Error | ClassAccessorModifierMustBePublicOrInternal
AEP005 | AutoEndpointsGenerator | Error | ConflictingEndpointVsEndpointGroupAttributes
AEP006 | AutoEndpointsGenerator | Warning | ConflictingAllowAnonymousVsAuthorizeAttributes
AEP007 | AutoEndpointsGenerator | Warning | ConflictingDisableCookieRedirectVsAllowCookieRedirectAttributes
AEP008 | AutoEndpointsGenerator | Warning | ConflictingDisableCorsVsEnableCorsAttributes
AEP009 | AutoEndpointsGenerator | Warning | ConflictingDisableOutputCacheVsOutputCacheAttributes
AEP010 | AutoEndpointsGenerator | Warning | ConflictingDisableRateLimitingVsEnableRateLimitingAttributes
AEP011 | AutoEndpointsGenerator | Warning | ConflictingDisableRequestTimeoutVsRequestTimeoutAttributes
AEP012 | AutoEndpointsGenerator | Warning | ConflictingDisableValidationVsEnableValidationAttributes
AEP013 | AutoEndpointsGenerator | Hidden | ClassUnknownAttribute

AEP101 | AutoEndpointsGenerator | Error | ClassMissingEndpointAttribute
AEP102 | AutoEndpointsGenerator | Error | ClassMustDeclareHandlerMethod
AEP103 | AutoEndpointsGenerator | Error | ClassHandlerMethodMustBePublicOrInternal
AEP104 | AutoEndpointsGenerator | Error | EndpointAttributeVersionArgumentIsInvalid
AEP105 | AutoEndpointsGenerator | Error | EndpointGroupNotFound

AEP201 | AutoEndpointsGenerator | Error | ClassMissingEndpointGroupAttribute
AEP202 | AutoEndpointsGenerator | Error | EndpointGroupAttributePrefixArgumentIsEmpty