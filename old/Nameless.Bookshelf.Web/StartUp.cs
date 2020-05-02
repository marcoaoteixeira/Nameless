using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Nameless.Bookshelf.Web {
    public partial class StartUp {
        #region Private Fields

        private ILifetimeScope _container;
        private IConfiguration _configuration;

        #endregion

        #region Public Constructors

        public StartUp (IConfiguration configuration) {
            Prevent.ParameterNull (configuration, nameof (configuration));

            _configuration = configuration;
        }

        #endregion

        #region Public Methods

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostApplicationLifetime lifetime, IWebHostEnvironment env) {
            ConfigureRouting (app);
            ConfigureExceptionHandler (app, env);
            ConfigureCors (app);
            ConfigureSecurity (app, env);
            ConfigureHttps (app);
            ConfigureStaticFiles (app);
            ConfigureCookiePolicy (app);
            ConfigureAuth (app);
            ConfigureLocalization (app);
            ConfigureMvc (app);

            _container = app.ApplicationServices.GetAutofacRoot ();

            // Tear down the composition root and free all resources.
            lifetime.ApplicationStopped.Register (() => _container.Dispose ());
        }

        #endregion
    }
}