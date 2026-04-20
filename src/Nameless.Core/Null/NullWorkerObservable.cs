using System.Diagnostics.CodeAnalysis;
using Nameless.Workers.Notification;

namespace Nameless.Null;

/// <summary>
///     Singleton + Null Object Pattern implementation for
///     <see cref="IObservable{T}" /> of <see cref="WorkerProgress" />.
///     Immediately completes any subscriber without delivering any values.
///     <list type="bullet">
///         <item>
///             <term>Singleton Pattern</term>
///             <description>
///                 <a href="https://en.wikipedia.org/wiki/Singleton_pattern">See here</a>
///             </description>
///         </item>
///         <item>
///             <term>Null-Object Pattern</term>
///             <description>
///                 <a href="https://en.wikipedia.org/wiki/Null_object_pattern">See here</a>
///             </description>
///         </item>
///     </list>
/// </summary>
[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.TrivialCode)]
public sealed class NullWorkerObservable : IObservable<WorkerProgress> {
    /// <summary>
    ///     Gets the unique instance of <see cref="NullWorkerObservable" />.
    /// </summary>
    public static IObservable<WorkerProgress> Instance { get; } = new NullWorkerObservable();

    // Explicit static constructor to tell the C# compiler
    // not to mark type as beforefieldinit
    static NullWorkerObservable() { }

    private NullWorkerObservable() { }

    /// <inheritdoc />
    public IDisposable Subscribe(IObserver<WorkerProgress> observer) {
        observer.OnCompleted();

        return NullDisposable.Instance;
    }
}
