DECLARE @RoleID UNIQUEIDENTIFIER

SELECT
    @RoleID = ID
FROM Roles (NOLOCK)
WHERE
    Name = @RoleName

IF @RoleID IS NULL
    RAISERROR ('Role not found.', 16, 1)

DELETE FROM UsersInRoles
WHERE
    UserID = @UserID
AND RoleID = @RoleID