using AutoMapper;
using Nameless.Logging;

namespace Nameless.CommandQuery {

    public abstract class QueryHandlerBase<TQuery, TResult> : IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult> {

        #region Private Fields

        private ILogger _logger = default!;

        #endregion

        #region Protected Properties

        protected IMapper Mapper { get; }
        protected ILogger Logger {
            get { return _logger ??= NullLogger.Instance; }
            set { _logger = value ?? NullLogger.Instance; }
        }

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
