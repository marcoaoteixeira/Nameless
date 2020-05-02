using System;

namespace Nameless.Bookshelf.Models {
    public class Publisher : EntityBase {
        #region Public Virtual Properties

        public virtual string Name { get; set; }

        #endregion

        #region Public Virtual Methods

        public virtual bool Equals (Publisher obj) {
            return obj != null && string.Equals (obj.Name, Name, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion

        #region Public Override Methods

        public override bool Equals (object obj) {
            return Equals (obj as Publisher);
        }

        public override int GetHashCode () {
            var hash = 13;
            unchecked {
                hash += (Name ?? string.Empty).GetHashCode () * 7;
            }
            return hash;
        }

        #endregion
    }
}