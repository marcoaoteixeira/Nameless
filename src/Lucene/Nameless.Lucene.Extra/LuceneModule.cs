using Autofac;
using Autofac.Core;
using Nameless.Autofac;

namespace Nameless.Lucene {
    public sealed class LuceneModule : ModuleBase {
        #region Private Constants

        private const string ANALYZER_PROVIDER_KEY = "bb2792e9-8251-4191-ad9e-51d574e8a9b9";
        private const string ANALYZER_SELECTOR_KEY = "396285b4-dcf3-4cd1-91b6-9635c2b4683d";

        #endregion

        #region Public Properties

        public Type[] AnalyzerSelectorImplementations { get; set; } = Array.Empty<Type>();

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .RegisterType<IndexProvider>()
                .As<IIndexProvider>()
                .WithParameter(ResolvedParameter.ForNamed<IAnalyzerProvider>(ANALYZER_PROVIDER_KEY))
                .SingleInstance();

            var analyzerSelectors = AnalyzerSelectorImplementations.IsNullOrEmpty()
                ? SearchForImplementations<IAnalyzerSelector>()
                : AnalyzerSelectorImplementations;
            builder
                .RegisterTypes(analyzerSelectors)
                .Named<IAnalyzerSelector>(ANALYZER_SELECTOR_KEY)
                .SingleInstance();

            builder
                .RegisterType<AnalyzerProvider>()
                .Named<IAnalyzerProvider>(ANALYZER_PROVIDER_KEY)
                .WithParameter(ResolvedParameter.ForNamed<IAnalyzerSelector[]>(ANALYZER_SELECTOR_KEY))
                .SingleInstance();

            base.Load(builder);
        }

        #endregion
    }
}
