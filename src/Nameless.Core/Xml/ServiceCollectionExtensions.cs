using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Xml;

public static class ServiceCollectionExtensions {
    /// <summary>
    ///     Registers service <see cref="IXmlSchemaValidator" />
    /// </summary>
    /// <param name="self">The current <see cref="IServiceCollection" />.</param>
    /// <returns>
    ///     The current <see cref="IServiceCollection" />, so other actions can be chained.
    /// </returns>
    public static IServiceCollection RegisterXmlSchemaValidatorServices(this IServiceCollection self) {
        return self.AddSingleton<IXmlSchemaValidator, XmlSchemaValidator>();
    }
}