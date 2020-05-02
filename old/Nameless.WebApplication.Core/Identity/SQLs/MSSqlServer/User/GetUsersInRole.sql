DECLARE @RoleID UNIQUEIDENTIFIER

SELECT
    @RoleID = ID
FROM Roles (NOLOCK)
WHERE
    Name = @RoleName

IF @RoleID IS NULL
    RAISERROR ('Role not found.', 16, 1)

SELECT
    Users.ID,
    Users.Email,
    Users.EmailConfirmed,
    Users.NormalizedEmail,
    Users.UserName,
    Users.NormalizedUserName,
    Users.PhoneNumber,
    Users.PhoneNumberConfirmed,
    Users.LockoutEnabled,
    Users.LockoutEnd,
    Users.AccessFailedCount,
    Users.PasswordHash,
    Users.SecurityStamp,
    Users.TwoFactorEnabled
FROM Users (NOLOCK)
    INNER JOIN UsersInRoles (NOLOCK) ON UsersInRoles.UserID = Users.ID
WHERE
    UsersInRoles.RoleID = @RoleID