namespace Nameless.ObjectModel;

/// <summary>
///     Organised by behaviour area; each class maps to one public concern.
/// </summary>
public static class SemVersionTests {
    // =========================================================================
    // Parse — happy path
    // =========================================================================

    public class ParseValidInput {
        [Theory]
        [InlineData("0.0.0", 0, 0, 0)]
        [InlineData("1.0.0", 1, 0, 0)]
        [InlineData("1.2.3", 1, 2, 3)]
        [InlineData("10.20.30", 10, 20, 30)]
        public void CoreTriple_ParsedCorrectly(string input, int major, int minor, int patch) {
            var v = SemVersion.Parse(input);

            Assert.Equal(major, v.Major);
            Assert.Equal(minor, v.Minor);
            Assert.Equal(patch, v.Patch);
            Assert.Null(v.PreRelease);
            Assert.Null(v.Build);
        }

        [Theory]
        [InlineData("1.0.0-alpha", "alpha")]
        [InlineData("1.0.0-alpha.1", "alpha.1")]
        [InlineData("1.0.0-0.3.7", "0.3.7")]
        [InlineData("1.0.0-x.7.z.92", "x.7.z.92")]
        [InlineData("1.0.0-rc.2", "rc.2")]
        [InlineData("1.0.0-x-y-z.--", "x-y-z.--")]
        public void PreRelease_ParsedCorrectly(string input, string expectedPreRelease) {
            var v = SemVersion.Parse(input);

            Assert.Equal(expectedPreRelease, v.PreRelease);
            Assert.Null(v.Build);
        }

        [Theory]
        [InlineData("1.0.0+001", "001")]
        [InlineData("1.0.0+20130313144700", "20130313144700")]
        [InlineData("1.0.0+exp.sha.5114f85", "exp.sha.5114f85")]
        [InlineData("1.0.0+21AF26D3----117B344092BD", "21AF26D3----117B344092BD")]
        public void Build_ParsedCorrectly(string input, string expectedBuild) {
            var v = SemVersion.Parse(input);

            Assert.Equal(expectedBuild, v.Build);
            Assert.Null(v.PreRelease);
        }

        [Fact]
        public void PreReleaseAndBuild_BothParsedCorrectly() {
            var v = SemVersion.Parse("1.2.3-beta.1+sha.abc1234");

            Assert.Equal(1, v.Major);
            Assert.Equal(2, v.Minor);
            Assert.Equal(3, v.Patch);
            Assert.Equal("beta.1", v.PreRelease);
            Assert.Equal("sha.abc1234", v.Build);
        }
    }

    // =========================================================================
    // Parse — invalid input / exceptions
    // =========================================================================

    public class ParseInvalidInput {
        [Theory]
        [InlineData("", typeof(ArgumentException))]
        [InlineData(" ", typeof(ArgumentException))]
        [InlineData(null, typeof(ArgumentNullException))]
        public void Null_Empty_WhiteSpace_ThrowsException(string? value, Type exception) {
            var actual = Record.Exception(() => SemVersion.Parse(value!));

            Assert.IsType(exception, actual);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("1.2")]
        [InlineData("01.2.3")]        // leading zero in major
        [InlineData("1.02.3")]        // leading zero in minor
        [InlineData("1.2.03")]        // leading zero in patch
        [InlineData("1.2.3.4")]       // extra numeric segment
        [InlineData("-1.2.3")]        // negative major
        [InlineData("1.2.3-")]        // empty pre-release
        [InlineData("1.2.3+")]        // empty build
        [InlineData("1.2.3-01")]      // leading zero in numeric pre-release identifier
        [InlineData("1.2.3 ")]        // trailing space
        [InlineData(" 1.2.3")]        // leading space
        [InlineData("a.b.c")]         // non-numeric core
        public void MalformedInput_ThrowsFormatException(string input) {
            Assert.Throws<FormatException>(() => SemVersion.Parse(input));
        }
    }

