﻿using System.Reflection;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless.Autofac {
    public abstract class ModuleBase : global::Autofac.Module {
        #region Protected Properties

        /// <summary>
        /// Gets the support assemblies.
        /// </summary>
        protected Assembly[] SupportAssemblies { get; }

        #endregion

        #region Protected Constructors

        /// <summary>
        /// Protected constructor.
        /// </summary>
        protected ModuleBase() {
            SupportAssemblies = [];
        }

        /// <summary>
        /// Protected constructor.
        /// </summary>
        /// <param name="supportAssemblies">The support assemblies.</param>
        protected ModuleBase(Assembly[] supportAssemblies) {
            SupportAssemblies = supportAssemblies ?? [];
        }

        #endregion

        #region Protected Static Methods

        protected static ILogger<T> GetLoggerFromContext<T>(IComponentContext ctx)
            where T : class {
            var loggerFactory = ctx.ResolveOptional<ILoggerFactory>();
            return loggerFactory is not null
                ? loggerFactory.CreateLogger<T>()
                : NullLogger<T>.Instance;
        }

        protected static TOptions? GetOptionsFromContext<TOptions>(IComponentContext ctx)
            where TOptions : class {
            var configuration = ctx.ResolveOptional<IConfiguration>();
            return configuration is not null
                ? configuration
                    .GetSection(typeof(TOptions).Name.RemoveTail(Root.Defaults.OptionsSettingsTails))
                    .Get<TOptions>()
                : default;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Retrieves, from support assemblies, a single implementation from the given service type.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <returns>The service implementation <see cref="Type"/>.</returns>
        /// <exception cref="InvalidOperationException">If more than one implementation were found.</exception>
        protected Type? GetImplementation<TService>()
            => GetImplementations(typeof(TService)).SingleOrDefault();

        /// <summary>
        /// Retrieves, from support assemblies, a single implementation from the given service type.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>The service implementation <see cref="Type"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="serviceType"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">If more than one implementation were found.</exception>
        protected Type? GetImplementation(Type serviceType)
            => GetImplementations(serviceType).SingleOrDefault();

        /// <summary>
        /// Retrieves, from support assemblies, all implementations from the given service type.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <returns>An <see cref="IEnumerable{Type}"/> that contains all possible implementation types.</returns>
        protected IEnumerable<Type> GetImplementations<TService>()
            => GetImplementations(typeof(TService));

        /// <summary>
        /// Retrieves, from support assemblies, all implementations from the given service type.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>An <see cref="IEnumerable{Type}"/> that contains all possible implementation types.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="serviceType"/> is <c>null</c>.</exception>
        protected IEnumerable<Type> GetImplementations(Type serviceType) {
            Guard.Against.Null(serviceType, nameof(serviceType));

            return SupportAssemblies
                .SelectMany(assembly => assembly.GetExportedTypes())
                .Where(type => !type.IsInterface)
                .Where(type => !type.IsAbstract)
                .Where(type => !type.HasAttribute<SingletonAttribute>())
                .Where(type => serviceType.IsAssignableFrom(type) || serviceType.IsAssignableFromGenericType(type));
        }

        #endregion
    }
}
