using System.ComponentModel;

namespace Nameless.NHibernate.Objects {
    public enum ReleaseMode {
        [Description("auto")]
        Auto,

        [Description("on_close")]
        OnClose,

        [Description("after_transaction")]
        AfterTransaction
    }
}
