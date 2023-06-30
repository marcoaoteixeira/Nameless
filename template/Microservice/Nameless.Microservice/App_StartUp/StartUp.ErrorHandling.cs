namespace Nameless.Microservice {
    public partial class StartUp {
        #region Private Methods

        private void UseErrorHandling(IApplicationBuilder applicationBuilder, IWebHostEnvironment webHostEnvironment) {
            if (webHostEnvironment.IsDevelopment()) {
                applicationBuilder.UseDeveloperExceptionPage();
            }
        }

        #endregion
    }
}