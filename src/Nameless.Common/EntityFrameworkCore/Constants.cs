using System.Diagnostics.CodeAnalysis;
using Nameless.Diagnostics.CodeAnalysis;

namespace Nameless.EntityFrameworkCore;

/// <summary>
///     
/// </summary>
[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.OnlyConstants)]
public static class Constants {
    /// <summary>
    ///     String maximum length values.
    /// </summary>
    public static class MaxLength {
        /// <summary>
        ///     Size "micro", only 64 chars.
        /// </summary>
        public const int Micro = 64;

        /// <summary>
        ///     Size "tiny", only 256 chars.
        /// </summary>
        public const int Tiny = 256;

        /// <summary>
        ///     Size "small", only 1024 chars.
        /// </summary>
        public const int Small = 1024;

        /// <summary>
        ///     Size "big", only 4086 chars.
        /// </summary>
        public const int Big = 4086;

        /// <summary>
        ///     Size "huge", <see cref="int.MaxValue"/>.
        /// </summary>
        public const int Huge = int.MaxValue;
    }
}
