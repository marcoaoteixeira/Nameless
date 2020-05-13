using System.ComponentModel.DataAnnotations;

namespace Nameless.Skeleton.Web.Areas.Auth.Models {
    public class SignInViewModel {
        #region Public Properties

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
        
        public bool RememberMe { get; set; }

        #endregion
    }
}