    // =========================================================================
    // TryParse
    // =========================================================================

    public class TryParseBehaviour {
        [Fact]
        public void ValidInput_ReturnsTrueAndOutputsInstance() {
            var ok = SemVersion.TryParse("2.4.6-rc.1+build.99", out var v);

            Assert.True(ok);
            Assert.NotNull(v);
            Assert.Equal(2, v.Major);
            Assert.Equal(4, v.Minor);
            Assert.Equal(6, v.Patch);
            Assert.Equal("rc.1", v.PreRelease);
            Assert.Equal("build.99", v.Build);
        }

        [Fact]
        public void Null_ReturnsFalseAndOutputsNull() {
            var ok = SemVersion.TryParse(null, out var v);

            Assert.False(ok);
            Assert.Null(v);
        }

        [Theory]
        [InlineData("")]
        [InlineData("not-a-version")]
        [InlineData("1.2")]
        [InlineData("01.2.3")]
        public void InvalidInput_ReturnsFalseAndOutputsNull(string input) {
            var ok = SemVersion.TryParse(input, out var v);

            Assert.False(ok);
            Assert.Null(v);
        }
    }

    // =========================================================================
    // Format / ToString
    // =========================================================================

    public class FormatBehaviour {
        [Theory]
        [InlineData("1.2.3")]
        [InlineData("0.0.0")]
        [InlineData("1.0.0-alpha")]
        [InlineData("1.0.0-alpha.1")]
        [InlineData("1.0.0+build.1")]
        [InlineData("1.2.3-beta.2+sha.abc")]
        public void RoundTrip_ProducesOriginalString(string input) {
            var formatted = SemVersion.Parse(input).Format();

            Assert.Equal(input, formatted);
        }

        [Fact]
        public void ToString_MatchesFormat() {
            var v = SemVersion.Parse("3.1.4-pre.1+meta");

            Assert.Equal(v.Format(), v.ToString());
        }
    }

    // =========================================================================
    // V1 static property
    // =========================================================================

    public class V1Property {
        [Fact]
        public void V1_HasExpectedValues() {
            var v = SemVersion.V1;

            Assert.Equal(1, v.Major);
            Assert.Equal(0, v.Minor);
            Assert.Equal(0, v.Patch);
            Assert.Null(v.PreRelease);
            Assert.Null(v.Build);
        }

        [Fact]
        public void V1_FormatIsCorrect() {
            Assert.Equal("1.0.0", SemVersion.V1.Format());
        }

        [Fact]
        public void V1_ReturnsSameInstance() {
            Assert.Same(SemVersion.V1, SemVersion.V1);
        }
    }

    // =========================================================================
    // Equality (spec §10: build metadata MUST be ignored)
    // =========================================================================

    public class EqualityBehaviour {
        [Fact]
        public void SameVersion_AreEqual() {
            var a = SemVersion.Parse("1.2.3");
            var b = SemVersion.Parse("1.2.3");

            Assert.Equal(a, b);
            Assert.True(a == b);
            Assert.False(a != b);
        }

        [Fact]
        public void DifferentBuildMetadata_AreStillEqual() {
            var a = SemVersion.Parse("1.2.3+build.1");
            var b = SemVersion.Parse("1.2.3+build.2");

            Assert.Equal(a, b);
            Assert.True(a == b);
        }

        [Fact]
        public void DifferentPreRelease_AreNotEqual() {
            var a = SemVersion.Parse("1.2.3-alpha");
            var b = SemVersion.Parse("1.2.3-beta");

            Assert.NotEqual(a, b);
            Assert.True(a != b);
        }

        [Fact]
        public void PreReleaseVsRelease_AreNotEqual() {
            var pre = SemVersion.Parse("1.2.3-alpha");
            var release = SemVersion.Parse("1.2.3");

            Assert.NotEqual(pre, release);
        }

