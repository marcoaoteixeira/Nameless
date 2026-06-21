using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Http.Endpoints.Infrastructure;

namespace Nameless.Web.Http.Endpoints.UseCases;

[UnitTest]
public class SimpleEndpointGroup {
    [Fact]
    public void Generate_WithSimpleEndpointGroup_ThenEmitsCorrectCode() {
        var raw = SourceCodeHelper.Write("""
                                         [EndpointGroup(prefix: "/api", Tags = "SimpleGroup")]
                                         public partial class SimpleGroupEndpointGroup;
                                         
                                         [Endpoint(Group = typeof(SimpleGroupEndpointGroup))]
                                         public partial class SimpleEndpoint {
                                            public Task<IResult> HandleAsync() {
                                                throw new NotImplementedException();
                                            }
                                         }
                                         """);

        var code = GeneratorTestHelper.GetGeneratedSource(raw);

        Assert.Contains("SimpleGroup", code);
    }
}
