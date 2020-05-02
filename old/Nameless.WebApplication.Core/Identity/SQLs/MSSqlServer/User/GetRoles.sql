SELECT
    Roles.Name
FROM Roles (NOLOCK)
    INNER JOIN UsersInRoles (NOLOCK) ON UsersInRoles.RoleID = Roles.ID
WHERE
    UsersInRoles.UserID = @UserID;