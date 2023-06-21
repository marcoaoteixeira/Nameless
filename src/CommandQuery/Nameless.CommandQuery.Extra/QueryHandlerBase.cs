using AutoMapper;
using Nameless.Logging;

namespace Nameless.CommandQuery {

    public abstract class QueryHandlerBase<TQuery, TResult> : IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult> {
        #region Private Fields

        private ILogger? _logger = null;

        #endregion

        #region Public Properties

        protected ILogger Logger {
            get { return _logger ??= NullLogger.Instance; }
            set { _logger = value; }
        }

        #endregion

        #region Protected Properties

        protected IMapper Mapper { get; }

        #endregion

        #region Protected Constructors

        protected QueryHandlerBase(IMapper mapper) {
            Prevent.Null(mapper, nameof(mapper));

            Mapper = mapper;
        }

        #endregion

        #region IQueryHandler<TQuery, TResult> Members

        public abstract Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);

        #endregion
    }
}
