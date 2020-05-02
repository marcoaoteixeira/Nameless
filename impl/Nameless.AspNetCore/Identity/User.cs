using System;

namespace Nameless.AspNetCore.Identity {
    public class User {
        #region Public Virtual Properties

        public virtual string ID { get; set; }
        public virtual string Email { get; set; }
        public virtual bool EmailConfirmed { get; set; }
        public virtual string NormalizedEmail { get; set; }
        public virtual string UserName { get; set; }
        public virtual string NormalizedUserName { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual bool PhoneNumberConfirmed { get; set; }
        public virtual bool LockoutEnabled { get; set; }
        public virtual DateTimeOffset? LockoutEnd { get; set; }
        public virtual int AccessFailedCount { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual string SecurityStamp { get; set; }
        public virtual bool TwoFactorEnabled { get; set; }
        public virtual string AvatarUrl { get; set; }

        #endregion

        #region Public Virtual Methods

        public virtual bool Equals (User obj) => obj != null && obj.ID == ID;

        #endregion

        #region Public Override Methods

        public override bool Equals (object obj) => Equals (obj as User);

        public override int GetHashCode () {
            var hash = 13;
            unchecked {
                hash += (ID ?? string.Empty).GetHashCode () * 7;
            }
            return hash;
        }

        #endregion
    }
}