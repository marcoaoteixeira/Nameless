using System;

namespace Nameless.AspNetCore.Identity {
    public class UserToken {
        #region Public Properties

        public Guid UserID { get; set; }
        public string LoginProvider { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        #endregion
    }
}