using System;
using System.Threading;
using System.Threading.Tasks;
using Nameless.CQRS;
using Nameless.Logging;
using NHibernate;

namespace Nameless.Bookshelf.Queries {
    public abstract class QueryHandlerBase<TQuery, TResult> : IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult> {

        #region Public Properties

        private ILogger _logger;
        public ILogger Logger {
            get { return _logger ?? (_logger = NullLogger.Instance); }
            set { _logger = value ?? NullLogger.Instance; }
        }

        #endregion

        #region Protected Properties

        protected ISession Session { get; }

        #endregion

        #region Protected Constructors

        protected QueryHandlerBase (ISession session) {
            Prevent.ParameterNull (session, nameof (session));

            Session = session;
        }

        #endregion

        #region ICommandHandler<TCommand> Members

        public abstract Task<TResult> HandleAsync (TQuery query, IProgress<int> progress = null, CancellationToken token = default);

        #endregion
    }
}