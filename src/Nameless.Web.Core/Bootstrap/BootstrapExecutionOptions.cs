using Nameless.Bootstrap.Infrastructure;

namespace Nameless.Web.Bootstrap;

public class BootstrapExecutionOptions {
    public FlowContext Context { get; set; } = [];
    public int Timeout { get; set; } = -1;
}