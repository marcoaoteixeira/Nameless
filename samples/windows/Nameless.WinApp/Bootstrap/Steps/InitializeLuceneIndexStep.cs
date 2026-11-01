using Nameless.Contracts;
using Nameless.Lucene;
using Nameless.WinApp.Infrastructure;
using Nameless.Windows.Bootstrap;
using Nameless.Windows.Bootstrap.Notification;
using Nameless.Windows.Localization;

namespace Nameless.WinApp.Bootstrap.Steps;

public class InitializeLuceneIndexStep : StepBase {
    private readonly IIndexProvider _indexProvider;
    private readonly ITranslator _translator;

    public InitializeLuceneIndexStep(IIndexProvider indexProvider, ITranslator translator) {
        _indexProvider = Throws.When.Null(indexProvider);
        _translator = Throws.When.Null(translator);
    }

    public override Task ExecuteAsync(IProgress<StepProgress> progress, CancellationToken cancellationToken) {
        progress.ReportStart(Name, _translator.Get("Startup"));

        _ = _indexProvider.Get();

        progress.ReportStart(Name, _translator.Get("Finish"));

        return Task.CompletedTask;
    }
}
