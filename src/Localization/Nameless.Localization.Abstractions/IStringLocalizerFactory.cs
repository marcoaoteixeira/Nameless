namespace Nameless.Localization {
    public interface IStringLocalizerFactory {
        #region Methods

        /// <summary>
        /// Creates a <see cref="IStringLocalizer"/>, using the source type.
        /// </summary>
        /// <param name="resource">The resource type.</param>
        /// <returns>An instance of <see cref="IStringLocalizer"/> implementation.</returns>
        IStringLocalizer Create(Type resource);
        /// <summary>
        /// Creates a <see cref="IStringLocalizer"/>, using the source name and source path.
        /// </summary>
        /// <param name="resourceName">The resource name.</param>
        /// <param name="resourcePath">The resource path.</param>
        /// <returns>An instance of <see cref="IStringLocalizer"/> implementation.</returns>
        IStringLocalizer Create(string resourceName, string resourcePath);

        #endregion
    }
}