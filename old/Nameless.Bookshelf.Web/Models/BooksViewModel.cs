namespace Nameless.Bookshelf.Web.Models {
    public sealed class BooksViewModel {
        #region Public Properties

        public BookSearchModel SearchForm { get; set; }
        public BookSearchResultViewModel SearchResult { get; set; }

        #endregion
    }
}