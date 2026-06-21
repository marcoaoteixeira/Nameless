using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Nameless.Diagnostics.CodeAnalysis;

namespace Nameless.ComponentModel;

[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.InternalCode)]
internal static class LoggerExtensions {
    extension(ILogger<AssemblyTypeConverter> self) {
        internal void Failure(Exception exception) {
            Log.Failure(
                logger: self,
                tag: "ASSEMBLY_TYPE_CONVERTER",
                actionName: $"{nameof(AssemblyTypeConverter)}.{nameof(AssemblyTypeConverter.ConvertTo)}",
                exception: exception
            );
        }
    }
}