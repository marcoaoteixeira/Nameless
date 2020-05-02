DELETE FROM UsersInRoles
WHERE
    RoleID = @ID;

DELETE FROM RoleClaims
WHERE
    RoleID = @ID;

DELETE FROM Roles
WHERE
    ID = @ID;