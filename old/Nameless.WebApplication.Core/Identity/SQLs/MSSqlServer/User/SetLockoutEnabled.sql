UPDATE Users SET
    LockoutEnabled = @LockoutEnabled
WHERE
    ID = @ID