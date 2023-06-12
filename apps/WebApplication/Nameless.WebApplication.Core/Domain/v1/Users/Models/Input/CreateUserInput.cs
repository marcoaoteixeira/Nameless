namespace Nameless.WebApplication.Domain.v1.Users.Models.Input {

    public sealed class CreateUserInput {

        #region Public Properties

        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string ConfirmPassword { get; set; } = default!;
        public string? PhoneNumber { get; set; }

        #endregion
    }
}
