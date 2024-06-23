using System.ComponentModel;

namespace Nameless.NHibernate.Objects {
    public enum HbmToDdlKeyword {
        [Description("none")]
        None,

        [Description("keywords")]
        Keywords,

        [Description("auto-quote")]
        AutoQuote
    }
}
