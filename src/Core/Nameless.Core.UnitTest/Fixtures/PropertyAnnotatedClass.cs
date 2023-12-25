using System.Reflection;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace Nameless.Fixtures {
    public class PropertyAnnotatedClass {
        [Description("Name of something")]
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public PropertyInfo GetPropertyWithDescription()
            => GetType().GetProperty(nameof(FirstName))!;

        public PropertyInfo GetPropertyWithoutDescription()
            => GetType().GetProperty(nameof(LastName))!;
    }
}
