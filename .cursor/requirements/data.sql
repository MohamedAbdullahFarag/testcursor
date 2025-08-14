-- =============================================
-- data_safe.sql - Safe seed data insertion with duplicate handling
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

-- Delete existing role permissions to prevent duplicates
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

-- Tree Nodes section with duplicate checks
PRINT 'Inserting tree nodes...';

-- Check for existing root node
PRINT 'Checking for root node...';
DECLARE @root_exists BIT = 0;
IF EXISTS(SELECT 1 FROM dbo.TreeNodes WHERE Code = 'GEN')
BEGIN
    SET @root_exists = 1;
END

IF @root_exists = 0
BEGIN
    PRINT 'Inserting root node...';
    -- Insert root node 
    INSERT INTO dbo.TreeNodes (Name, Code, Description, TreeNodeTypeId, ParentId, OrderIndex, Path, CreatedAt)
    VALUES 
      (N'General Education | التعليم العام', 'GEN', 'General Education root node', 
       (SELECT TreeNodeTypeId FROM dbo.TreeNodeTypes WHERE Name = 'Root'), 
       NULL, 1, '-1-', SYSUTCDATETIME());
END
GO

-- Get the root node ID (in a separate batch to ensure it's defined)
PRINT 'Getting root node ID...';
DECLARE @rootId INT;
SELECT @rootId = TreeNodeId FROM dbo.TreeNodes WHERE Code = 'GEN';

-- Insert Stage nodes under root if they don't exist
PRINT 'Inserting stage nodes...';
MERGE INTO dbo.TreeNodes AS Target
USING (VALUES
    (N'Elementary School | المرحلة الابتدائية', 'ELEM', 'Elementary school (Grades 1–5)', 1),
    (N'Middle School    | المرحلة المتوسطة',    'MID',  'Middle school (Grades 6–8)',    2),
    (N'High School      | المرحلة الثانوية',     'HIGH', 'High school (Grades 9–12)',     3)
) AS Source(Name, Code, Description, OrderIndex)
ON Target.Code = Source.Code AND Target.ParentId = @rootId
WHEN NOT MATCHED THEN
    INSERT (Name, Code, Description, TreeNodeTypeId, ParentId, OrderIndex, Path, CreatedAt)
    VALUES (
        Source.Name, 
        Source.Code, 
        Source.Description, 
        (SELECT TreeNodeTypeId FROM dbo.TreeNodeTypes WHERE Name = 'Stage'),
        @rootId, 
        Source.OrderIndex, 
        CONCAT('-', @rootId, '-', Source.OrderIndex, '-'), 
        SYSUTCDATETIME()
    );
GO

-- Generate Grade and Semester and Subject hierarchy safely (in separate batch)
PRINT 'Generating grade hierarchy...';
DECLARE @rootId INT;
SELECT @rootId = TreeNodeId FROM dbo.TreeNodes WHERE Code = 'GEN';

DECLARE @stageId INT, @start INT, @end INT, @gradePath NVARCHAR(100), @stageOrderIndex INT;
DECLARE @gradeId INT, @semId INT, @semPath NVARCHAR(100);

-- Create cursor for stages
DECLARE stage_cursor CURSOR FOR
SELECT TreeNodeId, 
       CASE Code WHEN 'ELEM' THEN 1 WHEN 'MID' THEN 6 ELSE 9 END AS StartGrade,
       CASE Code WHEN 'ELEM' THEN 5 WHEN 'MID' THEN 8 ELSE 12 END AS EndGrade,
       Path,
       OrderIndex
FROM dbo.TreeNodes
WHERE ParentId = @rootId 
  AND Code IN ('ELEM', 'MID', 'HIGH');

OPEN stage_cursor;
FETCH NEXT FROM stage_cursor INTO @stageId, @start, @end, @gradePath, @stageOrderIndex;
WHILE @@FETCH_STATUS = 0
BEGIN
    -- Grades
    DECLARE @g INT = @start;
    WHILE @g <= @end
    BEGIN
        -- Check if grade exists
        IF NOT EXISTS (SELECT 1 FROM dbo.TreeNodes WHERE ParentId = @stageId AND Code = CONCAT('GR', @g))
        BEGIN
            -- Insert grade
            INSERT INTO dbo.TreeNodes (Name, Code, Description, TreeNodeTypeId, ParentId, OrderIndex, Path, CreatedAt)
            VALUES (
                CONCAT(N'Grade ', @g, N' | الصف ', @g),
                CONCAT('GR', @g),
                CONCAT(@g, N'th grade curriculum | منهج الصف ', @g),
                (SELECT TreeNodeTypeId FROM dbo.TreeNodeTypes WHERE Name = 'Grade'),
                @stageId,
                @g - @start + 1,
                CONCAT(@gradePath, @g - @start + 1, '-'),
                SYSUTCDATETIME()
            );
            
            SET @gradeId = SCOPE_IDENTITY();
            SET @gradePath = (SELECT Path FROM dbo.TreeNodes WHERE TreeNodeId = @gradeId);
            
            -- Semesters under each grade
            DECLARE @s1 INT = 1;
            WHILE @s1 <= 2
            BEGIN
                -- Insert semester
                INSERT INTO dbo.TreeNodes (Name, Code, Description, TreeNodeTypeId, ParentId, OrderIndex, Path, CreatedAt)
                VALUES (
                    CONCAT(N'Semester ', @s1, N' | الفصل ', @s1),
                    CONCAT(N'GR', @g, '-SEM', @s1),
                    CONCAT(N'Semester ', @s1, ' for Grade ', @g),
                    (SELECT TreeNodeTypeId FROM dbo.TreeNodeTypes WHERE Name = 'Semester'),
                    @gradeId,
                    @s1,
                    CONCAT(@gradePath, @s1, '-'),
                    SYSUTCDATETIME()
                );
                
                SET @semId = SCOPE_IDENTITY();
                SET @semPath = (SELECT Path FROM dbo.TreeNodes WHERE TreeNodeId = @semId);
                
                -- Subject (Mathematics) under each semester
                INSERT INTO dbo.TreeNodes (Name, Code, Description, TreeNodeTypeId, ParentId, OrderIndex, Path, CreatedAt)
                VALUES (
                    N'Mathematics | الرياضيات',
                    CONCAT(N'GR', @g, '-SEM', @s1, '-MATH'),
                    CONCAT(N'Mathematics for Grade ', @g, ' Semester ', @s1, N' | الرياضيات'),
                    (SELECT TreeNodeTypeId FROM dbo.TreeNodeTypes WHERE Name = 'Subject'),
                    @semId,
                    1,
                    CONCAT(@semPath, '1-'),
                    SYSUTCDATETIME()
                );
                
                SET @s1 = @s1 + 1;
            END;
        END
        ELSE
        BEGIN
            -- Grade exists, get grade ID
            SET @gradeId = (SELECT TreeNodeId FROM dbo.TreeNodes WHERE ParentId = @stageId AND Code = CONCAT('GR', @g));
            
            -- Check if semesters exist
            DECLARE @s2 INT = 1;
            WHILE @s2 <= 2
            BEGIN
                IF NOT EXISTS (SELECT 1 FROM dbo.TreeNodes WHERE ParentId = @gradeId AND Code = CONCAT('GR', @g, '-SEM', @s2))
                BEGIN
                    -- Insert semester
                    INSERT INTO dbo.TreeNodes (Name, Code, Description, TreeNodeTypeId, ParentId, OrderIndex, Path, CreatedAt)
                    VALUES (
                        CONCAT(N'Semester ', @s2, N' | الفصل ', @s2),
                        CONCAT(N'GR', @g, '-SEM', @s2),
                        CONCAT(N'Semester ', @s2, ' for Grade ', @g),
                        (SELECT TreeNodeTypeId FROM dbo.TreeNodeTypes WHERE Name = 'Semester'),
                        @gradeId,
                        @s2,
                        CONCAT((SELECT Path FROM dbo.TreeNodes WHERE TreeNodeId = @gradeId), @s2, '-'),
                        SYSUTCDATETIME()
                    );
                    
                    SET @semId = SCOPE_IDENTITY();
                    SET @semPath = (SELECT Path FROM dbo.TreeNodes WHERE TreeNodeId = @semId);
                    
                    -- Subject (Mathematics) under each semester
                    INSERT INTO dbo.TreeNodes (Name, Code, Description, TreeNodeTypeId, ParentId, OrderIndex, Path, CreatedAt)
                    VALUES (
                        N'Mathematics | الرياضيات',
                        CONCAT(N'GR', @g, '-SEM', @s2, '-MATH'),
                        CONCAT(N'Mathematics for Grade ', @g, ' Semester ', @s2, N' | الرياضيات'),
                        (SELECT TreeNodeTypeId FROM dbo.TreeNodeTypes WHERE Name = 'Subject'),
                        @semId,
                        1,
                        CONCAT(@semPath, '1-'),
                        SYSUTCDATETIME()
                    );
                END;
                
                SET @s2 = @s2 + 1;
            END;
        END;
        
        SET @g = @g + 1;
    END;
    FETCH NEXT FROM stage_cursor INTO @stageId, @start, @end, @gradePath, @stageOrderIndex;
END;
CLOSE stage_cursor;
DEALLOCATE stage_cursor;
GO

-- Update materialized paths if needed
PRINT 'Updating materialized paths...';
WITH CTE AS (
    SELECT TreeNodeId, ParentId,
           CAST('-' + CAST(TreeNodeId AS VARCHAR(MAX)) + '-' AS NVARCHAR(100)) AS NewPath
    FROM dbo.TreeNodes
    WHERE ParentId IS NULL
    UNION ALL
    SELECT t.TreeNodeId, t.ParentId,
           CAST(c.NewPath + CAST(t.TreeNodeId AS VARCHAR(MAX)) + '-' AS NVARCHAR(100))
    FROM dbo.TreeNodes t
    JOIN CTE c ON t.ParentId = c.TreeNodeId
)
UPDATE dbo.TreeNodes
SET Path = c.NewPath
FROM dbo.TreeNodes tn
JOIN CTE c ON tn.TreeNodeId = c.TreeNodeId;
GO

-- Insert questions with proper column names
PRINT 'Inserting questions...';
DECLARE @leafId INT, @stageCode NVARCHAR(10), @q INT, @questionText NVARCHAR(500);
DECLARE leaf_cursor CURSOR FOR
SELECT l.TreeNodeId, s.Code
FROM dbo.TreeNodes l
JOIN dbo.TreeNodes sem ON l.ParentId = sem.TreeNodeId
JOIN dbo.TreeNodes grade ON sem.ParentId = grade.TreeNodeId
JOIN dbo.TreeNodes s ON grade.ParentId = s.TreeNodeId
WHERE l.TreeNodeTypeId = (SELECT TreeNodeTypeId FROM dbo.TreeNodeTypes WHERE Name = 'Subject');

OPEN leaf_cursor;
FETCH NEXT FROM leaf_cursor INTO @leafId, @stageCode;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @q = 1;
    WHILE @q <= 10
    BEGIN
        -- Get question text based on stage and question number
        IF @stageCode = 'ELEM'
        BEGIN
            SET @questionText = CASE @q
                WHEN 1 THEN N'حدد ناتج جمع العددين 3 و 5.'
                WHEN 2 THEN N'حدد نتيجة طرح العدد 7 من 12.'
                WHEN 3 THEN N'حدد ناتج ضرب العددين 4 و 6.'
                WHEN 4 THEN N'حدد نتيجة قسمة 20 على 4.'
                WHEN 5 THEN N'حدد مجموع الكسرين ½ و ¼.'
                WHEN 6 THEN N'ما العدد الذي يجعل المعادلة 8 + __ = 15 صحيحة؟'
                WHEN 7 THEN N'إذا كان لديك 5 تفاحات وأكلت 2 منها، فما عدد التفاحات المتبقية؟'
                WHEN 8 THEN N'ما العدد الذي إذا ضربته في 2 يعطي 14؟'
                WHEN 9 THEN N'حدد ناتج جمع العددين 9 و 0.'
                ELSE N'اختر العدد الأكبر من بين العددين 7 و 9.'
            END;
        END
        ELSE IF @stageCode = 'MID'
        BEGIN
            SET @questionText = CASE @q
                WHEN 1 THEN N'اوجد قيمة س عندما 2س + 3 = 11.'
                WHEN 2 THEN N'اجمع الكسرين ¾ و ⅛.'
                WHEN 3 THEN N'حساب ناتج ضرب 5 في ½.'
                WHEN 4 THEN N'اوجد قيمة س التي تحقق 3س - 4 = 14.'
                WHEN 5 THEN N'ما هو الكسر الذي يكمل ⅝ إلى 1؟'
                WHEN 6 THEN N'اوجد س في المعادلة 4س ÷ 2 = 8.'
                WHEN 7 THEN N'اجمع العددين العشريين 0.75 و 0.25.'
                WHEN 8 THEN N'احسب نتيجة قسمة 9 على 0.3.'
                WHEN 9 THEN N'اوجد قيمة س في المعادلة 5س = 20.'
                ELSE N'اوجد س بحيث 7س + 7 = 42.'
            END;
        END
        ELSE
        BEGIN
            SET @questionText = CASE @q
                WHEN 1 THEN N'اوجد جذري المعادلة التربيعية س^2 - 5س + 6 = 0.'
                WHEN 2 THEN N'احسب قيمة المميز b^2 - 4ac في 2س^2 - 4س + 1 = 0.'
                WHEN 3 THEN N'اوجد س بحيث 3س + 2 = 11.'
                WHEN 4 THEN N'حدد ميل الخط ص = 4س + 1.'
                WHEN 5 THEN N'احسب مساحة المستطيل طوله 5 سم وعرضه 3 سم.'
                WHEN 6 THEN N'اوجد س في المعادلة 5س ÷ 2 = 10.'
                WHEN 7 THEN N'ما حاصل جمع جذور المعادلة س^2 - س - 6 = 0.'
                WHEN 8 THEN N'حدد نقطة التقاطع مع محور y للمعادلة ص = 2س - 4.'
                WHEN 9 THEN N'احسب طول الوتر في مثلث أضلاعه القائمة 3 و 4.'
                ELSE N'اوجد س عند حل 4س^2 - 12س + 9 = 0.'
            END;
        END;
        
        -- Skip if question text already exists
        IF NOT EXISTS (
            SELECT 1 FROM dbo.Questions 
            WHERE Text = @questionText AND PrimaryTreeNodeId = @leafId
        )
        BEGIN
            -- Insert question
            INSERT INTO dbo.Questions (Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, PrimaryTreeNodeId, CreatedAt)
            VALUES (
                @questionText,
                (SELECT QuestionTypeId FROM dbo.QuestionTypes WHERE Name = 'MultipleChoice'),
                CASE 
                    WHEN @stageCode = 'ELEM' THEN (SELECT DifficultyLevelId FROM dbo.DifficultyLevels WHERE Name = 'Easy')
                    WHEN @stageCode = 'MID' THEN (SELECT DifficultyLevelId FROM dbo.DifficultyLevels WHERE Name = 'Medium')
                    ELSE (SELECT DifficultyLevelId FROM dbo.DifficultyLevels WHERE Name = 'Hard')
                END,
                (SELECT QuestionStatusId FROM dbo.QuestionStatuses WHERE Name = 'Published'),
                @leafId,
                SYSUTCDATETIME()
            );
        END;
        
        SET @q = @q + 1;
    END;
    FETCH NEXT FROM leaf_cursor INTO @leafId, @stageCode;
END;
CLOSE leaf_cursor;
DEALLOCATE leaf_cursor;
GO

-- Insert users with duplicate checks
PRINT 'Inserting users...';
MERGE INTO dbo.Users AS Target
USING (VALUES
    ('sysadmin',        'sysadmin@example.com',        'System',   'Admin',        '123123', NULL, 'ar', 1),
    ('reviewer',        'reviewer@example.com',        'Content',  'Reviewer',     '123123', NULL, 'ar', 1),
    ('creator',         'creator@example.com',         'Content',  'Creator',      '123123', NULL, 'ar', 1),
    ('grader',          'grader@example.com',          'Exam',     'Grader',       '123123', NULL, 'ar', 1),
    ('student1',        'student1@example.com',        'Student',  'One',          '123123', NULL, 'ar', 1),
    ('exammanager',     'exammanager@example.com',     'Exam',     'Manager',      '123123', NULL, 'ar', 1),
    ('supervisor',      'supervisor@example.com',      'Session',  'Supervisor',   '123123', NULL, 'ar', 1),
    ('qualityreviewer', 'qualityreviewer@example.com', 'Quality',  'Reviewer',     '123123', NULL, 'ar', 1)
) AS Source(Username, Email, FirstName, LastName, PasswordHash, PhoneNumber, PreferredLanguage, IsActive)
ON Target.Username = Source.Username
WHEN NOT MATCHED THEN
    INSERT (Username, Email, FirstName, LastName, PasswordHash, PhoneNumber, PreferredLanguage, IsActive)
    VALUES (Source.Username, Source.Email, Source.FirstName, Source.LastName, Source.PasswordHash, Source.PhoneNumber, Source.PreferredLanguage, Source.IsActive);
GO

-- Assign users to roles with duplicate checks
PRINT 'Assigning users to roles...';
-- sysadmin -> system-admin
IF NOT EXISTS (
    SELECT 1 FROM dbo.UserRoles ur
    JOIN dbo.Users u ON ur.UserId = u.UserId
    JOIN dbo.Roles r ON ur.RoleId = r.RoleId
    WHERE u.Username = 'sysadmin' AND r.Code = 'system-admin'
)
BEGIN
    INSERT INTO dbo.UserRoles (UserId, RoleId, AssignedAt)
    SELECT u.UserId, r.RoleId, SYSUTCDATETIME()
    FROM dbo.Users u
    CROSS JOIN dbo.Roles r
    WHERE u.Username = 'sysadmin' AND r.Code = 'system-admin';
END;

-- reviewer -> reviewer
IF NOT EXISTS (
    SELECT 1 FROM dbo.UserRoles ur
    JOIN dbo.Users u ON ur.UserId = u.UserId
    JOIN dbo.Roles r ON ur.RoleId = r.RoleId
    WHERE u.Username = 'reviewer' AND r.Code = 'reviewer'
)
BEGIN
    INSERT INTO dbo.UserRoles (UserId, RoleId, AssignedAt)
    SELECT u.UserId, r.RoleId, SYSUTCDATETIME()
    FROM dbo.Users u
    CROSS JOIN dbo.Roles r
    WHERE u.Username = 'reviewer' AND r.Code = 'reviewer';
END;

-- creator -> creator
IF NOT EXISTS (
    SELECT 1 FROM dbo.UserRoles ur
    JOIN dbo.Users u ON ur.UserId = u.UserId
    JOIN dbo.Roles r ON ur.RoleId = r.RoleId
    WHERE u.Username = 'creator' AND r.Code = 'creator'
)
BEGIN
    INSERT INTO dbo.UserRoles (UserId, RoleId, AssignedAt)
    SELECT u.UserId, r.RoleId, SYSUTCDATETIME()
    FROM dbo.Users u
    CROSS JOIN dbo.Roles r
    WHERE u.Username = 'creator' AND r.Code = 'creator';
END;

-- grader -> grader
IF NOT EXISTS (
    SELECT 1 FROM dbo.UserRoles ur
    JOIN dbo.Users u ON ur.UserId = u.UserId
    JOIN dbo.Roles r ON ur.RoleId = r.RoleId
    WHERE u.Username = 'grader' AND r.Code = 'grader'
)
BEGIN
    INSERT INTO dbo.UserRoles (UserId, RoleId, AssignedAt)
    SELECT u.UserId, r.RoleId, SYSUTCDATETIME()
    FROM dbo.Users u
    CROSS JOIN dbo.Roles r
    WHERE u.Username = 'grader' AND r.Code = 'grader';
END;

-- student1 -> student
IF NOT EXISTS (
    SELECT 1 FROM dbo.UserRoles ur
    JOIN dbo.Users u ON ur.UserId = u.UserId
    JOIN dbo.Roles r ON ur.RoleId = r.RoleId
    WHERE u.Username = 'student1' AND r.Code = 'student'
)
BEGIN
    INSERT INTO dbo.UserRoles (UserId, RoleId, AssignedAt)
    SELECT u.UserId, r.RoleId, SYSUTCDATETIME()
    FROM dbo.Users u
    CROSS JOIN dbo.Roles r
    WHERE u.Username = 'student1' AND r.Code = 'student';
END;

-- exammanager -> exam-manager
IF NOT EXISTS (
    SELECT 1 FROM dbo.UserRoles ur
    JOIN dbo.Users u ON ur.UserId = u.UserId
    JOIN dbo.Roles r ON ur.RoleId = r.RoleId
    WHERE u.Username = 'exammanager' AND r.Code = 'exam-manager'
)
BEGIN
    INSERT INTO dbo.UserRoles (UserId, RoleId, AssignedAt)
    SELECT u.UserId, r.RoleId, SYSUTCDATETIME()
    FROM dbo.Users u
    CROSS JOIN dbo.Roles r
    WHERE u.Username = 'exammanager' AND r.Code = 'exam-manager';
END;

-- supervisor -> supervisor
IF NOT EXISTS (
    SELECT 1 FROM dbo.UserRoles ur
    JOIN dbo.Users u ON ur.UserId = u.UserId
    JOIN dbo.Roles r ON ur.RoleId = r.RoleId
    WHERE u.Username = 'supervisor' AND r.Code = 'supervisor'
)
BEGIN
    INSERT INTO dbo.UserRoles (UserId, RoleId, AssignedAt)
    SELECT u.UserId, r.RoleId, SYSUTCDATETIME()
    FROM dbo.Users u
    CROSS JOIN dbo.Roles r
    WHERE u.Username = 'supervisor' AND r.Code = 'supervisor';
END;

-- qualityreviewer -> quality-reviewer
IF NOT EXISTS (
    SELECT 1 FROM dbo.UserRoles ur
    JOIN dbo.Users u ON ur.UserId = u.UserId
    JOIN dbo.Roles r ON ur.RoleId = r.RoleId
    WHERE u.Username = 'qualityreviewer' AND r.Code = 'quality-reviewer'
)
BEGIN
    INSERT INTO dbo.UserRoles (UserId, RoleId, AssignedAt)
    SELECT u.UserId, r.RoleId, SYSUTCDATETIME()
    FROM dbo.Users u
    CROSS JOIN dbo.Roles r
    WHERE u.Username = 'qualityreviewer' AND r.Code = 'quality-reviewer';
END;

-- =============================================
-- demo_questions.sql - Add realistic demo questions and answers
-- =============================================

GO

PRINT 'Starting insertion of demo questions and answers...';
GO

-- Helper variables for question types
DECLARE @multipleChoiceId INT, @multipleResponseId INT, @trueFalseId INT, @fillInBlankId INT;
DECLARE @easyId INT, @mediumId INT, @hardId INT, @publishedId INT;

-- Get reference IDs
SELECT @multipleChoiceId = QuestionTypeId FROM dbo.QuestionTypes WHERE Name = 'MultipleChoice';
SELECT @multipleResponseId = QuestionTypeId FROM dbo.QuestionTypes WHERE Name = 'MultipleResponse';
SELECT @trueFalseId = QuestionTypeId FROM dbo.QuestionTypes WHERE Name = 'TrueFalse';
SELECT @fillInBlankId = QuestionTypeId FROM dbo.QuestionTypes WHERE Name = 'FillInBlank';

SELECT @easyId = DifficultyLevelId FROM dbo.DifficultyLevels WHERE Name = 'Easy';
SELECT @mediumId = DifficultyLevelId FROM dbo.DifficultyLevels WHERE Name = 'Medium';
SELECT @hardId = DifficultyLevelId FROM dbo.DifficultyLevels WHERE Name = 'Hard';

SELECT @publishedId = QuestionStatusId FROM dbo.QuestionStatuses WHERE Name = 'Published';

-- =============================================
-- ELEMENTARY SCHOOL (GRADES 1-5) QUESTIONS
-- =============================================
PRINT 'Adding Elementary School mathematics questions...';

-- Grade 1 questions - Path pattern -1-2-5-6-7-
DECLARE @grade1_sem1_math_id INT = (
    SELECT TreeNodeId FROM dbo.TreeNodes 
    WHERE Path = '-1-2-5-6-7-' AND Name LIKE 'Mathematics%'
);

-- Multiple Choice Addition and Subtraction - Grade 1 Semester 1
-- Question 1: أي مما يلي يساوي 5 + 3؟
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'أي مما يلي يساوي 5 + 3؟', @multipleChoiceId, @easyId, @publishedId, 
     @grade1_sem1_math_id, N'8 هو ناتج جمع 5 و 3.', 30, 1, GETUTCDATE());

