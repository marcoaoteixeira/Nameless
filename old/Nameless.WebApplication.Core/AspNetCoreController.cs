using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Nameless.Logging;
using Nameless.Notification;

namespace Nameless.WebApplication {

    [ApiController]
    [Route ("api/v{version:apiVersion}/[controller]")]
    public abstract class WebApiControllerBase : ControllerBase {
        #region Public Properties

        private ILogger _log;
        public ILogger Log {
            get { return _log ?? (_log = NullLogger.Instance); }
            set { _log = value ?? NullLogger.Instance; }
        }

        private IStringLocalizer _localizer;
        public IStringLocalizer T {
            get { return _localizer ?? (_localizer = NullStringLocalizer.Instance); }
            set { _localizer = value ?? NullStringLocalizer.Instance; }
        }

        #endregion
    }

    [Route ("[controller]")]
    public abstract class MvcControllerBase : Controller {
        #region Public Properties

        private ILogger _log;
        public ILogger Log {
            get { return _log ?? (_log = NullLogger.Instance); }
            set { _log = value ?? NullLogger.Instance; }
        }

        private INotifier _notifier;
        public INotifier Notifier {
            get { return _notifier ?? (_notifier = NullNotifier.Instance); }
            set { _notifier = value ?? NullNotifier.Instance; }
        }

        private IStringLocalizer _localizer;
        public IStringLocalizer T {
            get { return _localizer ?? (_localizer = NullStringLocalizer.Instance); }
            set { _localizer = value ?? NullStringLocalizer.Instance; }
        }

        #endregion
    }
}