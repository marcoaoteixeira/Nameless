﻿using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Nameless.Services;

public interface IPluralizationRuleProvider {
    /// <summary>
    /// Tries to retrieve a valid pluralization delegate for the culture.
    /// </summary>
    /// <param name="culture">The culture</param>
    /// <param name="rule">The output pluralization rule delegate.</param>
    /// <returns><c>true</c> if pluralization rule was found; otherwise; <c>false</c>.</returns>
    bool TryGet(CultureInfo culture, [NotNullWhen(returnValue: true)] out PluralizationRuleDelegate? rule);
}