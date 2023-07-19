using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless.CommandQuery {
    public abstract class QueryHandlerBase<TQuery, TResult> : IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult> {
        #region Private Read-Only Fields

        private readonly IValidator<TQuery>? _validator;

        #endregion

        #region Public Properties

        private ILogger? _logger = null;
        public ILogger Logger {
            get { return _logger ??= NullLogger.Instance; }
            set { _logger = value; }
        }

        #endregion

        #region Protected Properties

        protected IMapper Mapper { get; }

        #endregion

        #region Protected Constructors

        protected QueryHandlerBase(IMapper mapper, IValidator<TQuery>? validator = null) {
            Mapper = Prevent.Against.Null(mapper, nameof(mapper));

            _validator = validator;
        }

        #endregion

        #region Public Abstract Methods

        public abstract Task<TResult> ExecuteAsync(TQuery query, CancellationToken cancellationToken = default);

        #endregion

        #region IQueryHandler<TQuery, TResult> Members

        public async Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default) {
            if (_validator != null) {
                await _validator.ValidateAsync(query, opts => opts.ThrowOnFailures(), cancellationToken);
            }

            return await ExecuteAsync(query, cancellationToken);
        }

        #endregion
    }
}
