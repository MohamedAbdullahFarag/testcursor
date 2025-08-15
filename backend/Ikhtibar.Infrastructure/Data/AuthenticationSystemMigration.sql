-- Authentication System Database Migration
-- This script creates all necessary tables for the comprehensive authentication system

-- Create RefreshTokens table for secure token rotation
CREATE TABLE [dbo].[RefreshTokens] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [TokenHash] NVARCHAR(255) NOT NULL,
    [UserId] INT NOT NULL,
    [IssuedAt] DATETIME2 NOT NULL,
    [ExpiresAt] DATETIME2 NOT NULL,
    [RevokedAt] DATETIME2 NULL,
    [RevocationReason] NVARCHAR(200) NULL,
    [ClientIpAddress] NVARCHAR(45) NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ModifiedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [DeletedAt] DATETIME2 NULL,
    CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id])
);

-- Create indexes for better performance
CREATE INDEX [IX_RefreshTokens_TokenHash] ON [dbo].[RefreshTokens] ([TokenHash]);
CREATE INDEX [IX_RefreshTokens_UserId] ON [dbo].[RefreshTokens] ([UserId]);
CREATE INDEX [IX_RefreshTokens_ExpiresAt] ON [dbo].[RefreshTokens] ([ExpiresAt]);
CREATE INDEX [IX_RefreshTokens_RevokedAt] ON [dbo].[RefreshTokens] ([RevokedAt]);
CREATE INDEX [IX_RefreshTokens_IsDeleted] ON [dbo].[RefreshTokens] ([IsDeleted]);

-- Create foreign key constraint to Users table
ALTER TABLE [dbo].[RefreshTokens] ADD CONSTRAINT [FK_RefreshTokens_Users] 
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]);

-- Create stored procedure for cleaning up expired refresh tokens
CREATE PROCEDURE [dbo].[CleanupExpiredRefreshTokens]
    @BatchSize INT = 100,
    @MaxAgeDays INT = 30
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Now DATETIME2 = GETUTCDATE();
    DECLARE @CutoffDate DATETIME2 = DATEADD(DAY, -@MaxAgeDays, @Now);
    
    -- Mark expired tokens as deleted
    UPDATE [dbo].[RefreshTokens]
    SET [IsDeleted] = 1,
        [DeletedAt] = @Now,
        [ModifiedAt] = @Now
    WHERE [Id] IN (
        SELECT TOP (@BatchSize) [Id]
        FROM [dbo].[RefreshTokens]
        WHERE [IsDeleted] = 0
          AND ([ExpiresAt] < @Now OR [RevokedAt] < @CutoffDate)
        ORDER BY [ExpiresAt] ASC
    );
    
    -- Return count of cleaned up tokens
    SELECT @@ROWCOUNT as CleanedUpCount;
END;

-- Create stored procedure for getting refresh token statistics
CREATE PROCEDURE [dbo].[GetRefreshTokenStats]
    @UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Now DATETIME2 = GETUTCDATE();
    
    SELECT 
        COUNT(*) as TotalTokens,
        SUM(CASE WHEN [RevokedAt] IS NULL AND [ExpiresAt] > @Now THEN 1 ELSE 0 END) as ActiveTokens,
        SUM(CASE WHEN [ExpiresAt] <= @Now THEN 1 ELSE 0 END) as ExpiredTokens,
        SUM(CASE WHEN [RevokedAt] IS NOT NULL THEN 1 ELSE 0 END) as RevokedTokens,
        MAX([IssuedAt]) as LastIssuedAt,
        MAX(CASE WHEN [RevokedAt] IS NOT NULL THEN [RevokedAt] ELSE [IssuedAt] END) as LastUsedAt
    FROM [dbo].[RefreshTokens]
    WHERE [IsDeleted] = 0
      AND (@UserId IS NULL OR [UserId] = @UserId);
END;

-- Create view for refresh token summary
CREATE VIEW [dbo].[v_RefreshTokenSummary] AS
SELECT 
    rt.[Id],
    rt.[UserId],
    u.[Email] as UserEmail,
    u.[FirstName] + ' ' + u.[LastName] as UserFullName,
    rt.[IssuedAt],
    rt.[ExpiresAt],
    rt.[RevokedAt],
    rt.[RevocationReason],
    rt.[ClientIpAddress],
    rt.[UserAgent],
    rt.[CreatedAt],
    CASE 
        WHEN rt.[RevokedAt] IS NOT NULL THEN 'Revoked'
        WHEN rt.[ExpiresAt] <= GETUTCDATE() THEN 'Expired'
        ELSE 'Active'
    END as Status,
    CASE 
        WHEN rt.[RevokedAt] IS NOT NULL THEN DATEDIFF(MINUTE, rt.[IssuedAt], rt.[RevokedAt])
        WHEN rt.[ExpiresAt] <= GETUTCDATE() THEN DATEDIFF(MINUTE, rt.[IssuedAt], rt.[ExpiresAt])
        ELSE DATEDIFF(MINUTE, rt.[IssuedAt], GETUTCDATE())
    END as DurationMinutes
