using Microsoft.AspNetCore.Mvc;
using Nameless.Localization;
using Nameless.Logging;

namespace Nameless.AspNetCore {
    public abstract class MvcController : Controller {
        #region Public Properties

        private ILogger _logger;
        public ILogger Logger {
            get { return _logger ?? (_logger = NullLogger.Instance); }
            set { _logger = value ?? NullLogger.Instance; }
        }

        private Localizer _localizer;
        public Localizer T {
            get { return _localizer ?? (_localizer = EmptyStringLocalizer.CreateLocalizer (GetType ())); }
            set { _localizer = value ?? EmptyStringLocalizer.CreateLocalizer (GetType ()); }
        }

        #endregion
    }
}