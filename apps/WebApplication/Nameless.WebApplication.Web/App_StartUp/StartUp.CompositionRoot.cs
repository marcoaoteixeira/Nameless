using System.Reflection;
using Autofac;
using Nameless.Caching.InMemory;
using Nameless.CQRS;
using Nameless.Environment;
using Nameless.FileProvider;
using Nameless.FileStorage.FileSystem;
using Nameless.IoC;
using Nameless.Localization.Json;
using Nameless.Logging.Log4net;
using Nameless.Mailing.MailKit;
using Nameless.NHibernate;
using Nameless.Notification;
using Nameless.ObjectMapper.AutoMapper;
using Nameless.PubSub;
using Nameless.Search.Lucene;
using Nameless.Security;
using Nameless.Serialization.Json;
using Nameless.Services;
using Nameless.Text;

namespace Nameless.WebApplication.Web {
    public partial class StartUp {
        #region Public Methods

        public void ConfigureContainer (ContainerBuilder builder) {
            var supportAssemblies = new [] {
                Assembly.Load ("Nameless.WebApplication.Core"),
                Assembly.Load ("Nameless.Bookshelf.Core"),
                Assembly.Load ("Nameless.AspNetCore")
            };

            builder
                .RegisterModule (new CachingServiceRegistration ())
                .RegisterModule (new CQRSServiceRegistration (supportAssemblies))
                .RegisterModule (new EnvironmentServiceRegistration ())
                .RegisterModule (new FileProviderServiceRegistration ())
                .RegisterModule (new FileStorageServiceRegistration ())
                .RegisterModule (new IoCServiceRegistration ())
                .RegisterModule (new LocalizationServiceRegistration ())
                .RegisterModule (new LoggingServiceRegistration ())
                .RegisterModule (new MailingServiceRegistration ())
                .RegisterModule (new NHibernateServiceRegistration ())
                .RegisterModule (new NotificationServiceRegistration ())
                .RegisterModule (new ObjectMapperServiceRegistration (supportAssemblies))
                .RegisterModule (new PubSubServiceRegistration ())
                .RegisterModule (new SearchServiceRegistration ())
                .RegisterModule (new SecurityServiceRegistration ())
                .RegisterModule (new SerializationServiceRegistration ())
                .RegisterModule (new ServicesServiceRegistration ())
                .RegisterModule (new TextServiceRegistration ());
        }

        #endregion
    }
}