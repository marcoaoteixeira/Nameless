using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Nameless.Notification {
    public class NotifierActionFilter : IActionFilter {
        #region Public Static Read-Only Fields

        public static readonly string NotifierTempDataKey = "__NOTIFIER__";

        #endregion

        #region Private Read-Only Fields

        private readonly INotifier _notifier;

        #endregion

        #region Public Constructors

        public NotifierActionFilter (INotifier notifier) {
            Prevent.ParameterNull (notifier, nameof (notifier));

            _notifier = notifier;
        }

        #endregion

        #region IActionFilter Members

        public void OnActionExecuted (ActionExecutedContext context) {
            if (context.Controller is Controller controller) {
                controller.TempData[NotifierTempDataKey] = _notifier.Flush ();
            }
        }

        public void OnActionExecuting (ActionExecutingContext context) {

        }

        #endregion
    }
}