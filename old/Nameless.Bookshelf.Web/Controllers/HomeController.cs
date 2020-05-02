using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nameless.Bookshelf.Queries;
using Nameless.Bookshelf.Web.Models;
using Nameless.CQRS;

namespace Nameless.Bookshelf.Web.Controllers {
    public sealed class HomeController : Controller {
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

        [Authorize (AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<IActionResult> Index () {
            var searchResult = new BookSearchResultViewModel ();
            var books = await _dispatcher.QueryAsync (new SearchBooksQuery ());
            foreach (var book in books) {
                searchResult.Result.Add (new BookSearchResultItemViewModel {
                    ID = book.ID,
                        Title = book.Title,
                        Summary = book.Review,
                        OwnerName = book.OwnerID.ToString (),
                        Rating = book.Rating,
                        Status = "Owned"
                });
            }
            var searchForm = new BookSearchModel ();
            var model = new BooksViewModel {
                SearchForm = searchForm,
                SearchResult = searchResult
            };

            return View (model);
        }

        [HttpPost]
        public async Task<IActionResult> Search (BookSearchModel model) {
            var books = await _dispatcher.QueryAsync (new SearchBooksQuery {
                Title = model.SearchTerm,
                    OwnerID = model.Owned ? User.GetUserID<Guid> () : new Guid?(),
                    PageIndex = model.PageIndex,
                    PageSize = model.PageSize
            });

            var result = new BooksViewModel {
                SearchForm = model,
                SearchResult = new BookSearchResultViewModel {
                CurrentPage = model.PageIndex + 1,
                TotalPageCount = 1
                }
            };
            foreach (var book in books) {
                result.SearchResult.Result.Add (new BookSearchResultItemViewModel {
                    ID = book.ID,
                        Title = book.Title,
                        Summary = book.Review,
                        OwnerName = book.OwnerID.ToString (),
                        Rating = book.Rating,
                        Status = "Owned"
                });
            }

            return View (nameof (Index), result);
        }

        #endregion
    }
}