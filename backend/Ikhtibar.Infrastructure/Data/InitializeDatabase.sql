-- Complete Ikhtibar Database Schema
-- Combines all required tables including audit logging

-- ================= Lookup Tables =================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QuestionTypes]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.QuestionTypes (
        QuestionTypeId    INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Unique question type identifier
        Name              NVARCHAR(100)  NOT NULL                             -- e.g. 'Multiple Choice'
    );
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DifficultyLevels]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.DifficultyLevels (
        DifficultyLevelId INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Unique difficulty level identifier
        Name              NVARCHAR(50)   NOT NULL                             -- e.g. 'Easy', 'Medium', 'Hard'
    );
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QuestionStatuses]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.QuestionStatuses (
        QuestionStatusId  INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- e.g. 'Draft', 'Published'
        Name              NVARCHAR(50)   NOT NULL
    );
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MediaTypes]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.MediaTypes (
        MediaTypeId       INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- e.g. 'Image', 'Video'
        Name              NVARCHAR(50)   NOT NULL
    );
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TreeNodeTypes]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.TreeNodeTypes (
        TreeNodeTypeId    INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- e.g. 'Category', 'Topic'
        Name              NVARCHAR(100)  NOT NULL
    );
END

-- ================= Core Tables =================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.Users (
        UserId            INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Primary key for users
        Username          NVARCHAR(100)  NOT NULL UNIQUE ,-- 'Login username',
        Email             NVARCHAR(200)  NOT NULL UNIQUE ,-- 'Contact email',
        FirstName         NVARCHAR(100)  NOT NULL ,-- 'User first name',
        LastName          NVARCHAR(100)  NOT NULL ,-- 'User last name',
        PasswordHash      NVARCHAR(256)  NOT NULL ,-- 'Hashed password',
        PhoneNumber       NVARCHAR(20)   NULL ,-- 'Optional phone number',
        PreferredLanguage NVARCHAR(10)   NULL ,-- 'e.g. en, ar',
        IsActive          BIT            NOT NULL CONSTRAINT DF_Users_IsActive      DEFAULT(1)   ,-- 'Flag if account is active',
        EmailVerified     BIT            NOT NULL CONSTRAINT DF_Users_EmailVerified DEFAULT(0)   ,-- 'Has email been verified',
        PhoneVerified     BIT            NOT NULL CONSTRAINT DF_Users_PhoneVerified DEFAULT(0)   ,-- 'Has phone been verified',
        CreatedAt         DATETIME2 NOT NULL CONSTRAINT DF_Users_CreatedAt     DEFAULT(SYSUTCDATETIME()) ,-- 'Record creation timestamp',
        CreatedBy         INT            NULL ,-- 'UserId who created record',
        ModifiedAt        DATETIME2 NULL ,-- 'Last modification timestamp',
        ModifiedBy        INT            NULL ,-- 'UserId who last modified',
        DeletedAt         DATETIME2 NULL ,-- 'Deletion timestamp for soft delete',
        DeletedBy         INT            NULL ,-- 'UserId who deleted (soft)',
        IsDeleted         BIT            NOT NULL CONSTRAINT DF_Users_IsDeleted    DEFAULT(0)   ,-- 'Soft delete flag',
        RowVersion        ROWVERSION     NOT NULL ,-- 'Optimistic concurrency token'
    );
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Roles]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.Roles (
        RoleId            INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Primary key for roles
        Code              NVARCHAR(50)   NOT NULL UNIQUE ,-- 'System role code',
        Name              NVARCHAR(100)  NOT NULL ,-- 'Role display name',
        Description       NVARCHAR(500)  NULL ,-- 'Role description',
        IsSystemRole      BIT            NOT NULL CONSTRAINT DF_Roles_IsSystemRole DEFAULT(0) ,-- 'Built-in role flag',
        CreatedAt         DATETIME2 NOT NULL CONSTRAINT DF_Roles_CreatedAt     DEFAULT(SYSUTCDATETIME()) ,-- 'Timestamp when role created',
        CreatedBy         INT            NULL ,-- 'UserId who created role',
        ModifiedAt        DATETIME2 NULL ,-- 'Timestamp when role modified',
        ModifiedBy        INT            NULL ,-- 'UserId who modified role',
        DeletedAt         DATETIME2 NULL ,-- 'Timestamp when role deleted',
        DeletedBy         INT            NULL ,-- 'UserId who deleted role',
        IsDeleted         BIT            NOT NULL CONSTRAINT DF_Roles_IsDeleted    DEFAULT(0) ,-- 'Soft delete flag',
        RowVersion        ROWVERSION     NOT NULL ,-- 'Concurrency token'
    );
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Permissions]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.Permissions (
        PermissionId      INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Primary key for permissions
        Code              NVARCHAR(100)  NOT NULL UNIQUE ,-- 'Permission code',
        Name              NVARCHAR(100)  NOT NULL ,-- 'Permission name',
        Description       NVARCHAR(500)  NULL ,-- 'Permission description',
        Category          NVARCHAR(100)  NOT NULL ,-- 'Permission category',
        CreatedAt         DATETIME2 NOT NULL CONSTRAINT DF_Permissions_CreatedAt DEFAULT(SYSUTCDATETIME()) ,-- 'When permission created',
        CreatedBy         INT            NULL ,-- 'UserId creator',
        ModifiedAt        DATETIME2 NULL ,-- 'Last modified timestamp',
        ModifiedBy        INT            NULL ,-- 'UserId last modifier',
        DeletedAt         DATETIME2 NULL ,-- 'When soft deleted',
        DeletedBy         INT            NULL ,-- 'UserId who deleted',
        IsDeleted         BIT            NOT NULL CONSTRAINT DF_Permissions_IsDeleted DEFAULT(0) ,-- 'Soft delete flag',
        RowVersion        ROWVERSION     NOT NULL ,-- 'Concurrency token'
    );
