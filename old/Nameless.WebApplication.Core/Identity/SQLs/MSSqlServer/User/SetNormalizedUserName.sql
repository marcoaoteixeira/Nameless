UPDATE Users SET
    NormalizedUserName = @NormalizedUserName
WHERE
    ID = @ID