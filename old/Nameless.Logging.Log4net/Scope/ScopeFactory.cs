using Nameless.Logging.Log4net.Scope.Registers;

namespace Nameless.Logging.Log4net.Scope {
    public sealed class ScopeFactory {
        #region Private Read-Only Fields

        private readonly ScopeRegistry _registry;

        #endregion

        #region Public Constructors

        public ScopeFactory (ScopeRegistry registry) {
            Prevent.ParameterNull (registry, nameof (registry));

            registry
                .SetRegister (new StringScopeRegister ())
                .SetRegister (new EnumerableScopeRegister ());
        }

        #endregion

        #region Public Methods

        public ScopeObject BeginScope<TScope> (TScope scope) => new ScopeObject (scope, _registry);

        #endregion
    }
}