END

-- Junction: Roles <-> Permissions
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RolePermissions]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.RolePermissions (
        RoleId            INT            NOT NULL ,-- 'FK to Roles',
        PermissionId      INT            NOT NULL ,-- 'FK to Permissions',
        AssignedAt        DATETIME2 NOT NULL CONSTRAINT DF_RP_AssignedAt DEFAULT(SYSUTCDATETIME()) ,-- 'When assigned',
        AssignedBy        INT            NULL ,-- 'UserId who assigned',
        CONSTRAINT PK_RolePermissions     PRIMARY KEY (RoleId, PermissionId),
        CONSTRAINT FK_RP_Roles            FOREIGN KEY (RoleId)       REFERENCES dbo.Roles(RoleId),
        CONSTRAINT FK_RP_Permissions      FOREIGN KEY (PermissionId) REFERENCES dbo.Permissions(PermissionId),
        CONSTRAINT FK_RP_Users_AssignedBy FOREIGN KEY (AssignedBy)   REFERENCES dbo.Users(UserId)
    );
END

-- Junction: Users <-> Roles
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserRoles]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.UserRoles (
        UserId            INT            NOT NULL ,-- 'FK to Users',
        RoleId            INT            NOT NULL ,-- 'FK to Roles',
        AssignedAt        DATETIME2 NOT NULL CONSTRAINT DF_UR_AssignedAt DEFAULT(SYSUTCDATETIME()) ,-- 'When assigned',
        AssignedBy        INT            NULL ,-- 'UserId who assigned',
        CONSTRAINT PK_UserRoles           PRIMARY KEY (UserId, RoleId),
        CONSTRAINT FK_UR_Users            FOREIGN KEY (UserId)       REFERENCES dbo.Users(UserId),
        CONSTRAINT FK_UR_Roles            FOREIGN KEY (RoleId)       REFERENCES dbo.Roles(RoleId),
        CONSTRAINT FK_UR_Users_AssignedBy FOREIGN KEY (AssignedBy)   REFERENCES dbo.Users(UserId)
    );
END

