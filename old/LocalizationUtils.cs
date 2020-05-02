using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Nameless.Localization.Json {
    internal class LocalizationUtils {
        #region Internal Methods

        internal static IEnumerable<string> ExpandPath (string name) {
            Prevent.ParameterNullOrWhiteSpace (name, nameof (name));

            var expansion = new StringBuilder ();

            // Start replacing periods, starting at the beginning.
            var components = name.Split (new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            for (var first = 0; first < components.Length; first++) {
                for (var second = 0; second < components.Length; second++) {
                    expansion.Append (components[second]).Append (second < first ? Path.DirectorySeparatorChar : '.');
                }
                // Remove trailing period.
                yield return expansion.Remove (expansion.Length - 1, 1).ToString ();
                expansion.Clear ();
            }
        }

        internal static string[] GetCultures (CultureInfo culture) {
            return !culture.IsNeutralCulture
                ? new[] { culture.Name, culture.Parent.Name, string.Empty }
                : new[] { culture.Name, string.Empty };
        }

        #endregion
    }
}