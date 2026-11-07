using Nameless.Testing.Tools.Attributes;
using Nameless.Windows.Navigation;
using Wpf.Ui.Controls;

namespace Nameless.Windows.Windows.Navigation;

[UnitTest]
public class NavigationViewItemAttributeTests {
    [Fact]
    public void SetProperties_Must_Return_Correct_Values() {
        // arrange & act
        var sut = new NavigationViewItemAttribute {
            Icon = SymbolRegular.AccessTime20,
            Title = "Test Title",
            ToolTip = "Test ToolTip",
            Position = 10,
            Placement = Placement.Footer,
            TitleResourceName = "Title Resource Name",
            ToolTipResourceName = "ToolTip Resource Name",
            ResourceType = typeof(NavigationViewItemAttributeTests)
        };

        // assert
        Assert.Multiple(
            () => Assert.Equal(SymbolRegular.AccessTime20, sut.Icon),
            () => Assert.Equal("Test Title", sut.Title),
            () => Assert.Equal("Test ToolTip", sut.ToolTip),
            () => Assert.Equal(10, sut.Position),
            () => Assert.Equal(Placement.Footer, sut.Placement),
            () => Assert.Equal("Title Resource Name", sut.TitleResourceName),
            () => Assert.Equal("ToolTip Resource Name", sut.ToolTipResourceName),
            () => Assert.Equal(typeof(NavigationViewItemAttributeTests), sut.ResourceType)
        );
    }

    [Fact]
    public void WhenTitleIsNotProvided_FallbackToResourceType() {
        const string ResourceName = "NavigationViewItemAttribute_Title";
        const string ResourceValue = "Title Okey-Dokey";

        // arrange & act
        var sut = new NavigationViewItemAttribute {
            Icon = SymbolRegular.AccessTime20,
            TitleResourceName = ResourceName,
            ResourceType = typeof(Resources.Resources)
        };

        // assert
        Assert.Multiple(
            () => Assert.Equal(ResourceValue, sut.Title),
            () => Assert.Equal(ResourceName, sut.TitleResourceName),
            () => Assert.Equal(typeof(Resources.Resources), sut.ResourceType)
        );
    }

    [Fact]
    public void WhenTitleIsNotProvided_WhenTitleResourceNameIsNotProvided_ThenThrowsException() {
        // arrange & act
        var sut = new NavigationViewItemAttribute {
            Icon = SymbolRegular.AccessTime20,
            ToolTip = "Test ToolTip",
            Position = 10,
            Placement = Placement.Footer,
            ToolTipResourceName = "ToolTip Resource Name",
            ResourceType = typeof(Resources.Resources)
        };

        // assert
        Assert.Throws<InvalidOperationException>(() => sut.Title);
    }

    [Fact]
    public void WhenTitleIsNotProvided_WhenTitleResourceNameIsProvided_WhenTitleResourceNameDoesNotMatches_ThenReturnsResourceName() {
        // arrange & act
        var sut = new NavigationViewItemAttribute {
            Icon = SymbolRegular.AccessTime20,
            ToolTip = "Test ToolTip",
            Position = 10,
            Placement = Placement.Footer,
            TitleResourceName = "Incorrect_Title_Resource_Name",
            ToolTipResourceName = "ToolTip Resource Name",
            ResourceType = typeof(Resources.Resources)
        };

        // assert
        Assert.Equal("Incorrect_Title_Resource_Name", sut.Title);
    }

    [Fact]
    public void WhenTitleIsNotProvided_WhenTitleResourceNameIsProvided_WhenResourceTypeIsNotProvided_ThenReturnsTitleResourceName() {
        const string ResourceName = "NavigationViewItemAttribute_Title";
        
        // arrange & act
        var sut = new NavigationViewItemAttribute {
            Icon = SymbolRegular.AccessTime20,
            TitleResourceName = ResourceName
        };

        // assert
        Assert.Multiple(
            () => Assert.Equal(ResourceName, sut.Title),
            () => Assert.Equal(ResourceName, sut.TitleResourceName),
            () => Assert.Null(sut.ResourceType)
        );
    }

    [Fact]
    public void WhenToolTipIsNotProvided_FallbackToResourceType() {
        const string ResourceName = "NavigationViewItemAttribute_ToolTip";
        const string ResourceValue = "ToolTip Okey-Dokey";

        // arrange & act
        var sut = new NavigationViewItemAttribute {
            Icon = SymbolRegular.AccessTime20,
            ToolTipResourceName = ResourceName,
            ResourceType = typeof(Resources.Resources)
        };

        // assert
        Assert.Multiple(
            () => Assert.Equal(ResourceValue, sut.ToolTip),
            () => Assert.Equal(ResourceName, sut.ToolTipResourceName),
            () => Assert.Equal(typeof(Resources.Resources), sut.ResourceType)
        );
    }

    [Fact]
    public void WhenToolTipIsNotProvided_WhenToolTipResourceNameIsNotProvided_ThenReturnsNull() {
        // arrange & act
        var sut = new NavigationViewItemAttribute {
            Icon = SymbolRegular.AccessTime20,
            ResourceType = typeof(Resources.Resources)
        };

        // assert
        Assert.Null(sut.ToolTip);
    }

    [Fact]
    public void WhenToolTipIsNotProvided_WhenToolTipResourceNameIsProvided_WhenToolTipResourceNameDoesNotMatches_ThenReturnsResourceName() {
        const string ResourceName = "Incorrect_ToolTip_Resource_Name";

        // arrange & act
        var sut = new NavigationViewItemAttribute {
            Icon = SymbolRegular.AccessTime20,
            ToolTipResourceName = ResourceName,
            ResourceType = typeof(Resources.Resources)
        };

        // assert
        Assert.Equal(ResourceName, sut.ToolTip);
    }

    [Fact]
    public void WhenToolTipIsNotProvided_WhenToolTipResourceNameIsProvided_WhenResourceTypeIsNotProvided_ThenReturnsToolTipResourceName() {
        const string ResourceName = "NavigationViewItemAttribute_ToolTip";

        // arrange & act
        var sut = new NavigationViewItemAttribute {
            Icon = SymbolRegular.AccessTime20,
            ToolTipResourceName = ResourceName
        };

        // assert
        Assert.Multiple(
            () => Assert.Equal(ResourceName, sut.ToolTip),
            () => Assert.Equal(ResourceName, sut.ToolTipResourceName),
            () => Assert.Null(sut.ResourceType)
        );
    }
}
