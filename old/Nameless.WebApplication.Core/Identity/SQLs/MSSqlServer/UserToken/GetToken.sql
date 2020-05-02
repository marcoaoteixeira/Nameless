SELECT
    Value
FROM UserTokens (NOLOCK)
WHERE
    UserID = @UserID
AND LoginProvider = @LoginProvider
AND Name = @Name;