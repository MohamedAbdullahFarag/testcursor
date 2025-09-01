-- Quick fix for audit logs authorization issue
-- Run this in your SQL Server Management Studio or database tool

-- First, check what users exist
SELECT Id, Email, UserName FROM Users;

-- Check current user roles
SELECT u.Id, u.Email, ur.RoleName 
FROM Users u 
LEFT JOIN UserRoles ur ON u.Id = ur.UserId;

-- Add Admin role to first user (adjust UserId as needed)
-- Replace '1' with the actual user ID you want to give admin access
INSERT INTO UserRoles (UserId, RoleName, CreatedAt) 
VALUES (1, 'Admin', GETUTCDATE());

-- Add Supervisor role as backup
INSERT INTO UserRoles (UserId, RoleName, CreatedAt) 
VALUES (1, 'Supervisor', GETUTCDATE());

-- Verify the roles were added
SELECT u.Id, u.Email, ur.RoleName 
FROM Users u 
LEFT JOIN UserRoles ur ON u.Id = ur.UserId
WHERE u.Id = 1;
