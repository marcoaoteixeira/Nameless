using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Nameless.Web.Http.Endpoints.Generator.Emitters;
using Nameless.Web.Http.Endpoints.Generator.Pipeline;

namespace Nameless.Web.Http.Endpoints.Generator;

[Generator(LanguageNames.CSharp)]
public sealed class AutoEndpointsGenerator : IIncrementalGenerator {
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        var groups = context.SyntaxProvider
                            .ForAttributeWithMetadataName(
                                fullyQualifiedMetadataName: Simple.EndpointGroupAttribute,
                                predicate: static (node, _) => node is ClassDeclarationSyntax,
                                transform: EndpointGroupExtractor.Extract)
                            .Collect();

        var endpoints = context.SyntaxProvider
                               .ForAttributeWithMetadataName(
                                   fullyQualifiedMetadataName: Simple.EndpointAttributeWithArity,
                                   predicate: static (node, _) => node is ClassDeclarationSyntax,
                                   transform: EndpointExtractor.Extract)
                               .Collect();

        var combine = endpoints.Combine(groups)
                               .Select(static (inputs, cancellationToken) =>
                                   GroupingCollector.Collect(inputs.Left, inputs.Right, cancellationToken)
                               );

        context.RegisterSourceOutput(combine, RegistrationEmitter.Emit);
    }
}