DECLARE @question1_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question1_id, N'7', 0),
    (@question1_id, N'8', 1),  -- Correct
    (@question1_id, N'9', 0),
    (@question1_id, N'6', 0);

-- Question 2: أوجد ناتج: 9 - 4
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'أوجد ناتج: 9 - 4', @multipleChoiceId, @easyId, @publishedId, 
     @grade1_sem1_math_id, N'عند طرح 4 من 9 نحصل على 5.', 30, 1, GETUTCDATE());

DECLARE @question2_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question2_id, N'4', 0),
    (@question2_id, N'5', 1),  -- Correct
    (@question2_id, N'6', 0),
    (@question2_id, N'3', 0);

-- Question 3: إذا كان لديك 7 تفاحات وأعطيت 2 منها لأخيك، فكم تفاحة بقيت معك؟
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'إذا كان لديك 7 تفاحات وأعطيت 2 منها لأخيك، فكم تفاحة بقيت معك؟', @multipleChoiceId, @easyId, @publishedId, 
     @grade1_sem1_math_id, N'7 - 2 = 5 تفاحات', 45, 1, GETUTCDATE());

DECLARE @question3_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question3_id, N'4', 0),
    (@question3_id, N'5', 1),  -- Correct
    (@question3_id, N'3', 0),
    (@question3_id, N'9', 0);

