using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Nameless.WebApplication.Entities {

    public sealed class User : IdentityUser<Guid> {

        #region Public Properties

        [MaxLength(2048)]
        public string? AvatarUrl { get; set; }

        #endregion
    }
}
