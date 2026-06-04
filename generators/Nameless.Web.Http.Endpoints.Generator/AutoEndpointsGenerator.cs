using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Nameless.Web.Http.Endpoints.Generator.Emitters;
using Nameless.Web.Http.Endpoints.Generator.Pipeline;
using static Nameless.Web.Http.Endpoints.Generator.Constants.Project.Classes.FullNames;

namespace Nameless.Web.Http.Endpoints.Generator;

[Generator(LanguageNames.CSharp)]
public sealed class AutoEndpointsGenerator : IIncrementalGenerator {
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        var endpointGroups = context.SyntaxProvider
                                    .ForAttributeWithMetadataName(
                                        fullyQualifiedMetadataName: EndpointGroupAttribute,
                                        predicate: static (node, _) => node is ClassDeclarationSyntax,
                                        transform: EndpointGroupExtractor.Extract)
                                    .Collect();

        var endpoints = context.SyntaxProvider
                               .ForAttributeWithMetadataName(
                                   fullyQualifiedMetadataName: EndpointAttributeWithArity,
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
