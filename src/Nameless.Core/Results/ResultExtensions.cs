namespace Nameless.Results;

public static class ResultExtensions {
    extension<T>(Result<T> self) {
        public bool Failure => !self.Success;
    }
}
