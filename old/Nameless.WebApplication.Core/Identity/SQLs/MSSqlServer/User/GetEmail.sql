SELECT
    Email
FROM Users (NOLOCK)
WHERE
    ID = @ID