namespace Nameless.Testing.Tools;

internal static class TypeExtensions {
    extension(Type self) {
        public string GetNameWithNamespace() {
            return !string.IsNullOrWhiteSpace(self.Namespace)
                ? $"{self.Namespace}.{self.Name}"
                : self.Name;
        }
    }
}
