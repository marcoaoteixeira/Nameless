using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nameless.Logging;
using NHibernate;

namespace Nameless.AspNetCore.Identity.Stores {
    public abstract class StoreBase {
        #region Protected Properties

        protected ISession Session { get; }

        #endregion

        #region Public Properties

        private ILogger _logger;
        public ILogger Logger {
            get => _logger ?? (_logger = NullLogger.Instance);
            set => _logger = value ?? NullLogger.Instance;
        }

        #endregion

        #region Protected Constructors

        protected StoreBase (ISession session) {
            Prevent.ParameterNull (session, nameof (session));

            Session = session;
        }

        #endregion

        #region Protected Methods

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