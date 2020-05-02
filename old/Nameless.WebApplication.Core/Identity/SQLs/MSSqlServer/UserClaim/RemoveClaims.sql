DELETE FROM UserClaims
WHERE
    UserID = @UserID
AND Type = @Type