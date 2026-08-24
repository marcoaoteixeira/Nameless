namespace Nameless.Diagnostics.CodeAnalysis;

/// <summary>
///     Code coverage
/// </summary>
public static class CodeCoverage {
    /// <summary>
    ///     Code coverage - Justifications
    /// </summary>
    public static class Justifications {
        /// <summary>
        ///     Excluded from code coverage as it represents a plain data
        ///     structure with no business logic to validate.
        /// </summary>
        public const string PlainCodeStructure = "Excluded from code coverage as it represents a plain data structure with no business logic to validate.";

        /// <summary>
        ///     Excluded from code coverage because it consists of internal
        ///     implementation details not intended for direct testing.
        /// </summary>
        public const string Internal = "Excluded from code coverage because it consists of internal implementation details not intended for direct testing.";

        /// <summary>
        ///     Excluded from code coverage due to trivial logic that does
        ///     not provide meaningful value when tested.
        /// </summary>
        public const string Trivial = "Excluded from code coverage due to trivial logic that does not provide meaningful value when tested.";

        /// <summary>
        ///     Excluded from code coverage as it is automatically generated
        ///     and not intended to be manually maintained or tested.
        /// </summary>
        public const string AutoGenCode = "Excluded from code coverage as it is automatically generated and not intended to be manually maintained or tested.";
    }
}
