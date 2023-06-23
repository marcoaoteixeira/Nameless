namespace Nameless {
    /// <summary>
    /// <see cref="EventHandler{TEventArgs}"/> extension methods.
    /// </summary>
    public static class EventHandlerExtension {
        #region Public Static Methods

        /// <summary>
        /// Before invokes the handler, checks if it is not <c>null</c>.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event handler.</typeparam>
        /// <param name="self">The event handler.</param>
        /// <param name="sender">The sender object.</param>
        /// <param name="args">The event arguments.</param>
        public static void SafeInvoke<TEventArgs>(this EventHandler<TEventArgs> self, object sender, TEventArgs args) where TEventArgs : EventArgs
            => self?.Invoke(sender, args);

        #endregion Public Static Methods
    }
}