using Nameless.EventSourcing.Projections;
using Nameless.EventSourcing.UpCasting;
using Nameless.Mediator.Events;
using Nameless.Registration;

namespace Nameless.EventSourcing.Registration;

/// <summary>
///     Provides configuration settings for registering Event Sourcing
///     services, including up-casters, projections, and assembly
///     scanning.
/// </summary>
public class EventSourcingRegistration : AssemblyScanAware<EventSourcingRegistration> {
    private readonly HashSet<Type> _upCasters = [];
    private readonly HashSet<Type> _projections = [];
    private readonly HashSet<Type> _events = [];

    /// <summary>
    ///     Gets the collection of up-caster types that are registered.
    /// </summary>
    public IReadOnlyCollection<Type> UpCasters => UseAssemblyScan
        ? ExecuteAssemblyScan(typeof(IEventUpCaster))
        : _upCasters;

    /// <summary>
    ///     Gets the collection of projection types that are registered.
    /// </summary>
    public IReadOnlyCollection<Type> Projections => UseAssemblyScan
        ? ExecuteAssemblyScan(typeof(IProjection))
        : _projections;

    /// <summary>
    ///     Gets the collection of events types that are registered.
    /// </summary>
    public IReadOnlyCollection<Type> Events => UseAssemblyScan
        ? [.. ExecuteAssemblyScan(typeof(IEvent)).Where(evt => evt.HasAttribute<EventTypeAttribute>())]
        : _events;

    /// <summary>
    ///     Registers an up-caster of the specified type.
    /// </summary>
    /// <typeparam name="TUpCaster">
    ///     The type of up-caster to register. Must implement
    ///     <see cref="IEventUpCaster"/>.
    /// </typeparam>
    /// <returns>
    ///     The current <see cref="EventSourcingRegistration"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public EventSourcingRegistration RegisterUpCaster<TUpCaster>()
        where TUpCaster : IEventUpCaster {
        return RegisterUpCaster(typeof(TUpCaster));
    }

    /// <summary>
    ///     Registers an up-caster of the specified type.
    /// </summary>
    /// <param name="type">
    ///     The type of up-caster to register. Must implement
    ///     <see cref="IEventUpCaster"/>.
    /// </param>
    /// <returns>
    ///     The current <see cref="EventSourcingRegistration"/> instance
    ///     so other actions can be chained.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     if <paramref name="type"/> is either abstract or interface;
    ///     is an open-generic type; is not assignable from
    ///     <see cref="IEventUpCaster"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="type"/> is <see langword="null"/>.
    /// </exception>
    public EventSourcingRegistration RegisterUpCaster(Type type) {
        Throws.When.IsNonConcreteType(type);
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNotAssignableFrom(type, typeof(IEventUpCaster));

        _upCasters.Add(type);

        return this;
    }

    /// <summary>
    ///     Registers a projection of the specified type.
    /// </summary>
    /// <typeparam name="TProjection">
    ///     The type of projection to register. Must implement
    ///     <see cref="IProjection"/>.
    /// </typeparam>
    /// <returns>
    ///     The current <see cref="EventSourcingRegistration"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public EventSourcingRegistration RegisterProjection<TProjection>()
        where TProjection : IProjection {
        return RegisterProjection(typeof(TProjection));
    }

    /// <summary>
    ///     Registers a projection of the specified type.
    /// </summary>
    /// <param name="type">
    ///     The type of projection to register. Must implement
    ///     <see cref="IProjection"/>.
    /// </param>
    /// <returns>
    ///     The current <see cref="EventSourcingRegistration"/> instance
    ///     so other actions can be chained.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     if <paramref name="type"/> is either abstract or interface;
    ///     is an open-generic type; is not assignable from
    ///     <see cref="IProjection"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="type"/> is <see langword="null"/>.
    /// </exception>
    public EventSourcingRegistration RegisterProjection(Type type) {
        Throws.When.IsNonConcreteType(type);
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNotAssignableFrom(type, typeof(IProjection));

        _projections.Add(type);

        return this;
    }

    /// <summary>
    ///     Registers an event of the specified type.
    /// </summary>
    /// <typeparam name="TEvent">
    ///     The type of event to register. Must implement
    ///     <see cref="IEvent"/>.
    /// </typeparam>
    /// <returns>
    ///     The current <see cref="EventSourcingRegistration"/> instance
    ///     so other actions can be chained.
    /// </returns>
    public EventSourcingRegistration RegisterEvent<TEvent>()
        where TEvent : IEvent {
        return RegisterEvent(typeof(TEvent));
    }

    /// <summary>
    ///     Registers an event of the specified type.
    /// </summary>
    /// <param name="type">
    ///     The type of projection to register. Must implement
    ///     <see cref="IEvent"/>.
    /// </param>
    /// <returns>
    ///     The current <see cref="EventSourcingRegistration"/> instance
    ///     so other actions can be chained.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     if <paramref name="type"/> is either abstract or interface;
    ///     is an open-generic type; is not assignable from
    ///     <see cref="IEvent"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="type"/> is <see langword="null"/>.
    /// </exception>
    public EventSourcingRegistration RegisterEvent(Type type) {
        Throws.When.IsNonConcreteType(type);
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNotAssignableFrom(type, typeof(IEvent));

        if (type.HasAttribute<EventTypeAttribute>()) {
            _events.Add(type);
        }

        return this;
    }
}