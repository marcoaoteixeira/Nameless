using System.ComponentModel;

namespace Nameless.NHibernate.Options {
    public enum FlushMode {
        [Description("Auto")]
        Auto,

        [Description("Manual")]
        Manual,

        [Description("Commit")]
        Commit,

        [Description("Always")]
        Always
    }
}
