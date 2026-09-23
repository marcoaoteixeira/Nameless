using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.EntityFrameworkCore;

[IntegrationTest]
public class ServiceCollectionExtensionsTests {
    private sealed class TestDbContext : DbContext {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
    }

    private sealed class FakeInterceptor : SaveChangesInterceptor;

    private sealed class FakeDatabaseSeeder : IDatabaseSeeder {
        public int Order => 0;
        public Task ExecuteAsync(DbContext dbContext, bool storeManagementOperation, CancellationToken cancellationToken)
            => Task.CompletedTask;
        public void Execute(DbContext dbContext, bool storeManagementOperation) { }
    }

    private static ServiceCollection CreateServices() {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());

        return services;
    }

    [Fact]
    public void RegisterEntityFrameworkCore_RegistersDbContext() {
        // arrange
        var services = CreateServices();

        // act
        var returned = services.RegisterEntityFrameworkCore<TestDbContext>();

        // assert
        Assert.Same(services, returned);

        var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<TestDbContext>());
    }

    [Fact]
    public void RegisterEntityFrameworkCore_WithUseDbContextFactory_RegistersFactory() {
        // arrange
        var services = CreateServices();

        // act
        services.RegisterEntityFrameworkCore<TestDbContext>(
            configure: registration => registration.WithUseDbContextFactory(true)
        );

        // assert
        var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<IDbContextFactory<TestDbContext>>());
    }

    [Fact]
    public void RegisterEntityFrameworkCore_WithInterceptor_RegistersInterceptor() {
        // arrange
        var services = CreateServices();

        // act
        services.RegisterEntityFrameworkCore<TestDbContext>(
            configure: registration => registration.WithInterceptor<FakeInterceptor>()
        );

        // assert
        var provider = services.BuildServiceProvider();

        Assert.Contains(provider.GetServices<IInterceptor>(), i => i is FakeInterceptor);
    }

    [Fact]
    public void RegisterEntityFrameworkCore_WithDatabaseSeeder_RegistersAggregator() {
        // arrange
        var services = CreateServices();

        // act
        services.RegisterEntityFrameworkCore<TestDbContext>(
            configure: registration => registration.WithDatabaseSeeder<FakeDatabaseSeeder>()
        );

        // assert
        var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<DatabaseSeederAggregator>());
    }

    [Fact]
    public void RegisterEntityFrameworkCore_WithCustomDbContextConfiguration_UsesCustomConfiguration() {
        // arrange
        var services = CreateServices();
        var invoked = false;

        // act
        services.RegisterEntityFrameworkCore<TestDbContext>(
            configure: registration => registration.WithDbContextConfiguration((_, builder) => {
                invoked = true;
                builder.UseSqlite("Data Source=:memory:");
            })
        );

        var provider = services.BuildServiceProvider();
        _ = provider.GetRequiredService<TestDbContext>();

        // assert
        Assert.True(invoked);
    }
}
