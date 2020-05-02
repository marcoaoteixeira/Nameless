UPDATE Users SET
    AccessFailedCount = 0
WHERE
    ID = @ID