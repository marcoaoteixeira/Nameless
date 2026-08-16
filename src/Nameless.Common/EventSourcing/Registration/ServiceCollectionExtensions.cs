using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Diagnostics.CodeAnalysis;
using Nameless.EventSourcing.EntityFrameworkCore;
using Nameless.EventSourcing.Projections;
using Nameless.EventSourcing.UpCasting;
using Nameless.Helpers;
using Nameless.Mediator.Events;

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
        /// <param name="registration">
        ///     An optional delegate to configure up-casters, projections,
        ///     or assembly scanning. If <see langword="null"/>, the
        ///     default configuration is used.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public IServiceCollection RegisterEventSourcing<TDbContext>(
            Action<EventSourcingRegistration>? registration = null)
            where TDbContext : DbContext {
            var settings = ActionHelper.FromDelegate(registration);

            self.TryAddSingleton<IEventTypeCatalog>(_ =>
                new EventTypeCatalog(settings.ExecuteAssemblyScan(typeof(IEvent))));

            self.TryAddEnumerable(CreateServiceDescriptors<IEventUpcaster>(settings, settings.UpCasters));
            self.TryAddEnumerable(CreateServiceDescriptors<IProjection>(settings, settings.Projections));

            self.TryAddSingleton<IEventSerializer, EventSerializer>();
            self.TryAddScoped<IEventStore, EventStore<TDbContext>>();
            self.TryAddScoped<ProjectionRebuilder>();
            self.TryAddTransient(typeof(Repository<>));

            return self;
        }
    }

    private static IEnumerable<ServiceDescriptor> CreateServiceDescriptors<TService>(EventSourcingRegistration settings,
        IReadOnlyCollection<Type> explicitTypes)
        where TService : class {
        var service = typeof(TService);
        var implementations = settings.UseAssemblyScan
            ? settings.ExecuteAssemblyScan(service)
            : explicitTypes;

        return implementations.Select(selector: implementation => ServiceDescriptor.Transient(service, implementation));
    }
}