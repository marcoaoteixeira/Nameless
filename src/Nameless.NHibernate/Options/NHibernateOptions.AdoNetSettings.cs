using System.ComponentModel;

namespace Nameless.NHibernate.Options;

public sealed record AdoNetSettings : SettingsBase {
    [Description(description: "adonet.batch_size")]
    public int? BatchSize { get; set; }

    [Description(description: "adonet.batch_versioned_data")]
    public bool? BatchVersionedData { get; set; }

    [Description(description: "adonet.factory_class")]
    public string? FactoryClass { get; set; }

    [Description(description: "adonet.wrap_result_sets")]
    public bool? WrapResultSets { get; set; }
}