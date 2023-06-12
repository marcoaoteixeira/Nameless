namespace Nameless.ErrorHandling {

    public interface IPolicy {

        #region Methods

        bool CanHandle(Exception ex);

        void Handle(Exception ex);

        #endregion
    }
}