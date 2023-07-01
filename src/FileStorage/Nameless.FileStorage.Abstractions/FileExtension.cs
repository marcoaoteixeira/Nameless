using Nameless.Helpers;

namespace Nameless.FileStorage {

    public static class FileExtension {

        #region Public Static Methods

        public static Stream Open(this IFile self) {
            Garda.Prevent.Null(self, nameof(self));

            return AsyncHelper.RunSync(() => self.OpenAsync());
        }

        #endregion
    }
}