-- Question 4: العدد الذي يأتي بعد 16 هو ___
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'العدد الذي يأتي بعد 16 هو ___', @multipleChoiceId, @easyId, @publishedId, 
     @grade1_sem1_math_id, N'17 يأتي بعد 16 في تسلسل الأعداد.', 20, 1, GETUTCDATE());

DECLARE @question4_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question4_id, N'15', 0),
    (@question4_id, N'17', 1),  -- Correct
    (@question4_id, N'18', 0),
    (@question4_id, N'16', 0);

-- Grade 3 questions - Path pattern -1-2-15-16-17-
DECLARE @grade3_sem1_math_id INT = (
    SELECT TreeNodeId FROM dbo.TreeNodes 
    WHERE Path = '-1-2-15-16-17-' AND Name LIKE 'Mathematics%'
);

-- Grade 3 - More complex operations and fractions
-- Question 1: ما هو ناتج 6 × 7؟
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'ما هو ناتج 6 × 7؟', @multipleChoiceId, @easyId, @publishedId, 
     @grade3_sem1_math_id, N'6 × 7 = 42', 30, 1, GETUTCDATE());

DECLARE @question5_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question5_id, N'36', 0),
    (@question5_id, N'42', 1),  -- Correct
    (@question5_id, N'49', 0),
    (@question5_id, N'13', 0);

