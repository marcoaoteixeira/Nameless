DECLARE @Result DATETIME

SELECT
    @Result = LockoutEnd
FROM Users (NOLOCK)
WHERE
    ID = @ID

SELECT @Result AS Result