using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Nameless.WebApplication.Web {

    public partial class StartUp {

        #region Public Properties

        public IConfiguration Configuration { get; }
        public IHostEnvironment HostEnvironment { get; }

        #endregion

        #region Public Constructors

        public StartUp(IConfiguration configuration, IHostEnvironment hostEnvironment) {
            Configuration = configuration;
            HostEnvironment = hostEnvironment;
        }

        #endregion

        #region Public Methods

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            ConfigureOptions(services, Configuration);
            
            ConfigureEntityFramework(services, Configuration);

            ConfigureIdentity(services);
            
            ConfigureAutoMapper(services);
            
            ConfigureFluentValidation(services);
            
            ConfigureRouting(services);

            ConfigureAuth(services, Configuration);
            
            ConfigureEndpoints(services);
            
            ConfigureSwagger(services);
            
            ConfigureCors(services);
            
            ConfigureVersioning(services);
            
            ConfigureErrorHandling(services, HostEnvironment);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder applicationBuilder,
            IApiVersionDescriptionProvider apiVersionDescriptorProvider,
            IHostApplicationLifetime hostApplicationLifetime) {
            UseRouting(applicationBuilder);
            
            UseCors(applicationBuilder);
            
            UseEntityFramework(applicationBuilder);
            
            UseSwagger(applicationBuilder, HostEnvironment, apiVersionDescriptorProvider);
            
            UseHttpSecurity(applicationBuilder, HostEnvironment);

            UseAuth(applicationBuilder);

            UseEndpoints(applicationBuilder);
            
            UseErrorHandling(applicationBuilder, HostEnvironment);

            var container = applicationBuilder.ApplicationServices.GetAutofacRoot();

            // Tear down the composition root and free all resources.
            hostApplicationLifetime.ApplicationStopped.Register(container.Dispose);
        }

        #endregion
    }
}
