using System.Diagnostics;
using Nameless.Bootstrap.Infrastructure;
using Nameless.Bootstrap.Notification;

namespace Nameless.Bootstrap.Fixtures.Steps;

[DebuggerDisplay("{Name,nq}")]
public class Step_Root : StepBase {
    public override IReadOnlyCollection<string> Dependencies => [
        nameof(Step_Child_A),
        nameof(Step_Child_B),
        nameof(Step_Child_C),
    ];

    public Step_Root(bool enabled = true) : base(enabled) { }

    public override Task ExecuteAsync(FlowContext context, IProgress<StepProgress> progress, CancellationToken cancellationToken) {
        progress.ReportInformation(Name, "[Root] Step: Executing...");

        return Task.CompletedTask;
    }
}