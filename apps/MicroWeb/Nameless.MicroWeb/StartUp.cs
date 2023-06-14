using Autofac;
using Nameless.MicroWeb.Broker;
using Nameless.ProducerConsumer;
using Nameless.ProducerConsumer.RabbitMQ;

namespace Nameless.MicroWeb {
    public class StartUp {
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

        // This is the default if you don't have an environment specific method.
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddHostedService<UserBackgroundConsumer>(factory => {
                var consumer = factory.GetService<IConsumer>();
                return new UserBackgroundConsumer(consumer, new Arguments({
                    { "", "" }
                }));
        });
        }

    // This is the default if you don't have an environment specific method.
    public void ConfigureContainer(ContainerBuilder builder) {
        var opts = Configuration.GetSection(GetSectionKey<RabbitMQSettings>()).Get<RabbitMQSettings>();

        opts ??= new RabbitMQSettings();

        builder.RegisterInstance(opts);
        builder.RegisterModule<ProducerConsumerModule>();
    }

    // This is the default if you don't have an environment specific method.
    public void Configure(IApplicationBuilder app) {
        if (HostEnvironment.IsDevelopment()) {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }

    private static string GetSectionKey<TNode>() {
        return typeof(TNode).Name
            .Replace("Options", string.Empty)
            .Replace("Settings", string.Empty);
    }
}
}
