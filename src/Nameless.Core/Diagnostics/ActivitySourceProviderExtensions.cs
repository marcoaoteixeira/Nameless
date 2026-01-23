using System.Reflection;

namespace Nameless.Diagnostics;

/// <summary>
///     <see cref="IActivitySourceProvider"/> extension methods.
/// </summary>
public static class ActivitySourceProviderExtensions {
    extension(IActivitySourceProvider self) {
        /// <summary>
        ///     Creates a new <see cref="IActivitySource"/> using the calling
        ///     assembly's.
        /// </summary>
        /// <returns>
        ///     An instance of <see cref="IActivitySource"/> class.
        /// </returns>
        public IActivitySource Create() {
            return self.Create(Assembly.GetCallingAssembly());
        }

        /// <summary>
        ///     Creates a new <see cref="IActivitySource"/> using the specified
        ///     assembly.
        /// </summary>
        /// <returns>
        ///     An instance of <see cref="IActivitySource"/> class.
        /// </returns>
        public IActivitySource Create(Assembly assembly) {
            return self.Create(
                assembly.GetSemanticName(),
                assembly.GetSemanticVersion()
            );
        }
    }
}