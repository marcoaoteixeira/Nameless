using Nameless.Bootstrap;
using Nameless.Bootstrap.Infrastructure;
using Nameless.Bootstrap.Notification;

namespace Nameless.WPF.Client.Bootstrap;

public class FirstFakeStep : StepBase {
    public override string DisplayName => "First Fake Step";

    public override async Task ExecuteAsync(FlowContext context, IProgress<StepProgress> progress, CancellationToken cancellationToken) {
        progress.ReportInformation(DisplayName, "Initializing first fake step...");

        await Task.Delay(500, cancellationToken);

        progress.ReportInformation(DisplayName, "Waiting first 500ms...");

        await Task.Delay(500, cancellationToken);

        progress.ReportInformation(DisplayName, "Waiting second 500ms...");

        await Task.Delay(500, cancellationToken);

        progress.ReportInformation(DisplayName, "Waiting third 500ms...");

        await Task.Delay(500, cancellationToken);

        progress.ReportInformation(DisplayName, "First fake step finished.");

        await Task.Delay(500, cancellationToken);
    }
}