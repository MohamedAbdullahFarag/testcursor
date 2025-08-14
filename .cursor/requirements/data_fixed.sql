-- =============================================
-- data_fixed.sql - Fixed seed data insertion with corrected syntax
-- =============================================

USE [Ekhtibar];
GO

PRINT 'Starting safe data insertion...';
GO

-- 1) Seed lookup tables with duplicate checks

-- QuestionTypes
PRINT 'Inserting QuestionTypes...';
MERGE INTO dbo.QuestionTypes AS Target
USING (VALUES
    ('MultipleChoice'),('MultipleResponse'),('TrueFalse'),
    ('FillInBlank'),('ShortAnswer'),('Essay'),
    ('Matching'),('Ordering'),('Coding'),
    ('MathEquation'),('FillInTheBlank')
) AS Source(Name)
ON Target.Name = Source.Name
WHEN NOT MATCHED THEN
    INSERT (Name) VALUES (Source.Name);
GO

-- DifficultyLevels
PRINT 'Inserting DifficultyLevels...';
MERGE INTO dbo.DifficultyLevels AS Target
USING (VALUES
    ('Easy'),('Medium'),('Hard'),('VeryHard')
) AS Source(Name)
ON Target.Name = Source.Name
WHEN NOT MATCHED THEN
    INSERT (Name) VALUES (Source.Name);
GO

-- QuestionStatuses
PRINT 'Inserting QuestionStatuses...';
MERGE INTO dbo.QuestionStatuses AS Target
USING (VALUES
    ('Draft'),('PendingReview'),('Reviewed'),
    ('Published'),('Archived'),('Rejected')
) AS Source(Name)
ON Target.Name = Source.Name
WHEN NOT MATCHED THEN
    INSERT (Name) VALUES (Source.Name);
GO

-- MediaTypes
PRINT 'Inserting MediaTypes...';
MERGE INTO dbo.MediaTypes AS Target
USING (VALUES
    ('Image'),('Audio'),('Video'),('Document')
) AS Source(Name)
ON Target.Name = Source.Name
WHEN NOT MATCHED THEN
    INSERT (Name) VALUES (Source.Name);
GO

-- TreeNodeTypes
PRINT 'Inserting TreeNodeTypes...';
MERGE INTO dbo.TreeNodeTypes AS Target
USING (VALUES
    ('Root'),('Subject'),('Topic'),('Subtopic'),
    ('TagGroup'),('Custom'),('Stage'),('Grade'),
    ('Semester'),('Unit'),('Lesson')
) AS Source(Name)
ON Target.Name = Source.Name
WHEN NOT MATCHED THEN
    INSERT (Name) VALUES (Source.Name);
GO

-- 2) Seed roles with duplicate checks
PRINT 'Inserting Roles...';
MERGE INTO dbo.Roles AS Target
USING (VALUES
    ('system-admin',    'SystemAdmin',     'Full access to all features',                  1),
    ('reviewer',        'Reviewer',        'Can review and approve questions',             1),
    ('creator',         'Creator',         'Can create and manage questions',              1),
    ('grader',          'Grader',          'Can grade exams and assessments',              1),
    ('student',         'Student',         'Can take exams and view results',              1),
    ('exam-manager',    'ExamManager',     'Can create and manage exams',                  1),
    ('supervisor',      'Supervisor',      'Can supervise exam sessions',                  1),
    ('quality-reviewer','QualityReviewer', 'Ensures educational standards are met',        1)
) AS Source(Code, Name, Description, IsSystemRole)
ON Target.Code = Source.Code
WHEN NOT MATCHED THEN
    INSERT (Code, Name, Description, IsSystemRole) 
    VALUES (Source.Code, Source.Name, Source.Description, Source.IsSystemRole);
GO

