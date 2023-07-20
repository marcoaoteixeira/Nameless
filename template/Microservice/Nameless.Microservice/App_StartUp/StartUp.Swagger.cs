using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Nameless.Microservice {
    public partial class StartUp {
        #region Private Static Methods

        private static void ConfigureSwagger(IServiceCollection services) {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        private static void UseSwagger(IApplicationBuilder app, IHostEnvironment env, IApiVersionDescriptionProvider versioning) {
            if (env.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI(opts => {
                    foreach (var description in versioning.ApiVersionDescriptions) {
                        opts.SwaggerEndpoint(
                            url: $"/swagger/{description.GroupName}/swagger.json",
                            name: description.GroupName.ToUpperInvariant()
                        );
                    }
                });
            }
        }

        #endregion
    }
}