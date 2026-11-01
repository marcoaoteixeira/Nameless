using Nameless.Contracts;
using Nameless.IO;
using Nameless.IO.Physical;
using Nameless.Mediator.Requests;
using Nameless.WinApp.Infrastructure;
using Nameless.Windows.Events;
using Nameless.Windows.Helpers;
using Nameless.Windows.Localization;
using Nameless.Windows.Notification;
using Nameless.Windows.Notification.SnackBar;

namespace Nameless.WinApp.Features.OpenCurrentLogFile;

public class OpenCurrentLogFileRequestHandler : IRequestHandler<OpenCurrentLogFileRequest> {
    private readonly IEventPublisher _eventPublisher;
    private readonly IFileManager _fileManager;
    private readonly ITranslator _translator;

    public OpenCurrentLogFileRequestHandler(IEventPublisher eventPublisher, PhysicalFileManagerFactory fileManagerFactory, ITranslator translator) {
        _eventPublisher = Throws.When.Null(eventPublisher);
        _fileManager = Throws.When.Null(fileManagerFactory).GetOrCreate(AppContext.BaseDirectory);
        _translator = Throws.When.Null(translator);
    }

    public async Task HandleAsync(OpenCurrentLogFileRequest request, CancellationToken cancellationToken) {
        var logDir = _fileManager.GetDirectory(Constants.Logging.Directory);
        var currentFile = logDir.GetFiles().OrderBy(file => file.LastWriteTime).FirstOrDefault();

        if (currentFile is null) {
            await _eventPublisher.PublishWarningAsync<SnackBarNotificationEvent>(
                title: _translator.Get("Notify_Missing_Log_File_Title"),
                message: _translator.Get("Notify_Missing_Log_File_Message")
            );

            return;
        }

        ProcessHelper.OpenDirectory(currentFile.Path);
    }
}