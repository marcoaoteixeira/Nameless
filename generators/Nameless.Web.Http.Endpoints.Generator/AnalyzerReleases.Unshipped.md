; Unshipped analyzer releases
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
ENDPOINTS002 | EndpointsGenerator | Error | MissingEndpointExecutionHandle
ENDPOINTS003 | EndpointsGenerator | Error | InvalidVersionString
ENDPOINTS005 | EndpointsGenerator | Warning | EndpointExecutionHandleNotAccessible
ENDPOINTS006 | EndpointsGenerator | Info | ConflictingAuthAttributes
ENDPOINTS007 | EndpointsGenerator | Error | EndpointGroupNotFound
ENDPOINTS008 | EndpointsGenerator | Error | EndpointVersionNotInGroupVersionSet
ENDPOINTS009 | EndpointsGenerator | Error | GroupMarkerEmptyName
