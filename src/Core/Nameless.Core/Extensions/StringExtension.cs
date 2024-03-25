using System.Collections.Specialized;
using System.Globalization;
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
        /// Remove diacritics from <paramref name="self"/> <see cref="string"/>.
        /// Diacritics are signs, such as an accent or cedilla, which when written above or below a letter indicates
        /// a difference in pronunciation from the same letter when unmarked or differently marked.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <returns>A new <see cref="string"/> without diacritics.</returns>
        /// <exception cref="NullReferenceException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static string RemoveDiacritics(this string self) {
            var normalized = self.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();
            foreach (var @char in normalized) {
                if (CharUnicodeInfo.GetUnicodeCategory(@char) != UnicodeCategory.NonSpacingMark) {
                    stringBuilder.Append(@char);
                }
            }
            return stringBuilder
                .ToString()
                .Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Repeats the <paramref name="self"/> N times.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <param name="times">Times to repeat.</param>
        /// <returns>A new <see cref="string"/> representing the <paramref name="self"/> repeated N times.</returns>
        public static string Repeat(this string self, int times) {
            if (self is null || times <= 0) { return self ?? string.Empty; }

            var builder = new StringBuilder();
            for (var counter = 0; counter < times; counter++) {
                builder.Append(self);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Transforms the <see cref="string"/> instance into a stream.
        /// </summary>
        /// <param name="self">The current string.</param>
        /// <param name="encoding">The encoding. Default is <see cref="Encoding.UTF8" /> without BOM</param>
        /// <returns>An instance of <see cref="MemoryStream"/> representing the current <see cref="string"/>.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static MemoryStream ToMemoryStream(this string self, Encoding? encoding = default)
            => new((encoding ?? Root.Defaults.Encoding).GetBytes(self));

        /// <summary>
        /// Separates a phrase by camel case.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <returns>A camel case separated <see cref="string"/> representing the current <see cref="string"/>.</returns>
        /// <exception cref="NullReferenceException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static string CamelFriendly(this string self) {
            var result = new StringBuilder(self);

            for (var idx = self.Length - 1; idx > 0; idx--) {
                var current = result[idx];
                if (current is >= 'A' and <='Z') {
                    result.Insert(idx, ' ');
                }
            }

            return result.ToString();
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
        /// <exception cref="ArgumentNullException">if <paramref name="ellipsis"/> is <c>null</c>.</exception>
        /// <exception cref="NullReferenceException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static string Ellipsize(this string self, int characterCount, string ellipsis = "&#160;&#8230;", bool wordBoundary = false) {
            Guard.Against.Null(ellipsis, nameof(ellipsis));

            if (characterCount < 0 || self.Length <= characterCount) {
                return self;
            }

            // search beginning of word
            var backup = characterCount;
            while (characterCount > 0 && self[characterCount - 1].IsLetter()) {
                characterCount--;
            }

            // search previous word
            while (characterCount > 0 && self[characterCount - 1].IsWhiteSpace()) {
                characterCount--;
            }

            // if it was the last word, recover it, unless boundary is requested
            if (characterCount == 0 && !wordBoundary) {
                characterCount = backup;
            }

            var trimmed = self[..characterCount];
            return string.Concat(trimmed, ellipsis);
        }

        /// <summary>
        /// Transforms a hex <see cref="string"/> value into a <see cref="byte"/> array.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <returns>An array of <see cref="byte"/>.</returns>
        /// <exception cref="NullReferenceException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static byte[] FromHexToByteArray(this string self)
            => Enumerable.Range(0, self.Length)
                .Where(_ => _ % 2 == 0)
                .Select(_ => Convert.ToByte(self.Substring(_, 2), fromBase: 16 /* hexadecimal */ ))
                .ToArray();

        /// <summary>
        /// Replaces all occurrences from <paramref name="self"/> with the values presents
        /// in <paramref name="replacements"/>.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <param name="replacements">The replacements.</param>
        /// <returns>A replaced <see cref="string"/>.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="self"/> or <paramref name="replacements"/> is <c>null</c>.</exception>
        public static string ReplaceAll(this string self, IDictionary<string, string> replacements) {
            Guard.Against.Null(replacements, nameof(replacements));

            var pattern = string.Format("{0}", string.Join("|", replacements.Keys));

            return Regex.Replace(self, pattern, match => replacements[match.Value]);
        }

        /// <summary>
        /// Converts the <paramref name="self"/> into a Base64 <see cref="string"/> representation.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <param name="encoding">The encoding. Default is <see cref="Encoding.UTF8"/> without BOM</param>
        /// <returns>The Base64 <see cref="string"/> representation.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static string ToBase64(this string self, Encoding? encoding = default)
            => Convert.ToBase64String((encoding ?? Root.Defaults.Encoding).GetBytes(self));

        /// <summary>
        /// Converts from a Base64 <see cref="string"/> representation.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <param name="encoding">The encoding. Default is <see cref="Encoding.UTF8"/> without BOM</param>
        /// <returns>The <see cref="string"/> representation.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static string FromBase64(this string self, Encoding? encoding = default)
            => (encoding ?? Root.Defaults.Encoding).GetString(Convert.FromBase64String(self));

        /// <summary>
        /// Strips a <see cref="string"/> by the specified <see cref="char"/> from <paramref name="chars"/>.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <param name="chars">Stripper values</param>
        /// <returns>A stripped version of the <paramref name="self"/> parameter.</returns>
        /// <exception cref="NullReferenceException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static string Strip(this string self, params char[] chars) {
            var result = new char[self.Length];
            var cursor = 0;
            for (var idx = 0; idx < self.Length; idx++) {
                var current = self[idx];
                if (Array.IndexOf(chars, current) < 0) {
                    result[cursor++] = current;
                }
            }

            return new string(result, 0, cursor);
        }

        /// <summary>
        /// Strips a <see cref="string"/> by the specified function.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <param name="predicate">The stripper function.</param>
        /// <returns>A stripped version of the <paramref name="self"/> parameter.</returns>
        /// <exception cref="NullReferenceException">if <paramref name="self"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">if <paramref name="predicate"/> is <c>null</c>.</exception>
        public static string Strip(this string self, Func<char, bool> predicate) {
            Guard.Against.Null(predicate, nameof(predicate));

            var result = new char[self.Length];
            var cursor = 0;

            for (var idx = 0; idx < self.Length; idx++) {
                var current = self[idx];
                if (!predicate(current)) {
                    result[cursor++] = current;
                }
            }

            return new string(result, 0, cursor);
        }

        /// <summary>
        /// Checks if there is any presence of the specified <see cref="char"/>s in the self <see cref="string"/>.
        /// </summary>
        /// <param name="self">The current <see cref="string"/></param>
        /// <param name="chars">The <see cref="char"/>s to check.</param>
        /// <returns><c>true</c> if any of the <see cref="char"/> exists, otherwise, <c>false</c>.</returns>
        /// <exception cref="NullReferenceException">if <paramref name="self"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException"> <paramref name="chars"/> is <c>null</c>.</exception>
        public static bool Any(this string self, params char[] chars) {
            Guard.Against.Null(chars, nameof(chars));

            if (chars.Length == 0) { return false; }

            for (var idx = 0; idx < self.Length; idx++) {
                var current = self[idx];
                if (Array.IndexOf(chars, current) >= 0) {
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
        /// <exception cref="NullReferenceException">if <paramref name="self"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">if or <paramref name="chars"/> is <c>null</c>.</exception>
        public static bool All(this string self, params char[] chars) {
            Guard.Against.Null(chars, nameof(chars));

            for (var idx = 0; idx < self.Length; idx++) {
                var current = self[idx];
                if (Array.IndexOf(chars, current) < 0) {
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
        /// <exception cref="ArgumentNullException"><paramref name="from"/> or <paramref name="to"/> is <c>null</c>.</exception>
        public static string Translate(this string self, char[] from, char[] to) {
            Guard.Against.Null(from, nameof(from));
            Guard.Against.Null(to, nameof(to));

            if (string.IsNullOrEmpty(self)) { return string.Empty; }

            if (from.Length != to.Length) {
                throw new ArgumentNullException(nameof(from), "Parameters must have the same length");
            }

            var map = new Dictionary<char, char>(from.Length);
            for (var idx = 0; idx < from.Length; idx++) {
                map[from[idx]] = to[idx];
            }

            var result = new char[self.Length];
            for (var idx = 0; idx < self.Length; idx++) {
                var current = self[idx];

                result[idx] = map.TryGetValue(current, out var value)
                    ? value
                    : current;
            }
            return new string(result);
        }

        /// <summary>
        /// Generates a valid technical name.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <param name="maxSize">The maximum size of the name.</param>
        /// <remarks>Uses a white list set of chars.</remarks>
        /// <exception cref="NullReferenceException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static string ToSafeName(this string self, int maxSize = 128) {
            var result = self.RemoveDiacritics();

            if (string.IsNullOrWhiteSpace(result)) { return string.Empty; }
            result = result.Strip(character => !character.IsLetter() && !char.IsDigit(character));

            if (string.IsNullOrWhiteSpace(result)) { return string.Empty; }
            result = result.Trim();

            // don't allow non A-Z chars as first letter, as they are not allowed in prefixes
            while (result.Length > 0 && !result[0].IsLetter()) {
                result = result[1..];
            }

            if (result.Length > maxSize) {
                result = result[..maxSize];
            }

            return result;
        }

        /// <summary>
        /// Removes all HTML tags from <paramref name="self"/> <see cref="string"/>.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <param name="htmlDecode"><c>true</c> if should HTML decode.</param>
        /// <returns>The <see cref="string"/> without HTML tags</returns>
        /// <exception cref="NullReferenceException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static string RemoveHtmlTags(this string self, bool htmlDecode = false) {
            var content = new char[self.Length];

            var cursor = 0;
            var inside = false;
            for (var idx = 0; idx < self.Length; idx++) {
                var current = self[idx];

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

            var result = new string(content, 0, cursor);
            return htmlDecode ? WebUtility.HtmlDecode(result) : result;
        }

        /// <summary>
        /// Checks if the <paramref name="self"/> <see cref="string"/> contains the specified value.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <param name="contains">The text that should look for.</param>
        /// <param name="stringComparison">Comparison style.</param>
        /// <returns><c>true</c> if contains, otherwise, <c>false</c>.</returns>
        /// <exception cref="NullReferenceException">if <paramref name="self"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">if <paramref name="contains"/> is <c>null</c>.</exception>
        public static bool Contains(this string self, string contains, StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
            => self.IndexOf(contains, stringComparison) > 0;

        /// <summary>
        /// Checks if the <paramref name="self"/> matches (Regexp) a specified pattern.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <param name="regExp">The regexp.</param>
        /// <param name="regexOptions">The regexp options.</param>
        /// <returns><c>true</c> if matches, otherwise, <c>false</c>.</returns>
        /// <exception cref="NullReferenceException">if <paramref name="self"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">if <paramref name="regExp"/> is <c>null</c>.</exception>
        public static bool IsMatch(this string self, string regExp, RegexOptions regexOptions = RegexOptions.None)
            => Regex.IsMatch(self, regExp, regexOptions);

        /// <summary>
        /// Replaces a specified value given a regexp.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <param name="regExp">The regexp.</param>
        /// <param name="replacement">The replacement value..</param>
        /// <param name="regexOptions">The regexp options</param>
        /// <returns>A <see cref="string"/> representing the new value.</returns>
        /// <exception cref="ArgumentNullException">
        /// if <paramref name="self"/> or <paramref name="regExp"/> or <paramref name="replacement"/> is <c>null</c>.
        /// </exception>
        public static string Replace(this string self, string regExp, string replacement, RegexOptions regexOptions = RegexOptions.None)
            => Regex.Replace(self, regExp, replacement, regexOptions);

        /// <summary>
        /// Splits the <paramref name="self"/> <see cref="string"/> by the specified regexp.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <param name="regExp">The regexp.</param>
        /// <param name="regexOptions">The regexp options.</param>
        /// <returns>A array of <see cref="string"/>.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="self"/> or <paramref name="regExp"/> is <c>null</c>.</exception>
        public static string[] Split(this string self, string regExp, RegexOptions regexOptions = RegexOptions.None)
            => Regex.Split(self, regExp, regexOptions);

        /// <summary>
        /// Returns a <see cref="byte"/> array representation of the <paramref name="self"/> <see cref="string"/>.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <param name="encoding">The encoding. Default is <see cref="Encoding.UTF8"/> without BOM</param>
        /// <returns>An array of <see cref="byte"/>.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static byte[] GetBytes(this string self, Encoding? encoding = default)
            => (encoding ?? Root.Defaults.Encoding).GetBytes(self);

        /// <summary>
        /// Splits the <paramref name="self"/> <see cref="string"/> by camel case.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <returns>An array of <see cref="string"/>.</returns>
        /// <remarks>Source: http://haacked.com/archive/2005/09/24/splitting-pascalcamel-cased-strings.aspx </remarks>
        /// <exception cref="NullReferenceException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static string[] SplitUpperCase(this string self) {
            var words = new StringCollection();
            var wordStartIndex = 0;
            var letters = self.ToCharArray();
            var previousChar = char.MinValue;

            // Skip the first letter. we don't care what case it is.
            for (var idx = 1; idx < letters.Length; idx++) {
                if (char.IsUpper(letters[idx]) && !char.IsWhiteSpace(previousChar)) {
                    //Grab everything before the current character.
                    words.Add(new string(letters, wordStartIndex, idx - wordStartIndex));
                    wordStartIndex = idx;
                }
                previousChar = letters[idx];
            }

            //We need to have the last word.
            words.Add(new string(letters, wordStartIndex, letters.Length - wordStartIndex));
            var wordArray = new string[words.Count];
            words.CopyTo(wordArray, 0);

            return wordArray;
        }

        /// <summary>
        /// Retrieves the MD5 for the current string.
        /// </summary>
        /// <param name="self">The current <see cref="string"/>.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>The MD5 representation for the <paramref name="self"/>.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static string GetMD5(this string self, Encoding? encoding = default) {
            var buffer = (encoding ?? Root.Defaults.Encoding).GetBytes(self);

#if NETSTANDARD2_1_OR_GREATER
            using var md5 = MD5.Create();
            var result = md5.ComputeHash(buffer);
#endif

#if NET6_0_OR_GREATER
            var result = MD5.HashData(buffer);
#endif

            return BitConverter.ToString(result);
        }

        /// <summary>
        /// Returns a substring value from the current <see cref="string"/> in a safe manner.
        /// </summary>
        /// <param name="self">The current <see cref="string"/></param>
        /// <param name="start">The start value.</param>
        /// <param name="length">The length value.</param>
        /// <returns>A <see cref="string"/> representing the substring value.</returns>
        public static string SafeSubstring(this string? self, int start, int length) {
            if (self is null || start < 0 || length <= 0 || self.Length <= start) {
                return string.Empty;
            }

            if (self.Length < start + length) {
                length = self.Length - start;
            }

            return self.Length > length ? self.Substring(start, length) : self;
        }

        public static bool ToBoolean(this string? self) {
            // we'll consider null as false.
            if (self is null) { return false; }

            // any numeric value less than 0 is false.
            if (double.TryParse(self, out var result)) {
                return result > 0d;
            }

            // self explanatory.
            return string.Equals(self, bool.TrueString, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Removes, from the start of the current string, the first occurence of
        /// any value of <paramref name="values"/>.
        /// </summary>
        /// <param name="self">The current instance of <see cref="string"/>.</param>
        /// <param name="values">A collection of values to match.</param>
        /// <param name="comparison">The comparison type.</param>
        /// <returns>The current <see cref="string"/> without the matching start, if exists.</returns>
        public static string RemoveHead(this string self, string[] values, StringComparison comparison = StringComparison.OrdinalIgnoreCase) {
            Guard.Against.Null(values, nameof(values));

            foreach (var value in values) {
                if (string.IsNullOrWhiteSpace(value)) {
                    continue;
                }

                if (self.StartsWith(value, comparison)) {
                    return self[value.Length..];
                }
            }

            return self;
        }

        /// <summary>
        /// Removes, from the end of the current string, the first occurence of
        /// any value of <paramref name="values"/>.
        /// </summary>
        /// <param name="self">The current instance of <see cref="string"/>.</param>
        /// <param name="values">A collection of values to match.</param>
        /// <param name="comparison">The comparison type.</param>
        /// <returns>The current <see cref="string"/> without the matching end, if exists.</returns>
        public static string RemoveTail(this string self, string[] values, StringComparison comparison = StringComparison.Ordinal) {
            Guard.Against.Null(values, nameof(values));

            foreach (var value in values) {
                if (string.IsNullOrWhiteSpace(value)) {
                    continue;
                }

                if (self.EndsWith(value, comparison)) {
                    return self[..^value.Length];
                }
            }

            return self;
        }

        #endregion
    }
}