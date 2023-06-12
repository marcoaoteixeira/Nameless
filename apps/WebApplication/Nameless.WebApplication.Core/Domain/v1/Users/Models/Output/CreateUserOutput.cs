namespace Nameless.WebApplication.Domain.v1.Users.Models.Output {

    public sealed class CreateUserOutput {

        #region Public Properties

        public Guid Id { get; set; }
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? PhoneNumber { get; set; }

        #endregion
    }
}
