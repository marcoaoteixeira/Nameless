using System.Reflection;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace Nameless.Fixtures;

public class PropertyAnnotatedClass {
    [Description(description: "Name of something")]
    public string FirstName { get; set; }

    public string LastName { get; set; }

    [Description]
    public int Age { get; set; }

    public PropertyInfo GetPropertyWithDescriptionAttribute() {
        return GetType().GetProperty(nameof(FirstName))!;
    }

    public PropertyInfo GetPropertyWithoutDescriptionAttribute() {
        return GetType().GetProperty(nameof(LastName))!;
    }

    public PropertyInfo GetPropertyWithEmptyDescriptionAttribute() {
        return GetType().GetProperty(nameof(Age))!;
    }
}