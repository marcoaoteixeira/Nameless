namespace Nameless.EventSourcing;

/// <summary>
///     Marks a method on an <see cref="AggregateRoot{TID}"/> as the
///     handler for a specific event type, inferred from the method's
///     single parameter. A source generator uses every <c>[Apply]</c>-decorated
///     method on a class to produce its <c>When(IEvent)</c> override.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class ApplyAttribute : Attribute;