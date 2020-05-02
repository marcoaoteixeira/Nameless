SELECT
    ID
FROM Users (NOLOCK)
WHERE
    Email = @Email;