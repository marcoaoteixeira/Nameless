using System.Security.Claims;

namespace Nameless.WebApplication.Domain.v1.Users.Models.Output {

    public sealed class UserOutput {

        #region Public Properties

        public Guid Id { get; set; }
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;

        #endregion

        #region Public Static Methods

        public static UserOutput FromPrincipal(ClaimsPrincipal principal) {
            var id = Guid.Parse(principal.Claims.Single(_ => _.Type == ClaimTypes.NameIdentifier).Value);
            var userName = principal.Claims.Single(_ => _.Type == ClaimTypes.Name).Value;
            var email = principal.Claims.Single(_ => _.Type == ClaimTypes.Email).Value;

            return new UserOutput {
                Id = id,
                UserName = userName,
                Email = email
            };
        }

        #endregion
    }
}
