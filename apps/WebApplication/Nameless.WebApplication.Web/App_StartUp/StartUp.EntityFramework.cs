using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Nameless.WebApplication.Entities;
using Nameless.WebApplication.Options;
using Sys_Environment = System.Environment;

namespace Nameless.WebApplication.Web {

    public partial class StartUp {

        #region Private Static Methods

        private static void ConfigureEntityFramework(IServiceCollection services, IConfiguration configuration) {
            services.AddDbContext<ApplicationDbContext>(opts => {
                // Get the environment variable telling that we're running on Docker
                var isDocker = Sys_Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER");

                var connectionStringName = string.Equals(isDocker, bool.TrueString, StringComparison.OrdinalIgnoreCase)
                    ? $"{nameof(ApplicationDbContext)}_Docker"
                    : $"{nameof(ApplicationDbContext)}";
                
                var connectionString = configuration.GetConnectionString(connectionStringName);

                opts.UseSqlServer(connectionString);
            });
        }

        private static void UseEntityFramework(IApplicationBuilder applicationBuilder) {
            // Force migration.
            applicationBuilder.ApplicationServices.GetService<ApplicationDbContext>()?.Database.Migrate();

            var userManager = applicationBuilder.ApplicationServices.GetService<UserManager<User>>();
            if (userManager != null) {
                var administratorOptions = applicationBuilder
                    .ApplicationServices
                    .GetService<IOptions<AdministratorOptions>>()?.Value ?? AdministratorOptions.Default;

                var hasAdminUser = userManager.Users.Any(_ => _.Email == administratorOptions.Email);
                if (!hasAdminUser) {
                    var adminUser = new User {
                        Id = administratorOptions.Id,
                        UserName = administratorOptions.UserName,
                        NormalizedUserName = administratorOptions.UserName.ToUpper(),
                        Email = administratorOptions.Email,
                        NormalizedEmail = administratorOptions.Email.ToUpper(),
                        EmailConfirmed = true,
                        PhoneNumber = administratorOptions.PhoneNumber,
                        PhoneNumberConfirmed = true
                    };

                    userManager.CreateAsync(adminUser).Wait();
                    userManager.AddPasswordAsync(adminUser, administratorOptions.Password).Wait();
                }
            }
        }

        #endregion
    }
}
