namespace Nameless.Registration;

[UnitTest]
public class RegistrationTests {
    // --- SkipAssemblyScanAttribute.IsPresent ---

    [Fact]
    public void IsPresent_WhenAttributeAbsent_ReturnsFalse() {
        // act & assert
        Assert.False(SkipAssemblyScanAttribute.Skip(typeof(IncludedScanType)));
    }

    [Fact]
    public void IsPresent_WhenAttributePresent_ReturnsTrue() {
        // act & assert
        Assert.True(SkipAssemblyScanAttribute.Skip(typeof(SkipdScanType)));
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
        scanner.WithAssemblyFrom<RegistrationTests>();

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
        scanner.WithAssemblies(assemblies);

        // assert
        Assert.Equal(2, scanner.Assemblies.Count);
    }

    [Fact]
    public void IncludeAssemblyFrom_ReturnsSameInstance_ForFluentChaining() {
        // arrange
        var scanner = new ConcreteScanner();

        // act
        var result = scanner.WithAssemblyFrom<RegistrationTests>();

        // assert
        Assert.Same(scanner, result);
    }

    [Fact]
    public void ExecuteAssemblyScan_WithAssembly_FindsPublicImplementations() {
        // arrange
        var scanner = new ConcreteScanner();
        scanner.WithAssemblyFrom<RegistrationTests>();

        // act
        var types = scanner.GetImplementations<IRegistrationTestMarker>();

        // assert
        Assert.Multiple(
            () => Assert.Contains(typeof(IncludedScanType), types),
            () => Assert.DoesNotContain(typeof(SkipdScanType), types)
        );
    }

    // --- test doubles ---

    private sealed class ConcreteScanner : AssemblyScanAware<ConcreteScanner> {
        public IReadOnlyCollection<Type> GetImplementations<T>() {
            return ExecuteAssemblyScan(typeof(T));
        }
    }
}

// Public types required for GetExportedTypes() to discover them during assembly scan
public interface IRegistrationTestMarker { }
public sealed class IncludedScanType : IRegistrationTestMarker { }
[SkipAssemblyScan]
public sealed class SkipdScanType : IRegistrationTestMarker { }
