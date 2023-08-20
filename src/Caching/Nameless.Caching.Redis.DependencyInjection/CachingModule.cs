﻿using System.Reflection;
using Autofac;
using Autofac.Core.Registration;
using Microsoft.Extensions.Configuration;
using Nameless.Autofac;
using Nameless.Caching.Redis.Impl;

namespace Nameless.Caching.Redis.DependencyInjection {
    public sealed class CachingModule : ModuleBase {
        #region Private Constants

        private const string CONNECTION_MULTIPLEXER_MANAGER_TOKEN = $"{nameof(IConnectionMultiplexerManager)}::ded40b71-3532-47ed-a3c9-9a5d76e37916";

        #endregion

        #region Public Constructors

        public CachingModule()
            : base(Array.Empty<Assembly>()) { }

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .Register(ResolveConnectionMultiplexerManager)
                .Named<IConnectionMultiplexerManager>(CONNECTION_MULTIPLEXER_MANAGER_TOKEN)
                .SingleInstance();

            builder
                .Register(ResolveRedisCache)
                .As<ICache>()
                .SingleInstance();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static RedisOptions? GetRedisOptions(IComponentContext ctx) {
            var configuration = ctx.ResolveOptional<IConfiguration>();
            var options = configuration?
                .GetSection(nameof(RedisOptions).RemoveTail(new[] { "Options" }))
                .Get<RedisOptions>();

            return options;
        }

        private static IConnectionMultiplexerManager ResolveConnectionMultiplexerManager(IComponentContext ctx) {
            var options = GetRedisOptions(ctx);
            var result = new ConnectionMultiplexerManager(options);

            return result;
        }

        private static ICache ResolveRedisCache(IComponentContext ctx) {
            var connectionMultiplexerManager = ctx.ResolveNamed<IConnectionMultiplexerManager>(CONNECTION_MULTIPLEXER_MANAGER_TOKEN);
            var multiplexer = connectionMultiplexerManager.GetMultiplexer();
            var database = multiplexer.GetDatabase();
            var result = new RedisCache(database);

            return result;
        }

        #endregion
    }

    public static class ContainerBuilderExtension {
        #region Public Static Methods

        public static IModuleRegistrar AddCaching(this ContainerBuilder self)
            => self.RegisterModule<CachingModule>();

        #endregion
    }
}