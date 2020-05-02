DECLARE @Result NVARCHAR (256)

SELECT
    @Result = PasswordHash
FROM Users (NOLOCK)
WHERE
    ID = @ID

SELECT @Result AS Result