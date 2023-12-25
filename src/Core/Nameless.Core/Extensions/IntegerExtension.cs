namespace Nameless {
    /// <summary>
    /// <see cref="int"/> extension methods.
    /// </summary>
    public static class IntegerExtension {
        #region Public Static Methods

        /// <summary>
        /// Runs a loop by the number of times that the <paramref name="self"/> parameter informs.
        /// Using the action specified.
        /// </summary>
        /// <param name="self">The self <see cref="int"/>.</param>
        /// <param name="action">The action of the interaction.</param>
        /// <exception cref="ArgumentNullException">if <paramref name="action"/> is <c>null</c>.</exception>
        public static void Times(this int self, Action action) {
            Guard.Against.Null(action, nameof(action));

            self.Times(_ => action());
        }

        /// <summary>
        /// Runs a loop by the number of times that the <paramref name="self"/> parameter informs.
        /// Using the action specified. In this case, the action receives a parameter, that will be
        /// the index of the interaction.
        /// </summary>
        /// <param name="self">The self <see cref="int"/>.</param>
        /// <param name="action">The action of the interaction.</param>
        /// <exception cref="ArgumentNullException">if <paramref name="action"/> is <c>null</c>.</exception>
        public static void Times(this int self, Action<int> action) {
            Guard.Against.Null(action, nameof(action));

            for (var number = 0; number < self; number++) {
                action(number);
            }
        }

        #endregion
    }
}