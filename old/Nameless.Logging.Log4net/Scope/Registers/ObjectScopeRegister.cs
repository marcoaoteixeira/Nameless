using System;
using System.Collections.Generic;

namespace Nameless.Logging.Log4net.Scope.Registers {
    public sealed class ObjectScopeRegister : ScopeRegisterBase {
        #region Public Constructors

        public ObjectScopeRegister () { ScopeRegisterType = typeof (object); }

        #endregion

        #region Public Override Methods

        public override IEnumerable<IDisposable> AddToScope (object state) {
            if (state != null) {
                yield return log4net.LogicalThreadContext.Stacks[DEFAULT_STACK_NAME].Push (state.ToString ());
            }
        }

        #endregion
    }
}