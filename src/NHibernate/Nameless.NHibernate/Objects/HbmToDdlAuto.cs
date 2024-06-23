using System.ComponentModel;

namespace Nameless.NHibernate.Objects {
    public enum HbmToDdlAuto {
        [Description("none")]
        None,

        [Description("create")]
        Create,

        [Description("create-drop")]
        CreateDrop,

        [Description("validate")]
        Validate,

        [Description("update")]
        Update
    }
}
