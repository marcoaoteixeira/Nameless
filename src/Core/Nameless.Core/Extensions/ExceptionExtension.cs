using System.Runtime.InteropServices;
using System.Security;

namespace Nameless {
    /// <summary>
    /// <see cref="Exception"/> extension methods.
    /// </summary>
    public static class ExceptionExtension {
        #region Public Static Methods

        /// <summary>
        /// Returns <c>true</c> if is a fatal exception.
        /// </summary>
        /// <param name="self">The self <see cref="Exception"/>.</param>
        /// <returns><c>true</c> if is fatal, otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// What would be considered fatal exception? All exceptions listed below:
        /// - <see cref="FatalException"/>
        /// - <see cref="StackOverflowException"/>
        /// - <see cref="OutOfMemoryException"/>
        /// - <see cref="AccessViolationException"/>
        /// - <see cref="AppDomainUnloadedException"/>
        /// - <see cref="ThreadAbortException"/>
        /// - <see cref="SecurityException"/>
        /// - <see cref="SEHException"/>
        /// </remarks>
        public static bool IsFatal(this Exception self) => self is FatalException or
                StackOverflowException or
                OutOfMemoryException or
                AccessViolationException or
                AppDomainUnloadedException or
                ThreadAbortException or
                SecurityException or
                SEHException;

        #endregion Public Static Methods
    }
}