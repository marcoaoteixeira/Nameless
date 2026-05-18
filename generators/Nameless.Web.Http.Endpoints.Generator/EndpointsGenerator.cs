using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Nameless.Web.Http.Endpoints.Generator.Emitters;
using Nameless.Web.Http.Endpoints.Generator.Pipeline;

namespace Nameless.Web.Http.Endpoints.Generator;

[Generator(LanguageNames.CSharp)]
public sealed class EndpointsGenerator : IIncrementalGenerator {
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        var groups = context.SyntaxProvider
                            .ForAttributeWithMetadataName(
                                fullyQualifiedMetadataName: NewFQN.EndpointGroupingAttribute,
                                predicate: static (node, _) => node is ClassDeclarationSyntax,
                                transform: GroupExtractor.Extract)
                            .Collect();

        var endpoints = context.SyntaxProvider
                               .ForAttributeWithMetadataName(
                                   fullyQualifiedMetadataName: NewFQN.EndpointAttributeWithArity,
                                   predicate: static (node, _) => node is ClassDeclarationSyntax,
                                   transform: EndpointExtractor.Extract)
                               .Collect();

        var combine = endpoints.Combine(groups)
                               .Select(static (inputs, cancellationToken) =>
                                   GroupCollector.Collect(inputs.Left, inputs.Right, cancellationToken)
                               );

        context.RegisterSourceOutput(combine, RegistrationEmitter.Emit);
    }
}
