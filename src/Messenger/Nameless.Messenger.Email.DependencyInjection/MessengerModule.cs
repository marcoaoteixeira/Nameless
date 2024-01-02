﻿using Autofac;
using Nameless.Autofac;
using Nameless.Infrastructure;
using Nameless.Messenger.Email.Impl;

namespace Nameless.Messenger.Email.DependencyInjection {
    public sealed class MessengerModule : ModuleBase {
        #region Private Constants

        private const string SMTP_CLIENT_FACTORY_TOKEN = $"{nameof(SmtpClientFactory)}::8e654864-5b82-47cc-8a71-feeea6d89bbc";
        private const string DELIVERY_HANDLER_TOKEN = $"{nameof(IDeliveryHandler)}::8e654864-5b82-47cc-8a71-feeea6d89bbc";

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .Register(ResolveSmtpClientFactory)
                .Named<ISmtpClientFactory>(SMTP_CLIENT_FACTORY_TOKEN)
                .SingleInstance();

            builder
                .Register(ResolveDeliveryHandler)
                .Named<IDeliveryHandler>(DELIVERY_HANDLER_TOKEN)
                .SingleInstance();

            builder
                .Register(ResolveMessenger)
                .As<IMessenger>()
                .SingleInstance();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static ISmtpClientFactory ResolveSmtpClientFactory(IComponentContext ctx) {
            var options = GetOptionsFromContext<MessengerOptions>(ctx)
                ?? MessengerOptions.Default;
            var result = new SmtpClientFactory(options);

            return result;
        }

        private static IDeliveryHandler ResolveDeliveryHandler(IComponentContext ctx) {
            var options = GetOptionsFromContext<MessengerOptions>(ctx)
                ?? MessengerOptions.Default;

            return options.DeliveryMode switch {
                DeliveryMode.Network => ResolveSmtpClientDeliveryHandler(ctx),
                DeliveryMode.PickupDirectory => ResolvePickupDirectoryDeliveryHandler(ctx),
                _ => throw new InvalidOperationException("Undefined delivery mode")
            };
        }

        private static SmtpClientDeliveryHandler ResolveSmtpClientDeliveryHandler(IComponentContext ctx) {
            var smtpClientFactory = ctx
                .ResolveNamed<ISmtpClientFactory>(SMTP_CLIENT_FACTORY_TOKEN);
            var smtpClientDeliveryHandler = new SmtpClientDeliveryHandler(
                smtpClientFactory
            );

            return smtpClientDeliveryHandler;
        }

        private static PickupDirectoryDeliveryHandler ResolvePickupDirectoryDeliveryHandler(IComponentContext ctx) {
            var applicationContext = ctx.ResolveOptional<IApplicationContext>()
                ?? NullApplicationContext.Instance;
            var options = GetOptionsFromContext<MessengerOptions>(ctx)
                ?? MessengerOptions.Default;

            var result = new PickupDirectoryDeliveryHandler(
                applicationContext,
                Root.Defaults.FileNameGeneratorFactory,
                options
            );

            return result;
        }

        private static IMessenger ResolveMessenger(IComponentContext ctx) {
            var deliveryHandler = ctx.ResolveNamed<IDeliveryHandler>(DELIVERY_HANDLER_TOKEN);
            var result = new EmailMessenger(deliveryHandler);

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
