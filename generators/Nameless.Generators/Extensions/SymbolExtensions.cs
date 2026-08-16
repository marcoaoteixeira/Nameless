using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace Nameless.Generators;

public static class SymbolExtensions {
    extension(ISymbol self) {
        public string GetFullName() {
            return self.ToDisplayString();
        }

        public string GetFullyQualifiedName() {
            return self.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }

        // ReSharper disable once OutParameterValueIsAlwaysDiscarded.Local
        public bool TryGetAttributes(Regex regex, out AttributeData[] output) {
            output = [.. self.GetAttributes().Where(Filter)];

            return output.Length > 0;

            bool Filter(AttributeData attributeData) {
                return attributeData.AttributeClass is not null &&
                       regex.IsMatch(attributeData.AttributeClass.GetFullyQualifiedName());
            }
        }
    }
}