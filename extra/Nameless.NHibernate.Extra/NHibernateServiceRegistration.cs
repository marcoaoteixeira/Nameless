using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Nameless.IoC.Autofac;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;

namespace Nameless.NHibernate {
    public sealed class NHibernateServiceRegistration : ServiceRegistrationBase {
        #region Private Constants

        private const string CONFIGURATION_KEY = nameof (CONFIGURATION_KEY);
        private const string SESSIONFACTORY_KEY = nameof (SESSIONFACTORY_KEY);

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="ISession" /> lifetime scope type. Default is <see cref="LifetimeScopeType.PerScope" />
        /// </summary>
        public LifetimeScopeType SessionLifetimeScopeType { get; set; } = LifetimeScopeType.PerScope;

        /// <summary>
        /// Gets or sets the entities mapping types.
        /// </summary>
        public Type[] EntitiesMappingTypes { get; set; } = Array.Empty<Type> ();

        #endregion

        #region Public Constructors

        public NHibernateServiceRegistration (IEnumerable<Assembly> supportAssemblies = null) : base (supportAssemblies) { }

        #endregion

        #region Protected Override Methods

        protected override void Load (ContainerBuilder builder) {
            builder
                .RegisterType<Configuration> ()
                .Named<Configuration> (CONFIGURATION_KEY)
                .OnActivating (ActivatingConfiguration)
                .SetLifetimeScope (LifetimeScopeType.Singleton);

            builder
                .Register (RegisterSessionFactory)
                .Named<ISessionFactory> (SESSIONFACTORY_KEY)
                .SetLifetimeScope (LifetimeScopeType.Singleton);

            builder
                .Register (RegisterSession)
                .As<ISession> ()
                .SetLifetimeScope (SessionLifetimeScopeType);
        }

        #endregion

        #region Private Static Methods

        private static ISession RegisterSession (IComponentContext ctx) {
            var configuration = ctx.ResolveNamed<Configuration> (CONFIGURATION_KEY);
            var factory = ctx.ResolveNamed<ISessionFactory> (SESSIONFACTORY_KEY);

            var session = factory.OpenSession ();
            try { new SchemaValidator (configuration).Validate (); }
            catch (SchemaValidationException) {
                new SchemaExport (configuration).Execute (
                    useStdOut: true,
                    execute: true,
                    justDrop: false, /* Just update if needed, do not DROP database */
                    connection : session.Connection,
                    exportOutput : TextWriter.Null
                );
            }
            return session;
        }

        private static ISessionFactory RegisterSessionFactory (IComponentContext ctx) {
            var configuration = ctx.ResolveNamed<Configuration> (CONFIGURATION_KEY);
            var sessionFactory = configuration.BuildSessionFactory ();

            return sessionFactory;
        }

        #endregion

        #region Private Methods

        private void ActivatingConfiguration (IActivatingEventArgs<Configuration> args) {
            var settings = args.Context.ResolveOptional<NHibernateSettings> () ?? new NHibernateSettings ();

            args.Instance.SetProperties (settings.ToPropertiesDictionary ());

            if (!EntitiesMappingTypes.IsNullOrEmpty ()) {
                var modelInspector = new ExplicitlyDeclaredModel ();
                var classMappingOptions = new ClassMappingOptions {
                    DbSchemaName = settings.DbSchemaName,
                    TablePrefix = settings.TablePrefix
                };
                var modelMapper = new ModelMapper (modelInspector, options : classMappingOptions);

                modelMapper.AddMappings (EntitiesMappingTypes);

                args.Instance.AddDeserializedMapping (
                    mappingDocument: modelMapper.CompileMappingForAllExplicitlyAddedEntities (),
                    documentFileName: null
                );
            }
        }

        #endregion
    }
}