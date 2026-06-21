using Nameless.Bootstrap;
using Nameless.Bootstrap.Notification;

namespace Nameless.WinApp.Bootstrap;

public class SecondFakeStep : StepBase {
    public override string DisplayName => "Second Fake Step";

    public override async Task ExecuteAsync(FlowContext context, IProgress<StepProgress> progress, CancellationToken cancellationToken) {
        progress.ReportInformation(DisplayName, "Initializing second fake step...");

        await Task.Delay(500, cancellationToken);

        progress.ReportInformation(DisplayName, "Waiting first 500ms...");

        await Task.Delay(500, cancellationToken);

        progress.ReportInformation(DisplayName, "Waiting second 500ms...");

        await Task.Delay(500, cancellationToken);

        progress.ReportInformation(DisplayName, "Waiting third 500ms...");

        await Task.Delay(500, cancellationToken);

        progress.ReportInformation(DisplayName, "Second fake step finished.");

        await Task.Delay(500, cancellationToken);
    }
}