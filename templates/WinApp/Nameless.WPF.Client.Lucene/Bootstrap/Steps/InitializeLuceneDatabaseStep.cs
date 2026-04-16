using Lucene.Net.Search;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Bootstrap;
using Nameless.Bootstrap.Infrastructure;
using Nameless.Bootstrap.Notification;
using Nameless.Lucene;
using Nameless.WPF.Client.Lucene.Resources;

namespace Nameless.WPF.Client.Lucene.Bootstrap.Steps;

/// <summary>
///     Lucene Bootstrap Step
/// </summary>
public class InitializeLuceneDatabaseStep : StepBase {
    private readonly IServiceProvider _provider;

    /// <inheritdoc />
    public override string DisplayName => Strings.InitializeLuceneStep_Name;
    
    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="InitializeLuceneDatabaseStep"/> class.
    /// </summary>
    /// <param name="provider">
    ///     The service provider.
    /// </param>
    public InitializeLuceneDatabaseStep(IServiceProvider provider) {
        _provider = provider;
    }

    /// <inheritdoc />
    public override async Task ExecuteAsync(FlowContext context, IProgress<StepProgress> progress, CancellationToken cancellationToken) {
        progress.ReportInformation(DisplayName, Strings.InitializeLuceneStep_Progress_Starting);

        await Task.Delay(250, cancellationToken);

        using var scope = _provider.CreateScope();
        var indexProvider = scope.ServiceProvider.GetRequiredService<IIndexProvider>();
        using var index = indexProvider.Get(LuceneConstants.UniqueIndexName);

        var documentCount = index.Count(new MatchAllDocsQuery());

        if (documentCount.Success) {
            progress.ReportInformation(DisplayName, string.Format(Strings.InitializeLuceneStep_Progress_DocumentCount, documentCount.Value));

            await Task.Delay(2500, cancellationToken);
        }

        progress.ReportInformation(DisplayName, Strings.InitializeLuceneStep_Progress_Success);

        await Task.Delay(250, cancellationToken);
    }
}
