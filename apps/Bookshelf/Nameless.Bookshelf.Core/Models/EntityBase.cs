using System;

namespace Nameless.Bookshelf.Models {
    public abstract class EntityBase {
        #region Public Virtual Properties

        public virtual Guid ID { get; set; }
        public virtual DateTime CreationDate { get; set; }
        public virtual DateTime? ModificationDate { get; set; }

        #endregion
    }

    public abstract class OwnedEntity : EntityBase {
        #region Public Virtual Properties

        public virtual Guid OwnerID { get; set; }

        #endregion
    }
}
