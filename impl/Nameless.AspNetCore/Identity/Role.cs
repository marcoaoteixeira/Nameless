namespace Nameless.AspNetCore.Identity {
    public class Role {
        #region Public Virtual Properties

        public virtual string ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string NormalizedName { get; set; }

        #endregion

        #region Public Virtual Methods

        public virtual bool Equals (Role obj) => obj != null && obj.ID == ID;

        #endregion

        #region Public Override Methods

        public override bool Equals (object obj) => Equals (obj as Role);

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