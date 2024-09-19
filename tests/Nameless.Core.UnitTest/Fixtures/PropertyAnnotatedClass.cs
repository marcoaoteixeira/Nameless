using System.Reflection;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace Nameless.Fixtures;

public class PropertyAnnotatedClass {
    [Description("Name of something")]
    public string FirstName { get; set; }

    public string LastName { get; set; }

    [Description]
    public int Age { get; set; }

    public PropertyInfo GetPropertyWithDescriptionAttribute()
        => GetType().GetProperty(nameof(FirstName))!;

    public PropertyInfo GetPropertyWithoutDescriptionAttribute()
        => GetType().GetProperty(nameof(LastName))!;

    public PropertyInfo GetPropertyWithEmptyDescriptionAttribute()
        => GetType().GetProperty(nameof(Age))!;
}