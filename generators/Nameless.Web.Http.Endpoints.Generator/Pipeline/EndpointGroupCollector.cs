using System.Collections.Immutable;
using Nameless.Web.Http.Endpoints.Generator.Diagnostics;
using Nameless.Web.Http.Endpoints.Generator.Infrastructure;
using Nameless.Web.Http.Endpoints.Generator.Models;

namespace Nameless.Web.Http.Endpoints.Generator.Pipeline;

public static class EndpointGroupCollector {
    public static DiagnosticAwareResult<EndpointGroupModelCollection> Collect(ImmutableArray<DiagnosticAwareResult<EndpointModel>> endpointExtractionResults, ImmutableArray<DiagnosticAwareResult<EndpointGroupModel>> endpointGroupExtractionResults, CancellationToken cancellationToken) {
        var diagnostics = new List<GeneratorDiagnostic>();
        var endpoints = new List<EndpointModel>();

        foreach (var item in endpointExtractionResults) {
            cancellationToken.ThrowIfCancellationRequested();
            diagnostics.AddRange(item.Diagnostics);

            if (item.Model is not null) {
                endpoints.Add(item.Model);
            }
        }

        // Group Endpoints by EndpointGroup that they belong to
        var endpointsByGroupLookup = new Dictionary<string, List<EndpointModel>>(StringComparer.Ordinal);
        foreach (var item in endpoints) {
            cancellationToken.ThrowIfCancellationRequested();

            if (!endpointsByGroupLookup.TryGetValue(item.Arguments.Group, out var output)) {
                output = [];
                endpointsByGroupLookup[item.Arguments.Group] = output;
            }

            output.Add(item);
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

        var groups = new List<EndpointGroupModel>();
        foreach (var endpointsByGroup in endpointsByGroupLookup) {
            cancellationToken.ThrowIfCancellationRequested();
            var group = endpointsByGroup.Key;

            // If an endpoint has no explicitly defined group, it is implicitly
            // assigned to the built-in "_SyntheticEndpointGroup_" group, which acts as a
            // catch-all for ungrouped endpoints.
            if (string.IsNullOrWhiteSpace(group)) {
                groups.Add(
                    CreateSyntheticEndpointGroup(endpointsByGroup.Value)
                );

                continue;
            }

            // Validate if there are any endpoint with missing group then report it.
            if (!endpointGroupLookup.TryGetValue(group, out var endpointGroup)) {
                foreach (var endpoint in endpointsByGroup.Value) {
                    diagnostics.Add(GeneratorDiagnostic.Create(
                        descriptor: DiagnosticDescriptors.EndpointGroupNotFound,
                        location: endpoint.Location,
                        messageArgs: [endpoint.Class.Name, group]
                    ));
                }

                continue;
            }

            groups.Add(endpointGroup with {
                Endpoints = [.. endpointsByGroup.Value],
                ReportVersions = GetReportVersions(endpointsByGroup.Value)
            });
        }

        return ([.. groups], [.. diagnostics]);
    }

    private static EndpointGroupModel CreateSyntheticEndpointGroup(List<EndpointModel> endpoints) {
        const string GroupName = EndpointGroupClass.ReservedName;
        var reportVersions = GetReportVersions(endpoints);

        return new EndpointGroupModel {
            Class = new ClassModel {
                Namespace = Project.Namespaces.Root,
                Name = GroupName,
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
