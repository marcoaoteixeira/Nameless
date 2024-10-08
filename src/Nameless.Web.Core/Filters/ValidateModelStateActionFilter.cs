﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Nameless.Web.Filters;

/// <summary>
/// Implementation of <see cref="ActionFilterAttribute"/> that validates
/// if the incoming model is valid.
/// </summary>
public sealed class ValidateModelStateActionFilter : ActionFilterAttribute {
    /// <inheritdoc />
    public override void OnActionExecuting(ActionExecutingContext context) {
        if (context.ModelState.IsValid) { return; }

        var errorCollection = context.ModelState.ToErrorCollection();
        context.Result = new BadRequestObjectResult(errorCollection);
    }
}