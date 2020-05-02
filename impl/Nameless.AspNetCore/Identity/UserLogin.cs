using System;
using Microsoft.AspNetCore.Identity;

namespace Nameless.AspNetCore.Identity {
    public class UserLogin {
        #region Public Virtual Properties

        public virtual string UserID { get; set; }
        public virtual string LoginProvider { get; set; }
        public virtual string ProviderKey { get; set; }
        public virtual string ProviderDisplayName { get; set; }

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

        #region Public Virtual Methods

        public virtual bool Equals (UserLogin obj) {
            return obj != null &&
                obj.UserID == UserID &&
                obj.LoginProvider == LoginProvider &&
                obj.ProviderKey == ProviderKey;
        }

        #endregion

        #region Public Override Methods

        public override bool Equals (object obj) => Equals (obj as UserLogin);

        public override int GetHashCode () {
            var hash = 13;
            unchecked {
                hash += (UserID ?? string.Empty).GetHashCode () * 7;
                hash += (LoginProvider ?? string.Empty).GetHashCode () * 7;
                hash += (ProviderKey ?? string.Empty).GetHashCode () * 7;
            }
            return hash;
        }

        #endregion
    }
}