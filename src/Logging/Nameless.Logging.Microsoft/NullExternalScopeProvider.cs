using MS_IExternalScopeProvider = Microsoft.Extensions.Logging.IExternalScopeProvider;

namespace Nameless.Logging.Microsoft {
    [Singleton]
    public sealed class NullExternalScopeProvider : MS_IExternalScopeProvider {
        #region Private Static Read-Only Fields

        private static readonly NullExternalScopeProvider _instance = new();

        #endregion

        #region Public Static Properties

        /// <summary>
        /// Gets the unique instance of <see cref="NullScope" />.
        /// </summary>
        public static MS_IExternalScopeProvider Instance => _instance;

        #endregion

        #region Static Constructors

        // Explicit static constructor to tell the C# compiler
        // not to mark type as beforefieldinit
        static NullExternalScopeProvider() { }

        #endregion

        #region Private Constructors

        private NullExternalScopeProvider() { }

        #endregion

        #region MS_IExternalScopeProvider Members

        public void ForEachScope<TState>(Action<object?, TState> callback, TState state) { }

        public IDisposable Push(object? state) => NullDisposable.Instance;

        #endregion
    }
}