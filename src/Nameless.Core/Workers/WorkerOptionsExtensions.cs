namespace Nameless.Workers;

public static class WorkerOptionsExtensions {
    extension(WorkerOptions self) {
        public bool IsDisabled => !self.IsEnabled;
    }
}