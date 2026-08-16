using System.Collections.Immutable;
using Nameless.Generators.Diagnostics;
using Nameless.Generators.EventSourcing.Diagnostics;
using Nameless.Generators.EventSourcing.Model;
using Nameless.Generators.Infrastructure;

namespace Nameless.Generators.EventSourcing.Pipeline;

/// <summary>
///     Groups every extracted <see cref="ApplyMethodModel"/> by its
///     containing class into one <see cref="AggregateModel"/> per class,
///     reporting a diagnostic instead when a class declares more than one
///     <c>[Apply]</c> method for the same event type.
/// </summary>
public static class AggregateModelCollector {
    /// <summary>
    ///     Groups <paramref name="results"/> by containing class.
    /// </summary>
    public static ImmutableArray<DiagnosticAwareResult<AggregateModel>> Collect(ImmutableArray<DiagnosticAwareResult<ApplyMethodModel>> results, CancellationToken cancellationToken) {
        var output = ImmutableArray.CreateBuilder<DiagnosticAwareResult<AggregateModel>>();

        foreach (var failed in results.Where(result => !result.Successful)) {
            output.Add(failed.Diagnostics);
        }

        var methodsByClass = results
            .Where(result => result.Successful)
            .Select(result => result.Model!)
            .GroupBy(method => method.Class.FullName);

        foreach (var group in methodsByClass) {
            cancellationToken.ThrowIfCancellationRequested();

            var methods = group.ToList();
            var duplicateDiagnostics = methods
                .GroupBy(method => method.EventTypeFullName)
                .Where(eventTypeGroup => eventTypeGroup.Count() > 1)
                .SelectMany(eventTypeGroup => eventTypeGroup.Skip(1))
                .Select(duplicate => GeneratorDiagnostic.Create(
                    descriptor: DiagnosticDescriptors.DuplicateApplyMethodForEventType,
                    location: duplicate.Location,
                    messageArgs: [duplicate.Class.Name, duplicate.EventTypeFullName])
                )
                .ToArray();

            output.Add(duplicateDiagnostics.Length > 0
                ? duplicateDiagnostics
                : new AggregateModel(methods[0].Class, [.. methods])
            );
        }

        return output.ToImmutable();
    }
}
