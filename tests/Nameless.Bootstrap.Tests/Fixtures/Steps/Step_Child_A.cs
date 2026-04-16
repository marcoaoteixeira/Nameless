using System.Diagnostics;
using Nameless.Bootstrap.Infrastructure;
using Nameless.Bootstrap.Notification;

namespace Nameless.Bootstrap.Fixtures.Steps;

[DebuggerDisplay("{DisplayName,nq}")]
public class Step_Child_A : StepBase {
    public Step_Child_A(bool enabled = true) : base(enabled) { }

    public override Task ExecuteAsync(FlowContext context, IProgress<StepProgress> progress, CancellationToken cancellationToken) {
        progress.ReportInformation(DisplayName, "[Child A] Step: Executing...");

        return Task.CompletedTask;
    }
}