-- Question 2: أكمل المعادلة: ___ ÷ 4 = 5
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'أكمل المعادلة: ___ ÷ 4 = 5', @fillInBlankId, @mediumId, @publishedId, 
     @grade3_sem1_math_id, N'20 ÷ 4 = 5، لذا الرقم المفقود هو 20.', 45, 2, GETUTCDATE());

DECLARE @question6_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question6_id, N'20', 1),  -- Correct
    (@question6_id, N'9', 0),
    (@question6_id, N'1', 0),
    (@question6_id, N'4', 0);

-- Question 3: أي الكسور التالية أكبر من 1/2؟
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'أي الكسور التالية أكبر من 1/2؟', @multipleChoiceId, @mediumId, @publishedId, 
     @grade3_sem1_math_id, N'3/4 > 1/2 لأن 3/4 = 0.75 وهو أكبر من 0.5', 60, 2, GETUTCDATE());

DECLARE @question7_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question7_id, N'1/4', 0),
    (@question7_id, N'2/5', 0),
    (@question7_id, N'3/4', 1),  -- Correct
    (@question7_id, N'1/3', 0);

-- Question 4: أحمد لديه 24 قلماً. أعطى 1/3 منها لصديقه. كم قلماً بقي مع أحمد؟
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'أحمد لديه 24 قلماً. أعطى 1/3 منها لصديقه. كم قلماً بقي مع أحمد؟', @multipleChoiceId, @mediumId, @publishedId, 
     @grade3_sem1_math_id, N'1/3 من 24 = 8، إذاً بقي 24 - 8 = 16 قلماً.', 60, 2, GETUTCDATE());

