using Autofac;
using Nameless.Autofac;
using Nameless.Services;
using Nameless.Services.Impl;
using Nameless.Web.Options;
using Nameless.Web.Services;
using Nameless.Web.Services.Impl;

namespace Nameless.Web.DependencyInjection {
    public sealed class WebModule : ModuleBase {
        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .Register(JwtServiceResolver)
                .As<IJwtService>()
                .SingleInstance();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static IJwtService JwtServiceResolver(IComponentContext ctx) {
            var options = ctx.GetOptions<JwtOptions>();
            var clock = ctx.ResolveOptional<IClock>()
                ?? SystemClock.Instance;
            var logger = ctx.GetLogger<JwtService>();
            var result = new JwtService(options, clock, logger);

            return result;
        }

        #endregion
    }

    public static class ContainerBuilderExtension {
        #region Public Static Methods

        public static ContainerBuilder RegisterWebModule(this ContainerBuilder self) {
            self.RegisterModule<WebModule>();

            return self;
        }

        #endregion
    }
}