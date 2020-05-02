using System.ComponentModel.DataAnnotations;

namespace Nameless.Bookshelf.Web.Models {
    public sealed class BookSearchModel {
        #region Public Properties

        [Display (Name = "Termo de busca")]
        public string SearchTerm { get; set; }

        [Display (Name = "Somente meus livros")]
        public bool Owned { get; set; }

        [Display (Name = "Resultados por página")]
        public int PageSize { get; set; } = 20;
        public int PageIndex { get; set; }

        #endregion
    }
}