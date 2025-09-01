USE IkhtibarDb;
GO

-- Update existing users with proper BCrypt hashes for password "password"
UPDATE Users 
SET PasswordHash = '$2a$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi'
WHERE Email IN ('admin@ikhtibar.com', 'teacher1@ikhtibar.com', 'student1@ikhtibar.com');

-- Verify the update
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

PRINT 'Password hashes updated successfully!';
PRINT 'All users now have password: password';
PRINT 'BCrypt hash length: 60 characters';
GO

