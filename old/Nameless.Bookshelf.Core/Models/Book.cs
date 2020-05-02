using System;
using System.Collections.Generic;
using System.Data;
using Nameless.Data;

namespace Nameless.Bookshelf.Models {
    public sealed class Book : OwnedEntity {
        #region Public Properties

        public string Title { get; set; }
        public string Review { get; set; }
        public string ISBN { get; set; }
        public int? Year { get; set; }
        public int? Edition { get; set; }
        public int? Volume { get; set; }
        public double? Rating { get; set; }
        public byte[] Image { get; set; }
        public Publisher Publisher { get; set; }
        public ISet<Author> Authors { get; } = new HashSet<Author> ();
        public ISet<Language> Languages { get; } = new HashSet<Language> ();

        #endregion

        #region Public Static Methods

        public static Book Map (IDataRecord record) {
            Prevent.ParameterNull (record, nameof (record));

            return new Book {
                ID = record.GetGuidOrDefault (nameof (ID)).GetValueOrDefault (),
                Title = record.GetStringOrDefault (nameof (Title)),
                Review = record.GetStringOrDefault (nameof (Review)),
                ISBN = record.GetStringOrDefault (nameof (ISBN)),
                Year = record.GetInt32OrDefault (nameof (Year)),
                Edition = record.GetInt32OrDefault (nameof (Edition)),
                Volume = record.GetInt32OrDefault (nameof (Volume)),
                Rating = record.GetDoubleOrDefault (nameof (Rating)),
                Image = record.GetBlobOrDefault (nameof (Image)),
                OwnerID = record.GetGuidOrDefault (nameof (OwnerID)).GetValueOrDefault (),
                CreationDate = record.GetDateTimeOrDefault (nameof (CreationDate)).GetValueOrDefault (),
                ModificationDate = record.GetDateTimeOrDefault (nameof (ModificationDate))
            };
        }

        #endregion

        #region Public Methods

        public bool Equals (Book obj) {
            return obj != null &&
                string.Equals (obj.Title, Title, StringComparison.InvariantCultureIgnoreCase) &&
                obj.OwnerID == OwnerID;
        }

        #endregion

        #region Public Override Methods

        public override bool Equals (object obj) {
            return Equals (obj as Book);
        }

        public override int GetHashCode () {
            var hash = 13;
            unchecked {
                hash += (Title ?? string.Empty).GetHashCode () * 7;
                hash += OwnerID.GetHashCode () * 7;
            }
            return hash;
        }

        #endregion
    }
}