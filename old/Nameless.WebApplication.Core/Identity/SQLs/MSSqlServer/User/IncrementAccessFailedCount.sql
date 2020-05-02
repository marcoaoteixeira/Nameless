UPDATE Users SET
    AccessFailedCount = (ISNULL (AccessFailedCount, 0) + 1)
WHERE
    ID = @ID;

SELECT
    AccessFailedCount
FROM Users (NOLOCK)
WHERE
    ID = @ID;