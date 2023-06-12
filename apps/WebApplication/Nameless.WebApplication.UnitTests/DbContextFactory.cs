using Microsoft.EntityFrameworkCore;
using Nameless.WebApplication.Entities;

namespace Nameless.WebApplication.UnitTests {

    public sealed class DbContextFactory {

        public static ApplicationDbContext CreateInMemory() {
            var opts = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite("Filename=:memory:")
                .Options;

            var context = new ApplicationDbContext(opts);

            context.Database.OpenConnection();

            if (!context.Database.EnsureCreated()) {
                throw new InvalidOperationException();
            }

            return context;
        }
    }
}
