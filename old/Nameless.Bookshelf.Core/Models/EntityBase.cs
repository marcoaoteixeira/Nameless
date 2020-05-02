using System;

namespace Nameless.Bookshelf.Models {
    public abstract class EntityBase {
        #region Public Properties

        public Guid ID { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }

        #endregion
    }

    public abstract class OwnedEntity : EntityBase {
        #region Public Properties

        public Guid OwnerID { get; set; }

        #endregion
    }
}
