﻿using Autofac;
using Nameless.Autofac;
using Nameless.Security.Cryptography;

namespace Nameless.Security.DependencyInjection {
    public sealed class SecurityModule : ModuleBase {
        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .Register(ResolveCryptoProvider)
                .As<ICryptoProvider>()
                .InstancePerLifetimeScope();

            builder
                .RegisterInstance(RandomPasswordGenerator.Instance)
                .As<IPasswordGenerator>()
                .SingleInstance();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static ICryptoProvider ResolveCryptoProvider(IComponentContext ctx) {
            var options = GetOptionsFromContext<RijndaelCryptoOptions>(ctx)
                ?? RijndaelCryptoOptions.Default;
            var result = new RijndaelCryptoProvider(options);

            return result;
        }

        #endregion
    }

    public static class ContainerBuilderExtension {
        #region Public Static Methods

        public static ContainerBuilder RegisterSecurityModule(this ContainerBuilder self) {
            self.RegisterModule<SecurityModule>();

            return self;
        }

        #endregion
    }
}