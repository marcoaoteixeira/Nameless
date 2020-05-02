using System;
using System.Collections.Generic;
using System.Linq;
using Nameless.Logging.Log4net.Scope.Registers;

namespace Nameless.Logging.Log4net.Scope {
    public sealed class ScopeRegistry {
        #region Private Read-Only Fields

        private readonly IDictionary<Type, Func<object, IEnumerable<IDisposable>>> _registry = new Dictionary<Type, Func<object, IEnumerable<IDisposable>>> ();

        #endregion

        #region Public Constructors

        public ScopeRegistry () {
            SetRegister (new ObjectScopeRegister ());
        }

        #endregion

        #region Public Methods

        public Func<object, IEnumerable<IDisposable>> GetRegister (Type type) {
            if (_registry.ContainsKey (type)) {
                return _registry[type];
            }

            foreach (var item in _registry.Where (_ => _.Key != typeof (object))) {
                if (item.Key.IsAssignableFrom (type)) {
                    return item.Value;
                }
            }

            return _registry[typeof (object)];
        }

        public ScopeRegistry SetRegister (ScopeRegisterBase register) => SetRegister (register.ScopeRegisterType, register.AddToScope);

        public ScopeRegistry SetRegister (Type type, Func<object, IEnumerable<IDisposable>> register) {
            _registry.TryAddOrChange (type, register);
            return this;
        }

        #endregion
    }
}