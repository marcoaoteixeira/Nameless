using System;
using System.IO;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;

namespace Nameless.NHibernate.Support {
    public sealed class SessionSource : IDisposable {
        private ISessionFactory _factory;
        private global::NHibernate.Cfg.Configuration _configuration;
        private NHibernateSettings _settings;
        private DbSchemaOptions _dbSchemaOptions;
        private bool _disposed;

        public SessionSource (NHibernateSettings settings = null, Type[] mappingTypes = null, DbSchemaOptions dbSchemaOptions = null) {
            _settings = settings ?? new NHibernateSettings ();
            _dbSchemaOptions = dbSchemaOptions ?? new DbSchemaOptions ();
            _configuration = new global::NHibernate.Cfg.Configuration ();
            _configuration.AddProperties (_settings.ToPropertiesDictionary ());

            if (mappingTypes != null && mappingTypes.Length > 0) {
                var classMappingOptions = new ClassMappingOptions {
                TablePrefix = _settings.TablePrefix,
                DbSchemaName = _settings.DbSchemaName
                };
                var modelInspector = new ExplicitlyDeclaredModel ();
                var modelMapper = new CustomModelMapper (modelInspector, options : classMappingOptions);

                modelMapper.AddMappings (mappingTypes);

                _configuration.AddDeserializedMapping (
                    mappingDocument: modelMapper.CompileMappingForAllExplicitlyAddedEntities (),
                    documentFileName: null
                );
            }

            _factory = _configuration.BuildSessionFactory ();

            BuildDbSchema (_factory.OpenSession ());
        }

        ~SessionSource () {
            Dispose (false);
        }

        public ISession CreateSession () {
            var session = _factory.OpenSession ();

            if (_settings.CreateOrUpdateSchema) {
                BuildDbSchema (session);
            }

            return session;
        }

        public void Dispose () {
            Dispose (true);
            GC.SuppressFinalize (this);
        }

        private void BuildDbSchema (ISession session) {
            try {
                new SchemaValidator (_configuration).Validate ();
            } catch (SchemaValidationException) {
                new SchemaExport (_configuration).Execute (
                    useStdOut: _dbSchemaOptions.UseStdOut,
                    execute: _dbSchemaOptions.Execute,
                    justDrop: _dbSchemaOptions.JustDrop,
                    connection: session.Connection,
                    exportOutput: _dbSchemaOptions.ExportOutput
                );
            } catch (Exception ex) {
                Console.WriteLine (ex.Message);
            }
        }

        private void Dispose (bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                if (_factory != null) {
                    _factory.Dispose ();
                }
            }

            _factory = null;
            _configuration = null;
            _disposed = true;
        }
    }

    public class DbSchemaOptions {
        public bool UseStdOut { get; set; } = true;
        public bool Execute { get; set; } = true;
        public bool JustDrop { get; set; } = false;
        public TextWriter ExportOutput { get; set; } = TextWriter.Null;
    }
}