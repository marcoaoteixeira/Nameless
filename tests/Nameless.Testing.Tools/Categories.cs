namespace Nameless.Testing.Tools;

public static class Categories {
    /// <summary>
    ///     Categories for unit test cases. Unit tests are tests that are run in
    ///     isolation and do not depend on any external resources or services.
    ///     They are typically fast and should not require any setup or teardown.
    /// </summary>
    public const string UnitTests = nameof(UnitTests);

    /// <summary>
    ///     Categories for integration test cases. Integration tests are tests that
    ///     verify the interaction between multiple components or systems. They
    ///     typically require more setup and teardown than unit tests and may
    ///     depend on external resources or services.
    /// </summary>
    public const string IntegrationTests = nameof(IntegrationTests);

    /// <summary>
    ///     Categories for end-to-end test cases. End-to-end tests are tests that
    ///     verify the entire system from end to end. They typically require the
    ///     most setup and teardown and most likely will depend on external
    ///     resources or services. They are typically the slowest tests and should
    ///     be run infrequently.
    /// </summary>
    public const string EndToEnd = nameof(EndToEnd);

    /// <summary>
    ///     Categories for test cases that only runs on developer machine. These
    ///     tests are typically used for testing features that are  not available
    ///     in the CI/CD pipeline or that require a specific environment to run.
    /// </summary>
    public const string RunsOnDevMachine = nameof(RunsOnDevMachine);
}