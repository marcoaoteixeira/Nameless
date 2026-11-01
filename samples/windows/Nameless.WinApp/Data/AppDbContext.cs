using Microsoft.EntityFrameworkCore;

namespace Nameless.WinApp.Data;

public class AppDbContext(DbContextOptions<AppDbContext> opts) : DbContext(opts);
