namespace Nameless.ObjectModel;

public static class ErrorExtensions {
    extension(IEnumerable<Error> self) {
        public string Flatten() {
            return string.Join("; ", self.Select(error => error.Flatten));
        }
    }
}
