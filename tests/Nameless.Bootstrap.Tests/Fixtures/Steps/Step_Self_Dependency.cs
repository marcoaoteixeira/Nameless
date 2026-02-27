using System.Diagnostics;
using Nameless.Bootstrap.Infrastructure;
using Nameless.Bootstrap.Notification;

namespace Nameless.Bootstrap.Fixtures.Steps;

[DebuggerDisplay("{Name,nq}")]
public class Step_Self_Dependency : StepBase {
    public override IReadOnlyCollection<string> Dependencies => [
        nameof(Step_Self_Dependency)
    ];

    public Step_Self_Dependency(bool enabled = true) : base(enabled) { }

    public override Task ExecuteAsync(FlowContext context, IProgress<StepProgress> progress, CancellationToken cancellationToken) {
        progress.ReportInformation(Name, "[Self Dependency] Step: Executing...");

        return Task.CompletedTask;
    }
}