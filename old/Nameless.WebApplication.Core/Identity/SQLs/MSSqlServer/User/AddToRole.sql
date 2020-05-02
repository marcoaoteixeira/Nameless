DECLARE @RoleID UNIQUEIDENTIFIER

SELECT
    @RoleID = ID
FROM Roles (NOLOCK)
WHERE
    Name = @RoleName

IF @RoleID IS NULL
    RAISERROR ('Role not found.', 16, 1)

IF NOT EXISTS (SELECT 1 FROM UsersInRoles (NOLOCK) WHERE UserID = @UserID AND RoleID = @RoleID)
    INSERT INTO UsersInRoles VALUES (@UserID, @RoleID)