namespace Nameless.Security.Crypto {
    /// <summary>
    /// Encryption key sizes.
    /// </summary>
    public enum KeySize {
        /// <summary>
        /// None
        /// </summary>
        None = 0,

        /// <summary>
        /// 128 bits.
        /// </summary>
        Low = 128,

        /// <summary>
        /// 192 bits.
        /// </summary>
        Medium = 192,

        /// <summary>
        /// 256 bits.
        /// </summary>
        Large = 256
    }
}
