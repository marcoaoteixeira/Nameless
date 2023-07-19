using System.Text;

namespace Nameless {
    public static class Defaults {
        #region Public Static Inner Classes

        public static class Encodings {

            #region Public Static Read-Only Properties

            /// <summary>
            /// Gets the encoding UTF8 (without BOM).
            /// </summary>
            public static Encoding UTF8 => new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

            #endregion
        }

        #endregion
    }
}
