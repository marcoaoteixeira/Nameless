using System;
using System.Security.Cryptography;

namespace Nameless.WebApplication.Auth {
    public class TokenGenerator : ITokenGenerator {
        #region ITokenGenerator Members

        public string Generate (int size = 32) {
            var randomNumber = new byte[size];
            using (var rng = RandomNumberGenerator.Create ()) {
                rng.GetBytes (randomNumber);
                return Convert.ToBase64String (randomNumber);
            }
        }

        #endregion
    }
}