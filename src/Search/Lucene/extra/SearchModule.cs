using Autofac;
using Nameless.DependencyInjection.Autofac;

namespace Nameless.Search.Lucene {
    /// <summary>
    /// Search service registration.
    /// </summary>
    public sealed class SearchModule : ModuleBase {

        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="IIndexProvider"/> lifetime scope type. Default is: <see cref="LifetimeScopeType.Singleton"/>.
        /// </summary>
        public LifetimeScopeType IndexProviderLifetimeScopeType { get; set; } = LifetimeScopeType.Singleton;

        /// <summary>
        /// Gets or sets the <see cref="IAnalyzerProvider"/> lifetime scope type. Default is: <see cref="LifetimeScopeType.Singleton"/>.
        /// </summary>
        public LifetimeScopeType AnalyzerProviderLifetimeScopeType { get; set; } = LifetimeScopeType.Singleton;

        /// <summary>
        /// Gets or sets the <see cref="IAnalyzerSelector"/> lifetime scope type. Default is: <see cref="LifetimeScopeType.Singleton"/>.
        /// </summary>
        public LifetimeScopeType AnalyzerSelectorLifetimeScopeType { get; set; } = LifetimeScopeType.Singleton;

        /// <summary>
        /// Gets or sets the <see cref="ISearchBuilder"/> lifetime scope type. Default is: <see cref="LifetimeScopeType.Singleton"/>.
        /// </summary>
        public LifetimeScopeType SearchBuilderLifetimeScopeType { get; set; } = LifetimeScopeType.Transient;

        #endregion

        #region Protected Override Methods

        /// <inheritdoc/>
        protected override void Load (ContainerBuilder builder) {
            builder
                .RegisterType<IndexProvider> ()
                .As<IIndexProvider> ()
                .SetLifetimeScope (IndexProviderLifetimeScopeType);

            builder
                .RegisterType<AnalyzerProvider> ()
                .As<IAnalyzerProvider> ()
                .SetLifetimeScope (AnalyzerProviderLifetimeScopeType);

            builder
                .RegisterType<DefaultAnalyzerSelector> ()
                .As<IAnalyzerSelector> ()
                .SetLifetimeScope (AnalyzerSelectorLifetimeScopeType);

            builder
                .RegisterType<SearchBuilder> ()
                .As<ISearchBuilder> ()
                .SetLifetimeScope (SearchBuilderLifetimeScopeType);

            base.Load (builder);
        }

        #endregion
    }
}