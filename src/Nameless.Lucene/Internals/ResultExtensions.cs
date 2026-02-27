using Nameless.Results;

namespace Nameless.Lucene.Internals;

internal static class ResultExtensions {
    extension<T>(Result<T> self) {
        public bool Failure => !self.Success;
    }
}
