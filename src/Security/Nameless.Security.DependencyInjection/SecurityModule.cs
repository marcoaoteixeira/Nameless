using Autofac;
using Nameless.Autofac;
using Nameless.Security.Crypto;
using Nameless.Security.Options;

namespace Nameless.Security.DependencyInjection {
    public sealed class SecurityModule : ModuleBase {
        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .Register(CryptographicServiceResolver)
                .As<ICryptographicService>()
                .InstancePerLifetimeScope();

            builder
                .RegisterInstance(RandomPasswordGenerator.Instance)
                .As<IPasswordGenerator>()
                .SingleInstance();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static ICryptographicService CryptographicServiceResolver(IComponentContext ctx) {
            var options = ctx.GetPocoOptions<RijndaelCryptoOptions>();
            var result = new RijndaelCryptographicService(options);

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