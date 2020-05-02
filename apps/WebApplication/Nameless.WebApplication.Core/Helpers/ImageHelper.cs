using System;

namespace Nameless.WebApplication.Core.Helpers {
    public static class ImageHelper {
        #region Public Enumerator

        public enum ImageType : int {
            PNG = 0,
            JPG = 1,
            GIF = 2,
            BMP = 3
        }

        #endregion

        #region Public Static Methods

        public static string GetBase64Image (byte[] array, ImageType imageType = ImageType.PNG) {
            if (array == null) { return null; }
            
            var base64 = Convert.ToBase64String (array);
            var result = $"data:image/{imageType};base64, {base64}";

            return result;
        }

        #endregion
    }
}