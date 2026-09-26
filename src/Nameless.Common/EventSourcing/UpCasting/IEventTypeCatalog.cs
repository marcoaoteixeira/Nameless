using Nameless.Mediator.Events;

namespace Nameless.EventSourcing.UpCasting;

/// <summary>
///     Resolves between an event's stable stored name and its current
///     CLR representation, as declared via <see cref="EventTypeAttribute"/>.
/// </summary>
public interface IEventTypeCatalog {
    /// <summary>
    ///     Resolves the CLR type registered for <paramref name="eventType"/>.
    /// </summary>
    /// <param name="eventType">
    ///     The event's stable stored name.
    /// </param>
    /// <returns>
    ///     The CLR type implementing <see cref="IEvent"/> for
    ///     <paramref name="eventType"/>.
    /// </returns>
    /// <exception cref="KeyNotFoundException">
    ///     Thrown when no type is registered for <paramref name="eventType"/>.
    /// </exception>
    Type Resolve(string eventType);

    /// <summary>
    ///     Gets the current schema version registered for
    ///     <paramref name="eventType"/>.
    /// </summary>
    /// <param name="eventType">
    ///     The event's stable stored name.
    /// </param>
    /// <returns>
    ///     The current schema version.
    /// </returns>
    /// <exception cref="KeyNotFoundException">
    ///     Thrown when no type is registered for <paramref name="eventType"/>.
    /// </exception>
    int GetCurrentVersion(string eventType);

    /// <summary>
    ///     Gets the stable stored name registered for <paramref name="clrType"/>.
    /// </summary>
    /// <param name="clrType">
    ///     The event's CLR type.
    /// </param>
    /// <returns>
    ///     The event's stable stored name.
    /// </returns>
    /// <exception cref="KeyNotFoundException">
    ///     Thrown when <paramref name="clrType"/> is not decorated with
    ///     <see cref="EventTypeAttribute"/>.
    /// </exception>
    string GetEventTypeName(Type clrType);
}