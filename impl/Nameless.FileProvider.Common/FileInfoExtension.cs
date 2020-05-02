using System.Text;
using Microsoft.Extensions.FileProviders;

namespace Nameless.FileProvider.Common {
    public static class FileInfoExtension {
        #region Public Static Methods

        public static string GetText (this IFileInfo self, Encoding encoding = null) {
            if (self == null) { return null; }
            using (var stream = self.CreateReadStream ()) {
                return stream.ToText (encoding);
            }
        }

        #endregion

    }
}