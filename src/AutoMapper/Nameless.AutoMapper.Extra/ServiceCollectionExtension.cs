﻿using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.AutoMapper {
    public static class ServiceCollectionExtension {
        #region Public Static Methods

        public static IServiceCollection RegisterAutoMapper(this IServiceCollection services, params Assembly[] assemblies)
            => services.AddAutoMapper(assemblies);

        #endregion
    }
}