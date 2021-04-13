using System;

namespace Nameless.Skeleton.Web.Areas.Identity.Api.v1.Models.Administration {
    public sealed class UserModel {
        #region Public Properties

        public Guid ID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool IsLocked { get; set; }
        public DateTimeOffset? LockoutEndDate { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public int AccessFailedCount { get; set; }

        #endregion
    }
}