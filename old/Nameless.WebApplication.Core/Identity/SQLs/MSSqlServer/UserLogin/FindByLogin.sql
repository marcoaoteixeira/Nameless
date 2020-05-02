SELECT
    Users.ID,
    Users.Email,
    Users.EmailConfirmed,
    Users.NormalizedEmail,
    Users.UserName,
    Users.NormalizedUserName,
    Users.PhoneNumber,
    Users.PhoneNumberConfirmed,
    Users.LockoutEnabled,
    Users.LockoutEnd,
    Users.AccessFailedCount,
    Users.PasswordHash,
    Users.SecurityStamp,
    Users.TwoFactorEnabled
FROM Users (NOLOCK)
    INNER JOIN UserLogins (NOLOCK) ON UserLogins.UserID = Users.ID
WHERE
    UserLogins.ProviderKey = @ProviderKey
AND UserLogins.LoginProvider = @LoginProvider