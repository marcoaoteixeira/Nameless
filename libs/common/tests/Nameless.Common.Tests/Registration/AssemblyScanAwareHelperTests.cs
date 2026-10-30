using System.Reflection;

namespace Nameless.Registration;

[UnitTest]
public class AssemblyScanAwareHelperTests {
    private sealed class Scanner : AssemblyScanAware<Scanner>;

    [Fact]
    public void MergeAssemblies_WithoutConfiguration_AppliesAssemblies() {
        // arrange
        Assembly[] assemblies = [typeof(string).Assembly, typeof(AssemblyScanAwareHelperTests).Assembly];
        var sut = new Scanner();

        // act
        AssemblyScanAwareHelper.MergeAssemblies<Scanner>(configuration: null, assemblies)(sut);

        // assert
        Assert.Equal(2, sut.Assemblies.Count);
    }

    [Fact]
    public void MergeAssemblies_WithConfiguration_AppliesBoth() {
        // arrange
        var sut = new Scanner();

        // act
        AssemblyScanAwareHelper.MergeAssemblies<Scanner>(
            configuration: scanner => scanner.WithUseAssemblyScan(false),
            assemblies: [typeof(string).Assembly]
        )(sut);

        // assert
        Assert.Multiple(
            () => Assert.Single(sut.Assemblies),
            () => Assert.False(sut.UseAssemblyScan)
        );
    }
}
