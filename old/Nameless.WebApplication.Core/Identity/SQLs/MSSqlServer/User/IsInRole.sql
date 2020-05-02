DECLARE @RoleID UNIQUEIDENTIFIER

SELECT
    @RoleID = ID
FROM Roles (NOLOCK)
WHERE
    Name = @RoleName

IF @RoleID IS NULL
    RAISERROR ('Role not found.', 16, 1)

DECLARE @Result BIT
SELECT
    @Result = 1
FROM UsersInRoles (NOLOCK)
WHERE
    UserID = @UserID
AND RoleID = @RoleID
SELECT CAST (ISNULL (@Result, 0) AS BIT) AS Result