FROM [dbo].[RefreshTokens] rt
INNER JOIN [dbo].[Users] u ON rt.[UserId] = u.[UserId]
WHERE rt.[IsDeleted] = 0;

-- Create view for active refresh tokens
CREATE VIEW [dbo].[v_ActiveRefreshTokens] AS
SELECT 
    rt.[Id],
    rt.[UserId],
    u.[Email] as UserEmail,
    u.[FirstName] + ' ' + u.[LastName] as UserFullName,
    rt.[IssuedAt],
    rt.[ExpiresAt],
    rt.[ClientIpAddress],
    rt.[UserAgent],
    DATEDIFF(MINUTE, GETUTCDATE(), rt.[ExpiresAt]) as MinutesUntilExpiry
FROM [dbo].[RefreshTokens] rt
INNER JOIN [dbo].[Users] u ON rt.[UserId] = u.[UserId]
WHERE rt.[IsDeleted] = 0
  AND rt.[RevokedAt] IS NULL
  AND rt.[ExpiresAt] > GETUTCDATE();

-- Create function to check if user has valid refresh tokens
CREATE FUNCTION [dbo].[HasValidRefreshTokens]
(
    @UserId INT
)
RETURNS BIT
AS
BEGIN
    DECLARE @HasValid BIT = 0;
    
    IF EXISTS (
        SELECT 1 
        FROM [dbo].[RefreshTokens] 
        WHERE [UserId] = @UserId 
          AND [IsDeleted] = 0 
          AND [RevokedAt] IS NULL 
          AND [ExpiresAt] > GETUTCDATE()
    )
    BEGIN
        SET @HasValid = 1;
    END
    
    RETURN @HasValid;
END;

-- Create function to get user's active token count
CREATE FUNCTION [dbo].[GetUserActiveTokenCount]
(
    @UserId INT
)
RETURNS INT
AS
BEGIN
    DECLARE @ActiveCount INT = 0;
    
    SELECT @ActiveCount = COUNT(*)
    FROM [dbo].[RefreshTokens] 
    WHERE [UserId] = @UserId 
      AND [IsDeleted] = 0 
      AND [RevokedAt] IS NULL 
      AND [ExpiresAt] > GETUTCDATE();
    
    RETURN @ActiveCount;
END;

-- Insert default data if needed (optional)
-- This can be used for testing or initial setup

-- Create a background job for cleaning up expired tokens (if using SQL Server Agent)
-- EXEC msdb.dbo.sp_add_job
--     @job_name = N'Cleanup Expired Refresh Tokens',
--     @enabled = 1,
--     @description = N'Clean up expired refresh tokens daily';

-- EXEC msdb.dbo.sp_add_jobstep
--     @job_name = N'Cleanup Expired Refresh Tokens',
--     @step_name = N'Cleanup Tokens',
--     @subsystem = N'TSQL',
--     @command = N'EXEC [dbo].[CleanupExpiredRefreshTokens] @BatchSize = 1000, @MaxAgeDays = 30';

-- EXEC msdb.dbo.sp_add_schedule
--     @schedule_name = N'Daily Token Cleanup',
--     @freq_type = 4, -- Daily
--     @freq_interval = 1,
--     @active_start_time = 020000; -- 2:00 AM

-- EXEC msdb.dbo.sp_attach_schedule
--     @job_name = N'Cleanup Expired Refresh Tokens',
--     @schedule_name = N'Daily Token Cleanup';

-- Grant permissions (adjust as needed for your security model)
-- GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[RefreshTokens] TO [YourAppRole];
-- GRANT EXECUTE ON [dbo].[CleanupExpiredRefreshTokens] TO [YourAppRole];
-- GRANT EXECUTE ON [dbo].[GetRefreshTokenStats] TO [YourAppRole];
-- GRANT SELECT ON [dbo].[v_RefreshTokenSummary] TO [YourAppRole];
-- GRANT SELECT ON [dbo].[v_ActiveRefreshTokens] TO [YourAppRole];
-- GRANT EXECUTE ON [dbo].[HasValidRefreshTokens] TO [YourAppRole];
-- GRANT EXECUTE ON [dbo].[GetUserActiveTokenCount] TO [YourAppRole];

PRINT 'Authentication system database migration completed successfully.';
PRINT 'Created RefreshTokens table with indexes and stored procedures.';
PRINT 'Created views and functions for token management.';
PRINT 'Remember to grant appropriate permissions to your application role.';
