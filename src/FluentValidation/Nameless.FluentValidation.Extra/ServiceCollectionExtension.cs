﻿using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.FluentValidation {
    public static class ServiceCollectionExtension {
        #region Public Static Methods

        public static IServiceCollection PrepareFluentValidation(this IServiceCollection services, params Assembly[] assemblies)
            => services.AddValidatorsFromAssemblies(assemblies);

        #endregion
    }
}
