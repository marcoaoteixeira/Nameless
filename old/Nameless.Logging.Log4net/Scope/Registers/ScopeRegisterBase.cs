using System;
using System.Collections.Generic;

namespace Nameless.Logging.Log4net.Scope.Registers {
    public abstract class ScopeRegisterBase {
        #region Protected Constants

        protected const string DEFAULT_STACK_NAME = "scope";

        #endregion

        #region Public Virtual Properties

        public virtual Type ScopeRegisterType { get; protected set; }

        #endregion

        #region Public Abstract Methods

        public abstract IEnumerable<IDisposable> AddToScope (object state);

        #endregion
    }
}