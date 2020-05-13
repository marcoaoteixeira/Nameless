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

            self
                .AddScoped<IUserStore<User>, UserStore> ()
                .AddScoped<IRoleStore<Role>, RoleStore> ();
        }

        #endregion
    }
}