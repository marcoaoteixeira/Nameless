using System.Diagnostics;
using Nameless.Bootstrap.Infrastructure;
using Nameless.Bootstrap.Notification;

namespace Nameless.Bootstrap.Fixtures.Steps;

[DebuggerDisplay("{Name,nq}")]
public class Step_Level_2_0 : StepBase {
    public override IReadOnlyCollection<string> Dependencies => [
        nameof(Step_Level_2_1),
        nameof(Step_Level_2_2),
        nameof(Step_Level_2_3),
    ];

    public Step_Level_2_0(bool enabled = true) : base(enabled) { }

    public override Task ExecuteAsync(FlowContext context, IProgress<StepProgress> progress, CancellationToken cancellationToken) {
        progress.ReportInformation(Name, "[2.0] Step: Executing...");

        return Task.CompletedTask;
    }
}