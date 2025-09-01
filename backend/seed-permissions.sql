-- Seed default permissions for Ikhtibar system
-- This script will insert the comprehensive set of permissions as defined in PermissionService.cs

-- First, check if any permissions already exist
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE IsDeleted = 0)
BEGIN
    PRINT 'Seeding default permissions...';
    
    -- User Management permissions
    INSERT INTO Permissions (Code, Name, Description, Category, IsDeleted, CreatedAt)
    VALUES 
    ('user.view', 'View Users', 'View and list user accounts', 'User Management', 0, GETDATE()),
    ('user.create', 'Create Users', 'Create new user accounts', 'User Management', 0, GETDATE()),
    ('user.edit', 'Edit Users', 'Edit and update user accounts', 'User Management', 0, GETDATE()),
    ('user.delete', 'Delete Users', 'Delete user accounts', 'User Management', 0, GETDATE()),
    ('user.manage_roles', 'Manage User Roles', 'Assign and modify user roles', 'User Management', 0, GETDATE()),
    ('user.reset_password', 'Reset User Password', 'Reset user passwords', 'User Management', 0, GETDATE()),
    ('user.activate_deactivate', 'Activate/Deactivate Users', 'Enable or disable user accounts', 'User Management', 0, GETDATE()),
    
    -- Question Management permissions
    ('question.view', 'View Questions', 'View and browse questions', 'Question Management', 0, GETDATE()),
    ('question.create', 'Create Questions', 'Create new questions', 'Question Management', 0, GETDATE()),
    ('question.edit', 'Edit Questions', 'Edit and update questions', 'Question Management', 0, GETDATE()),
    ('question.delete', 'Delete Questions', 'Delete questions', 'Question Management', 0, GETDATE()),
    ('question.review', 'Review Questions', 'Review and approve questions', 'Question Management', 0, GETDATE()),
    ('question.publish', 'Publish Questions', 'Publish questions for use', 'Question Management', 0, GETDATE()),
    
    -- Exam Management permissions
    ('exam.view', 'View Exams', 'View and list exams', 'Exam Management', 0, GETDATE()),
    ('exam.create', 'Create Exams', 'Create new exams', 'Exam Management', 0, GETDATE()),
    ('exam.edit', 'Edit Exams', 'Edit and update exams', 'Exam Management', 0, GETDATE()),
    ('exam.delete', 'Delete Exams', 'Delete exams', 'Exam Management', 0, GETDATE()),
    ('exam.schedule', 'Schedule Exams', 'Schedule and manage exam sessions', 'Exam Management', 0, GETDATE()),
    ('exam.monitor', 'Monitor Exams', 'Monitor ongoing exams', 'Exam Management', 0, GETDATE()),
    
    -- Grading permissions
    ('grading.view', 'View Grading', 'View grading and results', 'Grading', 0, GETDATE()),
    ('grading.grade_exams', 'Grade Exams', 'Grade and score exams', 'Grading', 0, GETDATE()),
    ('grading.review_grades', 'Review Grades', 'Review and verify grades', 'Grading', 0, GETDATE()),
    ('grading.publish_results', 'Publish Results', 'Publish exam results', 'Grading', 0, GETDATE()),
    
    -- Reports permissions
    ('reports.view', 'View Reports', 'View system reports', 'Reports', 0, GETDATE()),
    ('reports.generate', 'Generate Reports', 'Generate custom reports', 'Reports', 0, GETDATE()),
    ('reports.export', 'Export Reports', 'Export reports to various formats', 'Reports', 0, GETDATE()),
    
    -- Analytics permissions
    ('analytics.view', 'View Analytics', 'View system analytics', 'Analytics', 0, GETDATE()),
    ('analytics.dashboard', 'Analytics Dashboard', 'Access analytics dashboard', 'Analytics', 0, GETDATE()),
    
    -- System Administration permissions
    ('system.settings', 'System Settings', 'Manage system settings', 'System Administration', 0, GETDATE()),
    ('system.backup', 'System Backup', 'Perform system backups', 'System Administration', 0, GETDATE()),
    ('system.logs', 'View System Logs', 'View system logs and audit trails', 'System Administration', 0, GETDATE()),
    ('system.maintenance', 'System Maintenance', 'Perform system maintenance tasks', 'System Administration', 0, GETDATE());
    
    PRINT 'Default permissions seeded successfully.';
END
ELSE
BEGIN
    PRINT 'Permissions already exist. Skipping seeding.';
END

-- Show the count of permissions created
SELECT 
    Category,
    COUNT(*) as PermissionCount
FROM Permissions 
WHERE IsDeleted = 0
GROUP BY Category
ORDER BY Category;

PRINT 'Permission seeding completed.';
