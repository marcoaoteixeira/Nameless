using System;
using Nameless.AspNetCore.Identity.Stores.NHibernate.Models;
using NHibernate;

namespace Nameless.AspNetCore.Identity.NHibernate.Test {
    public static class IdentityHelper {
        public static User CreateUser (ISession session, string id) {
            return CreateUser (session, new User { ID = string.IsNullOrWhiteSpace (id) ? Guid.NewGuid ().ToString () : id });
        }

        public static User CreateUser (ISession session, User example) {
            var id = Guid.NewGuid ().ToString ();
            example = example ?? new User { ID = id };
            if (string.IsNullOrWhiteSpace (example.ID)) { example.ID = id; }

            var complement = example.ID.Replace ("-", string.Empty).Replace (" ", string.Empty);
            var result = new User {
                ID = example.ID,
                Email = example.Email ?? $"user_{complement}@user.com",
                EmailConfirmed = example.EmailConfirmed,
                NormalizedEmail = example.NormalizedEmail ?? $"USER_{complement}@USER.COM",
                UserName = example.UserName ?? $"User_{complement}",
                NormalizedUserName = example.NormalizedUserName ?? $"USER_{complement}",
                PhoneNumber = example.PhoneNumber ?? "000 000 000",
                PhoneNumberConfirmed = example.PhoneNumberConfirmed,
                LockoutEnabled = example.LockoutEnabled,
                LockoutEnd = example.LockoutEnd ?? null,
                AccessFailedCount = example.AccessFailedCount,
                PasswordHash = example.PasswordHash ?? "XYZ",
                SecurityStamp = example.SecurityStamp ?? "XYZ",
                TwoFactorEnabled = example.TwoFactorEnabled,
                AvatarUrl = example.AvatarUrl
            };
            session.Save (result);
            return result;
        }

        public static Role CreateRole (ISession session, string id) {
            return CreateRole (session, new Role { ID = string.IsNullOrWhiteSpace (id) ? Guid.NewGuid ().ToString () : id });
        }

        public static Role CreateRole (ISession session, Role example) {
            var id = Guid.NewGuid ().ToString ();
            example = example ?? new Role { ID = id };
            if (string.IsNullOrWhiteSpace (example.ID)) { example.ID = id; }

            var complement = example.ID.Replace ("-", string.Empty).Replace (" ", string.Empty);
            var result = new Role {
                ID = example.ID,
                Name = example.Name ?? $"role_{complement}",
                NormalizedName = example.NormalizedName ?? $"ROLE_{complement}"
            };
            session.Save (result);
            return result;
        }

        public static UserClaim CreateUserClaim (ISession session, string userID) {
            return CreateUserClaim (session, new UserClaim { UserID = string.IsNullOrWhiteSpace (userID) ? Guid.NewGuid ().ToString () : userID });
        }

        public static UserClaim CreateUserClaim (ISession session, UserClaim example) {
            var id = Guid.NewGuid ().ToString ();
            example = example ?? new UserClaim { UserID = id };
            if (string.IsNullOrWhiteSpace (example.UserID)) { example.UserID = id; }

            var complement = example.UserID.Replace ("-", string.Empty).Replace (" ", string.Empty);
            var result = new UserClaim {
                UserID = example.UserID,
                Type = example.Type ?? $"Type_{complement}",
                Value = example.Value ?? $"Value_{complement}"
            };
            session.Save (result);
            return result;
        }

        public static RoleClaim CreateRoleClaim (ISession session, string userID) {
            return CreateRoleClaim (session, new RoleClaim { RoleID = string.IsNullOrWhiteSpace (userID) ? Guid.NewGuid ().ToString () : userID });
        }

        public static RoleClaim CreateRoleClaim (ISession session, RoleClaim example) {
            var id = Guid.NewGuid ().ToString ();
            example = example ?? new RoleClaim { RoleID = id };
            if (string.IsNullOrWhiteSpace (example.RoleID)) { example.RoleID = id; }

            var complement = example.RoleID.Replace ("-", string.Empty).Replace (" ", string.Empty);
            var result = new RoleClaim {
                RoleID = example.RoleID,
                Type = example.Type ?? $"Type_{complement}",
                Value = example.Value ?? $"Value_{complement}"
            };
            session.Save (result);
            return result;
        }

        public static UserLogin CreateUserLogin (ISession session, string userID) {
            return CreateUserLogin (session, new UserLogin { UserID = string.IsNullOrWhiteSpace (userID) ? Guid.NewGuid ().ToString () : userID });
        }

        public static UserLogin CreateUserLogin (ISession session, UserLogin example) {
            var id = Guid.NewGuid ().ToString ();
            example = example ?? new UserLogin { UserID = id };
            if (string.IsNullOrWhiteSpace (example.UserID)) { example.UserID = id; }

            var complement = example.UserID.Replace ("-", string.Empty).Replace (" ", string.Empty);
            var result = new UserLogin {
                UserID = example.UserID,
                LoginProvider = example.LoginProvider ?? $"LoginProvider_{complement}",
                ProviderKey = example.ProviderKey ?? $"ProviderKey_{complement}",
                ProviderDisplayName = example.ProviderDisplayName ?? $"ProviderDisplayName_{complement}"
            };
            session.Save (result);
            return result;
        }

        public static UserToken CreateUserToken (ISession session, string userID) {
            return CreateUserToken (session, new UserToken { UserID = string.IsNullOrWhiteSpace (userID) ? Guid.NewGuid ().ToString () : userID });
        }

        public static UserToken CreateUserToken (ISession session, UserToken example) {
            var id = Guid.NewGuid ().ToString ();
            example = example ?? new UserToken { UserID = id };
            if (string.IsNullOrWhiteSpace (example.UserID)) { example.UserID = id; }

            var complement = example.UserID.Replace ("-", string.Empty).Replace (" ", string.Empty);
            var result = new UserToken {
                UserID = example.UserID,
                LoginProvider = example.LoginProvider ?? $"LoginProvider_{complement}",
                Name = example.Name ?? $"Name_{complement}",
                Value = example.Value ?? $"Value_{complement}"
            };
            session.Save (result);
            return result;
        }

        public static UserInRole CreateUserInRole (ISession session, string userID, string roleID) {
            Prevent.ParameterNullOrWhiteSpace (userID, nameof (userID));
            Prevent.ParameterNullOrWhiteSpace (roleID, nameof (roleID));

            var userInRole = new UserInRole { UserID = userID, RoleID = roleID };

            session.Save (userInRole);

            return userInRole;
        }
    }
}