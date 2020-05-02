namespace Nameless.WebApplication.Auth {
    public interface ITokenGenerator {
        #region Methods

        string Generate (int size = 32);

        #endregion
    }
}