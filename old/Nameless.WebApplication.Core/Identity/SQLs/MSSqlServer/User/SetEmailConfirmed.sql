UPDATE Users SET
    EmailConfirmed = @EmailConfirmed
WHERE
    ID = @ID