namespace Nameless.Web.Http.Endpoints.Attributes;

public sealed class GroupAttributeTests
{
    [Fact]
    public void GroupAttribute_WhenCreated_ThenNameAndPrefixAreStored()
    {
        var attr = new GroupAttribute("Users", "/api/users");

        Assert.Equal("Users", attr.Name);
        Assert.Equal("/api/users", attr.Prefix);
    }

    [Fact]
    public void GroupAttribute_WhenInspected_ThenAllowMultipleIsFalse()
    {
        var usage = (AttributeUsageAttribute)typeof(GroupAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .Single();

        Assert.False(usage.AllowMultiple);
    }

    [Fact]
    public void GroupAttribute_WhenInspected_ThenTargetsClass()
    {
        var usage = (AttributeUsageAttribute)typeof(GroupAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .Single();

        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Class));
    }
}
