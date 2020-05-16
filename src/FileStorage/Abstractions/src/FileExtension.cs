using System.IO;
using System.Text;
using System.Threading.Tasks;
using Nameless.Helpers;

namespace Nameless.FileStorage {
    public static class FileExtension {
        #region Public Static Methods

        public static Stream CreateStream (this IFile self) {
            if (self == null) { return null; }

            var stream = AsyncHelper.RunSync (() => self.CreateStreamAsync ());
            return stream;
        }

        public static void Copy (this IFile self, string destFilePath, bool overwrite = false) {
            if (self == null) { return; }

            AsyncHelper.RunSync (() => self.CopyAsync (destFilePath, overwrite));
        }

        public static void Move (this IFile self, string destFilePath) {
            if (self == null) { return; }

            AsyncHelper.RunSync (() => self.MoveAsync (destFilePath));
        }

        public static void Delete (this IFile self) {
            if (self == null) { return; }

            AsyncHelper.RunSync (() => self.DeleteAsync ());
        }

        public static async Task<string> GetTextAsync (this IFile self, Encoding encoding = null) {
            if (self == null) { return null; }

            using var stream = await self.CreateStreamAsync ();
            using var streamReader = new StreamReader (stream, encoding ?? Encoding.UTF8);

            return streamReader.ReadToEnd ();
        }

        public static string GetText (this IFile self, Encoding encoding = null) {
            if (self == null) { return null; }

            return AsyncHelper.RunSync (() => GetTextAsync (self, encoding));
        }

        #endregion
    }
}