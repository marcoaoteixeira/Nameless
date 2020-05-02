using System;
using System.Data;
using Nameless.Data;

namespace Nameless.Bookshelf.Models {
    public sealed class Publisher : EntityBase {
        #region Public Properties

        public string Name { get; set; }

        #endregion

        #region Public Static Methods

        public static Publisher Map (IDataRecord record) {
            Prevent.ParameterNull (record, nameof (record));

            return new Publisher {
                ID = record.GetGuidOrDefault (nameof (ID)).GetValueOrDefault (),
                Name = record.GetStringOrDefault (nameof (Name)),
                CreationDate = record.GetDateTimeOrDefault (nameof (CreationDate)).GetValueOrDefault (),
                ModificationDate = record.GetDateTimeOrDefault (nameof (ModificationDate))
            };
        }

        #endregion

        #region Public Methods

        public bool Equals (Publisher obj) {
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