SELECT
    UserID,
    Type,
    Value
FROM UserClaims (NOLOCK)
WHERE
    UserID = @UserID