-- Tree nodes with path enumeration backup
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TreeNodes]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.TreeNodes (
        TreeNodeId        INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Node identifier
        Name              NVARCHAR(200)  NOT NULL ,-- 'Node display name',
        Code              NVARCHAR(50)   NOT NULL ,-- 'Unique code for node',
        Description       NVARCHAR(500)  NULL ,-- 'Optional description',
        TreeNodeTypeId    INT            NOT NULL ,-- 'FK to TreeNodeTypes',
        ParentId          INT            NULL ,-- 'FK to parent TreeNodeId',
        OrderIndex        INT            NOT NULL CONSTRAINT DF_TN_OrderIndex DEFAULT(0) ,-- 'Display order',
        Path              NVARCHAR(100) NOT NULL ,-- 'Materialized path, e.g. -1-4-9-',
        IsActive          BIT            NOT NULL CONSTRAINT DF_TN_IsActive DEFAULT(1) ,-- 'Active flag',
        CreatedAt         DATETIME2 NOT NULL CONSTRAINT DF_TN_CreatedAt DEFAULT(SYSUTCDATETIME()) ,-- 'Creation timestamp',
        CreatedBy         INT            NULL ,-- 'UserId who created',
        ModifiedAt        DATETIME2 NULL ,-- 'Last modified timestamp',
        ModifiedBy        INT            NULL ,-- 'UserId who modified',
        DeletedAt         DATETIME2 NULL ,-- 'Soft delete timestamp',
        DeletedBy         INT            NULL ,-- 'UserId who deleted',
        IsDeleted         BIT            NOT NULL CONSTRAINT DF_TN_IsDeleted DEFAULT(0) ,-- 'Soft delete flag',
        RowVersion        ROWVERSION     NOT NULL ,-- 'Concurrency token',
        CONSTRAINT FK_TN_Parent            FOREIGN KEY (ParentId)       REFERENCES dbo.TreeNodes(TreeNodeId),
        CONSTRAINT FK_TN_Types             FOREIGN KEY (TreeNodeTypeId) REFERENCES dbo.TreeNodeTypes(TreeNodeTypeId),
        CONSTRAINT FK_TN_Users_CreatedBy   FOREIGN KEY (CreatedBy)      REFERENCES dbo.Users(UserId),
        CONSTRAINT FK_TN_Users_ModifiedBy  FOREIGN KEY (ModifiedBy)     REFERENCES dbo.Users(UserId),
        CONSTRAINT FK_TN_Users_DeletedBy   FOREIGN KEY (DeletedBy)      REFERENCES dbo.Users(UserId)
    );
END

-- Media attachments
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Media]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.Media (
        MediaId           INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Media identifier
        Url               NVARCHAR(2083) NOT NULL ,-- 'Resource URL',
        MediaTypeId       INT            NOT NULL ,-- 'FK to MediaTypes',
        UploadedBy        INT            NOT NULL ,-- 'UserId uploader',
        UploadedAt        DATETIME2 NOT NULL CONSTRAINT DF_Media_UploadedAt DEFAULT(SYSUTCDATETIME()) ,-- 'Upload timestamp',
        RowVersion        ROWVERSION     NOT NULL ,-- 'Concurrency token',
        CONSTRAINT FK_Media_Types          FOREIGN KEY (MediaTypeId) REFERENCES dbo.MediaTypes(MediaTypeId),
        CONSTRAINT FK_Media_Users          FOREIGN KEY (UploadedBy)   REFERENCES dbo.Users(UserId)
    );
END

