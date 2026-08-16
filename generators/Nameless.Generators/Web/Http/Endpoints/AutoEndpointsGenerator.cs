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

        var collection = endpoints.Combine(endpointGroups)
                                  .Select(static (inputs, cancellationToken) =>
                                      EndpointGroupCollector.Collect(inputs.Left, inputs.Right, cancellationToken)
                                  );

        context.RegisterSourceOutput(collection, RegistrationEmitter.Instance.Emit);
    }
}
