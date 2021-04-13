using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nameless.AspNetCore.Identity;
using Nameless.Skeleton.Web.Areas.Identity.Models;
using NHibernate.Linq;

namespace Nameless.Skeleton.Web.Areas.Identity.Pages.Administration {

    [AllowAnonymous]
    public sealed class UsersPageModel : PageModel {
        #region Private Read-Only Fields

        private readonly IQueryableUserStore<User> _queryableUserStore;

        #endregion

        #region Public Properties

        [BindProperty]
        public SearchInputModel Input { get; set; }

        public IEnumerable<UserViewModel> CurrentUsers { get; set; } = Array.Empty<UserViewModel> ();

        #endregion

        #region Public Constructors

        public UsersPageModel (IQueryableUserStore<User> queryableUserStore) {
            Prevent.ParameterNull (queryableUserStore, nameof (queryableUserStore));

            _queryableUserStore = queryableUserStore;
        }

        #endregion

        #region Public Methods

        public IActionResult OnGet () {
            return Page ();
        }

        public IActionResult OnPost () {
            var query = _queryableUserStore.Users;

            if (!string.IsNullOrWhiteSpace (Input.Term)) {
                query = query.Where (_ =>
                    _.UserName.Like (string.Concat ("%", Input.Term, "%")) ||
                    _.Email.Like (string.Concat ("%", Input.Term, "%"))
                );
            }

            CurrentUsers = query.ToList ().Select (_ => new UserViewModel {
                Id = _.Id,
                UserName = _.UserName,
                Email = _.Email,
                PhoneNumber = _.PhoneNumber,
                LockoutEnabled = _.LockoutEnabled,
                LockoutEnd = _.LockoutEnd,
                AvatarUrl = _.AvatarUrl,
                Position = "Software Developer",
                Roles = new[] { "User", "Administrator" }
            });

            return Page ();
        }

        #endregion

        #region Public Classes

        public sealed class SearchInputModel {
            #region Public Properties

            public string Term { get; set; }

            #endregion
        }

        #endregion
    }
}