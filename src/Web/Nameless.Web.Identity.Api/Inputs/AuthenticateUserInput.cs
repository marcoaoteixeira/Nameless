﻿using System.Text.Json.Serialization;
using Nameless.Validation.Abstractions;

namespace Nameless.Web.Identity.Api.Inputs {
    [Validate]
    public sealed record AuthenticateUserInput {
        #region Public Properties

        [JsonPropertyName("username")]
        public string Username { get; init; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; init; } = string.Empty;

        #endregion
    }
}