-- Questions
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Questions]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.Questions (
        QuestionId        INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Question identifier
        Text              NVARCHAR(500) NOT NULL ,-- 'Full question text',
        QuestionTypeId    INT            NOT NULL ,-- 'FK to QuestionTypes',
        DifficultyLevelId INT            NOT NULL ,-- 'FK to DifficultyLevels',
        Solution          NVARCHAR(500) NULL ,-- 'Solution explanation',
        EstimatedTimeSec  INT            NULL ,-- 'Estimated solve time (sec)',
        Points            INT            NULL ,-- 'Score points',
        QuestionStatusId  INT            NOT NULL ,-- 'FK to QuestionStatuses',
        PrimaryTreeNodeId INT            NOT NULL ,-- 'FK to primary TreeNodeId',
        Version           NVARCHAR(50)   NULL ,-- 'Version string',
        Tags              NVARCHAR(200)  NULL ,-- 'Comma-separated tags',
        Metadata          NVARCHAR(500) NULL ,-- 'JSON metadata',
        OriginalQuestionId INT           NULL ,-- 'FK to original question for versions',
        CreatedAt         DATETIME2 NOT NULL CONSTRAINT DF_Q_CreatedAt DEFAULT(SYSUTCDATETIME()) ,-- 'Creation timestamp',
        CreatedBy         INT            NULL ,-- 'UserId creator',
        ModifiedAt        DATETIME2 NULL ,-- 'Last mod timestamp',
        ModifiedBy        INT            NULL ,-- 'UserId modifier',
        DeletedAt         DATETIME2 NULL ,-- 'Soft delete timestamp',
        DeletedBy         INT            NULL ,-- 'UserId deleter',
        IsDeleted         BIT            NOT NULL CONSTRAINT DF_Q_IsDeleted DEFAULT(0) ,-- 'Soft delete flag',
        RowVersion        ROWVERSION     NOT NULL ,-- 'Concurrency token',
        CONSTRAINT FK_Q_Types               FOREIGN KEY (QuestionTypeId)    REFERENCES dbo.QuestionTypes(QuestionTypeId),
        CONSTRAINT FK_Q_DifficultyLevels    FOREIGN KEY (DifficultyLevelId) REFERENCES dbo.DifficultyLevels(DifficultyLevelId),
        CONSTRAINT FK_Q_Status             FOREIGN KEY (QuestionStatusId)   REFERENCES dbo.QuestionStatuses(QuestionStatusId),
        CONSTRAINT FK_Q_PrimaryTreeNode    FOREIGN KEY (PrimaryTreeNodeId)  REFERENCES dbo.TreeNodes(TreeNodeId),
        CONSTRAINT FK_Q_Users_CreatedBy     FOREIGN KEY (CreatedBy)          REFERENCES dbo.Users(UserId),
        CONSTRAINT FK_Q_Users_ModifiedBy    FOREIGN KEY (ModifiedBy)         REFERENCES dbo.Users(UserId),
        CONSTRAINT FK_Q_Users_DeletedBy     FOREIGN KEY (DeletedBy)          REFERENCES dbo.Users(UserId)
    );
END

-- Answers
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Answers]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.Answers (
        AnswerId          INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Answer identifier
        QuestionId        INT            NOT NULL ,-- 'FK to Questions',
        Text              NVARCHAR(500) NOT NULL ,-- 'Answer text',
        IsCorrect         BIT            NOT NULL CONSTRAINT DF_Answers_IsCorrect DEFAULT(0) ,-- 'Correct answer flag',
        CreatedAt         DATETIME2 NOT NULL CONSTRAINT DF_A_CreatedAt DEFAULT(SYSUTCDATETIME()) ,-- 'Creation timestamp',
        CreatedBy         INT            NULL ,-- 'UserId creator',
        ModifiedAt        DATETIME2 NULL ,-- 'Last mod timestamp',
        ModifiedBy        INT            NULL ,-- 'UserId modifier',
        DeletedAt         DATETIME2 NULL ,-- 'Soft delete timestamp',
        DeletedBy         INT            NULL ,-- 'UserId deleter',
        IsDeleted         BIT            NOT NULL CONSTRAINT DF_A_IsDeleted DEFAULT(0) ,-- 'Soft delete flag',
        RowVersion        ROWVERSION     NOT NULL ,-- 'Concurrency token',
        CONSTRAINT FK_A_Questions          FOREIGN KEY (QuestionId)   REFERENCES dbo.Questions(QuestionId),
        CONSTRAINT FK_A_Users_CreatedBy    FOREIGN KEY (CreatedBy)    REFERENCES dbo.Users(UserId),
        CONSTRAINT FK_A_Users_ModifiedBy   FOREIGN KEY (ModifiedBy)   REFERENCES dbo.Users(UserId),
        CONSTRAINT FK_A_Users_DeletedBy    FOREIGN KEY (DeletedBy)    REFERENCES dbo.Users(UserId)
    );
END

