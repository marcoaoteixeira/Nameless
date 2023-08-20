using System.Reflection;
using Autofac;
using Autofac.Core.Registration;
using Microsoft.Extensions.Configuration;
using Nameless.Autofac;
using Nameless.Web.Options;
using Nameless.Web.Services;
using Nameless.Web.Services.Impl;

namespace Nameless.NHibernate.DependencyInjection {
    public sealed class WebModule : ModuleBase {
        #region Public Constructors

        public WebModule()
            : base(Array.Empty<Assembly>()) { }

        public WebModule(Assembly[] supportAssemblies)
            : base(supportAssemblies) { }

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .Register(ResolveJwtService)
                .As<IJwtService>()
                .SingleInstance();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static JwtOptions? GetJwtOptions(IComponentContext ctx) {
            var configuration = ctx.ResolveOptional<IConfiguration>();
            var options = configuration?
                .GetSection(nameof(JwtOptions).RemoveTail(new[] { "Options" }))
                .Get<JwtOptions>();

            return options;
        }

        private static IJwtService ResolveJwtService(IComponentContext ctx) {
            var options = GetJwtOptions(ctx);
            var result = new JwtService(options);

            return result;
        }

        #endregion
    }

    public static class ContainerBuilderExtension {
        #region Public Static Methods

        public static IModuleRegistrar AddWeb(this ContainerBuilder self, params Assembly[] supportAssemblies)
            => self.RegisterModule(new WebModule(supportAssemblies));

        #endregion
    }
}