using System.ComponentModel.DataAnnotations;

namespace Nameless.Skeleton.Web.Areas.Identity.Models {
    public sealed class RegisterViewModel {
        #region Public Properties

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare (nameof (Password))]
        public string ConfirmPassword { get; set; }
        
        public bool TermsAgreement { get; set; }
        
        public string ReturnUrl { get; set; }

        #endregion
    }
}