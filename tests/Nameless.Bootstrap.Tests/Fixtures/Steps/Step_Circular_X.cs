using System.Diagnostics;
using Nameless.Bootstrap.Infrastructure;
using Nameless.Bootstrap.Notification;

namespace Nameless.Bootstrap.Fixtures.Steps;

[DebuggerDisplay("{Name,nq}")]
public class Step_Circular_X : StepBase {
    public override IReadOnlyCollection<string> Dependencies => [
        nameof(Step_Circular_Y)
    ];

    public Step_Circular_X(bool enabled = true) : base(enabled) { }

    public override Task ExecuteAsync(FlowContext context, IProgress<StepProgress> progress, CancellationToken cancellationToken) {
        progress.ReportInformation(Name, "[Circular X] Step: Executing...");

        return Task.CompletedTask;
    }
}