namespace Nameless.Patterns.Pipelines {

    /// <summary>
    /// Abstract implementation of <see cref="IFilter{T}"/>
    /// </summary>
    /// <typeparam name="T">Type of the data.</typeparam>
    public abstract class FilterBase<T> : IFilter<T> {
        #region Private Fields

        private IFilter<T> _next;

        #endregion Private Fields

        #region	Protected Abstract Methods

        /// <summary>
        /// Process the input data and returns the output.
        /// </summary>
        /// <param name="input">The input data.</param>
        /// <returns>The processed version of the input data.</returns>
        protected abstract T Process(T input);

        #endregion

        #region IFilter<T> Members

        /// <inheritdoc />
        public T Execute(T input) {
            var value = Process(input);

            return _next != null ? _next.Execute(value) : value;
        }

        /// <inheritdoc />
        public void RegisterFilter(IFilter<T> filter) {
            if (_next != null) { _next.RegisterFilter(filter); } else { _next = filter; }
        }

        #endregion
    }
}