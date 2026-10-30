using Nameless.Generators.Emitters;
using Nameless.Generators.Infrastructure;

namespace Nameless.Web.Generators.Http.Endpoints.Emitters;

public abstract class ClassEmitter<TModel> : ClassEmitterBase<TModel> where TModel : IClassEmitModel {
    protected override void WriteNamespaceUsings(CodeWriter cw, TModel model) {
        cw.WriteLine("using global::Asp.Versioning;");
        cw.WriteLine("using global::Asp.Versioning.ApiExplorer;");

        cw.WriteLine("using global::Microsoft.AspNetCore.Authorization;");
        cw.WriteLine("using global::Microsoft.AspNetCore.Builder;");
        cw.WriteLine("using global::Microsoft.AspNetCore.Cors;");
        cw.WriteLine("using global::Microsoft.AspNetCore.Http;");
        cw.WriteLine("using global::Microsoft.AspNetCore.Http.Timeouts;");
        cw.WriteLine("using global::Microsoft.AspNetCore.Mvc;");
        cw.WriteLine("using global::Microsoft.AspNetCore.OutputCaching;");
        cw.WriteLine("using global::Microsoft.AspNetCore.RateLimiting;");
        cw.WriteLine("using global::Microsoft.AspNetCore.Routing;");
        cw.WriteLine("using global::Microsoft.OpenApi;");

        cw.WriteLine("using global::Nameless.Web;");
        cw.WriteLine("using global::Nameless.Web.Filters.Validation;");
        cw.WriteLine($"using global::{Project.Namespaces.Attributes.Versioning};");
        
        base.WriteNamespaceUsings(cw, model);
    }
}
