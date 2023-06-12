using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless.AspNetCore {

    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public abstract class ApiControllerBase : ControllerBase {

        #region Private Fields

        private ILogger _logger = default!;

        #endregion

        #region Protected Properties

        protected ILogger Logger {
            get { return _logger ?? NullLogger.Instance; }
            set { _logger = value ?? NullLogger.Instance; }
        }

        #endregion
    }
}