using Microsoft.AspNetCore.Mvc;
using Nameless.AspNetCore.Localization;
using Nameless.Logging;

namespace Nameless.AspNetCore {
    [ApiController]
    public abstract class WebApiController : ControllerBase {
        #region Public Properties

        private ILogger _logger;
        public ILogger Logger {
            get { return _logger ??= NullLogger.Instance; }
            set { _logger = value ?? NullLogger.Instance; }
        }

        #endregion

        #region Protected Properties

        protected Microsoft.Extensions.Localization.IStringLocalizer T { get; }

        #endregion

        #region Protected Members

        protected WebApiController () { }

        protected WebApiController (Microsoft.Extensions.Localization.IStringLocalizer localizer) {
            T = localizer ?? NullStringLocalizer.Instance;
        }

        #endregion
    }
}