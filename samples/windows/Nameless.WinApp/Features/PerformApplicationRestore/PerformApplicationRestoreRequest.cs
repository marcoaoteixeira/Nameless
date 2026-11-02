using Nameless.Mediator.Requests;

namespace Nameless.WinApp.Features.PerformApplicationRestore;

/// <summary>
///     Request to restore the application's configuration file, database
///     and Lucene index from a previously created backup zip file.
/// </summary>
/// <param name="SourceZipFilePath">
///     The path to the backup zip file to restore from.
/// </param>
public sealed record PerformApplicationRestoreRequest(string SourceZipFilePath) : IRequest<RestoreOutcome>;
