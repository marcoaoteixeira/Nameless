DECLARE @Result BIT

SELECT
    @Result = TwoFactorEnabled
FROM Users (NOLOCK)
WHERE
    ID = @ID

SELECT CAST (ISNULL (@Result, 0) AS BIT) AS Result