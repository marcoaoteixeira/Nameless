using System.ComponentModel;

namespace Nameless.NHibernate.Objects;

/// <summary>
/// Specifies the modes for releasing resources in a controlled manner.
/// </summary>
/// <remarks>
/// This enumeration defines the available options for determining when
/// resources should be released. Use the appropriate mode based on the
/// desired timing of resource cleanup.
/// </remarks>
public enum ReleaseMode {
    [Description("auto")] Auto,

    [Description("on_close")] OnClose,

    [Description("after_transaction")] AfterTransaction
}