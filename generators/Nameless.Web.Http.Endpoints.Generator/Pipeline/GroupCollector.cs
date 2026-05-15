using System.Collections.Immutable;
using Nameless.Web.Http.Endpoints.Generator.Diagnostics;
using Nameless.Web.Http.Endpoints.Generator.Models;

namespace Nameless.Web.Http.Endpoints.Generator.Pipeline;

internal static class GroupCollector {
    internal static GroupCollectionMetadata Collect(ImmutableArray<ExtractionResult> extractions, ImmutableArray<GroupMarkerExtractionResult> groupMarkerCollection, CancellationToken cancellationToken) {
        var diagnostics = ImmutableArray.CreateBuilder<GeneratorDiagnostic>();
        var endpoints = ImmutableArray.CreateBuilder<EndpointModel>();

        foreach (var extraction in extractions) {
            cancellationToken.ThrowIfCancellationRequested();
            diagnostics.AddRange(extraction.Diagnostics);

            if (extraction.Model is not null) {
                endpoints.Add(extraction.Model);
            }
        }

        // Build marker lookup: TypeFqn → GroupMarkerModel, collecting any marker diagnostics.
        var markerLookup = new Dictionary<string, GroupMarkerModel>(StringComparer.Ordinal);
        foreach (var groupMarker in groupMarkerCollection) {
            diagnostics.AddRange(groupMarker.Diagnostics);

            if (groupMarker.Model is not null) {
                markerLookup[groupMarker.Model.TypeFqn] = groupMarker.Model;
            }
        }

        // Group endpoints by GroupTypeFqn (null = ungrouped, sentinel = empty string).
        var groupByType = new Dictionary<string, List<EndpointModel>>(StringComparer.Ordinal);
        foreach (var endpoint in endpoints) {
            var key = endpoint.GroupTypeFqn ?? string.Empty;

            if (!groupByType.TryGetValue(key, out var list)) {
                list = [];
                groupByType[key] = list;
            }

            list.Add(endpoint);
        }

        var groups = ImmutableArray.CreateBuilder<GroupModel>();

        foreach (var entry in groupByType) {
            cancellationToken.ThrowIfCancellationRequested();
            var groupKey = entry.Key;
            var endpointsByGroup = entry.Value;

            // Ungrouped endpoints: group by route template so endpoints sharing a route
            // can share a single API version set when they carry [Version] attributes.
            // The key is route-only (not route+verb) because API version sets are scoped
            // to a resource/route, not an individual HTTP method — consistent with how
            // explicit groups work. Known limitation: two routes that sanitize to the same
            // identifier (e.g. /foo-bar and /foo_bar) would produce a variable-name collision
            // in the emitted code; this is an exotic edge case and is not guarded against.
            if (string.IsNullOrEmpty(groupKey)) {
                var groupByRoute = endpointsByGroup.GroupBy(
                    keySelector: static endpoint => endpoint.Metadata.RouteTemplate,
                    comparer: StringComparer.Ordinal
                );

                foreach (var routeGroup in groupByRoute) {
                    var routeEndpoints = routeGroup.ToList();

                    var routeVersions = routeEndpoints.SelectMany(static endpoint => endpoint.Versions)
                                                      .Distinct()
                                                      .ToImmutableArray();

                    // Non-empty name → registration emitter creates a version set + MapGroup("").
                    // Empty name     → registration emitter maps directly on `self` (unchanged behavior).
                    var syntheticName = routeVersions.IsEmpty ? string.Empty : routeGroup.Key;

                    groups.Add(new GroupModel(
                        Name: syntheticName,
                        Prefix: string.Empty,
                        ClassName: null,
                        Namespace: string.Empty,
                        AccessModifier: string.Empty,
                        Versions: routeVersions,
                        Endpoints: [.. routeEndpoints],
                        RateLimitingPolicy: null,
                        DisableRateLimiting: false,
                        RequireAntiforgery: null,
                        DisableHttpMetrics: false,
                        OutputCachePolicy: null,
                        CorsPolicy: null,
                        AllowAnonymous: false,
                        RequireAuthorization: false,
                        AuthorizationPolicy: null,
                        RequestTimeoutPolicy: null,
                        DisableRequestTimeout: false,
                        AllowCookieRedirect: false,
                        FilterTypeNames: []
                    ));
                }

                continue;
            }

            // Resolve marker by type FQN.
            if (!markerLookup.TryGetValue(groupKey, out var marker)) {
                foreach (var endpoint in endpointsByGroup) {
                    diagnostics.Add(new GeneratorDiagnostic(
                        Descriptor: DiagnosticDescriptors.EndpointGroupNotFound,
                        FilePath: endpoint.FilePath,
                        StartLine: endpoint.StartLine,
                        StartCharacter: endpoint.StartCharacter,
                        MessageArgs: [endpoint.ClassName, groupKey]
                    ));
                }

                continue;
            }

            ImmutableArray<VersionModel> versions;

            if (!marker.DeclaredVersions.IsEmpty) {
                // Validate each endpoint's [Version] against the group's declared set.
                var declaredSet = new HashSet<(int, int, int)>(
                    marker.DeclaredVersions.Select(
                        static version => (version.Major, version.Minor, version.Patch)
                    )
                );

                foreach (var endpoint in endpointsByGroup) {
                    foreach (var version in endpoint.Versions) {
                        if (declaredSet.Contains((version.Major, version.Minor, version.Patch))) {
                            continue;
                        }

                        diagnostics.Add(new GeneratorDiagnostic(
                            Descriptor: DiagnosticDescriptors.EndpointVersionNotInGroupVersionSet,
                            FilePath: endpoint.FilePath,
                            StartLine: endpoint.StartLine,
                            StartCharacter: endpoint.StartCharacter,
                            MessageArgs: [
                                endpoint.ClassName,
                                VersionParser.Format(version.Major, version.Minor, version.Patch),
                                marker.Name,
                                string.Join(", ", marker.DeclaredVersions.Select(
                                    static version => VersionParser.Format(version.Major, version.Minor, version.Patch)
                                ))
                            ]
                        ));
                    }
                }

                versions = marker.DeclaredVersions;
            }
            else {
                // No declared versions on the group: union from endpoints.
                versions = [
                    ..endpointsByGroup.SelectMany(static endpoint => endpoint.Versions)
                                      .Distinct()
                ];
            }

            groups.Add(new GroupModel(
                Name: marker.Name,
                Prefix: marker.Prefix,
                ClassName: marker.ClassName,
                Namespace: marker.Namespace,
                AccessModifier: marker.AccessModifier,
                Versions: versions,
                Endpoints: [.. endpointsByGroup],
                RateLimitingPolicy: marker.RateLimitingPolicy,
                DisableRateLimiting: marker.DisableRateLimiting,
                RequireAntiforgery: marker.RequireAntiforgery,
                DisableHttpMetrics: marker.DisableHttpMetrics,
                OutputCachePolicy: marker.OutputCachePolicy,
                CorsPolicy: marker.CorsPolicy,
                AllowAnonymous: marker.AllowAnonymous,
                RequireAuthorization: marker.RequireAuthorization,
                AuthorizationPolicy: marker.AuthorizationPolicy,
                RequestTimeoutPolicy: marker.RequestTimeoutPolicy,
                DisableRequestTimeout: marker.DisableRequestTimeout,
                AllowCookieRedirect: marker.AllowCookieRedirect,
                FilterTypeNames: marker.FilterTypeNames
            ));
        }

        return new GroupCollectionMetadata(
            [.. groups],
            [.. diagnostics]
        );
    }
}