DECLARE @question8_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question8_id, N'8', 0),
    (@question8_id, N'16', 1),  -- Correct
    (@question8_id, N'18', 0),
    (@question8_id, N'12', 0);

-- =============================================
-- MIDDLE SCHOOL (GRADES 6-8) QUESTIONS
-- =============================================
PRINT 'Adding Middle School mathematics questions...';

-- Grade 6 questions - Path pattern -1-3-30-31-32-
DECLARE @grade6_sem1_math_id INT = (
    SELECT TreeNodeId FROM dbo.TreeNodes 
    WHERE Path = '-1-3-30-31-32-' AND Name LIKE 'Mathematics%'
);

-- Grade 6 - Algebra basics, ratios, decimals
-- Question 1: حل المعادلة: 3x + 7 = 22
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'حل المعادلة: 3x + 7 = 22', @multipleChoiceId, @mediumId, @publishedId, 
     @grade6_sem1_math_id, N'3x + 7 = 22\n3x = 15\nx = 5', 60, 2, GETUTCDATE());

DECLARE @question9_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question9_id, N'x = 4', 0),
    (@question9_id, N'x = 5', 1),  -- Correct
    (@question9_id, N'x = 6', 0),
    (@question9_id, N'x = 7.6', 0);

-- Question 2: أوجد النسبة المئوية: 15 من أصل 60
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'أوجد النسبة المئوية: 15 من أصل 60', @multipleChoiceId, @mediumId, @publishedId, 
     @grade6_sem1_math_id, N'(15 ÷ 60) × 100 = 25%', 45, 2, GETUTCDATE());

DECLARE @question10_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question10_id, N'20%', 0),
    (@question10_id, N'25%', 1),  -- Correct
    (@question10_id, N'30%', 0),
    (@question10_id, N'15%', 0);

-- Question 3: ما هو الرقم العشري المكافئ للكسر 3/8؟
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'ما هو الرقم العشري المكافئ للكسر 3/8؟', @multipleChoiceId, @mediumId, @publishedId, 
     @grade6_sem1_math_id, N'3/8 = 0.375', 30, 2, GETUTCDATE());

DECLARE @question11_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question11_id, N'0.375', 1),  -- Correct
    (@question11_id, N'0.38', 0),
    (@question11_id, N'0.35', 0),
    (@question11_id, N'0.3', 0);

-- Question 4: إذا كان الصف يحتوي على 30 طالباً، منهم 18 طالبة، فما نسبة الطالبات في الصف؟
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'إذا كان الصف يحتوي على 30 طالباً، منهم 18 طالبة، فما نسبة الطالبات في الصف؟', @multipleChoiceId, @mediumId, @publishedId, 
     @grade6_sem1_math_id, N'(18 ÷ 30) × 100 = 60%', 60, 2, GETUTCDATE());

