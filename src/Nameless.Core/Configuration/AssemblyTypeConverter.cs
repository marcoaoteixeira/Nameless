using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Nameless.Configuration;

/// <summary>
///     Defines a type converter that will convert a <see cref="string"/>
///     value into an <see cref="Assembly"/>. Useful when need to convert
///     a configuration value.
/// </summary>
public class AssemblyTypeConverter : TypeConverter {
    /// <inheritdoc />
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) {
        return sourceType == typeof(string);
    }

    /// <inheritdoc />
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType) {
        if (value is not string assembly) {
            return null;
        }

        try { return Assembly.Load(assembly); }
        catch (Exception ex) {
            context?.GetLogger<AssemblyTypeConverter>()
                   .LogError(ex, "Couldn't load assembly '{Assembly}'", assembly);
        }

        return null;
    }
}
