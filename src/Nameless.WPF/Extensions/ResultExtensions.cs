using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.WPF;

public static class ResultExtensions {
    extension<T>(Result<T> self) {
        public bool Failure => !self.Success;

        public Error[] GetErrors() {
            return self.Success ? [] : self.Errors;
        }
    }
}
