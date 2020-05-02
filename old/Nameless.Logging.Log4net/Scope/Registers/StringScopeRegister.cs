using System;
using System.Collections.Generic;

namespace Nameless.Logging.Log4net.Scope.Registers {
    public sealed class StringScopeRegister : ScopeRegisterBase {
        #region Public Constructors

        public StringScopeRegister () { ScopeRegisterType = typeof (string); }

        #endregion

        #region Public Override Methods

        public override IEnumerable<IDisposable> AddToScope (object state) {
            if (state is string text) {
                yield return log4net.LogicalThreadContext.Stacks[DEFAULT_STACK_NAME].Push (text);
            }
        }

        #endregion
    }
}