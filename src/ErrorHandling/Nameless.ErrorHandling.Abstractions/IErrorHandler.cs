namespace Nameless.ErrorHandling {

    public interface IErrorHandler {

        #region Methods

        void Run(Action action, bool ignoreError = false);

        #endregion
    }
}
