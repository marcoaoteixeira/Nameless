using Autofac;
using Nameless.Autofac;
using Nameless.CommandQuery;
using Nameless.Environment.System;
using Nameless.Extra;
using Nameless.FileStorage.System;
using Nameless.Infrastructure;
using Nameless.Localization.Microsoft;
using Nameless.Logging.Microsoft.Extra;
using Nameless.Lucene;
using Nameless.Security;
using Nameless.Serialization.Json;
using Nameless.WebApplication.Infrastructure;
using Nameless.WebApplication.Services;
using Nameless.WebApplication.Services.Impl;

namespace Nameless.WebApplication {

    public sealed class WebApplicationModule : ModuleBase {

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder.RegisterModule(new CommandQueryModule(new[] {
                typeof(WebApplicationModule).Assembly
            }));
            builder
                .RegisterModule<CoreModule>()
                .RegisterModule<EnvironmentModule>()
                .RegisterModule<FileStorageModule>()
                .RegisterModule<LocalizationModule>()
                .RegisterModule<LoggingModule>()
                .RegisterModule<LuceneModule>()
                .RegisterModule<SecurityModule>()
                .RegisterModule<SerializationModule>();

            // Override service from CoreModule
            builder
                .RegisterType<ApplicationContext>()
                .As<IApplicationContext>()
                .SingleInstance();

            builder
                .RegisterType<AccessTokenService>()
                .As<IAccessTokenService>()
                .InstancePerDependency();

            builder
                .RegisterType<RefreshTokenService>()
                .As<IRefreshTokenService>()
                .InstancePerDependency();

            base.Load(builder);
        }

        #endregion
    }
}
