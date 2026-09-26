using Microsoft.CodeAnalysis;

namespace Nameless.Generators.Shared.Extensions;

public static class AttributeDataExtensions {
    extension(AttributeData self) {
        public TypedConstant GetNamedArgument(string name) {
            var arg = self.NamedArguments.SingleOrDefault(item => item.Key == name);
            var success = !string.IsNullOrWhiteSpace(arg.Key);

            return success ? arg.Value : default;
        }

        public TypedConstant GetConstructorArgument(int index) {
            _ = self.TryGetConstructorArgument(index, out var output);

            return output;
        }

        public bool TryGetConstructorArgument(int index, out TypedConstant output) {
            output = default;

            if (index >= self.ConstructorArguments.Length) { return false; }

            output = self.ConstructorArguments[index];

            return true;
        }
    }
}