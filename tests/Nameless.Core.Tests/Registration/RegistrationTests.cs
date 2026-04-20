using System.Reflection;
using Nameless.Registration;

namespace Nameless;

public class RegistrationTests {
    // --- IgnoreAssemblyScanAttribute.IsNotPresent ---

    [Fact]
    public void IsNotPresent_WhenAttributeAbsent_ReturnsTrue() {
        // act & assert
        Assert.True(IgnoreAssemblyScanAttribute.IsNotPresent(typeof(IncludedScanType)));
    }

    [Fact]
    public void IsNotPresent_WhenAttributePresent_ReturnsFalse() {
        // act & assert
        Assert.False(IgnoreAssemblyScanAttribute.IsNotPresent(typeof(IgnoredScanType)));
    }

    // --- AssemblyScanAware ---

    [Fact]
    public void UseAssemblyScan_DefaultValue_IsTrue() {
        // arrange
        var scanner = new ConcreteScanner();

        // act & assert
        Assert.True(scanner.UseAssemblyScan);
    }

    [Fact]
    public void Assemblies_Default_IsEmpty() {
        // arrange
        var scanner = new ConcreteScanner();

        // act & assert
        Assert.Empty(scanner.Assemblies);
    }

    [Fact]
    public void IncludeAssemblyFrom_AddsAssembly() {
        // arrange
        var scanner = new ConcreteScanner();

        // act
        scanner.IncludeAssemblyFrom<RegistrationTests>();

        // assert
        Assert.Single(scanner.Assemblies);
    }

    [Fact]
    public void IncludeAssemblies_WithMultiple_AddsAll() {
        // arrange
        var scanner = new ConcreteScanner();
        var assemblies = new[] {
            typeof(RegistrationTests).Assembly,
            typeof(AssemblyScanAware<>).Assembly
        };

        // act
        scanner.IncludeAssemblies(assemblies);

        // assert
        Assert.Equal(2, scanner.Assemblies.Count);
    }

    [Fact]
    public void IncludeAssemblyFrom_ReturnsSameInstance_ForFluentChaining() {
        // arrange
        var scanner = new ConcreteScanner();

        // act
        var result = scanner.IncludeAssemblyFrom<RegistrationTests>();

        // assert
        Assert.Same(scanner, result);
    }

    [Fact]
    public void ExecuteAssemblyScan_WithAssembly_FindsPublicImplementations() {
        // arrange
        var scanner = new ConcreteScanner();
        scanner.IncludeAssemblyFrom<RegistrationTests>();

        // act
        var types = scanner.ExecuteAssemblyScan<IRegistrationTestMarker>();

        // assert
        Assert.Multiple(() => {
            Assert.Contains(typeof(IncludedScanType), types);
            Assert.DoesNotContain(typeof(IgnoredScanType), types);
        });
    }

    // --- test doubles ---

    private sealed class ConcreteScanner : AssemblyScanAware<ConcreteScanner> { }
}

// Public types required for GetExportedTypes() to discover them during assembly scan
public interface IRegistrationTestMarker { }
public sealed class IncludedScanType : IRegistrationTestMarker { }
[IgnoreAssemblyScan]
public sealed class IgnoredScanType : IRegistrationTestMarker { }
