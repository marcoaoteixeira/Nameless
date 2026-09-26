using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Nameless.Generators.Web.Http.Endpoints.Emitters;
using Nameless.Generators.Web.Http.Endpoints.Pipeline;
using static Nameless.Generators.Web.Http.Endpoints.AutoEndpointsConstants.Project.Classes;

namespace Nameless.Generators.Web.Http.Endpoints;

[Generator(LanguageNames.CSharp)]
public sealed class AutoEndpointsGenerator : IIncrementalGenerator {
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        var endpointGroups = context.SyntaxProvider
                                    .ForAttributeWithMetadataName(
                                        fullyQualifiedMetadataName: FullNames.EndpointGroupAttribute,
                                        predicate: static (node, _) => node is ClassDeclarationSyntax,
                                        transform: EndpointGroupExtractor.Extract)
                                    .Collect();

        var endpoints = context.SyntaxProvider
                               .ForAttributeWithMetadataName(
                                   fullyQualifiedMetadataName: FullNames.EndpointAttribute,
                                   predicate: static (node, _) => node is ClassDeclarationSyntax,
                                   transform: EndpointExtractor.Extract)
                               .Collect();

        var assemblyName = context.CompilationProvider.Select(
            static (compilation, _) => compilation.AssemblyName ?? string.Empty
        );

        var collection = endpoints.Combine(endpointGroups)
                                  .Combine(assemblyName)
                                  .Select(static (input, cancellationToken) => EndpointGroupCollector.Collect(
                                      endpointExtractionResults: input.Left.Left,
                                      endpointGroupExtractionResults: input.Left.Right,
                                      assemblyName: input.Right,
                                      cancellationToken
                                  ));

        // Defense-in-depth: even if the generator assembly ends up wired in
        // as an analyzer for a project that never opted in (e.g. a stray
        // <Analyzer> item, or a build-asset import bug), refuse to emit
        // anything unless UseAutoEndpoints is explicitly "true" for the
        // assembly currently being compiled. The primary opt-in gate is
        // still whether the analyzer is referenced al all (see
        // Directory.Build.Targets / Nameless.Common.targets).
        var useAutoEndpointsProvider = context.AnalyzerConfigOptionsProvider.Select(
            static (provider, _) => provider.GlobalOptions.TryGetValue("build_property.UseAutoEndpoints", out var output) &&
                                    bool.TryParse(output, out var enabled) &&
                                    enabled
        );

        context.RegisterSourceOutput(
            source: collection.Combine(useAutoEndpointsProvider),
            action: static (sourceContext, input) => {
                var (groupings, useAutoEndpoints) = input;
                if (!useAutoEndpoints) { return; }

                RegistrationEmitter.Instance.Emit(sourceContext, groupings);
            }
        );
    }
}
