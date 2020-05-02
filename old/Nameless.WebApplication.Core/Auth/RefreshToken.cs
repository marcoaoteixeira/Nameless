using System;

namespace Nameless.WebApplication.Auth {
    public sealed class RefreshToken {
        #region Public Properties

        public string Token { get; private set; }
        public DateTime Expires { get; private set; }
        public int UserId { get; private set; }
        public bool Active => DateTime.UtcNow <= Expires;
        public string RemoteIpAddress { get; private set; }

        #endregion

        #region Public Constructors

        public RefreshToken (string token, DateTime expires, int userId, string remoteIpAddress) {
            Token = token;
            Expires = expires;
            UserId = userId;
            RemoteIpAddress = remoteIpAddress;
        }

        #endregion
    }
}