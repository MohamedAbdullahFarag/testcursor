-- Test-only DB adjustments: ensure Roles table has IsActive column used by repositories
IF OBJECT_ID('dbo.Roles') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Roles','IsActive') IS NULL
    BEGIN
        ALTER TABLE dbo.Roles ADD IsActive BIT NOT NULL CONSTRAINT DF_Roles_IsActive DEFAULT(1);
    END
END
ELSE
BEGIN
    CREATE TABLE dbo.Roles (
        RoleId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Code NVARCHAR(50) NOT NULL UNIQUE,
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Roles_IsActive DEFAULT(1),
        IsSystemRole BIT NOT NULL CONSTRAINT DF_Roles_IsSystemRole DEFAULT(0),
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Roles_CreatedAt DEFAULT(SYSUTCDATETIME()),
        CreatedBy INT NULL,
        ModifiedAt DATETIME2 NULL,
        ModifiedBy INT NULL,
        DeletedAt DATETIME2 NULL,
        DeletedBy INT NULL,
        IsDeleted BIT NOT NULL CONSTRAINT DF_Roles_IsDeleted DEFAULT(0),
        RowVersion ROWVERSION NOT NULL
    );
END

-- Seed minimal test data used by integration tests
PRINT 'Ensuring test roles and admin user exist...';

-- Ensure placeholder columns exist early so reflection-based INSERTs won't fail
IF OBJECT_ID('dbo.Roles') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Roles','Id') IS NULL
    BEGIN
        ALTER TABLE dbo.Roles ADD Id INT NULL;
    END
    IF COL_LENGTH('dbo.Roles','RolePermissions') IS NULL
    BEGIN
        ALTER TABLE dbo.Roles ADD RolePermissions NVARCHAR(MAX) NULL;
    END
    IF COL_LENGTH('dbo.Roles','UserRoles') IS NULL
    BEGIN
        ALTER TABLE dbo.Roles ADD UserRoles NVARCHAR(MAX) NULL;
    END
END

IF OBJECT_ID('dbo.Users') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Users','Id') IS NULL
    BEGIN
        ALTER TABLE dbo.Users ADD Id INT NULL;
    END
END

-- Ensure Roles 1 (admin), 2 (user), 3 (role3) exist
IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE RoleId = 1)
BEGIN
    SET IDENTITY_INSERT dbo.Roles ON;
    INSERT INTO dbo.Roles (RoleId, Code, Name, Description, IsActive, IsSystemRole, CreatedAt, IsDeleted)
    VALUES (1, 'admin', 'Administrator', 'System administrator role', 1, 1, SYSUTCDATETIME(), 0);
    SET IDENTITY_INSERT dbo.Roles OFF;
END

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE RoleId = 2)
BEGIN
    SET IDENTITY_INSERT dbo.Roles ON;
    INSERT INTO dbo.Roles (RoleId, Code, Name, Description, IsActive, IsSystemRole, CreatedAt, IsDeleted)
    VALUES (2, 'user', 'User', 'Regular user role', 1, 0, SYSUTCDATETIME(), 0);
    SET IDENTITY_INSERT dbo.Roles OFF;
END

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE RoleId = 3)
BEGIN
    SET IDENTITY_INSERT dbo.Roles ON;
    INSERT INTO dbo.Roles (RoleId, Code, Name, Description, IsActive, IsSystemRole, CreatedAt, IsDeleted)
    VALUES (3, 'role3', 'Role 3', 'Additional role for tests', 1, 0, SYSUTCDATETIME(), 0);
    SET IDENTITY_INSERT dbo.Roles OFF;
END

-- Ensure admin user exists (UserId = 1)
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE UserId = 1)
BEGIN
    SET IDENTITY_INSERT dbo.Users ON;
    INSERT INTO dbo.Users (UserId, Username, Email, FirstName, LastName, PasswordHash, IsActive, EmailVerified, CreatedAt, IsDeleted)
    VALUES (1, 'admin', 'admin@example.com', 'System', 'Administrator', '', 1, 1, SYSUTCDATETIME(), 0);
    SET IDENTITY_INSERT dbo.Users OFF;
END

-- Ensure mapping UserRoles (1,1) exists
-- Create UserRoles table if missing (simple schema for tests)
IF OBJECT_ID('dbo.UserRoles') IS NULL
BEGIN
    CREATE TABLE dbo.UserRoles (
        UserId INT NOT NULL,
        RoleId INT NOT NULL,
        AssignedAt DATETIME2 NOT NULL,
        CONSTRAINT PK_UserRoles PRIMARY KEY (UserId, RoleId)
    );
END

IF NOT EXISTS (SELECT 1 FROM dbo.UserRoles WHERE UserId = 1 AND RoleId = 1)
BEGIN
    INSERT INTO dbo.UserRoles (UserId, RoleId, AssignedAt)
    VALUES (1, 1, SYSUTCDATETIME());
END

-- Ensure Roles table has columns that the test runtime's reflection-based INSERT might include
-- (e.g., BaseEntity.Id, navigation properties). Add nullable placeholder columns when missing.
IF OBJECT_ID('dbo.Roles') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Roles','Id') IS NULL
    BEGIN
        ALTER TABLE dbo.Roles ADD Id INT NULL;
    END
    IF COL_LENGTH('dbo.Roles','RolePermissions') IS NULL
    BEGIN
        ALTER TABLE dbo.Roles ADD RolePermissions NVARCHAR(MAX) NULL;
    END
    IF COL_LENGTH('dbo.Roles','UserRoles') IS NULL
    BEGIN
        ALTER TABLE dbo.Roles ADD UserRoles NVARCHAR(MAX) NULL;
    END
END

-- Ensure Users table has Id column (placeholder) to match BaseEntity.Id when inserted via reflection
IF OBJECT_ID('dbo.Users') IS NOT NULL
BEGIN
    IF COL_LENGTH('dbo.Users','Id') IS NULL
    BEGIN
        ALTER TABLE dbo.Users ADD Id INT NULL;
    END
END

PRINT 'Test data seeding complete.';
