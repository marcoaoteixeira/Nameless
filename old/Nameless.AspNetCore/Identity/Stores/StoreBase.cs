using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using Nameless.Data;
using Nameless.FileProvider.Common;
using Nameless.Logging;
using Nameless.ObjectMapper;

namespace Nameless.AspNetCore.Identity {
    public abstract class StoreBase {
        #region Protected Properties

        protected IDatabase Database { get; }
        protected IFileProvider FileProvider { get; }
        protected IMapper Mapper { get; }

        #endregion

        #region Public Properties

        private ILogger _log;
        public ILogger Logger {
            get => _log ?? (_log = NullLogger.Instance);
            set => _log = value ?? NullLogger.Instance;
        }

        #endregion

        #region Protected Constructors

        protected StoreBase (IDatabase database, IFileProvider fileProvider, IMapper mapper) {
            Prevent.ParameterNull (database, nameof (database));
            Prevent.ParameterNull (fileProvider, nameof (fileProvider));
            Prevent.ParameterNull (mapper, nameof (mapper));

            Database = database;
            FileProvider = fileProvider;
            Mapper = mapper;
        }

        #endregion

        #region Protected Methods

        protected string GetSQLScript (string folder, string fileName) {
            fileName = fileName.EndsWith ("Async") ? fileName.Replace ("Async", string.Empty) : fileName;

            var filePath = $"Resources/Identity/SQLs/{Database.ProviderName}/{folder}/{fileName}.sql";
            var sql = FileProvider.GetFileInfo (filePath).GetText ();

            return sql;
        }

        protected IdentityResult IdentityResultContinuation (Task continuation) {
            if (continuation.Exception is AggregateException ex) {
                return IdentityResult.Failed (new IdentityError {
                    Description = ex.Flatten ().Message
                });
            }

            if (continuation.IsCanceled) {
                return IdentityResult.Failed (new IdentityError {
                    Description = "Task cancelled by the user."
                });
            }

            return IdentityResult.Success;
        }

        #endregion
    }
}