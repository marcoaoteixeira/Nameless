using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.Bootstrap;
using Nameless.Logging.Serilog;
using Nameless.Lucene;
using Nameless.Mediator;
using Nameless.Validation.FluentValidation;
using Nameless.Windows.Documents;
using Nameless.Windows.Localization;
using Nameless.Windows.Mvvm;
using Nameless.Windows.Navigation;
using Nameless.Windows.UI;

namespace Nameless.Windows.Hosting;

public sealed class WinHostSettings {
    public string[] Args { get; set; } = [];
    public string? Environment { get; set; }
    public Assembly[] Assemblies { get; set; } = [];
    public Action<IServiceCollection, IConfiguration, IHostEnvironment>? ConfigureAdditionalServices { get; set; }

    public bool DisableAppConfigurationManager { get; set; }
    
    public bool DisableBootstrap { get; set; }
    public Action<BootstrapRegistration>? ConfigureBootstrap { get; set; }

    public bool DisableCompressor { get; set; }
    
    public bool DisableContentDialogService { get; set; }
    
    public bool DisableDocumentServices { get; set; }
    public Action<DocumentServicesRegistration>? ConfigureDocumentServices { get; set; }

    public bool DisableFileSystemDialog { get; set; }
    
    public bool DisableFileExplorer { get; set; }
    
    public bool DisableGitHubHttpClient { get; set; }
    
    public bool DisableHttpClientDefaults { get; set; }
    public Action<IHttpClientBuilder>? ConfigureHttpClientDefaults { get; set; }
    
    public bool DisableLogging { get; set; }
    public Action<SerilogRegistration>? ConfigureLogging { get; set; }
    
    public bool DisableLucene { get; set; }
    public Action<LuceneRegistration>? ConfigureLucene { get; set; }
    
    public bool DisableMediator { get; set; }
    public Action<MediatorRegistration>? ConfigureMediator { get; set; }
    
    public bool DisableMessageDialog { get; set; }
    
    public bool DisableMessenger { get; set; }
    
    public bool DisableNavigation { get; set; }
    public Action<NavigationRegistration>? ConfigureNavigation { get; set; }
    
    public bool DisableOffice { get; set; }
    
    public bool DisableResilience { get; set; }
    
    public bool DisableSnackBar { get; set; }
    
    public bool DisableTaskRunner { get; set; }
    
    public bool DisableValidator { get; set; }
    public Action<ValidatorRegistration>? ConfigureValidator { get; set; }
    
    public bool DisableViewModels { get; set; }
    public Action<ViewModelRegistration>? ConfigureViewModel { get; set; }
    
    public bool DisableWindowsFactory { get; set; }
    public Action<WindowFactoryRegistration>? ConfigureWindowFactory { get; set; }
    
    public bool DisableLocalization { get; set; }
    public Action<LocalizationRegistration>? ConfigureLocalization { get; set; }

    /// <summary>
    ///     Whether it should disable Status Reporting feature.
    /// </summary>
    public bool DisableStatusReporting { get; set; }
}