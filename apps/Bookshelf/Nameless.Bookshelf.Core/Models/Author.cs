using System;
using System.Collections.Generic;

namespace Nameless.Bookshelf.Models {
    public class Author : EntityBase {
        #region Public Virtual Properties

        public virtual string Name { get; set; }
        public virtual ISet<Book> Books { get; set; } = new HashSet<Book> ();

        #endregion

        #region Public Virtual Methods

        public virtual bool Equals (Author obj) {
            return obj != null && string.Equals (obj.Name, Name, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion

        #region Public Override Methods

        public override bool Equals (object obj) {
            return Equals (obj as Author);
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