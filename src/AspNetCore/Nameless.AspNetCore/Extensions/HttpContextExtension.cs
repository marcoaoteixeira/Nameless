using Microsoft.AspNetCore.Http;

namespace Nameless.AspNetCore {

    public static class HttpContextExtension {

        #region Public Static Methods

        public static string GetIpAddress(this HttpContext? self) {
            if (self == default) { return string.Empty; }

            if (self.Request.Headers.ContainsKey("X-Forwarded-For")) {
                return self.Request.Headers["X-Forwarded-For"].ToString();
            }

            return self.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? string.Empty;
        }

        #endregion
    }
}
