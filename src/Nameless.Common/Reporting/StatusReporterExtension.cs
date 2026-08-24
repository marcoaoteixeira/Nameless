namespace Nameless.Reporting;

/// <summary>
///     <see cref="IStatusReporter{TService}"/> extension methods.
/// </summary>
public static class StatusReporterExtension {
    /// <typeparam name="TService">
    ///     Type of the service to report.
    /// </typeparam>
    /// <param name="self">
    ///     The current instance of <see cref="IStatusReporter{TService}"/>.
    /// </param>
    extension<TService>(IStatusReporter<TService> self) {
        /// <summary>
        ///     Reports information
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        public void ReportInfo(string message) {
            self.Report(message, StatusLevel.Info);
        }

        /// <summary>
        ///     Reports warning.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        public void ReportWarn(string message) {
            self.Report(message, StatusLevel.Warning);
        }

        /// <summary>
        ///     Reports error.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        public void ReportError(string message) {
            self.Report(message, StatusLevel.Error);
        }
    }
}
