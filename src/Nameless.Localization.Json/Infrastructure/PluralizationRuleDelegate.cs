namespace Nameless.Localization.Json.Infrastructure;

/// <summary>
/// Delegate representing a pluralization rule.
/// </summary>
/// <param name="count">The count.</param>
/// <returns>
/// Returns an integer representing the which pluralization form to use based on the count.
/// </returns>
public delegate int PluralizationRuleDelegate(int count);