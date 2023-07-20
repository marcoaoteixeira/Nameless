using Nameless.Helpers;

namespace Nameless.FileStorage {
    public static class FileExtension {
        #region Public Static Methods

        public static Stream Open(this IFile self)
            => AsyncHelper.RunSync(() => self.OpenAsync());

        #endregion
    }
}
