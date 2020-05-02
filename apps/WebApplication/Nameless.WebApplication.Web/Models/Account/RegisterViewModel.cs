using System.ComponentModel.DataAnnotations;

namespace Nameless.WebApplication.Web.Models.Account {
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

        #endregion
    }
}