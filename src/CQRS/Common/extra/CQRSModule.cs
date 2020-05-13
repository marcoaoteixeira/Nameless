using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Nameless.DependencyInjection.Autofac;

namespace Nameless.CQRS {
    /// <summary>
    /// The CQRS service registration.
    /// </summary>
    public sealed class CQRSModule : ModuleBase {

        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="IDispatcher"/> <see cref="LifetimeScopeType"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.Singleton"/>.</remarks>
        public LifetimeScopeType DispatcherLifetimeScope { get; set; } = LifetimeScopeType.Singleton;

        /// <summary>
        /// Gets or sets the <see cref="ICommandHandler{TCommand}"/> implementations.
        /// </summary>
        public Type[] CommandHandlerImplementations { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IQueryHandler{TQuery, TResult}"/> implementations.
        /// </summary>
        public Type[] QueryHandlerImplementations { get; set; }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="CQRSModule"/>.
        /// </summary>
        public CQRSModule () : base (supportAssemblies: null) { }

        /// <summary>
        /// Initializes a new instance of <see cref="CQRSModule"/>.
        /// </summary>
        /// <param name="supportAssemblies">The support assemblies.</param>
        public CQRSModule (IEnumerable<Assembly> supportAssemblies) : base (supportAssemblies) { }

        #endregion

        #region Protected Override Methods

        /// <inheritdoc/>
        protected override void Load (ContainerBuilder builder) {
            builder
                .RegisterType<Dispatcher> ()
                .As<IDispatcher> ()
                .SetLifetimeScope (DispatcherLifetimeScope);

            builder
                .RegisterTypes (CommandHandlerImplementations ?? SearchForImplementations (typeof (ICommandHandler<>)))
                .AsClosedTypesOf (typeof (ICommandHandler<>))
                .SetLifetimeScope (LifetimeScopeType.Transient);

            var queryHandlerImplementations = QueryHandlerImplementations ?? SearchForImplementations (typeof (IQueryHandler<,>));
            var queryImplementations = queryHandlerImplementations
                .Select (GetQuery)
                .Where (_ => _ != null)
                .ToArray ();
            builder
                .RegisterTypes (queryImplementations)
                .AsClosedTypesOf (typeof (IQuery<>))
                .SetLifetimeScope (LifetimeScopeType.Transient);

            builder
                .RegisterTypes (queryHandlerImplementations)
                .AsClosedTypesOf (typeof (IQueryHandler<,>))
                .SetLifetimeScope (LifetimeScopeType.Transient);

            base.Load (builder);
        }

        #endregion Public Override Methods

        #region Private Static Methods

        private static Type GetQuery (Type type) {
            var @interface = type
                .GetInterfaces ()
                .FirstOrDefault (_ => _.IsAssignableToGenericType (typeof (IQueryHandler<,>)));
            if (@interface == null) { return null; }
            var arg = @interface
                .GetGenericArguments ()
                .FirstOrDefault (_ => _.IsAssignableToGenericType (typeof (IQuery<>)));
            return arg;
        }

        #endregion
    }
}