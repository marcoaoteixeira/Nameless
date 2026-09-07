using System.Collections.Immutable;
using Nameless.Generators.Shared.Diagnostics;
using Nameless.Generators.Shared.Infrastructure;
using Nameless.Generators.Shared.Models;
using Nameless.Generators.Web.Http.Endpoints.Diagnostics;
using Nameless.Generators.Web.Http.Endpoints.Models;

namespace Nameless.Generators.Web.Http.Endpoints.Pipeline;

public static class EndpointGroupCollector {
    public static DiagnosticAwareResult<EndpointGroupModelCollection> Collect(ImmutableArray<DiagnosticAwareResult<EndpointModel>> endpointExtractionResults, ImmutableArray<DiagnosticAwareResult<EndpointGroupModel>> endpointGroupExtractionResults, string assemblyName, CancellationToken cancellationToken) {
        var diagnostics = new List<GeneratorDiagnostic>();
        var endpoints = new List<EndpointModel>();

        foreach (var item in endpointExtractionResults) {
            cancellationToken.ThrowIfCancellationRequested();

            diagnostics.AddRange(item.Diagnostics);

            if (item.Model is not null) {
                endpoints.Add(item.Model);
            }
        }
        
        // Build a lookup of EndpointGroupModel by EndpointGroup class
        // full name, collecting any diagnostics along the way.
        var endpointGroupLookup = new Dictionary<string, EndpointGroupModel>(StringComparer.Ordinal);
        foreach (var item in endpointGroupExtractionResults) {
            cancellationToken.ThrowIfCancellationRequested();

            diagnostics.AddRange(item.Diagnostics);

            if (item.Model is not null) {
                endpointGroupLookup[
                    item.Model.Class.FullName
                ] = item.Model;
            }
        }

        // Group Endpoints by EndpointGroup that they belong to
        var endpointsByGroupLookup = new Dictionary<string, List<EndpointModel>>(StringComparer.Ordinal);
        foreach (var item in endpoints) {
            cancellationToken.ThrowIfCancellationRequested();

            var groupKey = item.Arguments.Group;

            if (!endpointsByGroupLookup.TryGetValue(groupKey, out var output)) {
                output = [];
                endpointsByGroupLookup[groupKey] = output;
            }

            output.Add(item);
        }

        var groups = new List<EndpointGroupModel>();
        foreach (var endpointsByGroup in endpointsByGroupLookup) {
            cancellationToken.ThrowIfCancellationRequested();

            var groupKey = endpointsByGroup.Key;

            // If an endpoint has no explicitly defined group, it is implicitly
            // assigned to the built-in "_SyntheticEndpointGroup_" group, which acts as a
            // catch-all for ungrouped endpoints.
            if (string.IsNullOrWhiteSpace(groupKey)) {
                groups.Add(
                    CreateSyntheticEndpointGroup(endpointsByGroup.Value, assemblyName)
                );

                continue;
            }

            // Validate if there are any endpoint with missing group then report it.
            if (!endpointGroupLookup.TryGetValue(groupKey, out var endpointGroup)) {
                foreach (var endpoint in endpointsByGroup.Value) {
                    diagnostics.Add(GeneratorDiagnostic.Create(
                        descriptor: DiagnosticDescriptors.EndpointGroupNotFound,
                        location: endpoint.Location,
                        messageArgs: [endpoint.Class.Name, groupKey]
                    ));
                }

                continue;
            }

            groups.Add(endpointGroup with {
                Endpoints = [.. endpointsByGroup.Value],
                ReportVersions = GetReportVersions(endpointsByGroup.Value)
            });
        }

        return (
            Model: new EndpointGroupModelCollection([.. groups], assemblyName),
            Diagnostics: [.. diagnostics]
        );
    }

    private static EndpointGroupModel CreateSyntheticEndpointGroup(List<EndpointModel> endpoints, string assemblyName) {
        var reportVersions = GetReportVersions(endpoints);

        return new EndpointGroupModel {
            Class = new ClassModel {
                Namespace = $"{assemblyName}.AutoGenCode",
                Name = EndpointGroupClass.ReservedName,
                Accessibility = "public"
            },
            Arguments = new EndpointGroupArgumentsModel {
                Prefix = string.Empty
            },
            Conventions = [],
            Endpoints = [.. endpoints],
            ReportVersions = reportVersions,
            Location = default
        };
    }

    private static VersionModel[] GetReportVersions(IEnumerable<EndpointModel> endpoints) {
        return [.. endpoints.Select(ExtractVersion).Distinct()];

        static VersionModel ExtractVersion(EndpointModel endpoint) {
            return endpoint.Arguments.Version != default
                ? endpoint.Arguments.Version
                : VersionModel.V1;
        }
    }
}
