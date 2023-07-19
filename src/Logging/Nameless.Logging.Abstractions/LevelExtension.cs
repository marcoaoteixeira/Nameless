namespace Nameless.Logging {
    /// <summary>
    /// <see cref="Level"/> extension methods.
    /// </summary>
    public static class LevelExtension {
        #region Public Static Methods

        /// <summary>
        /// Checks if the specified flag is setted on the current flag.
        /// </summary>
        /// <param name="self">The source (<see cref="Level"/>).</param>
        /// <param name="flags">The flag (<see cref="Level"/>).</param>
        /// <returns><c>true</c> if is setted, otherwise, <c>false</c>.</returns>
        public static bool IsSet(this Level self, Level flags)
            => (self & flags) == flags;

        #endregion
    }
}
