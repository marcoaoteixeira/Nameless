using Microsoft.AspNetCore.Mvc;
using Nameless.Localization;
using Nameless.Logging;

namespace Nameless.AspNetCore {
    [ApiController]
    public abstract class WebApiController : ControllerBase {
        #region Public Properties

        private ILogger _logger;
        public ILogger Logger {
            get { return _logger ?? (_logger = NullLogger.Instance); }
            set { _logger = value ?? NullLogger.Instance; }
        }

        private Localizer _localizer;
        public Localizer T {
            get { return _localizer ?? (_localizer = EmptyStringLocalizer.Create (GetType ()).Get); }
            set { _localizer = value ?? EmptyStringLocalizer.Create (GetType ()).Get; }
        }

        #endregion
    }
}