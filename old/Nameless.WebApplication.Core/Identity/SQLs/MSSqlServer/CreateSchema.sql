-- Tables
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE [name] = 'Users')
    CREATE TABLE Users (
        ID                      UNIQUEIDENTIFIER    NOT NULL,
        Email                   NVARCHAR (256)      NOT NULL,
        EmailConfirmed          BIT                 NOT NULL    DEFAULT 0,
        NormalizedEmail         NVARCHAR (256)      NULL,
        UserName                NVARCHAR (256)      NOT NULL,
        NormalizedUserName      NVARCHAR (256)      NULL,
        PhoneNumber             NVARCHAR (256)      NULL,
        PhoneNumberConfirmed    BIT                 NOT NULL    DEFAULT 0,
        LockoutEnabled          BIT                 NOT NULL    DEFAULT 0,
        LockoutEnd              DATETIMEOFFSET      NULL,
        AccessFailedCount       INT                 NOT NULL    DEFAULT 0,
        PasswordHash            NVARCHAR (256)      NULL,
        SecurityStamp           NVARCHAR (256)      NULL,
        TwoFactorEnabled        BIT                 NOT NULL    DEFAULT 0,
		AvatarUrl               NVARCHAR (2048)     NULL,

        CONSTRAINT PK_Users PRIMARY KEY NONCLUSTERED (ID),

        CONSTRAINT UQ_Email UNIQUE (Email)
    );

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE [name] = 'UserClaims')
    CREATE TABLE UserClaims (
        UserID  UNIQUEIDENTIFIER    NOT NULL,
        Type    NVARCHAR (256)      NOT NULL,
        Value   NVARCHAR (1024)     NULL,

        CONSTRAINT UQ_UserID_Type UNIQUE (UserID, Type)
    );

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE [name] = 'UserLogins')
    CREATE TABLE UserLogins (
        UserID              UNIQUEIDENTIFIER    NOT NULL,
        ProviderKey         VARCHAR (256)       NOT NULL,
        LoginProvider       VARCHAR (256)       NOT NULL,
        ProviderDisplayName NVARCHAR (1024)     NULL,

        CONSTRAINT UQ_UserID_ProviderKey_LoginProvider UNIQUE (UserID, ProviderKey, LoginProvider)
    );

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE [name] = 'UserTokens')
    CREATE TABLE UserTokens (
        UserID          UNIQUEIDENTIFIER    NOT NULL,
        LoginProvider   VARCHAR (256)       NOT NULL,
        Name            NVARCHAR (256)      NOT NULL,
        Value           NVARCHAR (1024)     NULL,

        CONSTRAINT UQ_UserID_LoginProvider_Name UNIQUE (UserID, LoginProvider, Name)
    );

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE [name] = 'Roles')
    CREATE TABLE Roles (
        ID                  UNIQUEIDENTIFIER    NOT NULL,
        Name                NVARCHAR (256)      NOT NULL,
        NormalizedName      NVARCHAR (256)      NULL,

        CONSTRAINT PK_Roles PRIMARY KEY NONCLUSTERED (ID),

        CONSTRAINT UQ_Name UNIQUE (Name)
    );

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE [name] = 'RoleClaims')
    CREATE TABLE RoleClaims (
        RoleID  UNIQUEIDENTIFIER    NOT NULL,
        Type    NVARCHAR (256)      NOT NULL,
        Value   NVARCHAR (1024)     NULL,

        CONSTRAINT UQ_RoleID_Type UNIQUE (RoleID, Type)
    );

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE [name] = 'UsersInRoles')
    CREATE TABLE UsersInRoles (
        UserID  UNIQUEIDENTIFIER    NOT NULL,
        RoleID  UNIQUEIDENTIFIER    NOT NULL,

        CONSTRAINT UQ_UserID_RoleID UNIQUE (UserID, RoleID)
    );

-- Foreign Keys
IF EXISTS (SELECT 1 FROM sys.tables WHERE [name] = 'UserClaims')
    ALTER TABLE UserClaims
        ADD CONSTRAINT FK_UserClaims_To_Users FOREIGN KEY (UserID) REFERENCES Users (ID) ON DELETE CASCADE;

IF EXISTS (SELECT 1 FROM sys.tables WHERE [name] = 'UserLogins')
    ALTER TABLE UserLogins
        ADD CONSTRAINT FK_UserLogins_To_Users FOREIGN KEY (UserID) REFERENCES Users (ID) ON DELETE CASCADE;

IF EXISTS (SELECT 1 FROM sys.tables WHERE [name] = 'UserTokens')
    ALTER TABLE UserTokens
        ADD CONSTRAINT FK_UserTokens_To_Users FOREIGN KEY (UserID) REFERENCES Users (ID) ON DELETE CASCADE;

IF EXISTS (SELECT 1 FROM sys.tables WHERE [name] = 'RoleClaims')
    ALTER TABLE RoleClaims
        ADD CONSTRAINT FK_RoleClaims_To_Roles FOREIGN KEY (RoleID) REFERENCES Roles (ID) ON DELETE CASCADE;

IF EXISTS (SELECT 1 FROM sys.tables WHERE [name] = 'UsersInRoles')
BEGIN
    ALTER TABLE UsersInRoles
        ADD CONSTRAINT FK_UsersInRoles_To_Users FOREIGN KEY (UserID) REFERENCES Users (ID);

    ALTER TABLE UsersInRoles
        ADD CONSTRAINT FK_UsersInRoles_To_Roles FOREIGN KEY (RoleID) REFERENCES Roles (ID);
END

-- Indexes
IF EXISTS (SELECT 1 FROM sys.tables WHERE [name] = 'Users')
    CREATE INDEX IDX_Users_UserName     ON Users (UserName);

IF EXISTS (SELECT 1 FROM sys.tables WHERE [name] = 'UserClaims')
    CREATE INDEX IDX_UserClaims_Type    ON UserClaims (Type);

IF EXISTS (SELECT 1 FROM sys.tables WHERE [name] = 'RoleClaims')
    CREATE INDEX IDX_RoleClaims_Type    ON RoleClaims (Type);