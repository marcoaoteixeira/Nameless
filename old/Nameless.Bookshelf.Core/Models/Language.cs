using System;
using System.Data;
using Nameless.Data;

namespace Nameless.Bookshelf.Models {
    public sealed class Language : EntityBase {
        #region Public Properties

        public string Name { get; set; }

        #endregion

        #region Public Static Methods

        public static Language Map (IDataRecord record) {
            Prevent.ParameterNull (record, nameof (record));

            return new Language {
                ID = record.GetGuidOrDefault (nameof (ID)).GetValueOrDefault (),
                Name = record.GetStringOrDefault (nameof (Name)),
                CreationDate = record.GetDateTimeOrDefault (nameof (CreationDate)).GetValueOrDefault (),
                ModificationDate = record.GetDateTimeOrDefault (nameof (ModificationDate))
            };
        }

        #endregion

        #region Public Methods

        public bool Equals (Language obj) {
            return obj != null && string.Equals (obj.Name, Name, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion

        #region Public Override Methods

        public override bool Equals (object obj) {
            return Equals (obj as Language);
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