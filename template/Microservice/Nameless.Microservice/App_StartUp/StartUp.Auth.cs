namespace Nameless.Microservice {
    public partial class StartUp {
        #region Private Static Methods

        private static void UseAuth(IApplicationBuilder applicationBuilder) {
            applicationBuilder.UseAuthentication();
        }

        #endregion
    }
}