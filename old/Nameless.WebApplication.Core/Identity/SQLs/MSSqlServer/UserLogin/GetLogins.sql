SELECT
    LoginProvider,
    ProviderDisplayName,
    ProviderKey
FROM UserLogins (NOLOCK)
WHERE
    UserID = @UserID