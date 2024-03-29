﻿using Autofac;
using Nameless.Autofac;
using Nameless.Infrastructure;
using Nameless.Lucene.Impl;
using Nameless.Lucene.Options;

namespace Nameless.Lucene.DependencyInjection {
    public sealed class LuceneModule : ModuleBase {
        #region Private Constants

        private const string ANALYZER_PROVIDER_TOKEN = $"{nameof(AnalyzerProvider)}::6b375c0e-bffc-4d98-9e3d-3692216e6b15";
        private const string ANALYZER_SELECTOR_TOKEN = $"{nameof(IAnalyzerSelector)}::72f43237-7a7b-4914-80ad-6c958a425021";

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .Register(IndexProviderResolver)
                .As<IIndexManager>()
                .SingleInstance();

            var analyzerSelectors = GetImplementations<IAnalyzerSelector>().ToArray();
            builder
                .RegisterTypes(analyzerSelectors)
                .Named<IAnalyzerSelector>(ANALYZER_SELECTOR_TOKEN)
                .SingleInstance();

            builder
                .Register(AnalyzerProviderResolver)
                .Named<IAnalyzerProvider>(ANALYZER_PROVIDER_TOKEN)
                .SingleInstance();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static IAnalyzerProvider AnalyzerProviderResolver(IComponentContext ctx) {
            var analyzerSelector = ctx.ResolveNamed<IAnalyzerSelector[]>(ANALYZER_SELECTOR_TOKEN);
            var result = new AnalyzerProvider(analyzerSelector);

            return result;
        }

        private static IIndexManager IndexProviderResolver(IComponentContext ctx) {
            var applicationContext = ctx.Resolve<IApplicationContext>();
            var analyzerProvider = ctx.ResolveNamed<IAnalyzerProvider>(ANALYZER_PROVIDER_TOKEN);
            var options = ctx.GetPocoOptions<LuceneOptions>();
            var result = new IndexManager(applicationContext, analyzerProvider, options);

            return result;
        }

        #endregion
    }

    public static class ContainerBuilderExtension {
        #region Public Static Methods

        public static ContainerBuilder RegisterLuceneModule(this ContainerBuilder self) {
            self.RegisterModule<LuceneModule>();

            return self;
        }

        #endregion
    }
}
