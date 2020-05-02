DELETE FROM UsersInRoles
WHERE
    UserID = @ID;

DELETE FROM UserClaims
WHERE
    UserID = @ID;

DELETE FROM UserLogins
WHERE
    UserID = @ID;

DELETE FROM UserTokens
WHERE
    UserID = @ID;

DELETE FROM Users
WHERE
    ID = @ID;