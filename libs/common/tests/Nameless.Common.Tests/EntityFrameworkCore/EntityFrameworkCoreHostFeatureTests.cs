using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Hosting;
using Nameless.Testing.Tools.Mockers.Hosting;

namespace Nameless.EntityFrameworkCore;

[IntegrationTest]
public class EntityFrameworkCoreHostFeatureTests {
    private sealed class TestDbContext : DbContext {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
    }

    private sealed class FakeInterceptor : SaveChangesInterceptor;

    private sealed class FakeDatabaseSeeder : IDatabaseSeeder {
        public int Order => 0;
        public Task ExecuteAsync(DbContext dbContext, bool storeManagementOperation, CancellationToken cancellationToken) {
            return Task.CompletedTask;
        }

        public void Execute(DbContext dbContext, bool storeManagementOperation) { }
    }
    
    private static IServiceCollection CreateEntityFrameworkCoreHostFeature(Action<EntityFrameworkCoreFeatureOptions>? configure = null) {
        var hostApplicationBuilder = new HostApplicationBuilderMocker().WithServices(services =>
            services.AddLogging()
                    .AddSingleton<IConfiguration>(new ConfigurationBuilder().Build())
        ).Build();

        var sut = new EntityFrameworkCoreHostFeature<TestDbContext>();

        if (configure is not null) {
            sut.Configure(configure);
        }

        sut.Register(hostApplicationBuilder, new HostFeatureContext([], NullLogger.Instance));

        return hostApplicationBuilder.Services;
    }

    [Fact]
    public void RegisterEntityFrameworkCore_RegistersDbContext() {
        // arrange
        var services = CreateEntityFrameworkCoreHostFeature();

        // act
        var provider = services.BuildServiceProvider();

        // assert
        Assert.NotNull(provider.GetRequiredService<TestDbContext>());
    }

    [Fact]
    public void RegisterEntityFrameworkCore_WithUseDbContextFactory_RegistersFactory() {
        // arrange
        var services = CreateEntityFrameworkCoreHostFeature(
            options => options.WithUseDbContextFactory(true)
        );

        // act
        var provider = services.BuildServiceProvider();

        // assert
        Assert.NotNull(provider.GetRequiredService<IDbContextFactory<TestDbContext>>());
    }

    [Fact]
    public void RegisterEntityFrameworkCore_WithInterceptor_RegistersInterceptor() {
        // arrange
        var services = CreateEntityFrameworkCoreHostFeature(
            options => options.WithInterceptor<FakeInterceptor>()
        );

        // act
        var provider = services.BuildServiceProvider();

        // assert
        Assert.Contains(provider.GetServices<IInterceptor>(), i => i is FakeInterceptor);
    }

    [Fact]
    public void RegisterEntityFrameworkCore_WithDatabaseSeeder_RegistersAggregator() {
        // arrange
        var services = CreateEntityFrameworkCoreHostFeature(
            options => options.WithDatabaseSeeder<FakeDatabaseSeeder>()
        );

        // act
        var provider = services.BuildServiceProvider();

        // assert
        Assert.NotNull(provider.GetRequiredService<DatabaseSeederAggregator>());
    }

    [Fact]
    public void RegisterEntityFrameworkCore_WithCustomDbContextConfiguration_UsesCustomConfiguration() {
        // arrange
        var invoked = false;
        var services = CreateEntityFrameworkCoreHostFeature(
            options => options.WithDbContextConfiguration((_, builder) => {
                invoked = true;
                builder.UseSqlite("Data Source=:memory:");
            })
        );
        
        // act
        var provider = services.BuildServiceProvider();
        _ = provider.GetRequiredService<TestDbContext>();

        // assert
        Assert.True(invoked);
    }
}
