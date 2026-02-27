using System.Diagnostics;
using Nameless.Bootstrap.Infrastructure;
using Nameless.Bootstrap.Notification;

namespace Nameless.Bootstrap.Fixtures.Steps;

[DebuggerDisplay("{Name,nq}")]
public class Step_Circular_Y : StepBase {
    public override IReadOnlyCollection<string> Dependencies => [
        nameof(Step_Circular_X)
    ];

    public Step_Circular_Y(bool enabled = true) : base(enabled) { }

    public override Task ExecuteAsync(FlowContext context, IProgress<StepProgress> progress, CancellationToken cancellationToken) {
        progress.ReportInformation(Name, "[Circular Y] Step: Executing...");

        return Task.CompletedTask;
    }
}