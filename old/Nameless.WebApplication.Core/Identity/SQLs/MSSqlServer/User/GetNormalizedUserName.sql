SELECT
    NormalizedUserName
FROM Users (NOLOCK)
WHERE
    ID = @ID