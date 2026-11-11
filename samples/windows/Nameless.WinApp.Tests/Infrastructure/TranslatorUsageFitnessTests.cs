using System.Reflection;
using Nameless.Windows.Localization;

namespace Nameless.WinApp.Infrastructure;

/// <summary>
///     Fitness function (#199, ADR-003): every <c>Nameless.WinApp</c>
///     consumer that needs a translator declares the typed dependency
///     (<see cref="ITranslator{T}"/>), not the untyped root
///     <see cref="ITranslator"/>. The allowlist starts empty — a new
///     consumer that regresses to the untyped root must add itself here
///     deliberately, with a reason, not slip through unnoticed.
/// </summary>
public class TranslatorUsageFitnessTests {
    // TranslatorExtension (the XAML markup extension) is a library type,
    // not part of this assembly, and is explicitly exempt by design
    // (ADR-003): it resolves the shared root translator for full-key XAML
    // bindings, not a typed one.
    private static readonly Type[] Allowlist = [];

    [Fact]
    [UnitTest]
    public void NoConcreteTypeInTheWinAppAssembly_HasAConstructorParameterOfThePlainRootITranslatorType() {
        var assembly = typeof(App).Assembly;

        var offenders = assembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && !Allowlist.Contains(type))
            .Where(type => type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                .Any(ctor => ctor.GetParameters()
                                                 .Any(parameter => parameter.ParameterType == typeof(ITranslator))))
            .ToArray();

        Assert.True(offenders.Length == 0,
            $"The following types still take the untyped root ITranslator instead of ITranslator<T>: " +
            $"[{string.Join(", ", offenders.Select(type => type.FullName))}].");
    }
}