-- 3) Seed permissions with duplicate checks
PRINT 'Inserting Permissions...';
MERGE INTO dbo.Permissions AS Target
USING (VALUES
    -- UserManagement
    ('user-management.view-users',        'ViewUsers',          'View users',                     'UserManagement'),
    ('user-management.create-user',       'CreateUser',         'Create new users',               'UserManagement'),
    ('user-management.edit-user',         'EditUser',           'Edit existing users',            'UserManagement'),
    ('user-management.delete-user',       'DeleteUser',         'Delete users',                   'UserManagement'),
    ('user-management.manage-roles',      'ManageRoles',        'Manage role assignments',        'UserManagement'),
    ('user-management.manage-permissions','ManagePermissions',  'Manage permissions',             'UserManagement'),
    ('user-management.impersonate-user',  'ImpersonateUser',    'Impersonate other users',        'UserManagement'),
    ('user-management.view-audit-logs',   'ViewAuditLogs',      'View audit trails',              'UserManagement'),
    -- QuestionBank
    ('question-bank.view-questions',   'ViewQuestions',         'View question bank entries',     'QuestionBank'),
    ('question-bank.create-question',  'CreateQuestion',        'Create questions',               'QuestionBank'),
    ('question-bank.edit-question',    'EditQuestion',          'Edit questions',                 'QuestionBank'),
    ('question-bank.delete-question',  'DeleteQuestion',        'Delete questions',               'QuestionBank'),
    ('question-bank.review-question',  'ReviewQuestion',        'Review submitted questions',     'QuestionBank'),
    ('question-bank.approve-question', 'ApproveQuestion',       'Approve questions for exams',    'QuestionBank'),
    ('question-bank.import-questions', 'ImportQuestions',       'Bulk import questions',          'QuestionBank'),
    ('question-bank.export-questions', 'ExportQuestions',       'Bulk export questions',          'QuestionBank'),
    ('question-bank.manage-categories','ManageCategories',      'Manage question categories',     'QuestionBank'),
    -- ExamManagement
    ('exam-management.view-exams',     'ViewExams',             'View exams',                     'ExamManagement'),
    ('exam-management.create-exam',    'CreateExam',            'Create new exams',               'ExamManagement'),
    ('exam-management.edit-exam',      'EditExam',              'Edit exams',                     'ExamManagement'),
    ('exam-management.delete-exam',    'DeleteExam',            'Delete exams',                   'ExamManagement'),
    ('exam-management.publish-exam',   'PublishExam',           'Publish exams',                  'ExamManagement'),
    ('exam-management.grade-exam',     'GradeExam',             'Grade exams',                    'ExamManagement'),
    ('exam-management.view-results',   'ViewResults',           'View results',                   'ExamManagement'),
    ('exam-management.supervise-exam', 'SuperviseExam',         'Supervise exam sessions',        'ExamManagement'),
    -- System
    ('system.manage-settings',      'ManageSettings',           'Manage system settings',         'System'),
    ('system.view-logs',            'ViewLogs',                 'View system logs',               'System'),
    ('system.manage-integrations',  'ManageIntegrations',       'Manage integrations',            'System'),
    ('system.manage-tree-nodes',    'ManageTreeNodes',          'Manage tree structure',          'System'),
    -- Reporting
    ('reporting.view-reports',           'ViewReports',         'View reports',                   'Reporting'),
    ('reporting.generate-reports',       'GenerateReports',     'Generate new reports',           'Reporting'),
    ('reporting.export-reports',         'ExportReports',       'Export report files',            'Reporting'),
    ('reporting.manage-report-templates','ManageReportTemplates','Manage report templates',       'Reporting')
) AS Source(Code, Name, Description, Category)
ON Target.Code = Source.Code
WHEN NOT MATCHED THEN
    INSERT (Code, Name, Description, Category) 
    VALUES (Source.Code, Source.Name, Source.Description, Source.Category);
GO

-- 4) Seed RolePermissions (junction) with duplicate checks
PRINT 'Inserting RolePermissions...';

-- SystemAdmin gets every permission
PRINT 'Assigning permissions to SystemAdmin role...';
INSERT INTO dbo.RolePermissions (RoleId, PermissionId, AssignedAt)
SELECT r.RoleId, p.PermissionId, SYSUTCDATETIME()
FROM dbo.Roles r
CROSS JOIN dbo.Permissions p
WHERE r.Code = 'system-admin'
AND NOT EXISTS (
    SELECT 1 FROM dbo.RolePermissions rp 
    WHERE rp.RoleId = r.RoleId AND rp.PermissionId = p.PermissionId
);
GO

-- Reviewer: review & approve question bank
PRINT 'Assigning permissions to Reviewer role...';
INSERT INTO dbo.RolePermissions (RoleId, PermissionId, AssignedAt)
SELECT r.RoleId, p.PermissionId, SYSUTCDATETIME()
FROM dbo.Roles r
JOIN dbo.Permissions p 
  ON p.Code IN ('question-bank.review-question','question-bank.approve-question')
WHERE r.Code = 'reviewer'
AND NOT EXISTS (
    SELECT 1 FROM dbo.RolePermissions rp 
    WHERE rp.RoleId = r.RoleId AND rp.PermissionId = p.PermissionId
);
GO

-- Creator: create/edit/import/export questions
PRINT 'Assigning permissions to Creator role...';
INSERT INTO dbo.RolePermissions (RoleId, PermissionId, AssignedAt)
SELECT r.RoleId, p.PermissionId, SYSUTCDATETIME()
FROM dbo.Roles r
JOIN dbo.Permissions p 
  ON p.Code IN (
    'question-bank.create-question',
    'question-bank.edit-question',
    'question-bank.import-questions',
    'question-bank.export-questions'
  )
