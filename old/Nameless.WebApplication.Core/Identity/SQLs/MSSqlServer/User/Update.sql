UPDATE Users SET
    Email = @Email,
    EmailConfirmed = @EmailConfirmed,
    NormalizedEmail = @NormalizedEmail,
    UserName = @UserName,
    NormalizedUserName = @NormalizedUserName,
    PhoneNumber = @PhoneNumber,
    PhoneNumberConfirmed = @PhoneNumberConfirmed,
    LockoutEnabled = @LockoutEnabled,
    LockoutEnd = @LockoutEnd,
    AccessFailedCount = @AccessFailedCount,
    PasswordHash = @PasswordHash,
    SecurityStamp = @SecurityStamp,
    TwoFactorEnabled = @TwoFactorEnabled
WHERE
    ID = @ID;