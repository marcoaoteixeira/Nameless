; Unshipped analyzer releases
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
AEP001 | AutoEndpointsGenerator | Error | WrongTargetSymbol
AEP002 | AutoEndpointsGenerator | Error | MissingEndpointExecutionHandler
AEP003 | AutoEndpointsGenerator | Error | InvalidVersionString
AEP005 | AutoEndpointsGenerator | Error | EndpointExecutionHandlerNotAccessible
AEP006 | AutoEndpointsGenerator | Error | ConflictingAuthAttributes
AEP007 | AutoEndpointsGenerator | Error | EndpointGroupNotFound
AEP008 | AutoEndpointsGenerator | Error | EndpointVersionNotInGroupVersionSet
AEP009 | AutoEndpointsGenerator | Error | GroupMarkerEmptyName
AEP010 | AutoEndpointsGenerator | Error | ClassMustBePartial
AEP011 | AutoEndpointsGenerator | Error | ClassMustBePublicOrInternal
AEP999 | AutoEndpointsGenerator | Error | MissingEndpointAttribute