using System;
using System.Threading;
using System.Threading.Tasks;
using Nameless.CQRS;
using Nameless.Logging;
using NHibernate;

namespace Nameless.Bookshelf.Commands {
    public abstract class CommandHandlerBase<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand {

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

        protected CommandHandlerBase (ISession session) {
            Prevent.ParameterNull (session, nameof (session));

            Session = session;
        }

        #endregion

        #region ICommandHandler<TCommand> Members

        public abstract Task HandleAsync (TCommand command, IProgress<int> progress = null, CancellationToken token = default);

        #endregion
    }
}