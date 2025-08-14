USE IkhtibarDb_Development;
GO

-- Clear and reseed with basic data
DELETE FROM UserRoles;
DELETE FROM Users;
DELETE FROM Roles;
DELETE FROM QuestionTypes;
DELETE FROM DifficultyLevels;
DELETE FROM QuestionStatuses;
DELETE FROM MediaTypes;
DELETE FROM TreeNodeTypes;

-- Reset identity seeds
DBCC CHECKIDENT ('Users', RESEED, 0);
DBCC CHECKIDENT ('Roles', RESEED, 0);
DBCC CHECKIDENT ('QuestionTypes', RESEED, 0);
DBCC CHECKIDENT ('DifficultyLevels', RESEED, 0);
DBCC CHECKIDENT ('QuestionStatuses', RESEED, 0);
DBCC CHECKIDENT ('MediaTypes', RESEED, 0);
DBCC CHECKIDENT ('TreeNodeTypes', RESEED, 0);

-- Insert basic lookup data
INSERT INTO QuestionTypes (Name) VALUES 
    ('Multiple Choice'),
    ('True/False'),
    ('Short Answer'),
    ('Essay');

INSERT INTO DifficultyLevels (Name) VALUES 
    ('Easy'),
    ('Medium'),
    ('Hard');

INSERT INTO QuestionStatuses (Name) VALUES 
    ('Draft'),
    ('Published'),
    ('Archived');

INSERT INTO MediaTypes (Name) VALUES 
    ('Image'),
    ('Video'),
    ('Audio'),
    ('Document');

INSERT INTO TreeNodeTypes (Name) VALUES 
    ('Subject'),
    ('Chapter'),
    ('Topic');

-- Insert basic roles
INSERT INTO Roles (Code, Name, Description, IsSystemRole, CreatedAt) VALUES 
    ('ADMIN', 'Administrator', 'System administrator with full access', 1, SYSUTCDATETIME()),
    ('TEACHER', 'Teacher', 'Can create questions and manage exams', 0, SYSUTCDATETIME()),
    ('STUDENT', 'Student', 'Can take exams and view results', 0, SYSUTCDATETIME());

-- Insert test users with simple password hash (bcrypt hash for "password")
INSERT INTO Users (Username, Email, FirstName, LastName, PasswordHash, PreferredLanguage, IsActive, EmailVerified, CreatedAt) VALUES 
    ('admin', 'admin@ikhtibar.com', 'System', 'Administrator', '$2a$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'en', 1, 1, SYSUTCDATETIME()),
    ('teacher1', 'teacher1@ikhtibar.com', 'Ahmad', 'Al-Teacher', '$2a$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'ar', 1, 1, SYSUTCDATETIME()),
    ('student1', 'student1@ikhtibar.com', 'Fatima', 'Al-Student', '$2a$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'ar', 1, 1, SYSUTCDATETIME());

-- Assign roles to users (get actual user IDs)
DECLARE @AdminUserId INT = (SELECT UserId FROM Users WHERE Username = 'admin');
DECLARE @TeacherUserId INT = (SELECT UserId FROM Users WHERE Username = 'teacher1');
DECLARE @StudentUserId INT = (SELECT UserId FROM Users WHERE Username = 'student1');

DECLARE @AdminRoleId INT = (SELECT RoleId FROM Roles WHERE Code = 'ADMIN');
DECLARE @TeacherRoleId INT = (SELECT RoleId FROM Roles WHERE Code = 'TEACHER');
DECLARE @StudentRoleId INT = (SELECT RoleId FROM Roles WHERE Code = 'STUDENT');

INSERT INTO UserRoles (UserId, RoleId, AssignedAt) VALUES 
    (@AdminUserId, @AdminRoleId, SYSUTCDATETIME()),
    (@TeacherUserId, @TeacherRoleId, SYSUTCDATETIME()),
    (@StudentUserId, @StudentRoleId, SYSUTCDATETIME());

PRINT 'Basic seed data inserted successfully!';
GO
