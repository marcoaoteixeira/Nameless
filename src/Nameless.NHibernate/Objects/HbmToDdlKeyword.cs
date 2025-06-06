using System.ComponentModel;

namespace Nameless.NHibernate.Objects;

/// <summary>
/// Specifies the mapping behavior for handling database keywords during the
/// conversion from HBM (Hibernate Mapping) to DDL (Data Definition Language).
/// </summary>
/// <remarks>
/// This enumeration defines options for how database keywords are treated
/// during schema generation. The behavior is determined by the selected
/// value: <see cref="None"/>, <see cref="Keywords"/>, or
/// <see cref="AutoQuote"/>.
/// </remarks>
public enum HbmToDdlKeyword {
    [Description("none")] None,

    [Description("keywords")] Keywords,

    [Description("auto-quote")] AutoQuote
}