-- Media junctions
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QuestionMedia]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.QuestionMedia (
        QuestionId        INT            NOT NULL ,-- 'FK to Questions',
        MediaId           INT            NOT NULL ,-- 'FK to Media',
        CONSTRAINT PK_QuestionMedia       PRIMARY KEY (QuestionId, MediaId),
        CONSTRAINT FK_QM_Questions         FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId),
        CONSTRAINT FK_QM_Media             FOREIGN KEY (MediaId)    REFERENCES dbo.Media(MediaId)
    );
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AnswerMedia]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.AnswerMedia (
        AnswerId          INT            NOT NULL ,-- 'FK to Answers',
        MediaId           INT            NOT NULL ,-- 'FK to Media',
        CONSTRAINT PK_AnswerMedia         PRIMARY KEY (AnswerId, MediaId),
        CONSTRAINT FK_AM_Answers           FOREIGN KEY (AnswerId)   REFERENCES dbo.Answers(AnswerId),
        CONSTRAINT FK_AM_Media             FOREIGN KEY (MediaId)    REFERENCES dbo.Media(MediaId)
    );
END

-- ================= Authentication Tables =================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RefreshTokens]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.RefreshTokens (
        RefreshTokenId    INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Primary key
        TokenHash         NVARCHAR(256)  NOT NULL ,-- 'Hashed refresh token value',
        UserId            INT            NOT NULL ,-- 'FK to Users',
        IssuedAt          DATETIME2      NOT NULL CONSTRAINT DF_RT_IssuedAt DEFAULT(SYSUTCDATETIME()) ,-- 'When token was issued',
        ExpiresAt         DATETIME2      NOT NULL ,-- 'When token expires',
        RevokedAt         DATETIME2      NULL ,-- 'When token was revoked',
        ReplacedByToken   NVARCHAR(256)  NULL ,-- 'Hash of token that replaced this one',
        ReasonRevoked     NVARCHAR(500)  NULL ,-- 'Why was token revoked',
        CreatedAt         DATETIME2      NOT NULL CONSTRAINT DF_RT_CreatedAt DEFAULT(SYSUTCDATETIME()) ,
        CreatedBy         INT            NULL ,
        ModifiedAt        DATETIME2      NULL ,
        ModifiedBy        INT            NULL ,
        IsDeleted         BIT            NOT NULL CONSTRAINT DF_RT_IsDeleted DEFAULT(0) ,
        RowVersion        ROWVERSION     NOT NULL ,
        CONSTRAINT FK_RT_Users           FOREIGN KEY (UserId)     REFERENCES dbo.Users(UserId),
        CONSTRAINT FK_RT_Users_CreatedBy FOREIGN KEY (CreatedBy)  REFERENCES dbo.Users(UserId),
        CONSTRAINT FK_RT_Users_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId)
    );
END

-- Login attempts tracking table for security auditing
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LoginAttempts]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.LoginAttempts (
        LoginAttemptId    INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Primary key
        Username          NVARCHAR(100)  NOT NULL,     -- Username attempted (may not exist)
        UserId            INT            NULL,         -- FK to Users (NULL for failed logins with invalid username)
        Timestamp         DATETIME2      NOT NULL CONSTRAINT DF_LA_Timestamp DEFAULT(SYSUTCDATETIME()), -- When attempt occurred
        Success           BIT            NOT NULL CONSTRAINT DF_LA_Success DEFAULT(0), -- Was login successful
        IpAddress         NVARCHAR(45)   NULL,         -- IP address of client (IPv4/IPv6)
        UserAgent         NVARCHAR(500)  NULL,         -- Browser/client user agent
        FailureReason     NVARCHAR(100)  NULL,         -- Reason for failure if unsuccessful (e.g., "Invalid credentials", "Account locked")
        AttemptLocation   NVARCHAR(100)  NULL,         -- Optional geolocation data
        CreatedAt         DATETIME2      NOT NULL CONSTRAINT DF_LA_CreatedAt DEFAULT(SYSUTCDATETIME()),
        IsDeleted         BIT            NOT NULL CONSTRAINT DF_LA_IsDeleted DEFAULT(0),
        CONSTRAINT FK_LA_Users           FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId)
    );
