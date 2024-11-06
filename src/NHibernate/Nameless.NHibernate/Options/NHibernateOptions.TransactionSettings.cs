using System.ComponentModel;

namespace Nameless.NHibernate.Options;

public sealed record TransactionSettings : ConfigurationNode {
    [Description("transaction.factory_class")]
    public string? FactoryClass { get; set; }

    [Description("transaction.use_connection_on_system_prepare")]
    public bool? UseConnectionOnSystemPrepare { get; set; }

    [Description("transaction.system_completion_lock_timeout")]
    public int? SystemCompletionLockTimeout { get; set; }
}