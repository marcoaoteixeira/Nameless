﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Nameless.CQRS;
using Nameless.Data;
using Nameless.Logging;

namespace Nameless.Bookshelf.Queries {
    public abstract class QueryHandlerBase<TQuery, TResult> : IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult> {
        #region Protected Static Read-Only Fields

        protected static readonly string ResourcePath = "Resources/Bookshelf/SQLs/MSSqlServer/Queries";

        #endregion

        #region Public Properties

        private ILogger _logger;
        public ILogger Logger {
            get { return _logger ?? (_logger = NullLogger.Instance); }
            set { _logger = value ?? NullLogger.Instance; }
        }

        #endregion

        #region Protected Properties

        protected IDatabase Database { get; }
        protected IFileProvider FileProvider { get; }

        #endregion

        #region Protected Constructors

        protected QueryHandlerBase (IDatabase database, IFileProvider fileProvider) {
            Prevent.ParameterNull (database, nameof (database));
            Prevent.ParameterNull (fileProvider, nameof (fileProvider));

            Database = database;
            FileProvider = fileProvider;
        }

        #endregion

        #region ICommandHandler<TCommand> Members

        public abstract Task<TResult> HandleAsync (TQuery query, IProgress<int> progress = null, CancellationToken token = default);

        #endregion
    }
}