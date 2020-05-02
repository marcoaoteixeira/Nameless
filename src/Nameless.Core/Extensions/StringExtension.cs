using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Nameless {

    /// <summary>
    /// Extension methods for <see cref="string"/>.
    /// </summary>
    public static class StringExtension {

        #region Public Static Methods

        /// <summary>
        /// Returns <paramref name="fallback"/> if <see cref="string"/> is <c>null</c>, empty or white spaces.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <param name="fallback">The fallback <see cref="string"/>.</param>
        /// <returns>The <paramref name="self"/> if not <c>null</c>, empty or white spaces, otherwise, <paramref name="fallback"/>.</returns>
        public static string OnBlank (this string self, string fallback) => string.IsNullOrWhiteSpace (self) ? fallback : self;

        /// <summary>
        /// Remove diacritics from <paramref name="self"/> <see cref="string"/>.
        /// Diacritics are signs, such as an accent or cedilla, which when written above or below a letter indicates
        /// a difference in pronunciation from the same letter when unmarked or differently marked.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <returns>A new <see cref="string"/> without diacritics.</returns>
        public static string RemoveDiacritics (this string self) {
            if (string.IsNullOrWhiteSpace (self)) {
                return self;
            }

            var normalized = self.Normalize (NormalizationForm.FormD);
            var stringBuilder = new StringBuilder ();

            foreach (var @char in normalized) {
                if (CharUnicodeInfo.GetUnicodeCategory (@char) != UnicodeCategory.NonSpacingMark) {
                    stringBuilder.Append (@char);
                }
            }

            return stringBuilder.ToString ().Normalize (NormalizationForm.FormC);
        }

        /// <summary>
        /// Repeats the <paramref name="self"/> N times.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <param name="times">Times to repeat.</param>
        /// <returns>A new <see cref="string"/> representing the <paramref name="self"/> repeated N times.</returns>
        public static string Repeat (this string self, int times) {
            if (string.IsNullOrWhiteSpace (self)) {
                return string.Empty;
            }

            if (times <= 0) {
                return self;
            }

            using (var writer = new StringWriter ()) {
                for (var counter = 0; counter < times; counter++) {
                    writer.Write (self);
                }

                return writer.GetStringBuilder ().ToString ();
            }
        }

        /// <summary>
        /// Transforms the <see cref="string"/> instance into a stream.
        /// </summary>
        /// <param name="self">The current string.</param>
        /// <param name="encoding">The encoding. Default is <see cref="Encoding.ASCII" /></param>
        /// <returns>An instance of <see cref="MemoryStream"/> representing the current <see cref="string"/>.</returns>
        public static Stream ToStream (this string self, Encoding encoding = null) {
            if (self == null) { return Stream.Null; }

            return new MemoryStream ((encoding ?? Encoding.ASCII).GetBytes (self));
        }

        /// <summary>
        /// Separates a phrase by camel case.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <returns>A camel case separated <see cref="string"/> representing the current <see cref="string"/>.</returns>
        public static string CamelFriendly (this string self) {
            if (string.IsNullOrWhiteSpace (self)) {
                return string.Empty;
            }

            var result = new StringBuilder (self);

            for (var idx = self.Length - 1; idx > 0; idx--) {
                var current = result[idx];

                if ('A' <= current && current <= 'Z') {
                    result.Insert (idx, ' ');
                }
            }

            return result.ToString ();
        }

        /// <summary>
        /// Slice the current <see cref="string"/> by <paramref name="characterCount"/> and adds
        /// an ellipsis HTML symbol at the end.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <param name="characterCount">The number of characters to slice.</param>
        /// <returns>A <see cref="string"/> representation of the sliced <see cref="string"/>.</returns>
        public static string Ellipsize (this string self, int characterCount) {
            return self.Ellipsize (characterCount, "&#160;&#8230;");
        }

        /// <summary>
        /// Slice the current <see cref="string"/> by <paramref name="characterCount"/> and adds
        /// an ellipsis defined symbol at the end.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <param name="characterCount">The number of characters to slice.</param>
        /// <param name="ellipsis">The ellipsis symbol.</param>
        /// <param name="wordBoundary">Use word boundary to slice.</param>
        /// <returns>A <see cref="string"/> representation of the sliced <see cref="string"/>.</returns>
        public static string Ellipsize (this string self, int characterCount, string ellipsis, bool wordBoundary = false) {
            if (string.IsNullOrWhiteSpace (self)) {
                return string.Empty;
            }

            if (characterCount < 0 || self.Length <= characterCount) {
                return self;
            }

            // search beginning of word
            var backup = characterCount;
            while (characterCount > 0 && self[characterCount - 1].IsLetter ()) {
                characterCount--;
            }

            // search previous word
            while (characterCount > 0 && self[characterCount - 1].IsSpace ()) {
                characterCount--;
            }

            // if it was the last word, recover it, unless boundary is requested
            if (characterCount == 0 && !wordBoundary) {
                characterCount = backup;
            }

            var trimmed = self.Substring (0, characterCount);
            return string.Concat (trimmed, ellipsis);
        }

        /// <summary>
        /// Transforms a hexa <see cref="string"/> value into a <see cref="byte"/> array.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <returns>An array of <see cref="byte"/>.</returns>
        public static byte[] FromHexToByteArray (this string self) {
            return Enumerable.Range (0, self.Length).
            Where (_ => (_ % 2) == 0).
            Select (_ => Convert.ToByte (self.Substring (_, 2), fromBase : 16 /* hexadecimal */ )).
            ToArray ();
        }

        /// <summary>
        /// Replaces all occurences from <paramref name="self"/> with the values presents
        /// in <paramref name="replacements"/>.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <param name="replacements">The replacements.</param>
        /// <returns>A replacemented <see cref="string"/>.</returns>
        public static string ReplaceAll (this string self, IDictionary<string, string> replacements) {
            var pattern = string.Format ("{0}", string.Join ("|", replacements.Keys));

            return Regex.Replace (self, pattern, match => replacements[match.Value]);
        }

        /// <summary>
        /// Converts the <paramref name="self"/> into a Base64 <see cref="string"/> representation.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <param name="encoding">The encoding. Default is <see cref="Encoding.ASCII"/></param>
        /// <returns>The Base64 <see cref="string"/> representation.</returns>
        public static string ToBase64 (this string self, Encoding encoding = null) => Convert.ToBase64String ((encoding ?? Encoding.ASCII).GetBytes (self));

        /// <summary>
        /// Converts from a Base64 <see cref="string"/> representation.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <param name="encoding">The encoding. Default is <see cref="Encoding.ASCII"/></param>
        /// <returns>The <see cref="string"/> representation.</returns>
        public static string FromBase64 (this string self, Encoding encoding = null) => (encoding ?? Encoding.ASCII).GetString (Convert.FromBase64String (self));

        /// <summary>
        /// Strips a <see cref="string"/> by the specified <see cref="char"/> from <paramref name="stripped"/>.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <param name="stripped">Stripper values</param>
        /// <returns>A stripped version of the <paramref name="self"/> parameter.</returns>
        public static string Strip (this string self, params char[] stripped) {
            if (string.IsNullOrWhiteSpace (self) || stripped.IsNullOrEmpty ()) {
                return self;
            }

            var result = new char[self.Length];
            var cursor = 0;
            for (var idx = 0; idx < self.Length; idx++) {
                var current = self[idx];
                if (Array.IndexOf (stripped, current) < 0) {
                    result[cursor++] = current;
                }
            }

            return new string (result, 0, cursor);
        }

        /// <summary>
        /// Strips a <see cref="string"/> by the specified function.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <param name="predicate">The stripper function.</param>
        /// <returns>A stripped version of the <paramref name="self"/> parameter.</returns>
        public static string Strip (this string self, Func<char, bool> predicate) {
            var result = new char[self.Length];
            var cursor = 0;

            for (var idx = 0; idx < self.Length; idx++) {
                var current = self[idx];
                if (!predicate (current)) {
                    result[cursor++] = current;
                }
            }

            return new string (result, 0, cursor);
        }

        /// <summary>
        /// Checks if there is any presence of the specified <see cref="char"/>s in the self <see cref="string"/>.
        /// </summary>
        /// <param name="self">The current <see cref="string"/></param>
        /// <param name="chars">The <see cref="char"/>s to check.</param>
        /// <returns><c>true</c> if any of the <see cref="char"/> exists, otherwise, <c>false</c>.</returns>
        public static bool Any (this string self, params char[] chars) {
            if (string.IsNullOrEmpty (self) || chars == null || chars.Length == 0) {
                return false;
            }

            for (var idx = 0; idx < self.Length; idx++) {
                var current = self[idx];
                if (Array.IndexOf (chars, current) >= 0) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if there is all presences of the specified <see cref="char"/>s in the self <see cref="string"/>.
        /// </summary>
        /// <param name="self">The current <see cref="string"/></param>
        /// <param name="chars">The <see cref="char"/>s to check.</param>
        /// <returns><c>true</c> if all of the <see cref="char"/> exists, otherwise, <c>false</c>.</returns>
        public static bool All (this string self, params char[] chars) {
            if (string.IsNullOrEmpty (self)) {
                return true;
            }

            if (chars == null || chars.Length == 0) {
                return false;
            }

            for (var idx = 0; idx < self.Length; idx++) {
                var current = self[idx];
                if (Array.IndexOf (chars, current) < 0) {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Changes the specified <see cref="char"/>s of <paramref name="from"/> with the
        /// specified <see cref="char"/>s of <paramref name="to"/>.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <param name="from">"from" <see cref="char"/> array</param>
        /// <param name="to">"to" <see cref="char"/> array</param>
        /// <returns>The translated representation of <paramref name="self"/>.</returns>
        public static string Translate (this string self, char[] from, char[] to) {
            if (string.IsNullOrEmpty (self)) {
                return self;
            }

            if (from == null || to == null) {
                throw new ArgumentNullException ();
            }

            if (from.Length != to.Length) {
                throw new ArgumentNullException ("from", "Parameters must have the same length");
            }

            var map = new Dictionary<char, char> (from.Length);
            for (var idx = 0; idx < from.Length; idx++) {
                map[from[idx]] = to[idx];
            }

            var result = new char[self.Length];

            for (var idx = 0; idx < self.Length; idx++) {
                var current = self[idx];
                result[idx] = map.ContainsKey (current) ?
                    map[current] :
                    current;
            }

            return new string (result);
        }

        /// <summary>
        /// Generates a valid technical name.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <remarks>Uses a white list set of chars.</remarks>
        public static string ToSafeName (this string self) {
            if (string.IsNullOrWhiteSpace (self)) {
                return string.Empty;
            }

            var result = RemoveDiacritics (self)
                .Strip (character => !character.IsLetter () && !Char.IsDigit (character))
                .Trim ();

            // don't allow non A-Z chars as first letter, as they are not allowed in prefixes
            while (result.Length > 0 && !result[0].IsLetter ()) {
                result = result.Substring (1);
            }

            if (result.Length > 128) {
                result = result.Substring (0, 128);
            }

            return result;
        }

        /// <summary>
        /// Removes all HTML tags from <paramref name="self"/> <see cref="string"/>.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <param name="htmlDecode"><c>true</c> if should HTML decode.</param>
        /// <returns></returns>
        public static string RemoveHtmlTags (this string self, bool htmlDecode = false) {
            if (string.IsNullOrEmpty (self)) {
                return string.Empty;
            }

            var content = new char[self.Length];

            var cursor = 0;
            var inside = false;
            for (var idx = 0; idx < self.Length; idx++) {
                char current = self[idx];

                switch (current) {
                    case '<':
                        inside = true;
                        continue;
                    case '>':
                        inside = false;
                        continue;
                }

                if (!inside) {
                    content[cursor++] = current;
                }
            }

            var result = new string (content, 0, cursor);
            return htmlDecode ? WebUtility.HtmlDecode (result) : result;
        }

        /// <summary>
        /// Checks if the <paramref name="self"/> <see cref="string"/> contains the specified value.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <param name="contains">The text that should look for.</param>
        /// <returns><c>true</c> if contains, otherwise, <c>false</c>.</returns>
        public static bool Contains (this string self, string contains) => Contains (self, contains, StringComparison.CurrentCultureIgnoreCase);

        /// <summary>
        /// Checks if the <paramref name="self"/> <see cref="string"/> contains the specified value.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <param name="contains">The text that should look for.</param>
        /// <param name="stringComparison">Comparison style.</param>
        /// <returns><c>true</c> if contains, otherwise, <c>false</c>.</returns>
        public static bool Contains (this string self, string contains, StringComparison stringComparison) {
            Prevent.ParameterNull (contains, nameof (contains));

            if (self == null) { return false; }

            return self.IndexOf (contains, stringComparison) > 0;
        }

        /// <summary>
        /// Checks if the <paramref name="self"/> matchs (Regexp) a specified pattern.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <param name="regExp">The regexp.</param>
        /// <param name="regexOptions">The regexp options.</param>
        /// <returns><c>true</c> if matchs, otherwise, <c>false</c>.</returns>
        public static bool IsMatch (this string self, string regExp, RegexOptions regexOptions = RegexOptions.None) {
            Prevent.ParameterNull (regExp, nameof (regExp));

            if (self == null) { return false; }

            return Regex.IsMatch (self, regExp, regexOptions);
        }

        /// <summary>
        /// Replaces a specified value given a regexp.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <param name="regExp">The regexp.</param>
        /// <param name="replacement">The replacement value..</param>
        /// <param name="regexOptions">The regexp options</param>
        /// <returns>A <see cref="string"/> representing the new value.</returns>
        public static string Replace (this string self, string regExp, string replacement, RegexOptions regexOptions = RegexOptions.None) {
            Prevent.ParameterNull (regExp, nameof (regExp));
            Prevent.ParameterNull (replacement, nameof (replacement));

            if (self == null) { return self; }

            return Regex.Replace (self, regExp, replacement, regexOptions);
        }

        /// <summary>
        /// Splits the <paramref name="self"/> <see cref="string"/> by the specified regexp.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <param name="regExp">The regexp.</param>
        /// <param name="regexOptions">The regexp options.</param>
        /// <returns>A array of <see cref="string"/>.</returns>
        public static string[] Split (this string self, string regExp, RegexOptions regexOptions = RegexOptions.None) {
            Prevent.ParameterNull (regExp, nameof (regExp));

            if (self == null) { return new string[0]; }

            return Regex.Split (self, regExp, regexOptions);
        }

        /// <summary>
        /// Returns a <see cref="byte"/> array representation of the <paramref name="self"/> <see cref="string"/>.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <param name="encoding">The encoding. Default is <see cref="Encoding.ASCII"/></param>
        /// <returns>An array of <see cref="byte"/>.</returns>
        public static byte[] GetBytes (this string self, Encoding encoding = null) {
            if (self == null) { return new byte[0]; }

            return (encoding ?? Encoding.ASCII).GetBytes (self);
        }

        /// <summary>
        /// Splits the <paramref name="self"/> <see cref="string"/> by camel case.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <returns>An array of <see cref="string"/>.</returns>
        /// <remarks>Source: http://haacked.com/archive/2005/09/24/splitting-pascalcamel-cased-strings.aspx </remarks>
        public static string[] SplitUpperCase (this string self) {
            if (string.IsNullOrWhiteSpace (self)) { return new string[0]; }

            var words = new StringCollection ();
            var wordStartIndex = 0;
            var letters = self.ToCharArray ();
            var previousChar = char.MinValue;

            // Skip the first letter. we don't care what case it is.
            for (var idx = 1; idx < letters.Length; idx++) {
                if (char.IsUpper (letters[idx]) && !char.IsWhiteSpace (previousChar)) {
                    //Grab everything before the current character.
                    words.Add (new string (letters, wordStartIndex, idx - wordStartIndex));
                    wordStartIndex = idx;
                }

                previousChar = letters[idx];
            }

            //We need to have the last word.
            words.Add (new string (letters, wordStartIndex, letters.Length - wordStartIndex));

            var wordArray = new string[words.Count];

            words.CopyTo (wordArray, 0);

            return wordArray;
        }

        /// <summary>
        /// Retrieves the MD5 for the current string.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <returns>The MD5 representation for the <paramref name="self"/>.</returns>
        public static string GetMD5 (this string self) {
            if (self == null) { return null; }

            using (var provider = MD5.Create ()) {
                var buffer = Encoding.UTF8.GetBytes (self);
                var result = provider.ComputeHash (buffer);

                return BitConverter.ToString (result);
            }
        }

        /// <summary>
        /// Removes the prefix.
        /// </summary>
        /// <param name="self">The self string.</param>
        /// <param name="prefix">The specified prefix</param>
        /// <returns></returns>
        public static string TrimPrefix (this string self, string prefix) {
            if (self == null) { return null; }
            if (prefix == null) { return self; }

            return self.StartsWith (prefix, StringComparison.Ordinal) ? self.Substring (prefix.Length) : self;
        }

        /// <summary>
        /// Removes the suffix.
        /// </summary>
        /// <param name="self">The self string.</param>
        /// <param name="suffix">The specified suffix.</param>
        /// <returns></returns>
        public static string TrimSuffix (this string self, string suffix) {
            if (self == null) { return null; }
            if (suffix == null) { return self; }

            return self.EndsWith (suffix, StringComparison.Ordinal) ? self.Substring (0, self.Length - suffix.Length) : self;
        }

        public static string SafeSubstring (this string self, int start, int length) {
            if (self == null) { return null; }
            if (start < 0 || length <= 0 || self.Length <= start) { return null; }

            if (self.Length < (start + length)) {
                length = self.Length - start;
            }

            return self.Length > length ?
                self.Substring (start, length) :
                self;
        }

        #endregion Public Static Methods
    }
}