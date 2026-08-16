using Microsoft.CodeAnalysis;

namespace Nameless.Generators.EventSourcing.Diagnostics;

internal static class DiagnosticDescriptors
{
    private const string CATEGORY = "EventSourcing";

    #region Common Diagnostics

    internal static readonly DiagnosticDescriptor WrongTargetSymbol = new(
        id: "AES001",
        title: "Target symbol is not the correct type",
        messageFormat: "Method '{0}' target symbol is not a valid method symbol",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor ClassTypeModifierMustBePartial = new(
        id: "AES002",
        title: "Class modifier must be partial",
        messageFormat: "Class '{0}' must be declared partial so the generator can supply the When override",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor ClassTypeModifierMustNotBeAbstract = new(
        id: "AES003",
        title: "Class modifier must not be abstract",
        messageFormat: "Class '{0}' must not be abstract",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor ClassMustDeriveFromAggregateRoot = new(
        id: "AES004",
        title: "Class must derive from AggregateRoot<TID>",
        messageFormat: $"Class '{{0}}' must derive from '{EventSourcingConstants.Project.Classes.FullNames.AggregateRoot}' to use [Apply] methods",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    #endregion

    #region Apply Method Diagnostics

    internal static readonly DiagnosticDescriptor ApplyMethodMustNotBeStatic = new(
        id: "AES101",
        title: "[Apply] method must not be static",
        messageFormat: "Method '{0}' decorated with [Apply] must not be static",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor ApplyMethodMustHaveExactlyOneParameter = new(
        id: "AES102",
        title: "[Apply] method must have exactly one parameter",
        messageFormat: "Method '{0}' decorated with [Apply] must have exactly one parameter, the event it handles",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor ApplyMethodParameterMustImplementIEvent = new(
        id: "AES103",
        title: "[Apply] method parameter must implement IEvent",
        messageFormat: $"Method '{{0}}' decorated with [Apply] must declare a parameter whose type implements '{EventSourcingConstants.FQN.EventInterface}'",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor ApplyMethodParameterMustBeConcrete = new(
        id: "AES104",
        title: "[Apply] method parameter must be a concrete type",
        messageFormat: "Method '{0}' decorated with [Apply] must declare a parameter whose type is a concrete class or struct, not an interface or abstract type",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor DuplicateApplyMethodForEventType = new(
        id: "AES105",
        title: "Duplicate [Apply] method for the same event type",
        messageFormat: "Class '{0}' declares more than one [Apply] method for event type '{1}'",
        category: CATEGORY,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    #endregion
}
