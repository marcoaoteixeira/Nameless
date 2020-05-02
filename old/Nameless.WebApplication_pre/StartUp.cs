using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Nameless.IoC;
using Nameless.IoC.Autofac;

namespace Nameless.WebApplication {
    public partial class StartUp {
        #region Private Read-Only Fields

        private readonly IConfiguration _configuration;
        private readonly ICompositionRoot _compositionRoot;

        #endregion

        #region Public Constructors

        public StartUp (IConfiguration configuration) {
            _configuration = configuration;
            _compositionRoot = new CompositionRoot ();
        }

        #endregion

        #region Public Methods

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IApplicationLifetime lifetime, IHostingEnvironment env) {
            ConfigureExceptionHandler (app, env);
            ConfigureCors (app);
            ConfigureSecurity (app, env);
            ConfigureHttps (app);
            ConfigureStaticFiles (app);
            ConfigureCookiePolicy (app);
            ConfigureAuth (app, env);
            ConfigureLocalization (app);
            ConfigureMvc (app);

            // Tear down the composition root and free all resources.
            lifetime.ApplicationStopped.Register (() => _compositionRoot.TearDown ());
        }

        #endregion
    }
}
