using System;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Caching.Memory;
using Nameless.Data.SqlClient;
using Nameless.Environment;
using Nameless.FileProvider.Physical;
using Nameless.IoC.Autofac;
using Nameless.Localization.Json;
using Nameless.Logging.Log4net;
using Nameless.Mailing.MailKit;
using Nameless.Notification.Extra;
using Nameless.ObjectMapper.AutoMapper;
using Nameless.Search.Lucene;
using Nameless.Security;
using Nameless.Serialization.Json;
using Nameless.Services;
using Nameless.Text;

namespace Nameless.WebApplication.Web {
    public partial class StartUp {
        #region Public Methods

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices (IServiceCollection services) {
            _compositionRoot.Compose (new ServiceRegistrationBase[] {
                new WebApplicationServiceRegistration (_configuration, services),

                    new CachingServiceRegistration (),
                    new DataServiceRegistration (),
                    new EnvironmentServiceRegistration (),
                    new FileProviderServiceRegistration (),
                    new LocalizationServiceRegistration (),
                    new LoggingServiceRegistration (),
                    new MailingServiceRegistration (),
                    new NotificationServiceRegistration (),
                    new ObjectMapperServiceRegistration (),
                    new SearchServiceRegistration (),
                    new SecurityServiceRegistration (),
                    new SerializationServiceRegistration (),
                    new ServicesServiceRegistration (),
                    new TextServiceRegistration (),
            });

            _compositionRoot.StartUp ();

            return new AutofacServiceProvider (((CompositionRoot) _compositionRoot).Container);
        }

        #endregion
    }
}