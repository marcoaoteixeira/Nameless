using System.Text;

namespace Nameless.Generators.Web.Http.Endpoints;

public static class SourceCodeHelper
{
    public static readonly string Namespace = "Nameless.CodeGeneration";

    public static string Write(string code)
    {
        var sb = new StringBuilder();

        WriteUsingBlock(sb);
        WriteNamespace(sb);

        sb.Append(code);

        return sb.ToString();
    }

    private static void WriteUsingBlock(StringBuilder sb)
    {
        sb.AppendLine("using global::Nameless.Common.Http.Endpoints;");
        sb.AppendLine("using global::Nameless.Common.Http.Endpoints.Attributes;");
        sb.AppendLine("using global::Nameless.Common.Http.Endpoints.Attributes.Filtering;");
        sb.AppendLine("using global::Nameless.Common.Http.Endpoints.Attributes.OutputCache;");
        sb.AppendLine("using global::Nameless.Common.Http.Endpoints.Attributes.Produces;");
        sb.AppendLine("using global::Nameless.Common.Http.Endpoints.Attributes.Validation;");
        sb.AppendLine("using global::Nameless.Common.Http.Endpoints.Attributes.Versioning;");

        sb.AppendLine("using global::System;");
        sb.AppendLine("using global::System.Collections.Generic;");
        sb.AppendLine("using global::System.Globalization;");
        sb.AppendLine("using global::System.Threading.Tasks;");

        sb.AppendLine("using global::Asp.Versioning;");
        sb.AppendLine("using global::Asp.Versioning.ApiExplorer;");

        sb.AppendLine("using global::Microsoft.AspNetCore.Authorization;");
        sb.AppendLine("using global::Microsoft.AspNetCore.Builder;");
        sb.AppendLine("using global::Microsoft.AspNetCore.Cors;");
        sb.AppendLine("using global::Microsoft.AspNetCore.Http;");
        sb.AppendLine("using global::Microsoft.AspNetCore.Http.Timeouts;");
        sb.AppendLine("using global::Microsoft.AspNetCore.Mvc;");
        sb.AppendLine("using global::Microsoft.AspNetCore.OutputCaching;");
        sb.AppendLine("using global::Microsoft.AspNetCore.RateLimiting;");
        sb.AppendLine("using global::Microsoft.AspNetCore.Routing;");
        sb.AppendLine("using global::Microsoft.OpenApi;");

        sb.AppendLine("using global::Microsoft.Extensions.DependencyInjection;");
        sb.AppendLine("using global::Microsoft.Extensions.DependencyInjection.Extensions;");

        sb.AppendLine();
    }

    private static void WriteNamespace(StringBuilder sb)
    {
        sb.AppendLine($"namespace {Namespace};");
        sb.AppendLine();
    }
}
