-- Audit Logging Migration Script
-- Creates comprehensive audit logging tables and indexes

-- Create AuditLogs table
CREATE TABLE dbo.AuditLogs (
    AuditLogId      INT            IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserId          INT            NULL,         -- FK to Users
    UserIdentifier  NVARCHAR(255)  NOT NULL,    -- Email/Username for audit trail
    Action          NVARCHAR(100)  NOT NULL,    -- Action performed
    EntityType      NVARCHAR(50)   NOT NULL,    -- Type of entity affected
    EntityId        NVARCHAR(50)   NULL,        -- ID of affected entity
    Details         NVARCHAR(MAX)  NULL,        -- JSON details
    OldValues       NVARCHAR(MAX)  NULL,        -- JSON of old values
    NewValues       NVARCHAR(MAX)  NULL,        -- JSON of new values
    IpAddress       NVARCHAR(45)   NULL,        -- Client IP
    UserAgent       NVARCHAR(500)  NULL,        -- Browser info
    SessionId       NVARCHAR(100)  NULL,        -- Session identifier
    Severity        TINYINT        NOT NULL DEFAULT(2), -- AuditSeverity enum
    Category        TINYINT        NOT NULL DEFAULT(0), -- AuditCategory enum
    Timestamp       DATETIME2      NOT NULL DEFAULT(SYSUTCDATETIME()),
    IsSystemAction  BIT            NOT NULL DEFAULT(0),
    CreatedAt       DATETIME2      NOT NULL DEFAULT(SYSUTCDATETIME()),
    CreatedBy       INT            NULL,
    ModifiedAt      DATETIME2      NULL,
    ModifiedBy      INT            NULL,
    DeletedAt       DATETIME2      NULL,
    DeletedBy       INT            NULL,
    IsDeleted       BIT            NOT NULL DEFAULT(0),
    RowVersion      ROWVERSION     NOT NULL,
    
    CONSTRAINT FK_AuditLogs_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId)
);

-- Create indexes for performance
CREATE INDEX IX_AuditLogs_UserId_Timestamp ON dbo.AuditLogs(UserId, Timestamp DESC);
CREATE INDEX IX_AuditLogs_Category_Severity ON dbo.AuditLogs(Category, Severity);
CREATE INDEX IX_AuditLogs_Timestamp ON dbo.AuditLogs(Timestamp DESC);
CREATE INDEX IX_AuditLogs_EntityType_EntityId ON dbo.AuditLogs(EntityType, EntityId);
CREATE INDEX IX_AuditLogs_Action ON dbo.AuditLogs(Action);
