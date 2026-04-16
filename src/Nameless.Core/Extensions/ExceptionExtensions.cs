using System.Runtime.InteropServices;
using System.Security;

namespace Nameless;

/// <summary>
///     <see cref="Exception" /> extension methods.
/// </summary>
public static class ExceptionExtensions {
    /// <param name="self">The self <see cref="Exception" />.</param>
    extension(Exception self) {
        /// <summary>
        ///     Returns <see langword="true"/> if is a fatal exception.
        /// </summary>
        /// <returns><see langword="true"/> if is fatal, otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        ///     What would be considered fatal exception? All exceptions listed below:
        ///     - <see cref="StackOverflowException" />
        ///     - <see cref="OutOfMemoryException" />
        ///     - <see cref="AccessViolationException" />
        ///     - <see cref="AppDomainUnloadedException" />
        ///     - <see cref="ThreadAbortException" />
        ///     - <see cref="SecurityException" />
        ///     - <see cref="SEHException" />
        /// </remarks>
        public bool IsFatal() {
            return self is StackOverflowException or
                           OutOfMemoryException or
                           AccessViolationException or
                           AppDomainUnloadedException or
                           ThreadAbortException or
                           SecurityException or
                           SEHException;
        }
    }
}