END

-- ================= Audit Logging Tables =================
-- Create AuditLogs table for comprehensive audit trail
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AuditLogs]') AND type in (N'U'))
BEGIN
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
END

-- ================= Extended Tables =================
-- Add CurriculumAlignments table to support curriculum-aligned tree design
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CurriculumAlignments]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CurriculumAlignments](
        [CurriculumAlignmentId] [int] IDENTITY(1,1) NOT NULL,
        [TreeNodeId] [int] NOT NULL,
        [StandardCode] [nvarchar](100) NOT NULL,
        [CurriculumVersion] [nvarchar](20) NULL,
        [EducationLevel] [nvarchar](50) NULL,
        [GradeLevel] [int] NULL,
        [SubjectArea] [nvarchar](100) NULL,
        [Strand] [nvarchar](200) NULL,
        [StandardUrl] [nvarchar](500) NULL,
        [CreatedAt] [datetime2](7) NOT NULL,
        [CreatedBy] [int] NOT NULL,
        [ModifiedAt] [datetime2](7) NULL,
        [ModifiedBy] [int] NULL,
        [IsDeleted] [bit] NOT NULL DEFAULT 0,
        CONSTRAINT [PK_CurriculumAlignments] PRIMARY KEY CLUSTERED 
        (
            [CurriculumAlignmentId] ASC
        ),
        CONSTRAINT [FK_CurriculumAlignments_TreeNodes] FOREIGN KEY([TreeNodeId])
        REFERENCES [dbo].[TreeNodes] ([TreeNodeId])
    );
END

-- Add Exams table to support exam creation and management
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exams]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Exams](
        [ExamId] [int] IDENTITY(1,1) NOT NULL,
        [Title] [nvarchar](200) NOT NULL,
        [Description] [nvarchar](500) NULL,
        [Instructions] [nvarchar](1000) NULL,
        [DurationMinutes] [int] NOT NULL,
        [PassingScore] [decimal](5,2) NULL,
        [MaxScore] [decimal](5,2) NOT NULL,
        [IsRandomized] [bit] NOT NULL DEFAULT 0,
        [AllowReview] [bit] NOT NULL DEFAULT 1,
        [ShowResults] [bit] NOT NULL DEFAULT 1,
        [StartDate] [datetime2](7) NULL,
        [EndDate] [datetime2](7) NULL,
        [Status] [nvarchar](50) NOT NULL DEFAULT 'Draft',
        [CreatedAt] [datetime2](7) NOT NULL DEFAULT SYSUTCDATETIME(),
        [CreatedBy] [int] NOT NULL,
        [ModifiedAt] [datetime2](7) NULL,
        [ModifiedBy] [int] NULL,
        [DeletedAt] [datetime2](7) NULL,
        [DeletedBy] [int] NULL,
        [IsDeleted] [bit] NOT NULL DEFAULT 0,
        [RowVersion] [rowversion] NOT NULL,
        CONSTRAINT [PK_Exams] PRIMARY KEY CLUSTERED ([ExamId] ASC),
        CONSTRAINT [FK_Exams_Users_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId]),
        CONSTRAINT [FK_Exams_Users_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[Users] ([UserId]),
        CONSTRAINT [FK_Exams_Users_DeletedBy] FOREIGN KEY ([DeletedBy]) REFERENCES [dbo].[Users] ([UserId])
    );
END

-- Add ExamQuestions junction table to link exams with questions
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExamQuestions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ExamQuestions](
        [ExamId] [int] NOT NULL,
        [QuestionId] [int] NOT NULL,
        [OrderIndex] [int] NOT NULL DEFAULT 0,
        [Points] [decimal](5,2) NOT NULL DEFAULT 1.00,
        CONSTRAINT [PK_ExamQuestions] PRIMARY KEY CLUSTERED ([ExamId] ASC, [QuestionId] ASC),
        CONSTRAINT [FK_EQ_Exams] FOREIGN KEY ([ExamId]) REFERENCES [dbo].[Exams] ([ExamId]),
        CONSTRAINT [FK_EQ_Questions] FOREIGN KEY ([QuestionId]) REFERENCES [dbo].[Questions] ([QuestionId])
    );
