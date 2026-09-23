using Microsoft.Extensions.Logging;
using Nameless.IO.System;
using Nameless.Logging;
using Microsoft.Extensions.Options;

namespace Nameless.Helpers;

public struct ConstraintStruct;

public sealed class ConstraintStructHolder<T> where T : struct;

public sealed class ConstraintNewHolder<T> where T : class, new();

public sealed class ConstraintKey;

public sealed class ConstraintNoDefaultConstructor(int value) {
    public int Value { get; } = value;
}

public interface IConstraintPairValue<TKey>;

public sealed class ConstraintPairValue : IConstraintPairValue<ConstraintKey>;

public sealed class ConstraintPairHolder<TKey, TValue>
    where TKey : class
    where TValue : IConstraintPairValue<TKey>;

public sealed class NamedCategoryLogger : ILogger {
    public string CategoryName => "Some.Namespace.CategoryClass";

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
    public bool IsEnabled(LogLevel logLevel) => false;
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
}

[UnitTest]
public class GenericTypeHelperConstraintTests {
    private static readonly System.Reflection.Assembly TestAssembly = typeof(GenericTypeHelperConstraintTests).Assembly;

    [Fact]
    public void GetArgumentsThatCloses_WithStructConstraint_FindsValueTypes() {
        // act
        var combinations = GenericTypeHelper.GetArgumentsThatCloses(typeof(ConstraintStructHolder<>), TestAssembly).ToList();

        // assert
        Assert.Multiple(
            () => Assert.Contains(combinations, c => c[0] == typeof(ConstraintStruct)),
            () => Assert.All(combinations, c => Assert.True(c[0].IsValueType))
        );
    }

    [Fact]
    public void GetArgumentsThatCloses_WithNewConstraint_FindsTypesWithParameterlessConstructor() {
        // act
        var combinations = GenericTypeHelper.GetArgumentsThatCloses(typeof(ConstraintNewHolder<>), TestAssembly).ToList();

        // assert
        Assert.Multiple(
            () => Assert.Contains(combinations, c => c[0] == typeof(ConstraintKey)),
            () => Assert.DoesNotContain(combinations, c => c[0] == typeof(ConstraintNoDefaultConstructor))
        );
    }

    [Fact]
    public void GetArgumentsThatCloses_WithCrossParameterConstraint_ReturnsOnlyValidCombinations() {
        // act
        var combinations = GenericTypeHelper.GetArgumentsThatCloses(typeof(ConstraintPairHolder<,>), TestAssembly).ToList();

        // assert
        Assert.Multiple(
            () => Assert.Contains(combinations, c => c[0] == typeof(ConstraintKey) && c[1] == typeof(ConstraintPairValue)),
            () => Assert.All(combinations, c => Assert.True(typeof(IConstraintPairValue<>).MakeGenericType(c[0]).IsAssignableFrom(c[1])))
        );
    }

    [Fact]
    public void GetArgumentsThatCloses_WithoutAssemblies_UsesDefaultAssemblies() {
        // act
        var exception = Record.Exception(() => GenericTypeHelper.GetArgumentsThatCloses(typeof(ConstraintNewHolder<>)).ToList());

        // assert
        Assert.Null(exception);
    }

    [Fact]
    public void StartStopwatchLogger_WithLoggerExposingCategoryName_UsesLastCategorySegment() {
        // arrange
        var logger = new NamedCategoryLogger();

        // act
        var exception = Record.Exception(() => { using var _ = logger.StartStopwatchLogger(); });

        // assert
        Assert.Null(exception);
    }

    [Fact]
    public void File_Monitor_ReturnsMonitorRootedAtProviderRoot() {
        // arrange
        var root = SysPath.Combine(SysPath.GetTempPath(), $"file-monitor-{Guid.NewGuid():N}");
        SysDirectory.CreateDirectory(root);

        try {
            var provider = new FileProvider(Options.Create(new FileProviderOptions { Root = root }));
            var file = provider.GetFile("watched.txt");

            // act
            using var monitor = file.Monitor();

            // assert
            Assert.Multiple(
                () => Assert.Equal("watched.txt", monitor.Glob),
                () => Assert.Equal(SysPath.GetFullPath(root), monitor.Root)
            );
        }
        finally {
            SysDirectory.Delete(root, recursive: true);
        }
    }
}
