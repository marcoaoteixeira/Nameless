namespace Nameless.Security {
    public sealed record PasswordOptions {
        #region Public Static Read-Only Properties

        public static PasswordOptions Default => new();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the minimum length for the password.
        /// </summary>
        /// <remarks>Default is 6</remarks>
        public int MinLength { get; set; } = 6;
        /// <summary>
        /// Gets or sets the maximum length for the password.
        /// </summary>
        /// <remarks>Default is 12</remarks>
        public int MaxLength { get; set; } = 12;
        /// <summary>
        /// Gets or sets whether will use special chars.
        /// </summary>
        /// <remarks>Default are <c>*$-+?_&=!%{}/</c></remarks>
        public string Symbols { get; set; } = "*$-+?_&=!%{}/";
        /// <summary>
        /// Gets or sets whether will use numbers.
        /// </summary>
        /// <remarks>Default are <c>0123456789</c></remarks>
        public string Numerics { get; set; } = "0123456789";
        /// <summary>
        /// Gets or sets whether will use lower case chars.
        /// </summary>
        /// <remarks>Default are <c>abcdefgijkmnopqrstwxyz</c></remarks>
        public string LowerCases { get; set; } = "abcdefgijkmnopqrstwxyz";
        /// <summary>
        /// Gets or sets whether will use upper case chars.
        /// </summary>
        /// <remarks>Default are <c>ABCDEFGIJKMNOPQRSTWXYZ</c></remarks>
        public string UpperCases { get; set; } = "ABCDEFGIJKMNOPQRSTWXYZ";

        #endregion
    }
}
