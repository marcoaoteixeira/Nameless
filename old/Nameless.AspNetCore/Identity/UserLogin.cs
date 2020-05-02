using System;
using Microsoft.AspNetCore.Identity;

namespace Nameless.AspNetCore.Identity {
    public class UserLogin {
        #region Public Properties

        public Guid UserID { get; set; }
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string ProviderDisplayName { get; set; }

        #endregion

        #region Public Static Methods

        public static UserLoginInfo Parse (UserLogin userLogin) {
            return new UserLoginInfo (
                userLogin.LoginProvider,
                userLogin.ProviderKey,
                userLogin.ProviderDisplayName
            );
        }

        #endregion
    }
}