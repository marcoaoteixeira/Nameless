namespace Nameless.WinApp.Data;

[UnitTest]
public class AppDbContextDesignTimeDbContextFactoryTests {
    [Fact]
    public void HappyPath() {
        using var sut = new AppDbContextDesignTimeDbContextFactory();
        using var dbContext = sut.CreateDbContext([]);

        Assert.NotNull(dbContext);
    }
}
