using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
using Nameless.DependencyInjection.Autofac;

namespace Nameless.Persistence.NHibernate {
    public class PersistenceModule : ModuleBase {
        #region Private Constants

        private const string CONFIGURATION_KEY = nameof (CONFIGURATION_KEY);
        private const string SESSION_FACTORY_KEY = nameof (SESSION_FACTORY_KEY);
        private const string SESSION_KEY = nameof (SESSION_KEY);
        private const string DIRECTIVE_EXECUTOR_KEY = nameof (DIRECTIVE_EXECUTOR_KEY);
        private const string PERSISTER_KEY = nameof (PERSISTER_KEY);
        private const string QUERIER_KEY = nameof (QUERIER_KEY);

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="IRepository"/><see cref="LifetimeScopeType"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.PerScope"/>.</remarks>
        public LifetimeScopeType RepositoryLifetimeScope { get; set; } = LifetimeScopeType.PerScope;

        /// <summary>
        /// Gets or sets the mappings. The dictionary key is the mapping type and the values are
        /// the constructor arguments, if any.
        /// </summary>
        public IDictionary<Type, object[]> Mappings { get; set; } = new Dictionary<Type, object[]> ();

        /// <summary>
        /// Gets or sets the persister bulk size. Default value is <see cref="Persister.DEFAULT_BULK_SIZE" />
        /// </summary>
        public int BulkSize { get; set; } = Persister.DEFAULT_BULK_SIZE;

        #endregion

        #region Protected Override Methods

        protected override void Load (ContainerBuilder builder) {
            builder
                .RegisterType<global::NHibernate.Cfg.Configuration> ()
                .Named<global::NHibernate.Cfg.Configuration> (CONFIGURATION_KEY)
                .OnActivating (ActivatingConfiguration)
                .SetLifetimeScope (LifetimeScopeType.Singleton);

            builder
                .Register (ctx => ctx.ResolveNamed<global::NHibernate.Cfg.Configuration> (CONFIGURATION_KEY).BuildSessionFactory ())
                .Named<global::NHibernate.ISessionFactory> (SESSION_FACTORY_KEY)
                .SetLifetimeScope (LifetimeScopeType.Singleton);

            builder
                .Register (ctx => ctx.ResolveNamed<global::NHibernate.ISessionFactory> (SESSION_FACTORY_KEY).OpenSession ())
                .Named<global::NHibernate.ISession> (SESSION_KEY)
                .SetLifetimeScope (RepositoryLifetimeScope);

            builder
                .RegisterType<DirectiveExecutor> ()
                .Named<IDirectiveExecutor> (DIRECTIVE_EXECUTOR_KEY)
                .WithParameter (ResolvedParameter.ForNamed<global::NHibernate.ISession> (SESSION_KEY))
                .SetLifetimeScope (RepositoryLifetimeScope);

            builder
                .RegisterType<Persister> ()
                .Named<IPersister> (PERSISTER_KEY)
                .WithParameter (ResolvedParameter.ForNamed<global::NHibernate.ISession> (SESSION_KEY))
                .WithParameter (TypedParameter.From (BulkSize))
                .SetLifetimeScope (RepositoryLifetimeScope);

            builder
                .RegisterType<Querier> ()
                .Named<IQuerier> (QUERIER_KEY)
                .WithParameter (ResolvedParameter.ForNamed<global::NHibernate.ISession> (SESSION_KEY))
                .SetLifetimeScope (RepositoryLifetimeScope);

            builder
                .RegisterType<Repository> ()
                .As<IRepository> ()
                .WithParameters (new [] {
                    ResolvedParameter.ForNamed<IDirectiveExecutor> (DIRECTIVE_EXECUTOR_KEY),
                    ResolvedParameter.ForNamed<IPersister> (PERSISTER_KEY),
                    ResolvedParameter.ForNamed<IQuerier> (QUERIER_KEY)
                })
                .SetLifetimeScope (RepositoryLifetimeScope);

            base.Load (builder);
        }

        #endregion

        #region Private Methods

        private void ActivatingConfiguration (IActivatingEventArgs<global::NHibernate.Cfg.Configuration> args) {
            var settings = args.Context.ResolveOptional<NHibernateSettings> () ?? new NHibernateSettings ();

            settings.SetConfigurationProperties (args.Instance);

            var mappings = Mappings
                .Where (kvp => typeof (global::NHibernate.Mapping.ByCode.IConformistHoldersProvider).IsAssignableFrom (kvp.Key))
                .Select (kvp => {
                    return !kvp.Value.IsNullOrEmpty () ?
                        (global::NHibernate.Mapping.ByCode.IConformistHoldersProvider) Activator.CreateInstance (kvp.Key, args : kvp.Value) :
                        (global::NHibernate.Mapping.ByCode.IConformistHoldersProvider) Activator.CreateInstance (kvp.Key);
                })
                .ToArray ();

            if (!mappings.IsNullOrEmpty ()) {
                var modelInspector = new global::NHibernate.Mapping.ByCode.ExplicitlyDeclaredModel ();
                var modelMapper = new global::NHibernate.Mapping.ByCode.ModelMapper (modelInspector);

                foreach (var mapping in mappings) {
                    modelMapper.AddMapping (mapping);
                }

                args.Instance.AddDeserializedMapping (
                    mappingDocument: modelMapper.CompileMappingForAllExplicitlyAddedEntities (),
                    documentFileName: null
                );
            }
        }

        #endregion
    }
}