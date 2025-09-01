-- Assign ADMIN role to the admin user
USE IkhtibarDb;
GO

-- Check if admin user exists and get the user ID
DECLARE @AdminUserId INT = (SELECT UserId FROM Users WHERE Email = 'admin@ikhtibar.com');
DECLARE @AdminRoleId INT = (SELECT RoleId FROM Roles WHERE Code = 'ADMIN');

-- Print current state
PRINT 'Admin User ID: ' + ISNULL(CAST(@AdminUserId AS VARCHAR), 'NOT FOUND');
PRINT 'Admin Role ID: ' + ISNULL(CAST(@AdminRoleId AS VARCHAR), 'NOT FOUND');

-- If the ADMIN role doesn't exist, create it
IF @AdminRoleId IS NULL
BEGIN
    INSERT INTO Roles (Code, Name, Description, IsSystemRole, CreatedAt) VALUES 
        ('ADMIN', 'Administrator', 'System administrator with full access', 1, SYSUTCDATETIME());
    SET @AdminRoleId = SCOPE_IDENTITY();
    PRINT 'Created ADMIN role with ID: ' + CAST(@AdminRoleId AS VARCHAR);
END

-- If the TEACHER role doesn't exist, create it
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Code = 'TEACHER')
BEGIN
    INSERT INTO Roles (Code, Name, Description, IsSystemRole, CreatedAt) VALUES 
        ('TEACHER', 'Teacher', 'Can create questions and manage exams', 0, SYSUTCDATETIME());
    PRINT 'Created TEACHER role';
END

-- Remove any existing role assignments for this user
DELETE FROM UserRoles WHERE UserId = @AdminUserId;

-- Assign ADMIN role to admin user
IF @AdminUserId IS NOT NULL AND @AdminRoleId IS NOT NULL
BEGIN
    INSERT INTO UserRoles (UserId, RoleId, AssignedAt) VALUES 
        (@AdminUserId, @AdminRoleId, SYSUTCDATETIME());
    PRINT 'Successfully assigned ADMIN role to admin user';
END
ELSE
BEGIN
    PRINT 'ERROR: Could not assign role - User or Role not found';
END

-- Verify the assignment
SELECT 
    u.UserId,
    u.Username, 
    u.Email,
    r.Code as RoleCode,
    r.Name as RoleName
FROM Users u
LEFT JOIN UserRoles ur ON u.UserId = ur.UserId
LEFT JOIN Roles r ON ur.RoleId = r.RoleId
WHERE u.Email = 'admin@ikhtibar.com';

GO
