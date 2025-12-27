using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nameless.Xml;

public static class ServiceCollectionExtensions {
    /// <param name="self">The current <see cref="IServiceCollection" />.</param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers service <see cref="IXmlSchemaValidator" />
        /// </summary>
        /// <returns>
        ///     The current <see cref="IServiceCollection" />, so other actions can be chained.
        /// </returns>
        public IServiceCollection RegisterXmlSchemaValidator() {
            self.TryAddSingleton<IXmlSchemaValidator, XmlSchemaValidator>();

            return self;
        }
    }
}