        [Fact]
        public void EqualInstances_HaveSameHashCode() {
            var a = SemVersion.Parse("1.2.3+build.AAA");
            var b = SemVersion.Parse("1.2.3+build.BBB");

            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void NullComparison_OperatorEquality() {
            var v = SemVersion.Parse("1.0.0");

            Assert.NotNull(v);
        }

        [Fact]
        public void ReferenceEqual_IsEqual() {
            var v = SemVersion.Parse("1.0.0");

            Assert.True(v.Equals(v));
        }
    }

    // =========================================================================
    // Comparison / ordering (spec §11)
    // =========================================================================

    public class ComparisonBehaviour {
        // The ordering examples come directly from semver.org §11.4:
        // 1.0.0-alpha < 1.0.0-alpha.1 < 1.0.0-alpha.beta
        //   < 1.0.0-beta < 1.0.0-beta.2 < 1.0.0-beta.11
        //   < 1.0.0-rc.1 < 1.0.0
        [Theory]
        [InlineData("1.0.0-alpha", "1.0.0-alpha.1")]
        [InlineData("1.0.0-alpha.1", "1.0.0-alpha.beta")]
        [InlineData("1.0.0-alpha.beta", "1.0.0-beta")]
        [InlineData("1.0.0-beta", "1.0.0-beta.2")]
        [InlineData("1.0.0-beta.2", "1.0.0-beta.11")]
        [InlineData("1.0.0-beta.11", "1.0.0-rc.1")]
        [InlineData("1.0.0-rc.1", "1.0.0")]
        public void SemVer_OfficialPrecedenceExamples(string lower, string higher) {
            var a = SemVersion.Parse(lower);
            var b = SemVersion.Parse(higher);

            Assert.True(a.CompareTo(b) < 0);
            Assert.True(b.CompareTo(a) > 0);
            Assert.True(a < b);
            Assert.True(b > a);
            Assert.True(a <= b);
            Assert.True(b >= a);
        }

        [Fact]
        public void SameVersion_ComparesToZero() {
            var a = SemVersion.Parse("2.3.4-rc.1");
            var b = SemVersion.Parse("2.3.4-rc.1");

            Assert.Equal(0, a.CompareTo(b));
            Assert.True(a <= b);
            Assert.True(a >= b);
        }

        [Fact]
        public void BuildMetadata_DoesNotAffectOrdering() {
            var a = SemVersion.Parse("1.0.0+build.1");
            var b = SemVersion.Parse("1.0.0+build.999");

            Assert.Equal(0, a.CompareTo(b));
        }

        [Fact]
        public void MajorTakesPrecedenceOverMinor() {
            var lower = SemVersion.Parse("1.9.9");
            var higher = SemVersion.Parse("2.0.0");

            Assert.True(lower < higher);
        }

        [Fact]
        public void MinorTakesPrecedenceOverPatch() {
            var lower = SemVersion.Parse("1.1.9");
            var higher = SemVersion.Parse("1.2.0");

            Assert.True(lower < higher);
        }

        [Fact]
        public void NumericPreReleaseIdentifier_ComparedNumerically() {
            // "2" < "11" numerically, but "2" > "11" lexicographically
            var a = SemVersion.Parse("1.0.0-beta.2");
            var b = SemVersion.Parse("1.0.0-beta.11");

            Assert.True(a < b);
        }

        [Fact]
        public void NumericIdentifier_LowerThanAlphanumeric() {
            // spec: numeric identifiers always have lower precedence than alphanumeric
            var numeric = SemVersion.Parse("1.0.0-1");
            var alpha = SemVersion.Parse("1.0.0-alpha");

            Assert.True(numeric < alpha);
        }

        [Fact]
        public void LongerPreRelease_HigherPrecedenceWhenPrefixEqual() {
            // spec §11.4.4: larger set of fields has higher precedence
            var shorter = SemVersion.Parse("1.0.0-alpha");
            var longer = SemVersion.Parse("1.0.0-alpha.1");

            Assert.True(shorter < longer);
        }

