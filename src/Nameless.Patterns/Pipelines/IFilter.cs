namespace Nameless.Patterns.Pipelines {

    /// <summary>
    /// Defines methods to implement a filter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFilter<T> {

        #region Methods

        /// <summary>
        /// Executes the filter and returns the processed version of the input data.
        /// </summary>
        /// <param name="input">The input data.</param>
        /// <returns>The processed version of the input data.</returns>
        T Execute(T input);

        /// <summary>
        /// Registers the next filter to execute.
        /// </summary>
        /// <param name="filter">The next filter to execute.</param>
        void RegisterFilter(IFilter<T> filter);

        #endregion Methods
    }
}