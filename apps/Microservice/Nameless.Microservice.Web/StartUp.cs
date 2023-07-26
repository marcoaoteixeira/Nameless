using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Nameless.Microservice.Web {
    public class StartUp {
        #region Public Properties

        public IConfiguration Config { get; }
        public IHostEnvironment Env { get; set; }

        #endregion

        #region Public Constructors

        public StartUp(IConfiguration config, IHostEnvironment env) {
            Config = Prevent.Against.Null(config, nameof(config));
            Env = Prevent.Against.Null(env, nameof(env));
        }

        #endregion

        #region Public Methods

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime, IApiVersionDescriptionProvider versioning) {
            

            // Tear down the composition root and free all resources.
            var container = app.ApplicationServices.GetAutofacRoot();
            lifetime.ApplicationStopped.Register(container.Dispose);
        }

        #endregion
    }
}
