using Nameless.Generators.Infrastructure;
using Nameless.Generators.Web.Http.Endpoints.Conventions;

namespace Nameless.Generators;

public static class CodeWriterExtensions {
    extension(CodeWriter self) {
        public CodeWriter WriteLine(Convention convention, string? closing = null) {
            return WriteConvention(convention, self.WriteLine, closing);
        }

        public CodeWriter Write(Convention convention, string? closing = null) {
            return WriteConvention(convention, self.Write, closing);
        }
    }

    private static CodeWriter WriteConvention(Convention convention, Func<string, CodeWriter> writeDelegate, string? closing) {
        var call = convention.Call;

        var lines = call.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length == 0) {
            return writeDelegate(string.Empty);
        }

        for (var idx = 0; idx < lines.Length - 1; idx++) {
            writeDelegate(lines[idx]);
        }

        return writeDelegate($"{lines[lines.Length - 1]}{closing}");
    }
}
