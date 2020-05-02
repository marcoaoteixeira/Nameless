using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Nameless.EventSourcing.Domain;
using Nameless.EventSourcing.Event;
using Nameless.EventSourcing.Repository;
using Nameless.EventSourcing.Snapshot;
using Nameless.IoC.Autofac;

namespace Nameless.EventSourcing {
    public sealed class EventSourcingServiceRegistration : ServiceRegistrationBase {

        #region Public Properties

        /// <summary>
        /// Gets or sets the event source implementation type.
        /// </summary>
        public Type EventStoreImplementationType { get; set; }

        /// <summary>
        /// Gets or sets the snapshot source implementation type.
        /// </summary>
        public Type SnapshotStoreImplementationType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IEventHandlerFactory"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.PerRequest"/>.</remarks>
        public LifetimeScopeType EventHandlerFactoryLifetimeScope { get; set; } = LifetimeScopeType.Transient;

        #endregion

        #region Public Constructors

        public EventSourcingServiceRegistration (IEnumerable<Assembly> supportAssemblies = null) : base (supportAssemblies) { }

        #endregion

        #region Protected Override Methods

        protected override void Load (ContainerBuilder builder) {
            builder
                .RegisterType<AggregateFactory> ()
                .As<IAggregateFactory> ()
                .SetLifetimeScope (LifetimeScopeType.Singleton);

            builder
                .RegisterType<EventPublisher> ()
                .As<IEventPublisher> ()
                .SetLifetimeScope (LifetimeScopeType.Singleton);

            builder
                .RegisterType<EventSerializer> ()
                .As<IEventSerializer> ()
                .SetLifetimeScope (LifetimeScopeType.Singleton);

            builder
                .RegisterType (EventStoreImplementationType)
                .As<IEventStore> ()
                .SetLifetimeScope (LifetimeScopeType.PerScope);

            builder
                .RegisterType<AggregateRepository> ()
                .As<IRepository> ()
                .SetLifetimeScope (LifetimeScopeType.PerScope);

            builder
                .RegisterType<AggregateSession> ()
                .As<ISession> ()
                .SetLifetimeScope (LifetimeScopeType.PerScope);

            builder
                .RegisterType (SnapshotStoreImplementationType)
                .As<ISnapshotStore> ()
                .SetLifetimeScope (LifetimeScopeType.PerScope);

            builder
                .RegisterType<SnapshotStrategy> ()
                .As<ISnapshotStrategy> ()
                .SetLifetimeScope (LifetimeScopeType.Singleton);
        }

        #endregion
    }
}