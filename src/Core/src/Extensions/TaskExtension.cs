using System.Threading.Tasks;

namespace Nameless {
    public static class TaskExtension {

        #region Public Static Methods

        public static bool CanContinue (this Task self) {
            return self.Exception == null && !self.IsCanceled && !self.IsFaulted && self.IsCompleted;
        }

        #endregion
    }
}
