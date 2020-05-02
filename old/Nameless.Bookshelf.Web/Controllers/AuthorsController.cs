using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nameless.Bookshelf.Commands;
using Nameless.Bookshelf.Models;
using Nameless.Bookshelf.Queries;
using Nameless.Bookshelf.Web.Models;
using Nameless.CQRS;

namespace Nameless.Bookshelf.Web.Controllers {
    public class AuthorsController : Controller {
        #region Private Read-Only Fields

        private readonly IDispatcher _dispatcher;

        #endregion

        #region Public Constructors

        public AuthorsController (IDispatcher dispatcher) {
            Prevent.ParameterNull (dispatcher, nameof (dispatcher));

            _dispatcher = dispatcher;
        }

        #endregion

        #region Public Methods

        [HttpGet]
        public IActionResult Create () {
            return View ();
        }

        [HttpPost]
        public async Task<IActionResult> Create (AuthorModel model) {
            if (!ModelState.IsValid) { return BadRequest (ModelState); }

            var command = new CreateAuthorCommand { Name = model.Name };

            try { await _dispatcher.CommandAsync (command); }
            catch (Exception ex) { return BadRequest (new { error = ex.Message }); }
            
            return Ok (new { id = command.ID });
        }

        [HttpGet]
        public async Task<IActionResult> Update (Guid id) {
            Author author;

            try { author = await _dispatcher.QueryAsync (new GetAuthorQuery { ID = id }); }
            catch (Exception ex) { return BadRequest (new { error = ex.Message }); }

            if (author == null) { return NotFound (); }

            return View (new AuthorModel {
                ID = author.ID,
                Name = author.Name
            });
        }

        [HttpPost]
        public async Task<IActionResult> Update (AuthorModel model) {
            if (!ModelState.IsValid) { return BadRequest (ModelState); }

            try {
                await _dispatcher.CommandAsync (new UpdateAuthorCommand {
                    ID = model.ID,
                    Name = model.Name
                });
            } catch (Exception ex) { return BadRequest (new { error = ex.Message }); }
            
            return Ok (new { id = model.ID });
        }

        [HttpGet]
        public IActionResult Show (Guid id) {
            return View ();
        }

        #endregion
    }
}