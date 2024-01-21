using System.ComponentModel;

namespace Nameless.NHibernate.Options {
    public enum ReleaseMode {
        [Description("auto")]
        Auto,

        [Description("on_close")]
        OnClose,

        [Description("after_transaction")]
        AfterTransaction
    }
}
