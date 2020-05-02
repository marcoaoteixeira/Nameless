DECLARE @Exists BIT
SELECT
    @Exists = 1
FROM UserTokens (NOLOCK)
WHERE
    UserID = @UserID
AND LoginProvider = @LoginProvider
AND Name = @Name;

IF ISNULL (@Exists, 0) = 0
    INSERT INTO UserTokens VALUES (
        @UserID,
        @LoginProvider,
        @Name,
        @Value
    );
ELSE
    UPDATE UserTokens SET
        Value = @Value
    WHERE
        UserID = @UserID
    AND LoginProvider = @LoginProvider
    AND Name = @Name;