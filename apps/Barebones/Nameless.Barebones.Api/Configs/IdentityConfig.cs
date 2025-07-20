using Nameless.Barebones.Domains;
using Nameless.Barebones.Domains.Entities.Identity;

namespace Nameless.Barebones.Api.Configs;

public static class IdentityConfig {
    public static WebApplicationBuilder RegisterIdentity(this WebApplicationBuilder self) {
        self.Services
            .AddIdentityApiEndpoints<User>(options => {
                options.SignIn.RequireConfirmedEmail = false;

                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(2);
            })
            .AddRoles<Role>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        return self;
    }

    public static WebApplication UseIdentityEndpoints(this WebApplication self) {
        self.MapIdentityApi<User>();

        return self;
    }
}
