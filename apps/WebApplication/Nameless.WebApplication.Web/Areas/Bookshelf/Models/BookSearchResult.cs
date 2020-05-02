using System.Collections.Generic;

namespace Nameless.WebApplication.Web.Areas.Bookshelf.Models {
    public class BookSearchResult {
        #region Public Properties

        public BookSearchParameters Parameters { get; set; } = new BookSearchParameters ();
        public IList<BookSearchItem> Result { get; set; } = new List<BookSearchItem> ();

        #endregion
    }
}