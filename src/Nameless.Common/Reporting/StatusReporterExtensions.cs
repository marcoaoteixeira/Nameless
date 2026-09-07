namespace Nameless.Reporting;

/// <summary>
///     <see cref="IStatusReporter"/> extension methods.
/// </summary>
public static class StatusReporterExtensions {
    extension(IStatusReporter self) {
        /// <summary>
        ///     Publishes a new information status, replacing whatever was reported
        ///     before.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <param name="metadata">
        ///     The metadata.
        /// </param>
        public void ReportInfo(string message, Dictionary<string, string>? metadata = null) {
            self.Report(message, level: StatusLevel.Info, metadata);
        }

        /// <summary>
        ///     Publishes a new warning status, replacing whatever was reported
        ///     before.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <param name="metadata">
        ///     The metadata.
        /// </param>
        public void ReportWarn(string message, Dictionary<string, string>? metadata = null) {
            self.Report(message, StatusLevel.Warning, metadata);
        }

        /// <summary>
        ///     Publishes a new error status, replacing whatever was reported
        ///     before.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <param name="metadata">
        ///     The metadata.
        /// </param>
        public void ReportError(string message, Dictionary<string, string>? metadata = null) {
            self.Report(message, StatusLevel.Error, metadata);
        }

        /// <summary>
        ///     Signals that this channel has finished with a failure. No
        ///     further <see cref="IStatusReporter.Report"/> calls will have
        ///     any effect. Consumers observe this as
        ///     <see cref="IObserver{T}.OnError"/> with a
        ///     <see cref="FaultException"/>.
        /// </summary>
        /// <param name="reason">
        ///     A human-readable description of why the channel faulted.
        /// </param>
        public void Fault(string reason) {
            self.Fault(reason, code: null);
        }
    }
}