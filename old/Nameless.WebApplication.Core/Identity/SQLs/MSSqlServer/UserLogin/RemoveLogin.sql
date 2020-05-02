DELETE FROM UserLogins
WHERE
    UserID = @UserID
AND LoginProvider = @LoginProvider
AND ProviderKey = @ProviderKey