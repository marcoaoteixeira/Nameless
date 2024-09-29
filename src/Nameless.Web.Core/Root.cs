namespace Nameless.Web;

/// <summary>
/// The only purpose of this class is to be an "entrypoint" for this
/// assembly.
/// 
/// *** DO NOT IMPLEMENT IMPORTANT THINGS HERE ***
/// 
/// But, it's OK to use it as a repository for all constants or default
/// values that you'll use throughout this assembly or shared assemblies.
/// </summary>
public static class Root {
    public static class HttpRequestHeaders {
        public const string X_FORWARDED_FOR = "X-Forwarded-For";
    }

    public static class HttpResponseHeaders {
        public const string X_JWT_EXPIRED = "X-JWT-Expired";
    }

    public static class HttpMethods {
        public const string GET = nameof(GET);
        public const string POST = nameof(POST);
        public const string DELETE = nameof(DELETE);
        public const string PUT = nameof(PUT);
        public const string PATCH = nameof(PATCH);
    }

    public static class Defaults {
        internal const string JWT_SECRET = "VGhlIG1vb24sIGEgY2VsZXN0aWFsIHBvZXQncyBwZWFybCwgYmF0aGVzIHRoZSBuaWdodCBjYW52YXMgaW4gYW4gZXRoZXJlYWwgZ2xvdywgd2hpc3BlcmluZyBhbmNpZW50IHNlY3JldHMgdG8gdGhlIHN0YXJnYXplcidzIHNvdWwsIGFuIGV0ZXJuYWwgZGFuY2Ugb2YgbGlnaHQgdGhhdCB3ZWF2ZXMgZHJlYW1zIGFjcm9zcyB0aGUgY29zbWljIHRhcGVzdHJ5Lg==";
    }
}