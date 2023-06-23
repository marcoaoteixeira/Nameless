using Autofac.Extensions.DependencyInjection;

namespace Nameless.Microservices {
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

            ConfigureAutoMapper(services);

            ConfigureFluentValidation(services);

            ConfigureCors(services);

            ConfigureRouting(services);

            ConfigureEndpoints(services);

            ConfigureSwagger(services);

            ConfigureVersioning(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder applicationBuilder, IWebHostEnvironment webHostEnvironment, IHostApplicationLifetime hostApplicationLifetime) {
            UseSwagger(applicationBuilder, webHostEnvironment);

            UseCors(applicationBuilder);

            UseRouting(applicationBuilder);

            UseEndpoints(applicationBuilder);

            UseHttpSecurity(applicationBuilder, webHostEnvironment);

            UseAuth(applicationBuilder);

            UseErrorHandling(applicationBuilder, webHostEnvironment);

            var container = applicationBuilder.ApplicationServices.GetAutofacRoot();

            // Tear down the composition root and free all resources.
            hostApplicationLifetime.ApplicationStopped.Register(container.Dispose);
        }

        #endregion
    }
}