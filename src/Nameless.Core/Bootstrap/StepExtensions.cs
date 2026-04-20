namespace Nameless.Bootstrap;

/// <summary>
///     <see cref="IStep"/> extension methods.
/// </summary>
public static class StepExtensions {
    extension(IStep self) {
        /// <summary>
        ///     Whether it is disabled.
        /// </summary>
        public bool IsDisabled => !self.IsEnabled;
    }
}
