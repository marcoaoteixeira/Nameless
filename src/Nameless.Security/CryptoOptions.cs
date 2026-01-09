namespace Nameless.Security;

/// <summary>
/// Options for configuring security features.
/// </summary>
public class CryptoOptions {
    public const int MINIMUM_ALLOWED_SALT_SIZE = 4;
    public const int MAXIMUM_ALLOWED_SALT_SIZE = 255;
    public const int MINIMUM_PASSWORD_ITERATIONS = 1;

    /// <summary>
    ///     Gets or sets the pass phrase.
    /// </summary>
    /// <remarks>
    ///     Passphrase from which a pseudo-random password will be derived.
    ///     The derived password will be used to generate the encryption key
    ///     Passphrase can be any string. Passphrase value must be kept in
    ///     secret.
    /// </remarks>
    public string Passphrase { get; set; } = Defaults.RIJNDAEL_PASS_PHRASE;

    /// <summary>
    ///     Gets or sets the initialization vector.
    /// </summary>
    /// <remarks>
    ///     Initialization vector (IV). This value is required to encrypt the
    ///     first block of plaintext data. For RijndaelManaged class IV must be
    ///     exactly 16 ASCII characters long. IV value does not have to be kept
    ///     in secret.
    /// </remarks>
    public string Iv { get; set; } = Defaults.RIJNDAEL_IV;

    /// <summary>
    ///     Gets or sets the salt.
    /// </summary>
    /// <remarks>
    ///     Salt value used for password hashing during key generation. This is
    ///     not the same as the salt we will use during encryption. This parameter
    ///     can be any string.
    /// </remarks>
    public string Salt { get; set; } = Defaults.RIJNDAEL_IV;

    /// <summary>
    ///     Gets or sets the key size.
    /// </summary>
    /// <remarks>
    ///     If key size is not specified, longest 256-bit key will be used.
    /// </remarks>
    public KeySize KeySize { get; set; }

    /// <summary>
    ///     Gets or sets the minimum salt size. This value should not be
    ///     smaller than 4 bytes, because we use the first 4 bytes of salt
    ///     to store its length.
    /// </summary>
    /// <remarks>
    ///     This value should not be smaller than
    ///     <see cref="MINIMUM_ALLOWED_SALT_SIZE"/> bytes, because we use the
    ///     first 4 bytes of salt to store its length.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     if the value is less than <see cref="MINIMUM_ALLOWED_SALT_SIZE"/> or
    ///     greater than <see cref="MAXIMUM_ALLOWED_SALT_SIZE"/>.
    /// </exception>
    public int MinimumSaltSize {
        get;
        set => field = Guard.Against.OutOfRange(value, MINIMUM_ALLOWED_SALT_SIZE, MAXIMUM_ALLOWED_SALT_SIZE);
    } = 16;

    /// <summary>
    ///     Gets or sets the maximum salt size. This value should not be
    ///     longer than 255 bytes, because we have only 1 byte to store
    ///     its length.
    /// </summary>
    /// <remarks>
    ///     This value should not be greater than
    ///     <see cref="MAXIMUM_ALLOWED_SALT_SIZE"/> bytes, because we use the
    ///     first 4 bytes of salt to store its length.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     if the value is less than <see cref="MINIMUM_ALLOWED_SALT_SIZE"/> or
    ///     greater than <see cref="MAXIMUM_ALLOWED_SALT_SIZE"/>.
    /// </exception>
    public int MaximumSaltSize {
        get;
        set => field = Guard.Against.OutOfRange(value, MINIMUM_ALLOWED_SALT_SIZE, MAXIMUM_ALLOWED_SALT_SIZE);
    } = 128;

    /// <summary>
    ///     Gets or sets the password iterations. Number of iterations used
    ///     to hash password. More iterations are considered more secure but
    ///     may take longer. Minimum of 1.
    /// </summary>
    /// <exception cref="ArgumentException">
    ///     if the value is less than <see cref="MINIMUM_PASSWORD_ITERATIONS"/>.
    /// </exception>
    public int PasswordIterations {
        get;
        set => field = Guard.Against.LowerThan(value, MINIMUM_PASSWORD_ITERATIONS);
    } = 1;
}