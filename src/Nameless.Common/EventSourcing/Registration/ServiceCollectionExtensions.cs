using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Diagnostics.CodeAnalysis;
using Nameless.EventSourcing.EntityFrameworkCore;
using Nameless.EventSourcing.Projections;
using Nameless.EventSourcing.UpCasting;
using Nameless.Helpers;

namespace Nameless.EventSourcing.Registration;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods.
/// </summary>
[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.Trivial)]
public static class ServiceCollectionExtensions {
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/> instance.
    /// </param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers the Event Sourcing services: an <see cref="IEventStore"/>
        ///     backed by <typeparamref name="TDbContext"/>, event
        ///     serialization/upcasting, and every registered projection.
        /// </summary>
        /// <remarks>
        ///     <typeparamref name="TDbContext"/> must expose
        ///     <c>DbSet&lt;EventEnvelope&gt;</c> and apply
        ///     <see cref="EventEnvelopeEntityTypeConfiguration"/> in its
        ///     own <c>OnModelCreating</c>; this method does not register
        ///     <typeparamref name="TDbContext"/> itself.
        /// </remarks>
        /// <param name="configure">
        ///     An optional delegate to configure up-casters, projections,
        ///     or assembly scanning. If <see langword="null"/>, the
        ///     default configuration is used.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public IServiceCollection RegisterEventSourcing<TDbContext>(
            Action<EventSourcingRegistration>? configure = null)
            where TDbContext : DbContext {
            var registration = ActionHelper.FromDelegate(configure);

            self.TryAddSingleton<IEventTypeCatalog>(_ => new EventTypeCatalog(registration.Events));
            
            self.TryAddEnumerable(registration.UpCasters.Select(
                implementation => ServiceDescriptor.Transient(typeof(IEventUpCaster), implementation)
            ));
            
            self.TryAddEnumerable(registration.Projections.Select(
                implementation => ServiceDescriptor.Transient(typeof(IProjection), implementation)
            ));

            self.TryAddSingleton<IEventSerializer, EventSerializer>();
            self.TryAddScoped<IEventStore, EventStore<TDbContext>>();
            self.TryAddScoped<ProjectionRebuilder>();
            self.TryAddTransient(typeof(Repository<>));

            return self;
        }
    }
}