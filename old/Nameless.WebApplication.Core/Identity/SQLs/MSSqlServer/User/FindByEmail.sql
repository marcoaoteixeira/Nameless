SELECT
    ID,
    Email,
    EmailConfirmed,
    NormalizedEmail,
    UserName,
    NormalizedUserName,
    PhoneNumber,
    PhoneNumberConfirmed,
    LockoutEnabled,
    LockoutEnd,
    AccessFailedCount,
    PasswordHash,
    SecurityStamp,
    TwoFactorEnabled
FROM Users (NOLOCK)
WHERE
    NormalizedEmail = @NormalizedEmail