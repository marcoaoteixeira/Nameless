SELECT
    NormalizedEmail
FROM Users (NOLOCK)
WHERE
    ID = @ID