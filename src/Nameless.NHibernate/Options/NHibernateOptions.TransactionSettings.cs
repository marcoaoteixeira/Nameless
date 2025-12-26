using System.ComponentModel;

namespace Nameless.NHibernate.Options;

public sealed record TransactionSettings : SettingsBase {
    [Description(description: "transaction.factory_class")]
    public string? FactoryClass { get; set; }

    [Description(description: "transaction.use_connection_on_system_prepare")]
    public bool? UseConnectionOnSystemPrepare { get; set; }

    [Description(description: "transaction.system_completion_lock_timeout")]
    public int? SystemCompletionLockTimeout { get; set; }
}