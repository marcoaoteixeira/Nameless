using System.ComponentModel;

namespace Nameless.NHibernate.Objects;

/// <summary>
/// Specifies the flushing behavior for a session or operation.
/// </summary>
/// <remarks>
/// The <see cref="FlushMode"/> enumeration defines the strategies for
/// determining when changes are synchronized with the underlying data store.
/// Use this enumeration to configure the  desired flush behavior based on the
/// application's requirements.
/// </remarks>
public enum FlushMode {
    [Description("Auto")] Auto,

    [Description("Manual")] Manual,

    [Description("Commit")] Commit,

    [Description("Always")] Always
}