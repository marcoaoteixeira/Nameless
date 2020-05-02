using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Nameless.WebApplication.Web.Areas.Bookshelf.Models {
    public class BookSearchParameters {
        #region Public Properties

        [DisplayName ("Título")]
        public string Title { get; set; }
        
        [DisplayName ("Somente os meus")]
        public bool Owned { get; set; }

        public int PageIndex { get; set; } = 0;
        
        [DisplayName ("Itens por página")]
        public int PageSize { get; set; } = 20;

        #endregion
    }
}