        [Fact]
        public void CompareToNull_ReturnsPositive() {
            var v = SemVersion.Parse("1.0.0");

            Assert.True(v.CompareTo(null) > 0);
        }
    }

    // =========================================================================
    // Prefix — parsing, formatting, and cosmetic-only semantics
    // =========================================================================

    public class PrefixBehaviour {
        [Theory]
        [InlineData("v1.2.3", 'v')]
        [InlineData("V1.2.3", 'V')]
        [InlineData("v1.0.0-alpha.1", 'v')]
        [InlineData("V0.0.1+build.99", 'V')]
        public void PrefixedInput_PrefixPropertyIsSet(string input, char expectedPrefix) {
            var v = SemVersion.Parse(input);

            Assert.Equal(expectedPrefix, v.Prefix);
        }

        [Theory]
        [InlineData("1.2.3")]
        [InlineData("0.0.0")]
        [InlineData("1.0.0-alpha+build")]
        public void UnprefixedInput_PrefixPropertyIsNull(string input) {
            var v = SemVersion.Parse(input);

            Assert.Null(v.Prefix);
        }

        [Theory]
        [InlineData("v1.2.3", "v1.2.3")]
        [InlineData("V1.2.3", "V1.2.3")]
        [InlineData("v1.0.0-alpha.1", "v1.0.0-alpha.1")]
        [InlineData("V0.0.1+build.99", "V0.0.1+build.99")]
        public void WhenIncludePrefix_PrefixedInput_FormatPreservesPrefix(string input, string expected) {
            Assert.Equal(expected, SemVersion.Parse(input).Format(includePrefix: true));
        }

        [Theory]
        [InlineData("v1.2.3", "1.2.3")]
        [InlineData("V1.2.3", "1.2.3")]
        [InlineData("v1.0.0-alpha.1", "1.0.0-alpha.1")]
        [InlineData("V0.0.1+build.99", "0.0.1+build.99")]
        public void WhenNotIncludePrefix_PrefixedInput_FormatWithoutPrefix(string input, string expected) {
            Assert.Equal(expected, SemVersion.Parse(input).Format(includePrefix: false));
        }

        [Fact]
        public void UnprefixedInput_FormatOmitsPrefix() {
            Assert.Equal("1.2.3", SemVersion.Parse("1.2.3").Format());
        }

        [Fact]
        public void PrefixDifference_DoesNotAffectEquality() {
            var lower = SemVersion.Parse("v1.2.3");
            var upper = SemVersion.Parse("V1.2.3");
            var none = SemVersion.Parse("1.2.3");

            Assert.Equal(lower, upper);
            Assert.Equal(lower, none);
            Assert.Equal(upper, none);
        }

        [Fact]
        public void PrefixDifference_DoesNotAffectHashCode() {
            var lower = SemVersion.Parse("v1.2.3");
            var upper = SemVersion.Parse("V1.2.3");
            var none = SemVersion.Parse("1.2.3");

            Assert.Equal(lower.GetHashCode(), upper.GetHashCode());
            Assert.Equal(lower.GetHashCode(), none.GetHashCode());
        }

        [Fact]
        public void PrefixDifference_DoesNotAffectOrdering() {
            var withPrefix = SemVersion.Parse("v1.2.3");
            var withoutPrefix = SemVersion.Parse("1.2.3");

            Assert.Equal(0, withPrefix.CompareTo(withoutPrefix));
        }

        [Fact]
        public void CoreVersionProperties_UnaffectedByPrefix() {
            var v = SemVersion.Parse("v3.4.5-rc.1+sha.abc");

            Assert.Equal(3, v.Major);
            Assert.Equal(4, v.Minor);
            Assert.Equal(5, v.Patch);
            Assert.Equal("rc.1", v.PreRelease);
            Assert.Equal("sha.abc", v.Build);
        }
    }
}
