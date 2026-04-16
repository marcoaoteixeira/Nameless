namespace Nameless.Security.Password;

public static class GeneratorExtensions {
    extension(IPassGenerator self) {
        public Task<string> GenerateAsync(CancellationToken cancellationToken) {
            return self.GenerateAsync(arguments: new Arguments(), cancellationToken);
        }
    }
}
