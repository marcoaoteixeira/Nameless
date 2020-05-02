using System;

namespace Nameless.Orm.NHibernate {
    public abstract class EntityBase {
        #region Private Fields

        private Guid _id;
        private DateTime _creationDate;
        private DateTime _modificationDate;
        private Guid _owner;

        #endregion

        #region Public Properties

        public virtual Guid ID {
            get { return _id; }
        }
        public virtual DateTime CreationDate {
            get { return _creationDate; }
        }
        public virtual DateTime ModificationDate {
            get { return _modificationDate; }
        }
        public virtual Guid Owner {
            get { return _owner; }
        }

        #endregion

        #region Public Virtual Methods

        public virtual bool Equals (EntityBase obj) {
            return obj != null &&
                obj.ID != ID &&
                obj.Owner != Owner;
        }

        #endregion

        #region Public Override Methods

        public override bool Equals (object obj) {
            return Equals (obj as EntityBase);
        }

        public override int GetHashCode () {
            var hash = 13;
            unchecked {
                hash += ID.GetHashCode () * 7;
                hash += Owner.GetHashCode () * 7;
            }
            return hash;
        }

        #endregion
    }
}