WHERE r.Code = 'creator'
AND NOT EXISTS (
    SELECT 1 FROM dbo.RolePermissions rp 
    WHERE rp.RoleId = r.RoleId AND rp.PermissionId = p.PermissionId
);
GO

-- Grader: grade exams
PRINT 'Assigning permissions to Grader role...';
INSERT INTO dbo.RolePermissions (RoleId, PermissionId, AssignedAt)
SELECT r.RoleId, p.PermissionId, SYSUTCDATETIME()
FROM dbo.Roles r
JOIN dbo.Permissions p 
  ON p.Code = 'exam-management.grade-exam'
WHERE r.Code = 'grader'
AND NOT EXISTS (
    SELECT 1 FROM dbo.RolePermissions rp 
    WHERE rp.RoleId = r.RoleId AND rp.PermissionId = p.PermissionId
);
GO

-- ExamManager: full exam management
PRINT 'Assigning permissions to ExamManager role...';
INSERT INTO dbo.RolePermissions (RoleId, PermissionId, AssignedAt)
SELECT r.RoleId, p.PermissionId, SYSUTCDATETIME()
FROM dbo.Roles r
JOIN dbo.Permissions p 
  ON p.Code IN (
    'exam-management.view-exams',
    'exam-management.create-exam',
    'exam-management.edit-exam',
    'exam-management.delete-exam',
    'exam-management.publish-exam',
    'exam-management.view-results'
  )
WHERE r.Code = 'exam-manager'
AND NOT EXISTS (
    SELECT 1 FROM dbo.RolePermissions rp 
    WHERE rp.RoleId = r.RoleId AND rp.PermissionId = p.PermissionId
);
GO

-- Supervisor: supervise exam sessions
PRINT 'Assigning permissions to Supervisor role...';
INSERT INTO dbo.RolePermissions (RoleId, PermissionId, AssignedAt)
SELECT r.RoleId, p.PermissionId, SYSUTCDATETIME()
FROM dbo.Roles r
JOIN dbo.Permissions p 
  ON p.Code = 'exam-management.supervise-exam'
WHERE r.Code = 'supervisor'
AND NOT EXISTS (
    SELECT 1 FROM dbo.RolePermissions rp 
    WHERE rp.RoleId = r.RoleId AND rp.PermissionId = p.PermissionId
);
GO

-- Student: view exams & results
PRINT 'Assigning permissions to Student role...';
INSERT INTO dbo.RolePermissions (RoleId, PermissionId, AssignedAt)
SELECT r.RoleId, p.PermissionId, SYSUTCDATETIME()
FROM dbo.Roles r
JOIN dbo.Permissions p 
  ON p.Code IN (
    'exam-management.view-exams',
    'exam-management.view-results'
  )
WHERE r.Code = 'student'
AND NOT EXISTS (
    SELECT 1 FROM dbo.RolePermissions rp 
    WHERE rp.RoleId = r.RoleId AND rp.PermissionId = p.PermissionId
);
GO

-- QualityReviewer: manage categories & view reports
PRINT 'Assigning permissions to QualityReviewer role...';
INSERT INTO dbo.RolePermissions (RoleId, PermissionId, AssignedAt)
SELECT r.RoleId, p.PermissionId, SYSUTCDATETIME()
FROM dbo.Roles r
JOIN dbo.Permissions p 
  ON p.Code IN (
    'question-bank.manage-categories',
    'reporting.view-reports'
  )
WHERE r.Code = 'quality-reviewer'
AND NOT EXISTS (
    SELECT 1 FROM dbo.RolePermissions rp 
    WHERE rp.RoleId = r.RoleId AND rp.PermissionId = p.PermissionId
);
GO

-- 5) Insert basic tree structure
PRINT 'Inserting tree nodes...';

-- Check for existing root node
DECLARE @root_exists BIT = 0;
IF EXISTS(SELECT 1 FROM dbo.TreeNodes WHERE Code = 'GEN')
BEGIN
    SET @root_exists = 1;
    PRINT 'Root node already exists, skipping tree creation';
END

