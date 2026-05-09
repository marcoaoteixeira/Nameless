namespace Nameless.Web.Http.Endpoints.Attributes;

public sealed class UseAntiforgeryAttributeTests
{
    [Fact]
    public void UseAntiforgeryAttribute_WhenCreated_ThenInstantiates()
    {
        var attr = new UseAntiforgeryAttribute();

        Assert.NotNull(attr);
    }

    [Fact]
    public void UseAntiforgeryAttribute_WhenInspected_ThenAllowMultipleIsFalse()
    {
        var usage = (AttributeUsageAttribute)typeof(UseAntiforgeryAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .Single();

        Assert.False(usage.AllowMultiple);
    }

    [Fact]
    public void UseAntiforgeryAttribute_WhenInspected_ThenTargetsClass()
    {
        var usage = (AttributeUsageAttribute)typeof(UseAntiforgeryAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .Single();

        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Class));
    }
}
