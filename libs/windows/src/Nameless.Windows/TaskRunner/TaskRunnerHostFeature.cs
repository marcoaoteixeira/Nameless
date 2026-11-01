using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Hosting;

namespace Nameless.Windows.TaskRunner;

/// <summary>
///     Task Runner Host Feature
/// </summary>
public class TaskRunnerHostFeature : HostFeature {
    /// <inheritdoc />
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        builder.Services.TryAddSingleton<ITaskRunner, TaskRunnerImpl>();
    }
}