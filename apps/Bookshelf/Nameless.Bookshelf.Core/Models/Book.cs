using System.Collections.Generic;

namespace Nameless.Bookshelf.Models {
    public class Book : OwnedEntity {
        #region Public Virtual Properties

        public virtual string Title { get; set; }
        public virtual string Review { get; set; }
        public virtual string ISBN { get; set; }
        public virtual int? Year { get; set; }
        public virtual int? Edition { get; set; }
        public virtual int? Volume { get; set; }
        public virtual double? Rating { get; set; }
        public virtual byte[] Image { get; set; }
        public virtual Publisher Publisher { get; set; }
        public virtual ISet<Author> Authors { get; } = new HashSet<Author> ();
        public virtual ISet<Language> Languages { get; } = new HashSet<Language> ();

        #endregion

        #region Public Virtual Methods

        public virtual bool Equals (Book obj) {
            return obj != null && obj.ID != ID && obj.OwnerID == OwnerID;
        }

        #endregion

        #region Public Override Methods

        public override bool Equals (object obj) {
            return Equals (obj as Book);
        }

        public override int GetHashCode () {
            var hash = 13;
            unchecked {
                hash += ID.GetHashCode () * 7;
                hash += OwnerID.GetHashCode () * 7;
            }
            return hash;
        }

        #endregion
    }
}