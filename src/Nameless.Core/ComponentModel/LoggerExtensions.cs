using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Nameless.ComponentModel;

[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.InternalCode)]
internal static class LoggerExtensions {
    extension(ILogger<AssemblyTypeConverter> self) {
        internal void Failure(Exception exception) {
            Log.Failure(
                self,
                "ASSEMBLY_TYPE_CONVERTER",
                $"{nameof(AssemblyTypeConverter)}.{nameof(AssemblyTypeConverter.ConvertTo)}",
                exception
            );
        }
    }
}