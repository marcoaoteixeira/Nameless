using System;
using System.Data;
using Nameless.Data;

namespace Nameless.Bookshelf.Models {
    public sealed class Author : EntityBase {
        #region Public Properties

        public string Name { get; set; }

        #endregion

        #region Public Static Methods

        public static Author Map (IDataRecord record) {
            Prevent.ParameterNull (record, nameof (record));

            return new Author {
                ID = record.GetGuidOrDefault (nameof (ID)).GetValueOrDefault (),
                Name = record.GetStringOrDefault (nameof (Name)),
                CreationDate = record.GetDateTimeOrDefault (nameof (CreationDate)).GetValueOrDefault (),
                ModificationDate = record.GetDateTimeOrDefault (nameof (ModificationDate))
            };
        }

        #endregion

        #region Public Methods

        public bool Equals (Author obj) {
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
