using System.Reflection;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Web.Http.Endpoints.Attributes.Authorization;

[UnitTest]
public sealed class UseAuthorizationAttributeTests {
    [Fact]
    public void WhenPolicyNameIsSet_ThenPolicyNameIsStored() {
        var attr = new UseAuthorizationAttribute { PolicyName = "AdminPolicy" };

        Assert.Equal("AdminPolicy", attr.PolicyName);
    }

    [Fact]
    public void WhenPolicyNameIsNotSet_ThenPolicyNameIsNull() {
        var attr = new UseAuthorizationAttribute();

        Assert.Null(attr.PolicyName);
    }

    [Fact]
    public void WhenRolesIsSet_ThenRolesIsStored() {
        var attr = new UseAuthorizationAttribute { Roles = "Admin,User" };

        Assert.Equal("Admin,User", attr.Roles);
    }

    [Fact]
    public void WhenRolesIsNotSet_ThenRolesIsNull() {
        var attr = new UseAuthorizationAttribute();

        Assert.Null(attr.Roles);
    }

    [Fact]
    public void WhenAuthenticationSchemesIsSet_ThenAuthenticationSchemesIsStored() {
        var attr = new UseAuthorizationAttribute { AuthenticationSchemes = "Bearer,Cookie" };

        Assert.Equal("Bearer,Cookie", attr.AuthenticationSchemes);
    }

    [Fact]
    public void WhenAuthenticationSchemesIsNotSet_ThenAuthenticationSchemesIsNull() {
        var attr = new UseAuthorizationAttribute();

        Assert.Null(attr.AuthenticationSchemes);
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenAllowMultipleIsTrue() {
        var attr = typeof(UseAuthorizationAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.True(usage.AllowMultiple);
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenTargetsClass() {
        var attr = typeof(UseAuthorizationAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Class));
    }

    [Fact]
    public void WhenInspectingAttributeProperties_ThenInheritedIsFalse() {
        var attr = typeof(UseAuthorizationAttribute);
        var usage = attr.GetCustomAttribute<AttributeUsageAttribute>(inherit: false);

        Assert.NotNull(usage);
        Assert.False(usage.Inherited);
    }
}
