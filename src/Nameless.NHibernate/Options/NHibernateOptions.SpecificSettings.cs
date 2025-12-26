using System.ComponentModel;

namespace Nameless.NHibernate.Options;

public sealed record SpecificSettings : SettingsBase {
    [Description(description: "firebird.disable_parameter_casting")]
    public bool? FirebirdDisableParameterCasting { get; set; }

    [Description(description: "oracle.use_n_prefixed_types_for_unicode")]
    public bool? OracleUseNPrefixedTypesForUnicode { get; set; }

    [Description(description: "odbc.explicit_datetime_scale")]
    public int? OdbcExplicitDateTimeScale { get; set; }
}