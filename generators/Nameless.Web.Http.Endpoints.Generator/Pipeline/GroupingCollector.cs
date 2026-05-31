using System.Collections.Immutable;
using Nameless.Web.Http.Endpoints.Generator.Conventions;
using Nameless.Web.Http.Endpoints.Generator.Diagnostics;
using Nameless.Web.Http.Endpoints.Generator.Extensions;
using Nameless.Web.Http.Endpoints.Generator.Infrastructure;
using Nameless.Web.Http.Endpoints.Generator.Models;

namespace Nameless.Web.Http.Endpoints.Generator.Pipeline;

public static class GroupingCollector {
    public static DiagnosticAwareResult<GroupingModel[]> Collect(ImmutableArray<DiagnosticAwareResult<EndpointModel>> endpointExtractionResults, ImmutableArray<DiagnosticAwareResult<EndpointGroupModel>> endpointGroupExtractionResults, CancellationToken cancellationToken) {
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

        var groups = new List<GroupingModel>();
        foreach (var endpointsByGroup in endpointsByGroupLookup) {
            cancellationToken.ThrowIfCancellationRequested();
            var group = endpointsByGroup.Key;

            // If an endpoint has no explicitly defined group, it is implicitly
            // assigned to the built-in "__synthetic__" group, which acts as a
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

            // We need to add a new convention "WithGroupName" for each endpoint
            // referencing the endpoint group
            foreach (var endpoint in endpointsByGroup.Value) {
                IncludeWithGroupNameConvention(endpoint, endpointGroup.Arguments.Name);
            }

            groups.Add(new GroupingModel {
                Class = endpointGroup.Class,
                Arguments = endpointGroup.Arguments,
                ReportVersions = endpointsByGroup.Value.CollectVersions(),
                Endpoints = [.. endpointsByGroup.Value],
                Conventions = endpointGroup.Conventions
            });
        }

        return ([.. groups], [.. diagnostics]);
    }

    private static GroupingModel CreateSyntheticEndpointGroup(List<EndpointModel> endpoints) {
        const string GroupName = EndpointGroupClass.ReservedName;
        var versions = endpoints.CollectVersions();

        // We need to add a new convention "WithGroupName" for each endpoint
        // referencing the synthetic endpoint group
        foreach (var endpoint in endpoints) {
            IncludeWithGroupNameConvention(endpoint, GroupName);
        }
        
        return new GroupingModel {
            Class = new ClassModel {
                Namespace = $"{EndpointGroupClass.ReservedName}EndpointGroupNamespace",
                Name = $"{EndpointGroupClass.ReservedName}EndpointGroupClassName",
                AccessorModifier = "public"
            },
            Arguments = new EndpointGroupArgumentsModel {
                Name = GroupName, 
                Prefix = string.Empty
            },
            ReportVersions = versions,
            Endpoints = [.. endpoints],
            Conventions = []
        };
    }

    private static void IncludeWithGroupNameConvention(EndpointModel endpoint, string groupName) {
        endpoint.Conventions.Add(new Convention(
            call: $".WithGroupName(\"{EscapeStringLiteral(groupName)}\")"
        ));
    }
}
