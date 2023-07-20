namespace Nameless.Microservice {
    public partial class StartUp {
        #region Private Static Methods

        private static void UseErrorHandling(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
        }

        #endregion
    }
}