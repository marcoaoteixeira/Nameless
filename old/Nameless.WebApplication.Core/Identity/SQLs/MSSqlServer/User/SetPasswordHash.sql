UPDATE Users SET
    PasswordHash = @PasswordHash
WHERE
    ID = @ID