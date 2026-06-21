using Microsoft.EntityFrameworkCore;
using Moq;
using Nameless.EntityFrameworkCore;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.EntityFrameworkCore;

[UnitTest]
public class NullDatabaseSeederTests {
    [Fact]
    public async Task SeedAsync_DoesNotThrow() {
        // arrange
        var dbContextMock = new Mock<DbContext>();
        var sut = NullDatabaseSeeder.Instance;

        // act
        var exception = await Record.ExceptionAsync(
            () => sut.ExecuteAsync(dbContextMock.Object, storeManagementOperation: false, CancellationToken.None)
        );

        // assert
        Assert.Null(exception);
    }

    [Fact]
    public void Execute_DoesNotThrow() {
        // arrange
        var dbContextMock = new Mock<DbContext>();
        var sut = NullDatabaseSeeder.Instance;

        // act
        var exception = Record.Exception(
            () => sut.Execute(dbContextMock.Object, storeManagementOperation: false)
        );

        // assert
        Assert.Null(exception);
    }

    [Fact]
    public void Instance_IsSingleton() {
        // arrange & act
        var first = NullDatabaseSeeder.Instance;
        var second = NullDatabaseSeeder.Instance;

        // assert
        Assert.Same(first, second);
    }
}