DECLARE @question12_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question12_id, N'60%', 1),  -- Correct
    (@question12_id, N'50%', 0),
    (@question12_id, N'40%', 0),
    (@question12_id, N'55%', 0);

-- Grade 8 questions - Path pattern -1-3-40-41-42-
DECLARE @grade8_sem1_math_id INT = (
    SELECT TreeNodeId FROM dbo.TreeNodes 
    WHERE Path = '-1-3-40-41-42-' AND Name LIKE 'Mathematics%'
);

-- Grade 8 - Pre-algebra, geometry, statistics
-- Question 1: إذا كانت نسبتان متساويتان a/b و c/d، فإن:
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'إذا كانت نسبتان متساويتان a/b و c/d، فإن:', @multipleChoiceId, @hardId, @publishedId, 
     @grade8_sem1_math_id, N'حسب خاصية النسب المتساوية، نحصل على a×d = b×c', 60, 3, GETUTCDATE());

DECLARE @question13_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question13_id, N'a×c = b×d', 0),
    (@question13_id, N'a×d = c×b', 1),  -- Correct
    (@question13_id, N'a×b = c×d', 0),
    (@question13_id, N'a×c = b×b', 0);

-- Question 2: مساحة مربع طول ضلعه 5 سم هي:
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'مساحة مربع طول ضلعه 5 سم هي:', @multipleChoiceId, @mediumId, @publishedId, 
     @grade8_sem1_math_id, N'مساحة المربع = الطول × العرض = 5 × 5 = 25 سم²', 30, 2, GETUTCDATE());

DECLARE @question14_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question14_id, N'20 سم²', 0),
    (@question14_id, N'25 سم²', 1),  -- Correct
    (@question14_id, N'10 سم²', 0),
    (@question14_id, N'5² سم²', 0);

-- Question 3: أوجد محيط دائرة نصف قطرها 7 سم (استخدم π = 3.14)
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'أوجد محيط دائرة نصف قطرها 7 سم (استخدم π = 3.14)', @multipleChoiceId, @hardId, @publishedId, 
     @grade8_sem1_math_id, N'محيط الدائرة = 2 × π × r = 2 × 3.14 × 7 = 43.96 سم', 60, 3, GETUTCDATE());

DECLARE @question15_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question15_id, N'21.98 سم', 0),
    (@question15_id, N'43.96 سم', 1),  -- Correct
    (@question15_id, N'153.86 سم²', 0),
    (@question15_id, N'22 سم', 0);

-- Question 4: أوجد المتوسط الحسابي للأعداد: 12, 15, 18, 25, 30
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'أوجد المتوسط الحسابي للأعداد: 12, 15, 18, 25, 30', @multipleChoiceId, @mediumId, @publishedId, 
     @grade8_sem1_math_id, N'المتوسط الحسابي = مجموع القيم ÷ عددها = (12 + 15 + 18 + 25 + 30) ÷ 5 = 100 ÷ 5 = 20', 60, 2, GETUTCDATE());

DECLARE @question16_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question16_id, N'18', 0),
    (@question16_id, N'20', 1),  -- Correct
    (@question16_id, N'21', 0),
    (@question16_id, N'22.5', 0);

-- =============================================
-- HIGH SCHOOL (GRADES 9-12) QUESTIONS
-- =============================================
PRINT 'Adding High School mathematics questions...';

-- Grade 10 questions - Path pattern -1-4-50-*-*-
DECLARE @grade10_sem1_math_id INT = (
    SELECT TreeNodeId FROM dbo.TreeNodes 
    WHERE Path LIKE '-1-4-50-%-%-' AND Name LIKE 'Mathematics%'
);

-- Grade 10 - Algebra II, trigonometry, functions
-- Question 1: أوجد حل المعادلة التربيعية: x² - 5x + 6 = 0
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'أوجد حل المعادلة التربيعية: x² - 5x + 6 = 0', @multipleChoiceId, @hardId, @publishedId, 
     @grade10_sem1_math_id, N'باستخدام صيغة الحل: x = (5 ± √(25-24))/2 = (5 ± 1)/2، لذا الحلول هي x = 2 أو x = 3', 90, 3, GETUTCDATE());

DECLARE @question17_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question17_id, N'x = 2 أو x = 4', 0),
    (@question17_id, N'x = 2 أو x = 3', 1),  -- Correct
    (@question17_id, N'x = -2 أو x = 3', 0),
    (@question17_id, N'x = 1 أو x = 6', 0);

-- Question 2: ما هي قيمة sin(30°)?
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'ما هي قيمة sin(30°)?', @multipleChoiceId, @mediumId, @publishedId, 
     @grade10_sem1_math_id, N'sin(30°) = 1/2 = 0.5', 30, 2, GETUTCDATE());

DECLARE @question18_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question18_id, N'0.5', 1),  -- Correct
    (@question18_id, N'0.866', 0),
    (@question18_id, N'0', 0),
    (@question18_id, N'1', 0);

-- Question 3: أوجد مجال الدالة f(x) = √(x - 3)
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'أوجد مجال الدالة f(x) = √(x - 3)', @multipleChoiceId, @hardId, @publishedId, 
     @grade10_sem1_math_id, N'يجب أن يكون x - 3 ≥ 0، وبالتالي x ≥ 3', 60, 3, GETUTCDATE());

DECLARE @question19_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question19_id, N'x ≥ 3', 1),  -- Correct
    (@question19_id, N'x > 3', 0),
    (@question19_id, N'x ≥ 0', 0),
    (@question19_id, N'جميع الأعداد الحقيقية', 0);

-- Question 4: ما هي قيمة tan(45°)?
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'ما هي قيمة tan(45°)?', @multipleChoiceId, @mediumId, @publishedId, 
     @grade10_sem1_math_id, N'tan(45°) = 1', 30, 2, GETUTCDATE());

DECLARE @question20_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question20_id, N'0', 0),
    (@question20_id, N'1', 1),  -- Correct
    (@question20_id, N'√3', 0),
    (@question20_id, N'∞', 0);

-- Grade 12 questions - Path pattern -1-4-60-*-*-
DECLARE @grade12_sem1_math_id INT = (
    SELECT TreeNodeId FROM dbo.TreeNodes 
    WHERE Path LIKE '-1-4-60-%-%-' AND Name LIKE 'Mathematics%'
);

