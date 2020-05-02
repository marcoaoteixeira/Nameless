DECLARE @Result BIT

SELECT
    @Result = EmailConfirmed
FROM Users (NOLOCK)
WHERE
    ID = @ID

SELECT CAST (ISNULL (@Result, 0) AS BIT) AS Result