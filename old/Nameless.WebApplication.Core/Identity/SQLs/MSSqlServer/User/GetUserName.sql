SELECT
    UserName
FROM Users (NOLOCK)
WHERE
    Email = @Email;