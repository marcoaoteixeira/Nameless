using System;

namespace Nameless.Skeleton.Web.Areas.Identity.Models {
    public class UserViewModel {
        #region Public Properties

        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Position { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public string AvatarUrl { get; set; }
        public string[] Roles { get; set; }

        #endregion
    }
}