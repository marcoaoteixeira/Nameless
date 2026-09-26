using Nameless.Generators.EventSourcing.Emit;
using Nameless.Generators.EventSourcing.Extractors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Nameless.Generators.EventSourcing.Pipeline;

namespace Nameless.Generators.EventSourcing;

/// <summary>
///     A Roslyn incremental source generator that finds methods decorated
///     with <c>[Apply]</c> and emits the <c>When(IEvent)</c> dispatch
///     override for their containing aggregate class.
/// </summary>
[Generator(LanguageNames.CSharp)]
public class EventSourcingGenerator : IIncrementalGenerator {
    /// <summary>
    ///     Registers a single incremental pipeline keyed on the
    ///     <c>[Apply]</c> attribute, groups the extracted methods by
    ///     containing class, and hands each group to <see cref="WhenDispatchEmitter"/>.
    /// </summary>
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        var applyMethods = context.SyntaxProvider
                                  .ForAttributeWithMetadataName(
                                      fullyQualifiedMetadataName: EventSourcingConstants.Project.Classes.FullNames.AggregateRoot,
                                      predicate: static (node, _) => node is MethodDeclarationSyntax,
                                      transform: ApplyMethodExtractor.Extract)
                                  .Collect();

        var aggregates = applyMethods.Select(
            static (results, cancellationToken) => AggregateModelCollector.Collect(results, cancellationToken)
        );

        context.RegisterSourceOutput(aggregates, static (productionContext, results) => {
            foreach (var result in results) {
                WhenDispatchEmitter.Instance.Emit(productionContext, result);
            }
        });
    }
}
