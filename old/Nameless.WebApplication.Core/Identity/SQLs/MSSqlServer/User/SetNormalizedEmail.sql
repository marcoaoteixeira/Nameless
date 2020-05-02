UPDATE Users SET
    NormalizedEmail = @NormalizedEmail
WHERE
    ID = @ID