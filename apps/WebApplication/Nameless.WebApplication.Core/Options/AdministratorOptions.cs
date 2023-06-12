namespace Nameless.WebApplication.Options {

    public sealed class AdministratorOptions {

        #region Public Static Read-Only Fields

        public static readonly AdministratorOptions Default = new();

        #endregion

        #region Public Properties

        public Guid Id { get; set; } = Guid.Parse("45b48360-4ebf-4569-a5d4-c3ce4e10fbe9");
        public string UserName { get; set; } = "Administrator";
        public string Email { get; set; } = "administrator@webapplication.com";
        public string Password { get; set; } = "123456AbC@";
        public string PhoneNumber { get; set; } = string.Empty;

        #endregion
    }
}
