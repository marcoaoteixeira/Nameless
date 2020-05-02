IF EXISTS (SELECT 1 FROM sys.tables WHERE [name] = 'UserClaims')
    DROP TABLE UserClaims;

IF EXISTS (SELECT 1 FROM sys.tables WHERE [name] = 'UserLogins')
    DROP TABLE UserLogins;

IF EXISTS (SELECT 1 FROM sys.tables WHERE [name] = 'UserTokens')
    DROP TABLE UserTokens;

IF EXISTS (SELECT 1 FROM sys.tables WHERE [name] = 'UsersInRoles')
    DROP TABLE UsersInRoles;

IF EXISTS (SELECT 1 FROM sys.tables WHERE [name] = 'RoleClaims')
    DROP TABLE RoleClaims;

IF EXISTS (SELECT 1 FROM sys.tables WHERE [name] = 'Users')
    DROP TABLE Users;

IF EXISTS (SELECT 1 FROM sys.tables WHERE [name] = 'Roles')
    DROP TABLE Roles;