DELETE FROM UserTokens
WHERE
    UserID = @UserID
AND LoginProvider = @LoginProvider
AND Name = @Name;