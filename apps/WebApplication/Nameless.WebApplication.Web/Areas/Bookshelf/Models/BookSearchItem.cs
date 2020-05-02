using System;

namespace Nameless.WebApplication.Web.Areas.Bookshelf.Models {
    public class BookSearchItem {
        #region Public Properties

        public Guid ID { get; set; }
        public string Title { get; set; }
        public string Review { get; set; }
        public string ISBN { get; set; }
        public int? Year { get; set; }
        public int? Edition { get; set; }
        public int? Volume { get; set; }
        public double? Rating { get; set; }
        public string Base64Image { get; set; }

        #endregion
    }
}