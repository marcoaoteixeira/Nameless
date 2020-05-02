namespace Nameless.Patterns.ChainOfResponsibility {
    public abstract class HandlerBase<T> : IHandler<T> where T : class {
        #region Private Fields

        private IHandler<T> _next;

        #endregion

        #region IHandler<T> Members

        public virtual object Handle (T request) {
            return _next?.Handle (request);
        }

        public IHandler<T> Next (IHandler<T> handler) {
            _next = handler;
            return handler;
        }

        #endregion
    }
}