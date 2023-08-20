using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Nameless.Web.Filters {
    /// <summary>
    /// Implementation of <see cref="ActionFilterAttribute"/> that validates
    /// if the incoming model is valid.
    /// </summary>
    public sealed class ValidateModelStateActionFilter : ActionFilterAttribute {
        #region Public Override Methods

        /// <inheritdoc />
        public override void OnActionExecuting(ActionExecutingContext context) {
            if (!context.ModelState.IsValid) {
                var errorCollection = context.ModelState.ToErrorCollection();
                context.Result = new BadRequestObjectResult(errorCollection);
            }
        }

        #endregion
    }
}
