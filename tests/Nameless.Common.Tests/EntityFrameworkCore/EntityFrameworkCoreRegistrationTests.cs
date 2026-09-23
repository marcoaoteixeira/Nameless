using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Nameless.EntityFrameworkCore;

[UnitTest]
public class EntityFrameworkCoreRegistrationTests {
    private sealed class FakeInterceptor : IInterceptor;

    private sealed class FakeDatabaseSeeder : IDatabaseSeeder {
        public int Order => 0;
        public Task ExecuteAsync(Microsoft.EntityFrameworkCore.DbContext dbContext, bool storeManagementOperation, CancellationToken cancellationToken)
            => Task.CompletedTask;
        public void Execute(Microsoft.EntityFrameworkCore.DbContext dbContext, bool storeManagementOperation) { }
    }

    [Fact]
    public void WithUseDbContextFactory_SetsValue() {
        // arrange
        var sut = new EntityFrameworkCoreRegistration();

        // act
        var returned = sut.WithUseDbContextFactory(true);

        // assert
        Assert.Multiple(
            () => Assert.Same(sut, returned),
            () => Assert.True(sut.UseDbContextFactory)
        );
    }

    [Fact]
    public void WithDbContextConfiguration_SetsValue() {
        // arrange
        var sut = new EntityFrameworkCoreRegistration();
        Action<IServiceProvider, Microsoft.EntityFrameworkCore.DbContextOptionsBuilder> configure = (_, _) => { };

        // act
        var returned = sut.WithDbContextConfiguration(configure);

        // assert
        Assert.Multiple(
            () => Assert.Same(sut, returned),
            () => Assert.Same(configure, sut.DbContextConfiguration)
        );
    }

    [Fact]
    public void WithInterceptor_Generic_AddsType() {
        // arrange
        var sut = new EntityFrameworkCoreRegistration();

        // act
        sut.WithInterceptor<FakeInterceptor>();

        // assert
        Assert.Contains(typeof(FakeInterceptor), sut.Interceptors);
    }

    [Fact]
    public void WithInterceptor_CalledTwiceWithSameType_AddsOnce() {
        // arrange
        var sut = new EntityFrameworkCoreRegistration();

        // act
        sut.WithInterceptor<FakeInterceptor>();
        sut.WithInterceptor<FakeInterceptor>();

        // assert
        Assert.Single(sut.Interceptors);
    }

    [Fact]
    public void WithInterceptor_WithTypeNotAssignableFromIInterceptor_Throws() {
        // arrange
        var sut = new EntityFrameworkCoreRegistration();

        // act & assert
        Assert.Throws<ArgumentException>(() => sut.WithInterceptor(typeof(string)));
    }

    [Fact]
    public void WithDatabaseSeeder_Generic_AddsType() {
        // arrange
        var sut = new EntityFrameworkCoreRegistration().WithUseAssemblyScan(false);

        // act
        sut.WithDatabaseSeeder<FakeDatabaseSeeder>();

        // assert
        Assert.Contains(typeof(FakeDatabaseSeeder), sut.DatabaseSeeders);
    }

    [Fact]
    public void WithDatabaseSeeder_CalledTwiceWithSameType_AddsOnce() {
        // arrange
        var sut = new EntityFrameworkCoreRegistration().WithUseAssemblyScan(false);

        // act
        sut.WithDatabaseSeeder<FakeDatabaseSeeder>();
        sut.WithDatabaseSeeder<FakeDatabaseSeeder>();

        // assert
        Assert.Single(sut.DatabaseSeeders);
    }

    [Fact]
    public void WithDatabaseSeeder_WithTypeNotAssignableFromIDatabaseSeeder_Throws() {
        // arrange
        var sut = new EntityFrameworkCoreRegistration();

        // act & assert
        Assert.Throws<ArgumentException>(() => sut.WithDatabaseSeeder(typeof(string)));
    }
}
