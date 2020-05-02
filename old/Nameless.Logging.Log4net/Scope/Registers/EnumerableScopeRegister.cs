using System;
using System.Collections;
using System.Collections.Generic;

namespace Nameless.Logging.Log4net.Scope.Registers {
    public sealed class EnumerableScopeRegister : ScopeRegisterBase {
        #region Public Constructors

        public EnumerableScopeRegister () { ScopeRegisterType = typeof (IEnumerable); }

        #endregion

        #region Public Override Methods

        public override IEnumerable<IDisposable> AddToScope (object state) {
            if (state is IEnumerable collection) {
                foreach (var item in collection) {
                    var itemType = item.GetType ();

                    if (itemType.IsAssignableFrom (typeof (KeyValuePair<string, string>))) {
                        var keyValuePair = (KeyValuePair<string, string>) item;
                        yield return log4net.LogicalThreadContext.Stacks[keyValuePair.Key].Push (keyValuePair.Value);
                    }

                    if (itemType.IsAssignableFrom (typeof (KeyValuePair<string, object>))) {
                        var keyValuePair = (KeyValuePair<string, object>) item;
                        yield return log4net.LogicalThreadContext.Stacks[keyValuePair.Key].Push (keyValuePair.Value?.ToString ());
                    }

                    if (itemType.IsAssignableFrom (typeof (object))) {
                        yield return log4net.LogicalThreadContext.Stacks[DEFAULT_STACK_NAME].Push (item?.ToString ());
                    }
                }
            }
        }

        #endregion
    }
}