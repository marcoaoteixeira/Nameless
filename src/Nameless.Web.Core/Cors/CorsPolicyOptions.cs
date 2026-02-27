namespace Nameless.Web.Cors;

public class CorsPolicyOptions {
    public static CorsPolicyEntry[] Defaults { get; } = [
        CorsPolicyEntry.AllowEverything
    ];

    public bool UseDefaultPolicies { get; set; } = true;

    public CorsPolicyEntry[] Entries { get; set; } = [];
}