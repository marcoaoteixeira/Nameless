using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Nameless.Web.Http.Endpoints.Generator.Emitters;
using Nameless.Web.Http.Endpoints.Generator.Pipeline;

namespace Nameless.Web.Http.Endpoints.Generator;

[Generator(LanguageNames.CSharp)]
public sealed class EndpointsGenerator : IIncrementalGenerator {
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        var groupMarkers = context.SyntaxProvider
                                  .ForAttributeWithMetadataName(
                                      fullyQualifiedMetadataName: FQN.ENDPOINT_GROUPING_ATTRIBUTE,
                                      predicate: static (node, _) => node is ClassDeclarationSyntax,
                                      transform: GroupMarkerExtractor.Extract)
                                  .Collect();

        var endpoints = context.SyntaxProvider
                               .ForAttributeWithMetadataName(
                                   fullyQualifiedMetadataName: FQN.ENDPOINT_ATTRIBUTE_WITH_ARITY,
                                   predicate: static (node, _) => node is ClassDeclarationSyntax,
                                   transform: SemanticExtractor.Extract)
                               .Collect();

        var grouped = endpoints.Combine(groupMarkers)
                               .Select(static (inputs, cancellationToken) =>
                                   GroupCollector.Collect(inputs.Left, inputs.Right, cancellationToken)
                               );

        context.RegisterSourceOutput(grouped, RegistrationEmitter.Emit);
    }
}
