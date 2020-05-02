UPDATE Users SET
    LockoutEnd = @LockoutEnd
WHERE
    ID = @ID