using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Nameless.Skeleton.Web {
    public partial class StartUp {
        #region Public Properties

        public IConfiguration Configuration { get; }

        #endregion

        #region Public Constructors

        public StartUp (IConfiguration configuration) {
            Configuration = configuration;
        }

        #endregion

        #region Public Methods

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime) {
            UsingErrorHandling (app, env);
            UsingHttpSecurity (app, env);
            UsingStaticFiles (app);
            UsingRouting (app);
            UsingAuth (app);
            UsingEndpoints (app);
            UsingCookiePolicy (app);
            UsingCors (app);
            UsingLocalization (app);

            var container = app.ApplicationServices.GetAutofacRoot ();

            // Tear down the composition root and free all resources.
            lifetime.ApplicationStopped.Register (() => container.Dispose ());
        }

        #endregion
    }
}