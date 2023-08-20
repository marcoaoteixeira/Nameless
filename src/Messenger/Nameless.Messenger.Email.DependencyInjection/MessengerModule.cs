﻿using System.Reflection;
using Autofac;
using Autofac.Core.Registration;
using Microsoft.Extensions.Configuration;
using Nameless.Autofac;
using Nameless.Infrastructure;

namespace Nameless.Messenger.Email.DependencyInjection {
    public sealed class MessengerModule : ModuleBase {
        #region Public Constructors

        public MessengerModule()
            : base(Array.Empty<Assembly>()) { }

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
                .GetSection(nameof(MessengerOptions).RemoveTail(new[] { "Options" }))
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

        public static IModuleRegistrar AddMessenger(this ContainerBuilder self)
            => self.RegisterModule<MessengerModule>();

        #endregion
    }
}