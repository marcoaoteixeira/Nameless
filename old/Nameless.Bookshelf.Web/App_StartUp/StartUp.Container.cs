using Autofac;
using Nameless.Caching.Memory;
using Nameless.CQRS;
using Nameless.Data.SqlClient;
using Nameless.Environment;
using Nameless.FileProvider.Physical;
using Nameless.IoC;
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

namespace Nameless.Bookshelf.Web {
    public partial class StartUp {
        #region Public Methods

        public void ConfigureContainer (ContainerBuilder builder) {
            var supportAssemblies = new [] {
                typeof (NullStringLocalizer).Assembly
            };

            builder
                .RegisterModule (new CachingServiceRegistration ())
                .RegisterModule (new CQRSServiceRegistration (supportAssemblies))
                .RegisterModule (new DataServiceRegistration ())
                .RegisterModule (new EnvironmentServiceRegistration ())
                .RegisterModule (new FileProviderServiceRegistration ())
                .RegisterModule (new IoCServiceRegistration ())
                .RegisterModule (new LocalizationServiceRegistration ())
                .RegisterModule (new LoggingServiceRegistration ())
                .RegisterModule (new MailingServiceRegistration ())
                .RegisterModule (new NotificationServiceRegistration ())
                .RegisterModule (new ObjectMapperServiceRegistration ())
                .RegisterModule (new SearchServiceRegistration ())
                .RegisterModule (new SecurityServiceRegistration ())
                .RegisterModule (new SerializationServiceRegistration ())
                .RegisterModule (new ServicesServiceRegistration ())
                .RegisterModule (new TextServiceRegistration ());
        }

        #endregion
    }
}