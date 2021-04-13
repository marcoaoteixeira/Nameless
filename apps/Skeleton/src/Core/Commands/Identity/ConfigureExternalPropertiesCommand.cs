using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nameless.AspNetCore.Identity;
using Nameless.CQRS;

namespace Nameless.Skeleton.Web.Commands.Identity {
    public class ConfigureExternalPropertiesCommand : ICommand {
        #region Public Properties

        public string Provider { get; set; }
        public string RedirectUrl { get; set; }

        #endregion
    }

    public sealed class ConfigureExternalPropertiesCommandHandler : ICommandHandler<ConfigureExternalPropertiesCommand> {
        #region Private Read-Only Fields

        private readonly SignInManager<User> _signInManager;

        #endregion

        #region Public Constructors

        public ConfigureExternalPropertiesCommandHandler (SignInManager<User> signInManager) {
            Prevent.ParameterNull (signInManager, nameof (signInManager));

            _signInManager = signInManager;
        }

        #endregion

        #region ICommandHandler<ConfigureExternalPropertiesCommand> Members

        public Task<ExecutionResult> HandleAsync (ConfigureExternalPropertiesCommand command, IProgress<int> progress = null, CancellationToken token = default) {
            var properties = _signInManager.ConfigureExternalAuthenticationProperties (command.Provider, command.RedirectUrl);

            return Task.FromResult (ExecutionResult.Success (new { Properties = properties }));
        }

        #endregion
    }
}