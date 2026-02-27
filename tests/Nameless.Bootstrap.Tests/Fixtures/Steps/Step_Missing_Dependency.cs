using System.Diagnostics;
using Nameless.Bootstrap.Infrastructure;
using Nameless.Bootstrap.Notification;

namespace Nameless.Bootstrap.Fixtures.Steps;

[DebuggerDisplay("{Name,nq}")]
public class Step_Missing_Dependency : StepBase {
    public override IReadOnlyCollection<string> Dependencies => [
        "Missing_Dependency_Step"
    ];

    public Step_Missing_Dependency(bool enabled = true) : base(enabled) { }

    public override Task ExecuteAsync(FlowContext context, IProgress<StepProgress> progress, CancellationToken cancellationToken) {
        progress.ReportInformation(Name, "[Step_Missing_Dependency]");

        return Task.CompletedTask;
    }
}