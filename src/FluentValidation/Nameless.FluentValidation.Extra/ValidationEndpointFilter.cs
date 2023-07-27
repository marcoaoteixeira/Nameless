﻿using Microsoft.AspNetCore.Http;

namespace Nameless.FluentValidation {
    public sealed class ValidationEndpointFilter : IEndpointFilter {
        #region Private Read-Only Fields

        private readonly IValidationService _validationService;

        #endregion

        #region Public Constructors

        public ValidationEndpointFilter(IValidationService validationService) {
            _validationService = Prevent.Against.Null(validationService, nameof(validationService));
        }

        #endregion

        #region IEndpointFilter Members

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next) {
            foreach (var argument in context.Arguments) {
                if (argument == null || !argument.GetType().HasAttribute<FluentValidateAttribute>()) {
                    continue;
                }

                var validationResult = await _validationService.ValidateAsync(argument, throwOnError: false);
                if (validationResult.Failure()) {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }
            }

            return await next(context);
        }

        #endregion
    }
}
