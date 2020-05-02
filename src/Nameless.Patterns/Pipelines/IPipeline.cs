namespace Nameless.Patterns.Pipelines {

    /// <summary>
    /// Defines methods to implement a pipeline for filters.
    /// </summary>
    /// <typeparam name="T">Type of the data.</typeparam>
    public interface IPipeline<T> {

        #region Methods

        /// <summary>
        /// Executes the pipe line with the given input data.
        /// </summary>
        /// <param name="input">The input data.</param>
        void Execute(T input);

        /// <summary>
        /// Registers a filter for the current pipeline.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>The current instance of the pipeline.</returns>
        IPipeline<T> RegisterFilter(IFilter<T> filter);

        #endregion Methods
    }
}