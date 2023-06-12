namespace Nameless.ErrorHandling {

    public sealed class ErrorHandler : IErrorHandler {

        #region Private Read-Only Fields

        private readonly IPolicyProvider _policyProvider;

        #endregion

        #region Public Constructors

        public ErrorHandler(IPolicyProvider policyProvider) {
            Prevent.Null(policyProvider, nameof(policyProvider));

            _policyProvider = policyProvider;
        }

        #endregion

        #region IErrorHandler Members

        public void Run(Action action, bool ignoreError = false) {
            try { action(); }
            catch (Exception ex) {
                foreach (var policy in _policyProvider.GetPoliciesFor(ex)) {
                    policy.Handle(ex);
                }
                if (!ignoreError) {
                    throw new ErrorHandlerException(ex);
                }
            }
        }

        #endregion
    }
}