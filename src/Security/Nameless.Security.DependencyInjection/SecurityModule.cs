using System.Reflection;
using Autofac;
using Autofac.Core.Registration;
using Microsoft.Extensions.Configuration;
using Nameless.Autofac;
using Nameless.Security.Cryptography;

namespace Nameless.Security.DependencyInjection {
    public sealed class SecurityModule : ModuleBase {
        #region Public Constructors

        public SecurityModule()
            : base(Array.Empty<Assembly>()) { }

        #endregion

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

        private static RijndaelCryptoOptions? GetRijndaelCryptoOptions(IComponentContext ctx) {
            var configuration = ctx.ResolveOptional<IConfiguration>();
            var options = configuration?
                .GetSection(nameof(RijndaelCryptoOptions).RemoveTail(new[] { "Options" }))
                .Get<RijndaelCryptoOptions>();

            return options;
        }

        private static ICryptoProvider ResolveCryptoProvider(IComponentContext ctx) {
            var options = GetRijndaelCryptoOptions(ctx);
            var result = new RijndaelCryptoProvider(options);

            return result;
        }

        #endregion
    }

    public static class ContainerBuilderExtension {
        #region Public Static Methods

        public static IModuleRegistrar AddSecurity(this ContainerBuilder self)
            => self.RegisterModule<SecurityModule>();

        #endregion
    }
}