END

-- ================ Indexes ================
-- Only create indexes that don't already exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Q_Type' AND object_id = OBJECT_ID('dbo.Questions'))
    CREATE INDEX IX_Q_Type ON dbo.Questions(QuestionTypeId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Q_Difficulty' AND object_id = OBJECT_ID('dbo.Questions'))
    CREATE INDEX IX_Q_Difficulty ON dbo.Questions(DifficultyLevelId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Q_Status' AND object_id = OBJECT_ID('dbo.Questions'))
    CREATE INDEX IX_Q_Status ON dbo.Questions(QuestionStatusId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Q_PrimaryTreeNode' AND object_id = OBJECT_ID('dbo.Questions'))
    CREATE INDEX IX_Q_PrimaryTreeNode ON dbo.Questions(PrimaryTreeNodeId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_A_QuestionId' AND object_id = OBJECT_ID('dbo.Answers'))
    CREATE INDEX IX_A_QuestionId ON dbo.Answers(QuestionId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TN_Parent' AND object_id = OBJECT_ID('dbo.TreeNodes'))
    CREATE INDEX IX_TN_Parent ON dbo.TreeNodes(ParentId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TN_Path' AND object_id = OBJECT_ID('dbo.TreeNodes'))
    CREATE INDEX IX_TN_Path ON dbo.TreeNodes(Path);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Media_Type' AND object_id = OBJECT_ID('dbo.Media'))
    CREATE INDEX IX_Media_Type ON dbo.Media(MediaTypeId);

-- Indexes for Exams
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Exams_Status' AND object_id = OBJECT_ID('dbo.Exams'))
    CREATE INDEX IX_Exams_Status ON dbo.Exams(Status);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Exams_StartDate' AND object_id = OBJECT_ID('dbo.Exams'))
    CREATE INDEX IX_Exams_StartDate ON dbo.Exams(StartDate);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Exams_CreatedBy' AND object_id = OBJECT_ID('dbo.Exams'))
    CREATE INDEX IX_Exams_CreatedBy ON dbo.Exams(CreatedBy);

-- Indexes for LoginAttempts
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LoginAttempts_Username' AND object_id = OBJECT_ID('dbo.LoginAttempts'))
    CREATE INDEX IX_LoginAttempts_Username ON dbo.LoginAttempts(Username);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LoginAttempts_UserId' AND object_id = OBJECT_ID('dbo.LoginAttempts'))
    CREATE INDEX IX_LoginAttempts_UserId ON dbo.LoginAttempts(UserId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LoginAttempts_Timestamp' AND object_id = OBJECT_ID('dbo.LoginAttempts'))
    CREATE INDEX IX_LoginAttempts_Timestamp ON dbo.LoginAttempts(Timestamp);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LoginAttempts_Success' AND object_id = OBJECT_ID('dbo.LoginAttempts'))
    CREATE INDEX IX_LoginAttempts_Success ON dbo.LoginAttempts(Success);

-- Indexes for AuditLogs for performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditLogs_UserId_Timestamp' AND object_id = OBJECT_ID('dbo.AuditLogs'))
    CREATE INDEX IX_AuditLogs_UserId_Timestamp ON dbo.AuditLogs(UserId, Timestamp DESC);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditLogs_Category_Severity' AND object_id = OBJECT_ID('dbo.AuditLogs'))
    CREATE INDEX IX_AuditLogs_Category_Severity ON dbo.AuditLogs(Category, Severity);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditLogs_Timestamp' AND object_id = OBJECT_ID('dbo.AuditLogs'))
    CREATE INDEX IX_AuditLogs_Timestamp ON dbo.AuditLogs(Timestamp DESC);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditLogs_EntityType_EntityId' AND object_id = OBJECT_ID('dbo.AuditLogs'))
    CREATE INDEX IX_AuditLogs_EntityType_EntityId ON dbo.AuditLogs(EntityType, EntityId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditLogs_Action' AND object_id = OBJECT_ID('dbo.AuditLogs'))
    CREATE INDEX IX_AuditLogs_Action ON dbo.AuditLogs(Action);
