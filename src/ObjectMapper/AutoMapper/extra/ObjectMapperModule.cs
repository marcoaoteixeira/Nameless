using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Nameless.DependencyInjection.Autofac;
using AF_Parameter = Autofac.Core.Parameter;

namespace Nameless.ObjectMapper.AutoMapper {
    /// <summary>
    /// The ObjectMapper service registration.
    /// </summary>
    public sealed class ObjectMapperModule : ModuleBase {

        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="IMapper"/><see cref="LifetimeScopeType"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.Singleton"/>.</remarks>
        public LifetimeScopeType MapperLifetimeScope { get; set; } = LifetimeScopeType.Singleton;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="ObjectMapperModule"/>.
        /// </summary>
        public ObjectMapperModule ()
            : base (supportAssemblies: null) { }

        /// <summary>
        /// Initializes a new instance of <see cref="ObjectMapperModule"/>.
        /// </summary>
        /// <param name="supportAssemblies">The support assemblies.</param>
        public ObjectMapperModule (IEnumerable<Assembly> supportAssemblies)
            : base (supportAssemblies) { }

        #endregion

        #region Protected Override Methods

        /// <inheritdoc/>
        protected override void Load (ContainerBuilder builder) {
            builder
                .RegisterType<Mapper> ()
                .As<IMapper> ()
                .OnPreparing (OnPreparingMapper)
                .SetLifetimeScope (MapperLifetimeScope);

            base.Load (builder);
        }

        #endregion

        #region Private Methods

        private void OnPreparingMapper (PreparingEventArgs args) {
            var profiles = SearchForImplementations<global::AutoMapper.Profile> ()
                .Select (_ => (global::AutoMapper.Profile)Activator.CreateInstance (_))
                .ToArray ();

            args.Parameters = args.Parameters.Union (new AF_Parameter[] {
                new NamedParameter (nameof (profiles), profiles)
            });
        }

        #endregion
    }
}