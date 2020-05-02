DECLARE @Result NVARCHAR (256)

SELECT
    @Result = PhoneNumber
FROM Users (NOLOCK)
WHERE
    ID = @ID

SELECT @Result AS Result