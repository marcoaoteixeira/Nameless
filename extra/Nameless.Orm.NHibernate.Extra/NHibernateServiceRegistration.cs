using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Nameless.IoC.Autofac;
using NHibernate;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Tool.hbm2ddl;
using CastleInterceptor = Castle.DynamicProxy.IInterceptor;
using NHConfig = NHibernate.Cfg.Configuration;

namespace Nameless.Orm.NHibernate.Extra {
    public class NHibernateServiceRegistration : ServiceRegistrationBase {
        #region Private Static Read-Only Fields

        private static readonly string DirectiveExecutorName = "{FF9DE61C-905F-4DED-8FBE-15E0DC3E3C46}";
        private static readonly string PersisterName = "{6859DDEC-FC4D-4C61-BFB4-FAF793D3A0C9}";
        private static readonly string QuerierName = "{585F24B4-A2D3-4670-A22E-573240CBA244}";
        private static readonly string SessionFactoryName = "{C638ECE3-3342-45B4-8B33-4BFC0A0F4AED}";
        private static readonly string SessionName = "{C3BE257C-FA7F-4799-A9D3-AE7D946DC22F}";

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="IRepository"/><see cref="LifetimeScopeType"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.PerScope"/>.</remarks>
        public LifetimeScopeType RepositoryLifetimeScope { get; set; } = LifetimeScopeType.PerScope;

        /// <summary>
        /// Gets or sets the <see cref="ISessionInterceptor"/> implementations.
        /// </summary>
        public Type[] SessionInterceptorImplementations { get; set; } = Array.Empty<Type> ();

        /// <summary>
        /// Gets or sets the <see cref="ClassMapping{T}"/> implementations.
        /// </summary>
        public Type[] ClassMappingImplementations { get; set; } = Array.Empty<Type> ();

        /// <summary>
        /// Gets or sets the <see cref="IFilterDefinitionPolicy"/> implementations.
        /// </summary>
        public Type[] FilterDefinitionPolicyImplementations { get; set; } = new[] {
            typeof (EntityOwnerTrimmingFilterDefinitionPolicy)
        };

        /// <summary>
        /// Gets or sets the NHibernate settings
        /// </summary>
        public NHibernateSettings Settings { get; set; } = new NHibernateSettings ();

        #endregion

        #region Public Constructors

        public NHibernateServiceRegistration (params Assembly[] supportAssemblies)
            : base (supportAssemblies) { }

        #endregion

        #region Protected Override Methods

        protected override void Load (ContainerBuilder builder) {
            builder
                .Register (RegisterSessionFactory)
                .Named<ISessionFactory> (SessionFactoryName)
                .SetLifetimeScope (LifetimeScopeType.Singleton);

            var sessionRegistration = builder
                .Register (ctx => ctx.ResolveNamed<ISessionFactory> (SessionFactoryName).OpenSession ())
                .Named<ISession> (SessionName);
            if (!SessionInterceptorImplementations.IsNullOrEmpty ()) {
                const string sessionInterceptorPrefix = "SessionInterceptor+";
                builder
                    .RegisterTypes (SessionInterceptorImplementations)
                    .Named<CastleInterceptor> (interceptor => string.Concat (sessionInterceptorPrefix, interceptor.FullName))
                    .SetLifetimeScope (LifetimeScopeType.PerScope);
            }
            sessionRegistration.SetLifetimeScope (LifetimeScopeType.PerScope);

            builder
                .RegisterType<DirectiveExecutor> ()
                .Named<IDirectiveExecutor> (DirectiveExecutorName)
                .WithParameter (ResolvedParameter.ForNamed<ISession> (SessionName))
                .SetLifetimeScope (RepositoryLifetimeScope);

            builder
                .RegisterType<Persister> ()
                .Named<IPersister> (PersisterName)
                .WithParameter (ResolvedParameter.ForNamed<ISession> (SessionName))
                .SetLifetimeScope (RepositoryLifetimeScope);

            builder
                .RegisterType<Querier> ()
                .Named<IQuerier> (QuerierName)
                .WithParameter (ResolvedParameter.ForNamed<ISession> (SessionName))
                .SetLifetimeScope (RepositoryLifetimeScope);

            builder
                .RegisterType<Repository> ()
                .As<IRepository> ()
                .WithParameters (new[] {
                    ResolvedParameter.ForNamed<IDirectiveExecutor> (DirectiveExecutorName),
                    ResolvedParameter.ForNamed<IPersister> (PersisterName),
                    ResolvedParameter.ForNamed<IQuerier> (QuerierName)
                })
                .SetLifetimeScope (RepositoryLifetimeScope);

            base.Load (builder);
        }

        #endregion

        #region Private Methods

        private ISessionFactory RegisterSessionFactory (IComponentContext ctx) {
            var configuration = new NHConfig ();

            foreach (var filterDefinitionPolicy in FilterDefinitionPolicyImplementations) {
                var policy = (IFilterDefinitionPolicy)Activator.CreateInstance (filterDefinitionPolicy);
                configuration.AddFilterDefinition (policy.GetPolicy ());
            }

            var classMappingImplementations = !ClassMappingImplementations.IsNullOrEmpty ()
                ? ClassMappingImplementations
                : SearchForImplementations (typeof (ClassMapping<>));
            var entityTypes = classMappingImplementations
                .Where (type => type.BaseType != null && type.BaseType.GetGenericArguments ().Any ())
                .Select (type => type.BaseType.GetGenericArguments ().First ())
                .Distinct ()
                .ToArray ();

            var modelInspector = new ModelInspector (entityTypes);
            var classMapper = new ClassMapper (modelInspector, classMappingImplementations);
            var settings = ctx.ResolveOptional<NHibernateSettings> () ?? Settings;
            configuration.AddProperties (settings.ToPropertiesDictionary ());
            classMapper.Map (configuration);

            var result = configuration.BuildSessionFactory ();
            using (var session = result.OpenSession ()) {
                var schema = new SchemaExport (configuration);
                schema.Execute (
                    useStdOut: true,
                    execute: false,
                    justDrop: false,
                    connection: session.Connection,
                    exportOutput: TextWriter.Null
                );
            }
            return result;
        }

        #endregion
    }
}