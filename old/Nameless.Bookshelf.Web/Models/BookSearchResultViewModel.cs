using System;
using System.Collections.Generic;

namespace Nameless.Bookshelf.Web.Models {
    public sealed class BookSearchResultViewModel {
        #region Public Properties

        public IList<BookSearchResultItemViewModel> Result { get; set; } = new List<BookSearchResultItemViewModel> ();
        public int CurrentPage { get; set; } = 1;
        public int TotalPageCount { get; set; } = 1;

        #endregion
    }

    public sealed class BookSearchResultItemViewModel {
        #region Public Properties

        public Guid ID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Base64Image { get; set; }
        public string OwnerName { get; set; }
        public string Status { get; set; }
        public double? Rating { get; set; }

        #endregion
    }
}
