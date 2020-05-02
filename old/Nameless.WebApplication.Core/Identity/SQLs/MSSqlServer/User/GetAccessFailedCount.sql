DECLARE @Result INT

SELECT
    @Result = AccessFailedCount
FROM Users (NOLOCK)
WHERE
    ID = @ID

SELECT ISNULL (@Result, 0) AS Result