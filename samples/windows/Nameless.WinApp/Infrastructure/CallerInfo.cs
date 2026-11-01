using Nameless.Contracts;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Nameless.WinApp.Infrastructure;

/// <summary>
///     Builds names/keys from the code that calls it, e.g.
///     <c>OrderService_Process</c> or <c>OrderService_Process_Monthly</c>.
/// </summary>
/// <remarks>
///     <para>
///         <b>Relies on the call stack.</b> Callers that must produce a
///         stable key in Release builds should be marked
///         <c>[MethodImpl(MethodImplOptions.NoInlining)]</c>, otherwise the
///         JIT may inline them and the name of the outer method is observed
///         instead.
///     </para>
///     <para>
///         <b>Performance.</b> Every call captures the <em>entire</em>
///         current stack, not just the frames being skipped, so the cost
///         grows with the total stack depth and does not depend on
///         <c>skipFrame</c>. The internal name cache only saves building the
///         string; it does not avoid the stack walk. Indicative figures
///         (Release, .NET 8, shared 2-core machine; expect different
///         absolute numbers on your hardware): about 10 µs and 2 KB of
///         garbage on a shallow stack, 20-25 µs and 6 KB at ~50 frames,
///         50-90 µs and 17 KB at ~200 frames, and 300-500 µs and ~80 KB
///         at ~1,000 frames. That is two to three orders of magnitude
///         slower than <c>[CallerMemberName]</c>.
///     </para>
///     <para>
///         <b>When to use it.</b> It is fine for occasional calls (per
///         request, per operation). Avoid it in hot paths and per-item
///         loops, and avoid it where the stack can get very deep (around
///         1,000 frames or more, typically recursion such as tree walks
///         or deeply nested JSON). Note that layered async code is deeper
///         than it looks: code resuming after an <c>await</c> runs on top
///         of the completion callbacks of the methods below it, roughly
///         10 frames per async level. If the key is constant at a call
///         site, compute it once and keep it in a <c>static readonly</c>
///         field.
///     </para>
/// </remarks>
internal static class CallerInfo {
    private const string UNKNOWN = "Unknown_Unknown";

    // Resolving a name for a given method never changes, so cache it.
    private static readonly ConcurrentDictionary<MethodBase, string> Cache = new();

    /// <summary>
    ///     Returns <c>Class_Method</c> for the code that called this
    ///     method.
    /// </summary>
    /// <param name="skipFrame">
    ///     Extra stack frames to skip: <c>0</c> is the direct caller,
    ///     <c>1</c> the caller's caller, and so on. If the stack is not
    ///     that deep, <c>Unknown_Unknown</c> is returned.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <paramref name="skipFrame"/> is negative.
    /// </exception>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static string GetCaller(int skipFrame = 0) {
        Throws.When.Negative(skipFrame);

        // The frame is captured here, not in a helper, so the depth is always 1 + skipFrame:
        // frame 0 = GetCaller, 1 = its caller.
        return Describe(new StackFrame(FrameDepth(skipFrame), false).GetMethod());
    }

    /// <summary>
    ///     Returns <c>Class_Method_PART</c> for the code that called
    ///     this method.
    /// </summary>
    /// <param name="part">
    ///     The user-selected part of the key.
    /// </param>
    /// <param name="skipFrame">
    ///     See <see cref="GetCaller"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="part"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <paramref name="skipFrame"/> is negative.
    /// </exception>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static string GetKey(string part, int skipFrame = 0) {
        Throws.When.Null(part);
        Throws.When.Negative(skipFrame);

        return $"{Describe(new StackFrame(FrameDepth(skipFrame), false).GetMethod())}_{part}";
    }

    // Pure arithmetic (adds no stack frame to what is observed). Clamped
    // so int.MaxValue cannot overflow.
    private static int FrameDepth(int skipFrame) {
        return Math.Min(skipFrame, int.MaxValue - 1) + 1;
    }

    private static string Describe(MethodBase? method) {
        return method is null ? UNKNOWN : Cache.GetOrAdd(method, BuildName);
    }

    private static string BuildName(MethodBase method) {
        var type = method.DeclaringType;
        var methodName = method.Name;

        // Async state machines, iterators and lambda closures live in
        // compiler-generated nested types: walk up to the real class
        // and recover the original method name.
        while (type?.DeclaringType is not null && IsCompilerGenerated(type)) {
            methodName = Unmangle(type.Name, methodName);
            type = type.DeclaringType;
        }

        methodName = Unmangle(methodName, methodName); // lambdas and local functions

        return $"{StripArity(type?.Name ?? "Unknown")}_{methodName}";
    }

    // The attribute is not applied everywhere (e.g. the state machine of an async lambda lacks it),
    // but the C# compiler always gives its generated types unspeakable names starting with '<'.
    private static bool IsCompilerGenerated(Type type) {
        return type.Name.StartsWith('<') ||
               type.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false);
    }


    // "<DoWorkAsync>d__3" -> "DoWorkAsync"; names that are not
    // compiler-mangled are left alone.
    private static string Unmangle(string name, string fallback) {
        if (name.Length < 3 || name[0] != '<')
            return fallback;

        var depth = 0;
        for (var i = 0; i < name.Length; i++) {
            switch (name[i]) {
                case '<':
                    depth++;
                    break;
                case '>' when --depth == 0: {
                    var inner = name[1..i];

                    return inner.Length == 0
                        ? fallback
                        : Unmangle(inner, inner);
                }
            }
        }

        return fallback;
    }

    // "Repository`1" -> "Repository"
    private static string StripArity(string typeName) {
        var tick = typeName.IndexOf('`');

        return tick < 0 ? typeName : typeName[..tick];
    }
}