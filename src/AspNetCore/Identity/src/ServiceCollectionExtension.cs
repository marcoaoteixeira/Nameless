using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.AspNetCore.Identity {
    public static class ServiceCollectionExtension {
        #region Public Static Methods

        public static void AddNamelessIdentity (this IServiceCollection self) {
            self
                .AddIdentity<User, Role> (opts => {
                    // Password settings.
                    opts.Password.RequireDigit = true;
                    opts.Password.RequireLowercase = true;
                    opts.Password.RequireNonAlphanumeric = true;
                    opts.Password.RequireUppercase = true;
                    opts.Password.RequiredLength = 6;
                    opts.Password.RequiredUniqueChars = 1;

                    // Lockout settings.
                    opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes (5);
                    opts.Lockout.MaxFailedAccessAttempts = 5;
                    opts.Lockout.AllowedForNewUsers = true;

                    // User settings.
                    //opts.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                    opts.User.RequireUniqueEmail = true;
                });

            var userStoreInterfaces = typeof (UserStore).GetInterfaces ();
            foreach (var userStoreInterface in userStoreInterfaces) {
                if (userStoreInterface == typeof (IDisposable)) continue;
                self.AddScoped (userStoreInterface, typeof (UserStore));
            }

            var roleStoreInterfaces = typeof (RoleStore).GetInterfaces ();
            foreach (var roleStoreInterface in roleStoreInterfaces) {
                if (roleStoreInterface == typeof (IDisposable)) continue;
                self.AddScoped (roleStoreInterface, typeof (RoleStore));
            }
        }

        #endregion
    }
}