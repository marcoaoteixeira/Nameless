using System.ComponentModel.DataAnnotations;

namespace Nameless.Bookshelf.Web.Models {
    public class SignInViewModel {
        #region Public Properties

        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public bool? RememberMe { get; set; }
        public string ReturnUrl { get; set; }

        #endregion
    }
}