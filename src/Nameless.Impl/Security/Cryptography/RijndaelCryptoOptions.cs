using Nameless.Attributes;

namespace Nameless.Security.Cryptography;

/// <summary>
/// Options for configuring security features.
/// </summary>
[ConfigurationSectionName("RijndaelCrypto")]
public class RijndaelCryptoOptions {
    private const string DEFAULT_PASS_PHRASE = "29850952b3ef9f90";
    private const string DEFAULT_VECTOR_INIT = "9e209040c863f84a";
    private const string DEFAULT_SALT = "c11083b4b0a7743af748c85d343dfee9fbb8b2576c05f3a7f0d632b0926aadfc2cf24dba5fb0a30e26e83b2ac5b9e29e1b161e5c1fa7425e73043362938b982408eac03b80adc33dc7d8fbe44b7c7b05d3a2c511166bdb43fcb710b03ba919e79e209040c863f84a31e719795b2577523954739fe5ed3b58a75cff2127075ed1e4ba5cbd251c98e6cd1c23f126a3b81d8d8328abc95387229850952b3ef9f904d1d3ec2e6f20fd420d50e2642992841d8338a314b8ea157c9e18477aaef226ab5206b8b8a996cf5320cb12ca91c7b790fba9f030408efe83ebb83548dc3007bda49670c3c18b9e079b9cfaf51634f563dc8ae3070db2c4a8544305df1b60f007";
    private const int DEFAULT_SALT_MIN_SIZE = 16;
    private const int DEFAULT_SALT_MAX_SIZE = 128;
    private const int DEFAULT_KEY_ITERATIONS_COUNT = 1;

    private const int MIN_ALLOWED_SALT_SIZE = 4;
    private const int MAX_ALLOWED_SALT_SIZE = 255;
    private const int MIN_KEY_ITERATIONS_COUNT = 1;

    /// <summary>
    ///     Gets or sets the pass phrase.
    /// </summary>
    /// <remarks>
    ///     Passphrase from which a pseudo-random password will be derived.
    ///     The derived password will be used to generate the encryption key
    ///     Passphrase can be any string. Passphrase value must be kept in
    ///     secret.
    /// </remarks>
    public string Passphrase { get; set; } = DEFAULT_PASS_PHRASE;

    /// <summary>
    ///     Gets or sets the initialization vector.
    /// </summary>
    /// <remarks>
    ///     Initialization vector (IV). This value is required to encrypt the
    ///     first block of plaintext data. For RijndaelManaged class IV must be
    ///     exactly 16 ASCII characters long. IV value does not have to be kept
    ///     in secret.
    /// </remarks>
    public string Iv { get; set; } = DEFAULT_VECTOR_INIT;

    /// <summary>
    ///     Gets or sets the salt.
    /// </summary>
    /// <remarks>
    ///     Salt value used for password hashing during key generation. This is
    ///     not the same as the salt we will use during encryption. This parameter
    ///     can be any string.
    /// </remarks>
    public string Salt { get; set; } = DEFAULT_SALT;

    /// <summary>
    ///     Gets or sets the key size.
    /// </summary>
    /// <remarks>
    ///     If key size is not specified, longest 256-bit key will be used.
    /// </remarks>
    public KeySize KeySize { get; set; }

    /// <summary>
    ///     Gets or sets the minimum salt size. This value should not be
    ///     smaller than <c>4 bytes</c>, because we use the first
    ///     <c>4 bytes</c> of salt to store its length.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     if the value is less than <c>4</c> or greater than <c>255</c>.
    /// </exception>
    public int MinimumSaltSize {
        get;
        set => field = Throws.When.OutOfRange(
            value,
            MIN_ALLOWED_SALT_SIZE,
            MAX_ALLOWED_SALT_SIZE
        );
    } = DEFAULT_SALT_MIN_SIZE;

    /// <summary>
    ///     Gets or sets the maximum salt size. This value should not be
    ///     longer than <c>255 bytes</c>, because we have only 1 byte to store
    ///     its length.
    /// </summary>
    /// <remarks>
    ///     This value should not be greater than <c>255 bytes</c>, because
    ///     we use the first <c>4 bytes</c> of salt to store its length.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     if the value is less than <c>4</c> or greater than <c>255</c>.
    /// </exception>
    public int MaximumSaltSize {
        get;
        set => field = Throws.When.OutOfRange(
            value,
            MIN_ALLOWED_SALT_SIZE,
            MAX_ALLOWED_SALT_SIZE
        );
    } = DEFAULT_SALT_MAX_SIZE;

    /// <summary>
    ///     Gets or sets the number of key iterations used to hash the crypto
    ///     password. More iterations are considered more secure but may take
    ///     longer. Minimum of <c>1</c>.
    /// </summary>
    /// <exception cref="ArgumentException">
    ///     if the value is less than <c>1</c>.
    /// </exception>
    public int KeyIterationsCount {
        get;
        set => field = Throws.When.LowerThan(value, MIN_KEY_ITERATIONS_COUNT);
    } = DEFAULT_KEY_ITERATIONS_COUNT;
}