IF @root_exists = 0
BEGIN
    PRINT 'Creating basic tree structure...';
    
    -- Insert root node 
    INSERT INTO dbo.TreeNodes (Name, Code, Description, TreeNodeTypeId, ParentId, OrderIndex, Path, CreatedAt)
    VALUES 
      (N'General Education | التعليم العام', 'GEN', 'General Education root node', 
       (SELECT TreeNodeTypeId FROM dbo.TreeNodeTypes WHERE Name = 'Root'), 
       NULL, 1, '-1-', SYSUTCDATETIME());

    DECLARE @rootId INT = SCOPE_IDENTITY();

    -- Insert basic stage nodes under root
    INSERT INTO dbo.TreeNodes (Name, Code, Description, TreeNodeTypeId, ParentId, OrderIndex, Path, CreatedAt)
    VALUES 
        (N'Elementary School | المرحلة الابتدائية', 'ELEM', 'Elementary school (Grades 1–5)', 
         (SELECT TreeNodeTypeId FROM dbo.TreeNodeTypes WHERE Name = 'Stage'), @rootId, 1, '-1-1-', SYSUTCDATETIME()),
        (N'Middle School | المرحلة المتوسطة', 'MID', 'Middle school (Grades 6–8)', 
         (SELECT TreeNodeTypeId FROM dbo.TreeNodeTypes WHERE Name = 'Stage'), @rootId, 2, '-1-2-', SYSUTCDATETIME()),
        (N'High School | المرحلة الثانوية', 'HIGH', 'High school (Grades 9–12)', 
         (SELECT TreeNodeTypeId FROM dbo.TreeNodeTypes WHERE Name = 'Stage'), @rootId, 3, '-1-3-', SYSUTCDATETIME());

    PRINT 'Basic tree structure created';
END;
GO

-- 6) Insert users with duplicate checks
PRINT 'Inserting users...';
MERGE INTO dbo.Users AS Target
USING (VALUES
    ('admin',           'admin@ikhtibar.com',           'System',   'Administrator', '123123', NULL, 'ar', 1),
    ('teacher1',        'teacher1@ikhtibar.com',        'Ahmed',    'Hassan',        '123123', NULL, 'ar', 1),
    ('student1',        'student1@ikhtibar.com',        'Fatima',   'Mohamed',       '123123', NULL, 'ar', 1)
) AS Source(Username, Email, FirstName, LastName, PasswordHash, PhoneNumber, PreferredLanguage, IsActive)
ON Target.Username = Source.Username OR Target.Email = Source.Email
WHEN NOT MATCHED THEN
    INSERT (Username, Email, FirstName, LastName, PasswordHash, PhoneNumber, PreferredLanguage, IsActive)
    VALUES (Source.Username, Source.Email, Source.FirstName, Source.LastName, Source.PasswordHash, Source.PhoneNumber, Source.PreferredLanguage, Source.IsActive);
GO

-- 7) Assign users to roles with duplicate checks
PRINT 'Assigning users to roles...';

-- admin -> system-admin
INSERT INTO dbo.UserRoles (UserId, RoleId, AssignedAt)
SELECT u.UserId, r.RoleId, SYSUTCDATETIME()
FROM dbo.Users u
CROSS JOIN dbo.Roles r
WHERE u.Username = 'admin' AND r.Code = 'system-admin'
AND NOT EXISTS (
    SELECT 1 FROM dbo.UserRoles ur2
    WHERE ur2.UserId = u.UserId AND ur2.RoleId = r.RoleId
);

-- teacher1 -> creator AND reviewer
INSERT INTO dbo.UserRoles (UserId, RoleId, AssignedAt)
SELECT u.UserId, r.RoleId, SYSUTCDATETIME()
FROM dbo.Users u
CROSS JOIN dbo.Roles r
WHERE u.Username = 'teacher1' AND r.Code IN ('creator', 'reviewer')
AND NOT EXISTS (
    SELECT 1 FROM dbo.UserRoles ur2
    WHERE ur2.UserId = u.UserId AND ur2.RoleId = r.RoleId
);

-- student1 -> student
INSERT INTO dbo.UserRoles (UserId, RoleId, AssignedAt)
SELECT u.UserId, r.RoleId, SYSUTCDATETIME()
FROM dbo.Users u
CROSS JOIN dbo.Roles r
WHERE u.Username = 'student1' AND r.Code = 'student'
AND NOT EXISTS (
    SELECT 1 FROM dbo.UserRoles ur2
    WHERE ur2.UserId = u.UserId AND ur2.RoleId = r.RoleId
);
GO

PRINT 'Data insertion completed successfully!';
PRINT 'Summary:';
PRINT '- Created lookup tables (QuestionTypes, DifficultyLevels, etc.)';
PRINT '- Created 8 roles with appropriate permissions';
PRINT '- Created basic tree structure (Root -> Stages)';
PRINT '- Created 3 test users: admin, teacher1, student1';
PRINT '- Assigned roles to users';
GO
