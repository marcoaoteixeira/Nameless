using Autofac;
using Nameless.Autofac;
using Nameless.Mailing.MailKit.Impl;
using Nameless.Mailing.MailKit.Options;

namespace Nameless.Mailing.MailKit.DependencyInjection {
    public sealed class MailingModule : ModuleBase {
        #region Private Constants

        private const string SMTP_CLIENT_FACTORY_TOKEN = $"{nameof(SmtpClientFactory)}::176b2c41-fefc-489e-a015-99647179f198";

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .Register(SmtpClientFactoryResolver)
                .Named<ISmtpClientFactory>(SMTP_CLIENT_FACTORY_TOKEN)
                .SingleInstance();

            builder
                .Register(ResolveMailingService)
                .As<IMailingService>()
                .SingleInstance();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static ISmtpClientFactory SmtpClientFactoryResolver(IComponentContext ctx) {
            var options = ctx.GetPocoOptions<ServerOptions>();
            var result = new SmtpClientFactory(options);

            return result;
        }

        private static IMailingService ResolveMailingService(IComponentContext ctx) {
            var smtpClientFactory = ctx.ResolveNamed<ISmtpClientFactory>(SMTP_CLIENT_FACTORY_TOKEN);
            var logger = ctx.GetLogger<MailingService>();
            var result = new MailingService(smtpClientFactory, logger);

            return result;
        }

        #endregion
    }

    public static class ContainerBuilderExtension {
        #region Public Static Methods

        public static ContainerBuilder RegisterMailingModule(this ContainerBuilder self) {
            self.RegisterModule<MailingModule>();

            return self;
        }

        #endregion
    }
}
