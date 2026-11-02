using Nameless.Mediator.Requests;

namespace Nameless.WinApp.Features.PerformApplicationBackup;

/// <summary>
///     Request to back up the application's configuration file, database
///     and Lucene index into a single zip file.
/// </summary>
public sealed class PerformApplicationBackupRequest : IRequest<string>;
