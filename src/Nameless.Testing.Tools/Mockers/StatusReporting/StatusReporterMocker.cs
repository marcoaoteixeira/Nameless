using System.Diagnostics.CodeAnalysis;
using Moq;
using Nameless.Reporting;

namespace Nameless.Testing.Tools.Mockers.StatusReporting;

[ExcludeFromCodeCoverage]
public sealed class StatusReporterMocker<TService> : Mocker<IStatusReporter<TService>> {
    private readonly string _channelKey;

    public StatusReporterMocker(string? channelKey = null) {
        _channelKey = string.IsNullOrWhiteSpace(channelKey) ? string.Empty : channelKey;
    }
    
    public StatusReporterMocker<TService> WithReport(Action<StatusUpdate> callback) {
        MockInstance
            .Setup(mock => mock.Report(It.IsAny<string>(), It.IsAny<StatusLevel>(), It.IsAny<Dictionary<string, string>?>()))
            .Callback(ReportCallback);

        return this;

        void ReportCallback(string message, StatusLevel statusLevel, Dictionary<string, string>? metadata) {
            callback(new StatusUpdate(
                typeof(TService).GetNameWithNamespace(),
                message,
                _channelKey,
                statusLevel,
                timestamp: DateTimeOffset.Now,
                metadata
            ));
        }
    }

    public StatusReporterMocker<TService> WithFault(Action<string, string?> callback) {
        MockInstance
            .Setup(mock => mock.Fault(It.IsAny<string>(), It.IsAny<string?>()))
            .Callback(callback);

        return this;
    }

    public StatusReporterMocker<TService> WithFault(Action<Exception> callback) {
        MockInstance
            .Setup(mock => mock.Fault(It.IsAny<Exception>()))
            .Callback(callback);

        return this;
    }
}
