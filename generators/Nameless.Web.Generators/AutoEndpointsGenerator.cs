using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Nameless.Web.Generators.Emitters;
using Nameless.Web.Generators.Pipeline;
using static Nameless.Web.Generators.Constants.Project.Classes;

namespace Nameless.Web.Generators;

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
