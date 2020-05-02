namespace Nameless.AspNetCore.Identity.Stores.NHibernate.Models {
    public class UserInRole {
        #region Public Virtual Properties

        public virtual string UserID { get; set; }
        public virtual string RoleID { get; set; }

        #endregion

        #region Public Virtual Methods

        public virtual bool Equals (UserInRole obj) {
            return obj != null &&
                obj.UserID == UserID &&
                obj.RoleID == RoleID;

        }

        #endregion

        #region Public Override Methods

        public override bool Equals (object obj) => Equals (obj as UserInRole);

        public override int GetHashCode () {
            var hash = 13;
            unchecked {
                hash += (UserID ?? string.Empty).GetHashCode () * 7;
                hash += (RoleID ?? string.Empty).GetHashCode () * 7;
            }
            return hash;
        }

        #endregion
    }
}