-- Grade 12 - Calculus, statistics, advanced algebra
-- Question 1: أوجد مشتقة الدالة f(x) = 3x² + 2x - 5
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'أوجد مشتقة الدالة f(x) = 3x² + 2x - 5', @multipleChoiceId, @hardId, @publishedId, 
     @grade12_sem1_math_id, N'f''(x) = 6x + 2', 60, 3, GETUTCDATE());

DECLARE @question21_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question21_id, N'6x + 2', 1),  -- Correct
    (@question21_id, N'3x + 2', 0),
    (@question21_id, N'6x² + 2', 0),
    (@question21_id, N'6x', 0);

-- Question 2: احسب التكامل: ∫(2x + 3) dx
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'احسب التكامل: ∫(2x + 3) dx', @multipleChoiceId, @hardId, @publishedId, 
     @grade12_sem1_math_id, N'∫(2x + 3) dx = x² + 3x + C', 60, 3, GETUTCDATE());

DECLARE @question22_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question22_id, N'x² + 3x + C', 1),  -- Correct
    (@question22_id, N'2x² + 3x + C', 0),
    (@question22_id, N'x² + 3 + C', 0),
    (@question22_id, N'2x² + 3 + C', 0);

-- Question 3: إذا كانت دالة الكثافة الاحتمالية للتوزيع الطبيعي المعياري هي f(z)، فإن P(0 < Z < 1.96) تساوي تقريباً:
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'إذا كانت دالة الكثافة الاحتمالية للتوزيع الطبيعي المعياري هي f(z)، فإن P(0 < Z < 1.96) تساوي تقريباً:', @multipleChoiceId, @hardId, @publishedId, 
     @grade12_sem1_math_id, N'احتمال أن تكون Z بين 0 و 1.96 في التوزيع الطبيعي المعياري هو 0.475، وهذا يمثل 47.5% من الاحتمال.', 90, 4, GETUTCDATE());

DECLARE @question23_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question23_id, N'0.95', 0),
    (@question23_id, N'0.475', 1),  -- Correct
    (@question23_id, N'0.5', 0),
    (@question23_id, N'0.025', 0);

-- Question 4: أوجد نهاية الدالة: lim(x→∞) (3x² + 2x)/(x² + 1)
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'أوجد نهاية الدالة: lim(x→∞) (3x² + 2x)/(x² + 1)', @multipleChoiceId, @hardId, @publishedId, 
     @grade12_sem1_math_id, N'عندما x→∞، يمكن أن نقسم البسط والمقام على أعلى أس وهو x²، فنحصل على lim(x→∞) (3 + 2/x)/(1 + 1/x²) = 3', 90, 4, GETUTCDATE());

DECLARE @question24_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question24_id, N'0', 0),
    (@question24_id, N'3', 1),  -- Correct
    (@question24_id, N'∞', 0),
    (@question24_id, N'1', 0);

-- =============================================
-- ADD TRUE/FALSE QUESTIONS
-- =============================================
PRINT 'Adding True/False mathematics questions...';

-- Add True/False questions across various levels
-- Grade 1 True/False: 7 + 8 = 15
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'7 + 8 = 15', @trueFalseId, @easyId, @publishedId, 
     @grade1_sem1_math_id, N'7 + 8 = 15 هذه العبارة صحيحة.', 15, 1, GETUTCDATE());

DECLARE @question25_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question25_id, N'صحيح', 1),
    (@question25_id, N'خطأ', 0);

-- Grade 3 True/False: 3/4 أصغر من 1/2
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'3/4 أصغر من 1/2', @trueFalseId, @easyId, @publishedId, 
     @grade3_sem1_math_id, N'3/4 = 0.75 و 1/2 = 0.5، إذاً 3/4 أكبر من 1/2. العبارة خاطئة.', 20, 1, GETUTCDATE());

DECLARE @question26_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question26_id, N'صحيح', 0),
    (@question26_id, N'خطأ', 1);

-- Grade 6 True/False: لكل مثلث، مجموع قياسات الزوايا الداخلية يساوي 360 درجة
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'لكل مثلث، مجموع قياسات الزوايا الداخلية يساوي 360 درجة', @trueFalseId, @mediumId, @publishedId, 
     @grade6_sem1_math_id, N'مجموع قياسات الزوايا الداخلية في أي مثلث يساوي 180 درجة، وليس 360. العبارة خاطئة.', 30, 1, GETUTCDATE());

DECLARE @question27_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question27_id, N'صحيح', 0),
    (@question27_id, N'خطأ', 1);

-- Grade 8 True/False: نظرية فيثاغورس تنطبق على جميع أنواع المثلثات
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'نظرية فيثاغورس تنطبق على جميع أنواع المثلثات', @trueFalseId, @mediumId, @publishedId, 
     @grade8_sem1_math_id, N'نظرية فيثاغورس تنطبق فقط على المثلثات القائمة الزاوية. العبارة خاطئة.', 30, 1, GETUTCDATE());

DECLARE @question28_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question28_id, N'صحيح', 0),
    (@question28_id, N'خطأ', 1);

-- Grade 10 True/False: sin²(θ) + cos²(θ) = 1 لجميع قيم θ
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'sin²(θ) + cos²(θ) = 1 لجميع قيم θ', @trueFalseId, @hardId, @publishedId, 
     @grade10_sem1_math_id, N'هذه هي المتطابقة الأساسية في علم المثلثات وهي صحيحة دائماً.', 30, 1, GETUTCDATE());

DECLARE @question29_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question29_id, N'صحيح', 1),
    (@question29_id, N'خطأ', 0);

-- Grade 12 True/False: اشتقاق الدالة f(x) = x³ هو f'(x) = 2x²
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'اشتقاق الدالة f(x) = x³ هو f''(x) = 2x²', @trueFalseId, @hardId, @publishedId, 
     @grade12_sem1_math_id, N'اشتقاق x³ هو 3x². العبارة خاطئة.', 30, 1, GETUTCDATE());

DECLARE @question30_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question30_id, N'صحيح', 0),
    (@question30_id, N'خطأ', 1);

-- =============================================
-- ADD FILL-IN-THE-BLANK QUESTIONS
-- =============================================
PRINT 'Adding Fill-in-the-blank mathematics questions...';

-- Add Fill-in-the-blank questions across various levels
-- Grade 1 Fill-in-blank: 2 + ___ = 9
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'2 + ___ = 9', @fillInBlankId, @easyId, @publishedId, 
     @grade1_sem1_math_id, N'2 + 7 = 9، لذا الرقم المفقود هو 7.', 30, 1, GETUTCDATE());

