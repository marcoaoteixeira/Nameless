using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Xml;
public static class ServiceCollectionExtensions {
    /// <summary>
    /// Registers service <see cref="IXmlSchemaValidator"/>
    /// </summary>
    /// <param name="self">The current <see cref="IServiceCollection"/>.</param>
    /// <returns>
    /// The current <see cref="IServiceCollection"/>, so other actions can be chained.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    public static IServiceCollection AddXmlSchemaValidator(this IServiceCollection self)
        => Prevent.Argument
                  .Null(self)
                  .AddSingleton<IXmlSchemaValidator, XmlSchemaValidator>();
}
