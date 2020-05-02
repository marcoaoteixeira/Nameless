namespace Nameless.WebApplication.Web.Models.Account {
    public class SignInViewModel {
        #region Public Properties

        public string Email { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }

        #endregion
    }
}