DECLARE @question31_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question31_id, N'7', 1),
    (@question31_id, N'6', 0),
    (@question31_id, N'8', 0),
    (@question31_id, N'5', 0);

-- Grade 3 Fill-in-blank: اكتب الكسر المكافئ لـ 1/3 مع مقام 12: ___
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'اكتب الكسر المكافئ لـ 1/3 مع مقام 12: ___', @fillInBlankId, @mediumId, @publishedId, 
     @grade3_sem1_math_id, N'1/3 = ?/12، إذاً ? = 4.', 45, 1, GETUTCDATE());

DECLARE @question32_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question32_id, N'4/12', 1),
    (@question32_id, N'3/12', 0),
    (@question32_id, N'6/12', 0),
    (@question32_id, N'2/12', 0);

-- Grade 6 Fill-in-blank: إذا كان 3x = 18، فإن x = ___
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'إذا كان 3x = 18، فإن x = ___', @fillInBlankId, @mediumId, @publishedId, 
     @grade6_sem1_math_id, N'3x = 18، إذاً x = 6.', 30, 1, GETUTCDATE());

DECLARE @question33_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question33_id, N'6', 1),
    (@question33_id, N'5', 0),
    (@question33_id, N'7', 0),
    (@question33_id, N'9', 0);

-- Grade 8 Fill-in-blank: مساحة مستطيل طوله 8 سم وعرضه 5 سم = ___ سم²
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'مساحة مستطيل طوله 8 سم وعرضه 5 سم = ___ سم²', @fillInBlankId, @mediumId, @publishedId, 
     @grade8_sem1_math_id, N'مساحة المستطيل = الطول × العرض = 8 × 5 = 40 سم².', 30, 1, GETUTCDATE());

DECLARE @question34_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question34_id, N'40', 1),
    (@question34_id, N'36', 0),
    (@question34_id, N'13', 0),
    (@question34_id, N'45', 0);

-- Grade 10 Fill-in-blank: cos(90°) = ___
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'cos(90°) = ___', @fillInBlankId, @mediumId, @publishedId, 
     @grade10_sem1_math_id, N'cos(90°) = 0', 20, 1, GETUTCDATE());

DECLARE @question35_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question35_id, N'0', 1),
    (@question35_id, N'1', 0),
    (@question35_id, N'-1', 0),
    (@question35_id, N'0.5', 0);

-- Grade 12 Fill-in-blank: اشتقاق الدالة f(x) = ln(x) هو f'(x) = ___
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'اشتقاق الدالة f(x) = ln(x) هو f''(x) = ___', @fillInBlankId, @hardId, @publishedId, 
     @grade12_sem1_math_id, N'اشتقاق ln(x) هو 1/x.', 30, 1, GETUTCDATE());

DECLARE @question36_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question36_id, N'1/x', 1),
    (@question36_id, N'1', 0),
    (@question36_id, N'ln(x)', 0),
    (@question36_id, N'x', 0);

-- =============================================
-- ADD MULTIPLE RESPONSE QUESTIONS
-- =============================================
PRINT 'Adding Multiple Response mathematics questions...';

-- Add Multiple Response questions
-- Grade 3 Multiple Response: اختر الأعداد الأولية من القائمة التالية:
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'اختر الأعداد الأولية من القائمة التالية:', @multipleResponseId, @mediumId, @publishedId, 
     @grade3_sem1_math_id, N'الأعداد الأولية هي التي تقبل القسمة فقط على 1 وعلى نفسها. من القائمة، الأعداد الأولية هي 3، 5، 7، 11.', 60, 2, GETUTCDATE());

DECLARE @question37_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question37_id, N'3', 1),
    (@question37_id, N'4', 0),
    (@question37_id, N'5', 1),
    (@question37_id, N'6', 0),
    (@question37_id, N'7', 1),
    (@question37_id, N'9', 0),
    (@question37_id, N'10', 0),
    (@question37_id, N'11', 1);

-- Grade 6 Multiple Response: اختر الصفات التي تنطبق على متوازي الأضلاع:
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'اختر الصفات التي تنطبق على متوازي الأضلاع:', @multipleResponseId, @mediumId, @publishedId, 
     @grade6_sem1_math_id, N'متوازي الأضلاع هو شكل رباعي فيه كل ضلعين متقابلين متوازيان ومتساويان في الطول، وزواياه المتقابلة متساوية.', 60, 2, GETUTCDATE());

DECLARE @question38_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question38_id, N'الأضلاع المتقابلة متوازية', 1),
    (@question38_id, N'الأضلاع المتقابلة متساوية في الطول', 1),
    (@question38_id, N'جميع الأضلاع متساوية في الطول', 0),
    (@question38_id, N'جميع الزوايا قائمة', 0),
    (@question38_id, N'الزوايا المتقابلة متساوية', 1),
    (@question38_id, N'القطران متساويان في الطول', 0);

-- Grade 10 Multiple Response: اختر العبارات الصحيحة عن الدالة y = 2x² - 4x + 1:
INSERT INTO dbo.Questions (
    Text, QuestionTypeId, DifficultyLevelId, QuestionStatusId, 
    PrimaryTreeNodeId, Solution, EstimatedTimeSec, Points, CreatedAt
)
VALUES 
    (N'اختر العبارات الصحيحة عن الدالة y = 2x² - 4x + 1:', @multipleResponseId, @hardId, @publishedId, 
     @grade10_sem1_math_id, N'هذه الدالة تربيعية مع معامل موجب للحد التربيعي، لذا تفتح لأعلى. معادلة تمثلها قطع مكافئ.', 90, 3, GETUTCDATE());

DECLARE @question39_id INT = SCOPE_IDENTITY();

INSERT INTO dbo.Answers (QuestionId, Text, IsCorrect) 
VALUES 
    (@question39_id, N'الدالة تمثل قطع مكافئ', 1),
    (@question39_id, N'الدالة تفتح لأعلى', 1),
    (@question39_id, N'أقل قيمة للدالة هي صفر', 0),
    (@question39_id, N'المعامل الثاني للدالة سالب', 1),
    (@question39_id, N'للدالة جذران حقيقيان فقط', 1),
    (@question39_id, N'الدالة ليس لها نقطة تقاطع مع محور y', 0);

PRINT 'Successfully inserted demo questions and answers!';
GO