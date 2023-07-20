namespace Nameless.Microservice {
    public partial class StartUp {
        #region Private Static Methods

        private static void UseHttpSecurity(IApplicationBuilder app, IWebHostEnvironment env) {
            if (!env.IsDevelopment()) {
                // The default HSTS value is 30 days. You may want to change
                // this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
        }

        #endregion
    }
}