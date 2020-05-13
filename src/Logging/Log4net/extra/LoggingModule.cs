using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Nameless.DependencyInjection.Autofac;
using Autofac_Parameter = Autofac.Core.Parameter;

namespace Nameless.Logging.Log4net {
    /// <summary>
    /// The logging service registration.
    /// </summary>
    public sealed class LoggingModule : ModuleBase {

        #region Protected Override Methods

        /// <inheritdoc/>
        protected override void Load (ContainerBuilder builder) {
            builder
                .RegisterType<LoggerFactory> ()
                .As<ILoggerFactory> ()
                .SetLifetimeScope (LifetimeScopeType.Singleton);

            builder
                .Register (CreateLogger)
                .As<ILogger> ()
                .SetLifetimeScope (LifetimeScopeType.Transient);
        }

        /// <inheritdoc/>
        protected override void AttachToComponentRegistration (IComponentRegistryBuilder componentRegistry, IComponentRegistration registration) {
            var implementationType = registration.Activator.LimitType;

            // verify if the implementation type needs logger injection via constructor.
            var needConstructorInjection = implementationType.GetConstructors ()
                .Any (constructor => constructor.GetParameters ()
                    .Any (parameter => parameter.ParameterType == typeof (ILogger)));

            // if need, inject and return.
            if (needConstructorInjection) {
                registration.Preparing += (sender, args) => {
                    args.Parameters = args.Parameters.Concat (new [] {
                        new TypedParameter (
                            type: typeof (ILogger),
                            value: args.Context.Resolve<ILogger> (new TypedParameter (typeof (Type), implementationType))
                        )
                    });
                };
                return;
            }

            // build an array of actions on this type to assign loggers to member properties
            var propertyInjectorCollection = BuildPropertyInjectorCollection (implementationType).ToArray ();

            // otherwise, whan an instance of this component is activated, inject the loggers on the instance
            registration.Activated += (sender, e) => {
                foreach (var injector in propertyInjectorCollection) {
                    injector (e.Context, e.Instance);
                }
            };
        }

        #endregion

        #region Private Static Methods

        private static ILogger CreateLogger (IComponentContext context, IEnumerable<Autofac_Parameter> parameters) {
            return context
                .Resolve<ILoggerFactory> ()
                .CreateLogger (parameters.TypedAs<Type> ());
        }

        #endregion

        #region Private Methods

        private IEnumerable<Action<IComponentContext, object>> BuildPropertyInjectorCollection (IReflect componentType) {
            // Look for settable properties of type "ILogger"
            var properties = componentType
                .GetProperties (BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance)
                .Select (property => new {
                    PropertyInfo = property,
                        property.PropertyType,
                        IndexParameters = property.GetIndexParameters ().ToArray (),
                        Accessors = property.GetAccessors (false)
                })
                .Where (property => property.PropertyType == typeof (ILogger)) // must be a logger
                .Where (property => property.IndexParameters.Length == 0) // must not be an indexer
                .Where (property => property.Accessors.Length != 1 || property.Accessors[0].ReturnType == typeof (void)); //must have get/set, or only set

            // Return an array of actions that resolve a logger and assign the property
            foreach (var property in properties) {
                yield return (context, instance) => {
                    if (instance.GetType () == (Type) componentType) {
                        property.PropertyInfo.SetValue (
                            obj: instance,
                            value: context.Resolve<ILogger> (new TypedParameter (typeof (Type), componentType)),
                            index : null
                        );
                    }
                };
            }
        }

        #endregion
    }
}