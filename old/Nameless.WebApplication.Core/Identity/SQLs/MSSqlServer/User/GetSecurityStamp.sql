DECLARE @Result NVARCHAR (256)

SELECT
    @Result = SecurityStamp
FROM Users (NOLOCK)
WHERE
    ID = @ID

SELECT @Result AS Result