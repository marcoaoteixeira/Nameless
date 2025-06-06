using System.ComponentModel;

namespace Nameless.NHibernate.Objects;

/// <summary>
/// Specifies the strategy for automatically managing the database schema in
/// relation to the entity mappings.
/// </summary>
/// <remarks>
/// This enumeration defines the options for how the database schema should be
/// handled during application runtime. The selected value determines whether
/// the schema is created, updated, validated, or left unchanged.
/// </remarks>
public enum HbmToDdlAuto {
    [Description("none")] None,

    [Description("create")] Create,

    [Description("create-drop")] CreateDrop,

    [Description("validate")] Validate,

    [Description("update")] Update
}