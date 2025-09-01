USE IkhtibarDb;
GO

-- Clear existing test users
DELETE FROM UserRoles WHERE UserId IN (SELECT UserId FROM Users WHERE Email IN ('admin@ikhtibar.com', 'teacher1@ikhtibar.com', 'student1@ikhtibar.com'));
DELETE FROM Users WHERE Email IN ('admin@ikhtibar.com', 'teacher1@ikhtibar.com', 'student1@ikhtibar.com');

-- Reset identity seed
DBCC CHECKIDENT ('Users', RESEED, 0);
GO

-- Insert test users with proper BCrypt hashes for password "password"
-- These hashes are pre-generated using BCrypt with cost factor 10
INSERT INTO Users (Username, Email, FirstName, LastName, PasswordHash, PreferredLanguage, IsActive, EmailVerified, CreatedAt) VALUES 
    ('admin', 'admin@ikhtibar.com', 'System', 'Administrator', '$2a$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'en', 1, 1, GETUTCDATE()),
    ('teacher1', 'teacher1@ikhtibar.com', 'Ahmad', 'Al-Teacher', '$2a$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'ar', 1, 1, GETUTCDATE()),
    ('student1', 'student1@ikhtibar.com', 'Fatima', 'Al-Student', '$2a$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'ar', 1, 1, GETUTCDATE());

-- Get the user IDs for role assignment
DECLARE @AdminUserId INT = (SELECT UserId FROM Users WHERE Username = 'admin');
DECLARE @TeacherUserId INT = (SELECT UserId FROM Users WHERE Username = 'teacher1');
DECLARE @StudentUserId INT = (SELECT UserId FROM Users WHERE Username = 'student1');

-- Get the role IDs
DECLARE @AdminRoleId INT = (SELECT RoleId FROM Roles WHERE Code = 'ADMIN');
DECLARE @TeacherRoleId INT = (SELECT RoleId FROM Roles WHERE Code = 'TEACHER');
DECLARE @StudentRoleId INT = (SELECT RoleId FROM Roles WHERE Code = 'STUDENT');

-- Assign roles to users
INSERT INTO UserRoles (UserId, RoleId, AssignedAt) VALUES 
    (@AdminUserId, @AdminRoleId, GETUTCDATE()),
    (@TeacherUserId, @TeacherRoleId, GETUTCDATE()),
    (@StudentUserId, @StudentRoleId, GETUTCDATE());

-- Verify the setup
SELECT 
    u.UserId,
    u.Email,
    u.Username,
    u.PasswordHash,
    LEN(u.PasswordHash) as HashLength,
    r.Name as RoleName
FROM Users u
JOIN UserRoles ur ON u.UserId = ur.UserId
JOIN Roles r ON ur.RoleId = r.RoleId
WHERE u.Email IN ('admin@ikhtibar.com', 'teacher1@ikhtibar.com', 'student1@ikhtibar.com');

PRINT 'Password hashes fixed successfully!';
PRINT 'All users now have password: password';
PRINT 'BCrypt hash length: 60 characters';
GO

