using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nameless.AspNetCore;
using Nameless.Bookshelf.Queries;
using Nameless.CQRS;
using Nameless.WebApplication.Core.Helpers;
using Nameless.WebApplication.Web.Areas.Bookshelf.Models;

namespace Nameless.WebApplication.Web.Areas.Bookshelf.Controllers {
    [Area ("Bookshelf")]
    public class HomeController : MvcController {
        #region Private Read-Only Fields

        private readonly IDispatcher _dispatcher;

        #endregion

        #region Public Constructors

        public HomeController (IDispatcher dispatcher) {
            Prevent.ParameterNull (dispatcher, nameof (dispatcher));

            _dispatcher = dispatcher;
        }

        #endregion

        #region Public Methods

        [HttpGet]
        public IActionResult Index () {
            return View ();
        }

        [HttpGet]
        public async Task<IActionResult> Search ([FromQuery] BookSearchParameters parameters) {
            BookSearchResult model;

            try { model = await DoSearch (parameters); }
            catch (Exception ex) { return BadRequest (new { error = ex.Message }); }

            return View (model);
        }

        #endregion

        #region Private Methods

        private async Task<BookSearchResult> DoSearch (BookSearchParameters parameters) {
            var books = await _dispatcher.QueryAsync (new BookSearchQuery {
                Title = parameters.Title,
                OwnerID = parameters.Owned ? User.GetUserID<Guid>() : new Guid?(),
                PageIndex = parameters.PageIndex,
                PageSize = parameters.PageSize
            });

            var result = books.Select (item => new BookSearchItem {
                ID = item.ID,
                Title = item.Title,
                Review = item.Review,
                ISBN = item.ISBN,
                Year = item.Year,
                Edition = item.Edition,
                Volume = item.Volume,
                Rating = item.Rating,
                Base64Image = ImageHelper.GetBase64Image (item.Image)
            });

            var model = new BookSearchResult {
                Parameters = parameters,
                Result = result.ToList ()
            };

            return model;
        }

        #endregion
    }
}