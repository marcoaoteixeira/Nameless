namespace Nameless.Bootstrap;

public static class StepExtensions {
    extension(IStep self) {
        public bool IsDisabled => !self.IsEnabled;
    }
}
