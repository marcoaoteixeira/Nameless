using Autofac;
using Microsoft.Extensions.Configuration;
using Nameless.Autofac;
using Nameless.Infrastructure;
using CoreRoot = Nameless.Root;

namespace Nameless.Messenger.Email.DependencyInjection {
    public sealed class MessengerModule : ModuleBase {
        #region Public Constructors

        public MessengerModule()
            : base([]) { }

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .Register(ResolveMessenger)
                .As<IMessenger>()
                .SingleInstance();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static MessengerOptions? GetMessengerOptions(IComponentContext ctx) {
            var configuration = ctx.ResolveOptional<IConfiguration>();
            var options = configuration?
                .GetSection(nameof(MessengerOptions).RemoveTail(CoreRoot.Defaults.OptsSetsTails))
                .Get<MessengerOptions>();

            return options;
        }

        private static IMessenger ResolveMessenger(IComponentContext ctx) {
            var applicationContext = ctx.ResolveOptional<IApplicationContext>()
                ?? NullApplicationContext.Instance;
            var options = GetMessengerOptions(ctx);
            var result = new EmailMessenger(applicationContext, options);

            return result;
        }

        #endregion
    }

    public static class ContainerBuilderExtension {
        #region Public Static Methods

        public static ContainerBuilder AddMessenger(this ContainerBuilder self) {
            self.RegisterModule<MessengerModule>();

            return self;
        }

        #endregion
    }
}
