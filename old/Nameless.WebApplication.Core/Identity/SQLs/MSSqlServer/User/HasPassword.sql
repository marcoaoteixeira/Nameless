DECLARE @Result BIT

SELECT
    @Result = CASE
        WHEN NOT PasswordHash IS NULL THEN 1
        ELSE 0
    END
FROM Users (NOLOCK)
WHERE
    ID = @ID

SELECT CAST (ISNULL (@Result, 0) AS BIT) AS Result