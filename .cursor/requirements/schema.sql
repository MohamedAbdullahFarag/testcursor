USE Ikhtibar;
GO

-- ================= Lookup Tables =================
CREATE TABLE dbo.QuestionTypes (
    QuestionTypeId    INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Unique question type identifier
    Name              NVARCHAR(100)  NOT NULL                             -- e.g. 'Multiple Choice'
);

CREATE TABLE dbo.DifficultyLevels (
    DifficultyLevelId INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Unique difficulty level identifier
    Name              NVARCHAR(50)   NOT NULL                             -- e.g. 'Easy', 'Medium', 'Hard'
);

CREATE TABLE dbo.QuestionStatuses (
    QuestionStatusId  INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- e.g. 'Draft', 'Published'
    Name              NVARCHAR(50)   NOT NULL
);

CREATE TABLE dbo.MediaTypes (
    MediaTypeId       INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- e.g. 'Image', 'Video'
    Name              NVARCHAR(50)   NOT NULL
);

CREATE TABLE dbo.TreeNodeTypes (
    TreeNodeTypeId    INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- e.g. 'Category', 'Topic'
    Name              NVARCHAR(100)  NOT NULL
);

-- ================= Core Tables =================
CREATE TABLE dbo.Users (
    UserId            INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Primary key for users
    Username          NVARCHAR(100)  NOT NULL UNIQUE,                    -- Login username
    Email             NVARCHAR(200)  NOT NULL UNIQUE,                    -- Contact email
    FirstName         NVARCHAR(100)  NOT NULL,                           -- User first name
    LastName          NVARCHAR(100)  NOT NULL,                           -- User last name
    PasswordHash      NVARCHAR(256)  NOT NULL,                           -- Hashed password
    PhoneNumber       NVARCHAR(20)   NULL,                               -- Optional phone number
    PreferredLanguage NVARCHAR(10)   NULL,                               -- e.g. en, ar
    Code              NVARCHAR(50)   NULL,                               -- User's unique code (for external systems)
    LastLoginAt       DATETIME2      NULL,                               -- User's last login timestamp
    IsActive          BIT            NOT NULL CONSTRAINT DF_Users_IsActive      DEFAULT(1),   -- Flag if account is active
    EmailVerified     BIT            NOT NULL CONSTRAINT DF_Users_EmailVerified DEFAULT(0),   -- Has email been verified
    PhoneVerified     BIT            NOT NULL CONSTRAINT DF_Users_PhoneVerified DEFAULT(0),   -- Has phone been verified
    CreatedAt         DATETIME2      NOT NULL CONSTRAINT DF_Users_CreatedAt     DEFAULT(SYSUTCDATETIME()), -- Record creation timestamp
    CreatedBy         INT            NULL,                               -- UserId who created record
    ModifiedAt        DATETIME2      NULL,                               -- Last modification timestamp
    ModifiedBy        INT            NULL,                               -- UserId who last modified
    DeletedAt         DATETIME2      NULL,                               -- Deletion timestamp for soft delete
    DeletedBy         INT            NULL,                               -- UserId who deleted (soft)
    IsDeleted         BIT            NOT NULL CONSTRAINT DF_Users_IsDeleted    DEFAULT(0),   -- Soft delete flag
    RowVersion        ROWVERSION     NOT NULL                            -- Optimistic concurrency token
);

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

-- Junction: Roles <-> Permissions
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

-- Junction: Users <-> Roles
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

-- Tree nodes with path enumeration backup
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

-- Media attachments
CREATE TABLE dbo.Media (
    MediaId           INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Media identifier
    Url               NVARCHAR(2083) NOT NULL,                           -- Resource URL
    MediaTypeId       INT            NOT NULL,                           -- FK to MediaTypes
    UploadedBy        INT            NOT NULL,                           -- UserId uploader
    UploadedAt        DATETIME2      NOT NULL CONSTRAINT DF_Media_UploadedAt DEFAULT(SYSUTCDATETIME()), -- Upload timestamp
    CreatedAt         DATETIME2      NOT NULL CONSTRAINT DF_Media_CreatedAt DEFAULT(SYSUTCDATETIME()),
    CreatedBy         INT            NULL,
    ModifiedAt        DATETIME2      NULL,
    ModifiedBy        INT            NULL,
    DeletedAt         DATETIME2      NULL,
    DeletedBy         INT            NULL,
    IsDeleted         BIT            NOT NULL CONSTRAINT DF_Media_IsDeleted DEFAULT(0),
    RowVersion        ROWVERSION     NOT NULL,                           -- Concurrency token
    CONSTRAINT FK_Media_Types          FOREIGN KEY (MediaTypeId) REFERENCES dbo.MediaTypes(MediaTypeId),
    CONSTRAINT FK_Media_Users          FOREIGN KEY (UploadedBy)   REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_Media_Users_CreatedBy FOREIGN KEY (CreatedBy)   REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_Media_Users_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_Media_Users_DeletedBy FOREIGN KEY (DeletedBy)   REFERENCES dbo.Users(UserId)
);

-- ================= Media Management Tables =================
CREATE TABLE dbo.MediaFiles (
    Id                INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Primary key
    OriginalFileName  NVARCHAR(255) NOT NULL,                           -- Original filename as uploaded
    StorageFileName   NVARCHAR(255) NOT NULL,                           -- Internal filename for storage
    ContentType       NVARCHAR(100) NOT NULL,                           -- MIME content type
    FileSizeBytes     BIGINT        NOT NULL,                           -- File size in bytes
    StoragePath       NVARCHAR(500) NOT NULL,                           -- Storage path or container location
    MediaType         INT            NOT NULL,                           -- Media type classification (enum)
    Status            INT            NOT NULL DEFAULT 0,                 -- Current processing status (enum)
    Title             NVARCHAR(255) NULL,                               -- Optional title for the media file
    Description       NVARCHAR(1000) NULL,                              -- Optional description
    AltText           NVARCHAR(255) NULL,                               -- Alternative text for accessibility
    FileHash          NVARCHAR(64)  NULL,                               -- File hash for duplicate detection
    CategoryId        INT            NULL,                               -- Category this media file belongs to
    UploadedBy        INT            NOT NULL,                           -- User who uploaded this file
    IsPublic          BIT            NOT NULL DEFAULT 0,                 -- Whether this file is publicly accessible
    CreatedAt         DATETIME2      NOT NULL CONSTRAINT DF_MediaFiles_CreatedAt DEFAULT(SYSUTCDATETIME()),
    CreatedBy         INT            NULL,
    ModifiedAt        DATETIME2      NULL,
    ModifiedBy        INT            NULL,
    DeletedAt         DATETIME2      NULL,
    DeletedBy         INT            NULL,
    IsDeleted         BIT            NOT NULL CONSTRAINT DF_MediaFiles_IsDeleted DEFAULT(0),
    RowVersion        ROWVERSION     NOT NULL,
    CONSTRAINT FK_MediaFiles_Users_UploadedBy FOREIGN KEY (UploadedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_MediaFiles_Users_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_MediaFiles_Users_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_MediaFiles_Users_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_MediaFiles_MediaCategories FOREIGN KEY (CategoryId) REFERENCES dbo.MediaCategories(Id)
);

CREATE TABLE dbo.MediaCategories (
    Id                INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Primary key
    Name              NVARCHAR(100) NOT NULL,                           -- Category name
    Description       NVARCHAR(500) NULL,                               -- Category description
    ParentCategoryId  INT            NULL,                               -- Parent category (for hierarchy)
    Icon              NVARCHAR(100) NULL,                               -- Icon identifier for UI
    Color             NVARCHAR(7)   NULL,                               -- Color code for UI (e.g., #FF0000)
    SortOrder         INT            NOT NULL DEFAULT 0,                 -- Display order
    IsActive          BIT            NOT NULL DEFAULT 1,                 -- Active flag
    CreatedAt         DATETIME2      NOT NULL CONSTRAINT DF_MediaCategories_CreatedAt DEFAULT(SYSUTCDATETIME()),
    CreatedBy         INT            NULL,
    ModifiedAt        DATETIME2      NULL,
    ModifiedBy        INT            NULL,
    DeletedAt         DATETIME2      NULL,
    DeletedBy         INT            NULL,
    IsDeleted         BIT            NOT NULL CONSTRAINT DF_MediaCategories_IsDeleted DEFAULT(0),
    RowVersion        ROWVERSION     NOT NULL,
    CONSTRAINT FK_MediaCategories_Parent FOREIGN KEY (ParentCategoryId) REFERENCES dbo.MediaCategories(Id),
    CONSTRAINT FK_MediaCategories_Users_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_MediaCategories_Users_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_MediaCategories_Users_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId)
);

CREATE TABLE dbo.MediaThumbnails (
    Id                INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Primary key
    MediaFileId       INT            NOT NULL,                           -- FK to MediaFiles
    ThumbnailSize     INT            NOT NULL,                           -- Thumbnail size category (enum)
    Width             INT            NOT NULL,                           -- Thumbnail width in pixels
    Height            INT            NOT NULL,                           -- Thumbnail height in pixels
    StoragePath       NVARCHAR(500) NOT NULL,                           -- Storage path for thumbnail
    FileSizeBytes     BIGINT        NOT NULL,                           -- Thumbnail file size
    Quality           INT            NOT NULL DEFAULT 80,                -- JPEG quality (1-100)
    GeneratedAt       DATETIME2      NOT NULL,                           -- When thumbnail was generated
    CreatedAt         DATETIME2      NOT NULL CONSTRAINT DF_MediaThumbnails_CreatedAt DEFAULT(SYSUTCDATETIME()),
    CreatedBy         INT            NULL,
    ModifiedAt        DATETIME2      NULL,
    ModifiedBy        INT            NULL,
    DeletedAt         DATETIME2      NULL,
    DeletedBy         INT            NULL,
    IsDeleted         BIT            NOT NULL CONSTRAINT DF_MediaThumbnails_IsDeleted DEFAULT(0),
    RowVersion        ROWVERSION     NOT NULL,
    CONSTRAINT FK_MediaThumbnails_MediaFiles FOREIGN KEY (MediaFileId) REFERENCES dbo.MediaFiles(Id),
    CONSTRAINT FK_MediaThumbnails_Users_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_MediaThumbnails_Users_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_MediaThumbnails_Users_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId)
);

CREATE TABLE dbo.MediaProcessingJobs (
    Id                INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Primary key
    MediaFileId       INT            NOT NULL,                           -- Media file being processed
    JobType           INT            NOT NULL,                           -- Type of processing job (enum)
    Status            INT            NOT NULL DEFAULT 1,                 -- Current status of the job (enum)
    Priority          INT            NOT NULL DEFAULT 5,                 -- Job priority (higher number = higher priority)
    AttemptCount      INT            NOT NULL DEFAULT 0,                 -- Number of processing attempts
    MaxAttempts       INT            NOT NULL DEFAULT 3,                 -- Maximum number of retry attempts
    StartedAt         DATETIME2      NULL,                               -- When the job started processing
    CompletedAt       DATETIME2      NULL,                               -- When the job completed (success or failure)
    NextRetryAt       DATETIME2      NULL,                               -- Next retry time if job failed
    ProcessingDurationMs BIGINT      NULL,                               -- Processing duration in milliseconds
    ProgressPercentage INT           NOT NULL DEFAULT 0,                 -- Percentage completion (0-100)
    CurrentStage      NVARCHAR(255) NULL,                               -- Current processing stage description
    JobParameters     NVARCHAR(MAX) NULL,                               -- Job parameters and configuration (JSON)
    JobResults        NVARCHAR(MAX) NULL,                               -- Job results and output information (JSON)
    ErrorMessage      NVARCHAR(1000) NULL,                              -- Error message if job failed
    CreatedAt         DATETIME2      NOT NULL CONSTRAINT DF_MediaProcessingJobs_CreatedAt DEFAULT(SYSUTCDATETIME()),
    CreatedBy         INT            NULL,
    ModifiedAt        DATETIME2      NULL,
    ModifiedBy        INT            NULL,
    DeletedAt         DATETIME2      NULL,
    DeletedBy         INT            NULL,
    IsDeleted         BIT            NOT NULL CONSTRAINT DF_MediaProcessingJobs_IsDeleted DEFAULT(0),
    RowVersion        ROWVERSION     NOT NULL,
    CONSTRAINT FK_MediaProcessingJobs_MediaFiles FOREIGN KEY (MediaFileId) REFERENCES dbo.MediaFiles(Id),
    CONSTRAINT FK_MediaProcessingJobs_Users_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_MediaProcessingJobs_Users_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_MediaProcessingJobs_Users_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId)
);

CREATE TABLE dbo.MediaCollections (
    Id                INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Primary key
    Name              NVARCHAR(200) NOT NULL,                           -- Collection name
    Description       NVARCHAR(1000) NULL,                              -- Collection description
    CollectionType    INT            NOT NULL,                           -- Type of collection (enum)
    IsPublic          BIT            NOT NULL DEFAULT 0,                 -- Whether collection is publicly accessible
    CoverMediaFileId  INT            NULL,                               -- Cover media file for the collection
    SortOrder         INT            NOT NULL DEFAULT 0,                 -- Display order
    CreatedAt         DATETIME2      NOT NULL CONSTRAINT DF_MediaCollections_CreatedAt DEFAULT(SYSUTCDATETIME()),
    CreatedBy         INT            NULL,
    ModifiedAt        DATETIME2      NULL,
    ModifiedBy        INT            NULL,
    DeletedAt         DATETIME2      NULL,
    DeletedBy         INT            NULL,
    IsDeleted         BIT            NOT NULL CONSTRAINT DF_MediaCollections_IsDeleted DEFAULT(0),
    RowVersion        ROWVERSION     NOT NULL,
    CONSTRAINT FK_MediaCollections_CoverMedia FOREIGN KEY (CoverMediaFileId) REFERENCES dbo.MediaFiles(Id),
    CONSTRAINT FK_MediaCollections_Users_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_MediaCollections_Users_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_MediaCollections_Users_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId)
);

CREATE TABLE dbo.MediaCollectionItems (
    Id                INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Primary key
    CollectionId      INT            NOT NULL,                           -- FK to MediaCollections
    MediaFileId       INT            NOT NULL,                           -- FK to MediaFiles
    SortOrder         INT            NOT NULL DEFAULT 0,                 -- Display order within collection
    AddedAt           DATETIME2      NOT NULL,                           -- When item was added to collection
    AddedBy           INT            NOT NULL,                           -- User who added item to collection
    CreatedAt         DATETIME2      NOT NULL CONSTRAINT DF_MediaCollectionItems_CreatedAt DEFAULT(SYSUTCDATETIME()),
    CreatedBy         INT            NULL,
    ModifiedAt        DATETIME2      NULL,
    ModifiedBy        INT            NULL,
    DeletedAt         DATETIME2      NULL,
    DeletedBy         INT            NULL,
    IsDeleted         BIT            NOT NULL CONSTRAINT DF_MediaCollectionItems_IsDeleted DEFAULT(0),
    RowVersion        ROWVERSION     NOT NULL,
    CONSTRAINT FK_MediaCollectionItems_Collections FOREIGN KEY (CollectionId) REFERENCES dbo.MediaCollections(Id),
    CONSTRAINT FK_MediaCollectionItems_MediaFiles FOREIGN KEY (MediaFileId) REFERENCES dbo.MediaFiles(Id),
    CONSTRAINT FK_MediaCollectionItems_Users_AddedBy FOREIGN KEY (AddedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_MediaCollectionItems_Users_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_MediaCollectionItems_Users_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_MediaCollectionItems_Users_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId)
);

CREATE TABLE dbo.MediaAccessLogs (
    Id                INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Primary key
    MediaFileId       INT            NOT NULL,                           -- FK to MediaFiles
    UserId            INT            NULL,                               -- User who accessed (NULL for anonymous)
    AccessAction      INT            NOT NULL,                           -- Type of access action (enum)
    AccessTimestamp   DATETIME2      NOT NULL,                           -- When access occurred
    IpAddress         NVARCHAR(45)  NULL,                               -- IP address of client
    UserAgent         NVARCHAR(500) NULL,                               -- Browser/client user agent
    ReferrerUrl       NVARCHAR(500) NULL,                               -- Referring URL
    SessionId         NVARCHAR(100) NULL,                               -- Session identifier
    AccessDuration    INT            NULL,                               -- Access duration in seconds
    CreatedAt         DATETIME2      NOT NULL CONSTRAINT DF_MediaAccessLogs_CreatedAt DEFAULT(SYSUTCDATETIME()),
    CreatedBy         INT            NULL,
    ModifiedAt        DATETIME2      NULL,
    ModifiedBy        INT            NULL,
    DeletedAt         DATETIME2      NULL,
    DeletedBy         INT            NULL,
    IsDeleted         BIT            NOT NULL CONSTRAINT DF_MediaAccessLogs_IsDeleted DEFAULT(0),
    RowVersion        ROWVERSION     NOT NULL,
    CONSTRAINT FK_MediaAccessLogs_MediaFiles FOREIGN KEY (MediaFileId) REFERENCES dbo.MediaFiles(Id),
    CONSTRAINT FK_MediaAccessLogs_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_MediaAccessLogs_Users_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_MediaAccessLogs_Users_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_MediaAccessLogs_Users_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId)
);

CREATE TABLE dbo.MediaMetadata (
    Id                INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Primary key
    MediaFileId       INT            NOT NULL,                           -- FK to MediaFiles
    MetadataKey       NVARCHAR(100) NOT NULL,                           -- Metadata key
    MetadataValue     NVARCHAR(MAX) NULL,                               -- Metadata value
    MetadataType      NVARCHAR(50)  NULL,                               -- Data type of metadata value
    IsSearchable      BIT            NOT NULL DEFAULT 1,                 -- Whether this metadata is searchable
    CreatedAt         DATETIME2      NOT NULL CONSTRAINT DF_MediaMetadata_CreatedAt DEFAULT(SYSUTCDATETIME()),
    CreatedBy         INT            NULL,
    ModifiedAt        DATETIME2      NULL,
    ModifiedBy        INT            NULL,
    DeletedAt         DATETIME2      NULL,
    DeletedBy         INT            NULL,
    IsDeleted         BIT            NOT NULL CONSTRAINT DF_MediaMetadata_IsDeleted DEFAULT(0),
    RowVersion        ROWVERSION     NOT NULL,
    CONSTRAINT FK_MediaMetadata_MediaFiles FOREIGN KEY (MediaFileId) REFERENCES dbo.MediaFiles(Id),
    CONSTRAINT FK_MediaMetadata_Users_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_MediaMetadata_Users_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_MediaMetadata_Users_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId)
);

-- Questions
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
    CONSTRAINT FK_Q_Users_DeletedBy     FOREIGN KEY (DeletedBy)          REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_Questions_QuestionBankCategories FOREIGN KEY (PrimaryTreeNodeId) REFERENCES dbo.QuestionBankCategories(Id)
);

-- ================= Question Management Tables =================
CREATE TABLE dbo.QuestionBanks (
    Id                INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Primary key
    Name              NVARCHAR(200) NOT NULL,                           -- Question bank name
    Description       NVARCHAR(1000) NULL,                              -- Question bank description
    IsPublic          BIT            NOT NULL DEFAULT 0,                 -- Whether question bank is publicly accessible
    IsActive          BIT            NOT NULL DEFAULT 1,                 -- Active flag
    CreatedAt         DATETIME2      NOT NULL CONSTRAINT DF_QuestionBanks_CreatedAt DEFAULT(SYSUTCDATETIME()),
    CreatedBy         INT            NULL,
    ModifiedAt        DATETIME2      NULL,
    ModifiedBy        INT            NULL,
    DeletedAt         DATETIME2      NULL,
    DeletedBy         INT            NULL,
    IsDeleted         BIT            NOT NULL CONSTRAINT DF_QuestionBanks_IsDeleted DEFAULT(0),
    RowVersion        ROWVERSION     NOT NULL,
    CONSTRAINT FK_QuestionBanks_Users_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_QuestionBanks_Users_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_QuestionBanks_Users_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId)
);

CREATE TABLE dbo.QuestionBankCategories (
    Id                INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Primary key
    QuestionBankId    INT            NOT NULL,                           -- FK to QuestionBanks
    Name              NVARCHAR(200) NOT NULL,                           -- Category name
    Description       NVARCHAR(1000) NULL,                              -- Category description
    ParentCategoryId  INT            NULL,                               -- Parent category (for hierarchy)
    SortOrder         INT            NOT NULL DEFAULT 0,                 -- Display order
    IsActive          BIT            NOT NULL DEFAULT 1,                 -- Active flag
    CreatedAt         DATETIME2      NOT NULL CONSTRAINT DF_QuestionBankCategories_CreatedAt DEFAULT(SYSUTCDATETIME()),
    CreatedBy         INT            NULL,
    ModifiedAt        DATETIME2      NULL,
    ModifiedBy        INT            NULL,
    DeletedAt         DATETIME2      NULL,
    DeletedBy         INT            NULL,
    IsDeleted         BIT            NOT NULL CONSTRAINT DF_QuestionBankCategories_IsDeleted DEFAULT(0),
    RowVersion        ROWVERSION     NOT NULL,
    CONSTRAINT FK_QuestionBankCategories_QuestionBanks FOREIGN KEY (QuestionBankId) REFERENCES dbo.QuestionBanks(Id),
    CONSTRAINT FK_QuestionBankCategories_Parent FOREIGN KEY (ParentCategoryId) REFERENCES dbo.QuestionBankCategories(Id),
    CONSTRAINT FK_QuestionBankCategories_Users_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_QuestionBankCategories_Users_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_QuestionBankCategories_Users_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId)
);

CREATE TABLE dbo.QuestionBankHierarchy (
    Id                INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Primary key
    AncestorId        INT            NOT NULL,                           -- Ancestor category ID
    DescendantId      INT            NOT NULL,                           -- Descendant category ID
    Depth             INT            NOT NULL,                           -- Depth in hierarchy
    CreatedAt         DATETIME2      NOT NULL CONSTRAINT DF_QuestionBankHierarchy_CreatedAt DEFAULT(SYSUTCDATETIME()),
    CreatedBy         INT            NULL,
    ModifiedAt        DATETIME2      NULL,
    ModifiedBy        INT            NULL,
    DeletedAt         DATETIME2      NULL,
    DeletedBy         INT            NULL,
    IsDeleted         BIT            NOT NULL CONSTRAINT DF_QuestionBankHierarchy_IsDeleted DEFAULT(0),
    RowVersion        ROWVERSION     NOT NULL,
    CONSTRAINT FK_QuestionBankHierarchy_Ancestor FOREIGN KEY (AncestorId) REFERENCES dbo.QuestionBankCategories(Id),
    CONSTRAINT FK_QuestionBankHierarchy_Descendant FOREIGN KEY (DescendantId) REFERENCES dbo.QuestionBankCategories(Id),
    CONSTRAINT FK_QuestionBankHierarchy_Users_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_QuestionBankHierarchy_Users_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_QuestionBankHierarchy_Users_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId)
);

CREATE TABLE dbo.QuestionCategorizations (
    Id                INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Primary key
    QuestionId        INT            NOT NULL,                           -- FK to Questions
    CategoryId        INT            NOT NULL,                           -- FK to QuestionBankCategories
    IsPrimary         BIT            NOT NULL DEFAULT 0,                 -- Whether this is the primary category
    SortOrder         INT            NOT NULL DEFAULT 0,                 -- Display order within category
    CreatedAt         DATETIME2      NOT NULL CONSTRAINT DF_QuestionCategorizations_CreatedAt DEFAULT(SYSUTCDATETIME()),
    CreatedBy         INT            NULL,
    ModifiedAt        DATETIME2      NULL,
    ModifiedBy        INT            NULL,
    DeletedAt         DATETIME2      NULL,
    DeletedBy         INT            NULL,
    IsDeleted         BIT            NOT NULL CONSTRAINT DF_QuestionCategorizations_IsDeleted DEFAULT(0),
    RowVersion        ROWVERSION     NOT NULL,
    CONSTRAINT FK_QuestionCategorizations_Questions FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId),
    CONSTRAINT FK_QuestionCategorizations_Categories FOREIGN KEY (CategoryId) REFERENCES dbo.QuestionBankCategories(Id),
    CONSTRAINT FK_QuestionCategorizations_Users_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_QuestionCategorizations_Users_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_QuestionCategorizations_Users_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId)
);

CREATE TABLE dbo.QuestionTags (
    Id                INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Primary key
    QuestionId        INT            NOT NULL,                           -- FK to Questions
    TagName           NVARCHAR(100) NOT NULL,                           -- Tag name
    TagValue          NVARCHAR(500) NULL,                               -- Tag value (optional)
    CreatedAt         DATETIME2      NOT NULL CONSTRAINT DF_QuestionTags_CreatedAt DEFAULT(SYSUTCDATETIME()),
    CreatedBy         INT            NULL,
    ModifiedAt        DATETIME2      NULL,
    ModifiedBy        INT            NULL,
    DeletedAt         DATETIME2      NULL,
    DeletedBy         INT            NULL,
    IsDeleted         BIT            NOT NULL CONSTRAINT DF_QuestionTags_IsDeleted DEFAULT(0),
    RowVersion        ROWVERSION     NOT NULL,
    CONSTRAINT FK_QuestionTags_Questions FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId),
    CONSTRAINT FK_QuestionTags_Users_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_QuestionTags_Users_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_QuestionTags_Users_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId)
);

CREATE TABLE dbo.QuestionTemplates (
    Id                INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Primary key
    Name              NVARCHAR(200) NOT NULL,                           -- Template name
    Description       NVARCHAR(1000) NULL,                              -- Template description
    TemplateData      NVARCHAR(MAX) NOT NULL,                           -- Template structure (JSON)
    IsActive          BIT            NOT NULL DEFAULT 1,                 -- Active flag
    CreatedAt         DATETIME2      NOT NULL CONSTRAINT DF_QuestionTemplates_CreatedAt DEFAULT(SYSUTCDATETIME()),
    CreatedBy         INT            NULL,
    ModifiedAt        DATETIME2      NULL,
    ModifiedBy        INT            NULL,
    DeletedAt         DATETIME2      NULL,
    DeletedBy         INT            NULL,
    IsDeleted         BIT            NOT NULL CONSTRAINT DF_QuestionTemplates_IsDeleted DEFAULT(0),
    RowVersion        ROWVERSION     NOT NULL,
    CONSTRAINT FK_QuestionTemplates_Users_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_QuestionTemplates_Users_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_QuestionTemplates_Users_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId)
);

CREATE TABLE dbo.QuestionVersions (
    Id                INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Primary key
    QuestionId        INT            NOT NULL,                           -- FK to Questions
    VersionNumber     NVARCHAR(50)  NOT NULL,                           -- Version number/identifier
    VersionStatus     INT            NOT NULL,                           -- Version status (enum)
    ChangeDescription NVARCHAR(1000) NULL,                              -- Description of changes in this version
    VersionData       NVARCHAR(MAX) NULL,                               -- Version-specific data (JSON)
    CreatedAt         DATETIME2      NOT NULL CONSTRAINT DF_QuestionVersions_CreatedAt DEFAULT(SYSUTCDATETIME()),
    CreatedBy         INT            NULL,
    ModifiedAt        DATETIME2      NULL,
    ModifiedBy        INT            NULL,
    DeletedAt         DATETIME2      NULL,
    DeletedBy         INT            NULL,
    IsDeleted         BIT            NOT NULL CONSTRAINT DF_QuestionVersions_IsDeleted DEFAULT(0),
    RowVersion        ROWVERSION     NOT NULL,
    CONSTRAINT FK_QuestionVersions_Questions FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId),
    CONSTRAINT FK_QuestionVersions_Users_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_QuestionVersions_Users_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_QuestionVersions_Users_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId)
);

CREATE TABLE dbo.QuestionValidations (
    Id                INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Primary key
    QuestionId        INT            NOT NULL,                           -- FK to Questions
    ValidatorId       INT            NOT NULL,                           -- FK to Users (validator)
    ValidationStatus  INT            NOT NULL,                           -- Validation status (enum)
    ValidationNotes   NVARCHAR(1000) NULL,                              -- Validation notes/comments
    ValidatedAt       DATETIME2      NOT NULL,                           -- When validation occurred
    CreatedAt         DATETIME2      NOT NULL CONSTRAINT DF_QuestionValidations_CreatedAt DEFAULT(SYSUTCDATETIME()),
    CreatedBy         INT            NULL,
    ModifiedAt        DATETIME2      NULL,
    ModifiedBy        INT            NULL,
    DeletedAt         DATETIME2      NULL,
    DeletedBy         INT            NULL,
    IsDeleted         BIT            NOT NULL CONSTRAINT DF_QuestionValidations_IsDeleted DEFAULT(0),
    RowVersion        ROWVERSION     NOT NULL,
    CONSTRAINT FK_QuestionValidations_Questions FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId),
    CONSTRAINT FK_QuestionValidations_Users_Validator FOREIGN KEY (ValidatorId) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_QuestionValidations_Users_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_QuestionValidations_Users_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_QuestionValidations_Users_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId)
);

CREATE TABLE dbo.QuestionUsageHistory (
    Id                INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Primary key
    QuestionId        INT            NOT NULL,                           -- FK to Questions
    UsageType         INT            NOT NULL,                           -- Type of usage (enum)
    UsedInExamId      INT            NULL,                               -- FK to Exams (if used in exam)
    UsedBy            INT            NULL,                               -- FK to Users (if used by specific user)
    UsedAt            DATETIME2      NOT NULL,                           -- When question was used
    CreatedAt         DATETIME2      NOT NULL CONSTRAINT DF_QuestionUsageHistory_CreatedAt DEFAULT(SYSUTCDATETIME()),
    CreatedBy         INT            NULL,
    ModifiedAt        DATETIME2      NULL,
    ModifiedBy        INT            NULL,
    DeletedAt         DATETIME2      NULL,
    DeletedBy         INT            NULL,
    IsDeleted         BIT            NOT NULL CONSTRAINT DF_QuestionUsageHistory_IsDeleted DEFAULT(0),
    RowVersion        ROWVERSION     NOT NULL,
    CONSTRAINT FK_QuestionUsageHistory_Questions FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId),
    CONSTRAINT FK_QuestionUsageHistory_Exams FOREIGN KEY (UsedInExamId) REFERENCES dbo.Exams(ExamId),
    CONSTRAINT FK_QuestionUsageHistory_Users_UsedBy FOREIGN KEY (UsedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_QuestionUsageHistory_Users_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_QuestionUsageHistory_Users_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_QuestionUsageHistory_Users_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId)
);

CREATE TABLE dbo.QuestionImportBatches (
    Id                INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Primary key
    BatchName         NVARCHAR(200) NOT NULL,                           -- Batch name/identifier
    ImportStatus      INT            NOT NULL,                           -- Import status (enum)
    TotalQuestions    INT            NOT NULL DEFAULT 0,                 -- Total questions in batch
    ImportedQuestions INT            NOT NULL DEFAULT 0,                 -- Successfully imported questions
    FailedQuestions   INT            NOT NULL DEFAULT 0,                 -- Failed imports
    ImportFile        NVARCHAR(500) NULL,                               -- Original import file path
    ImportLog         NVARCHAR(MAX) NULL,                               -- Import process log
    StartedAt         DATETIME2      NULL,                               -- When import started
    CompletedAt       DATETIME2      NULL,                               -- When import completed
    CreatedAt         DATETIME2      NOT NULL CONSTRAINT DF_QuestionImportBatches_CreatedAt DEFAULT(SYSUTCDATETIME()),
    CreatedBy         INT            NULL,
    ModifiedAt        DATETIME2      NULL,
    ModifiedBy        INT            NULL,
    DeletedAt         DATETIME2      NULL,
    DeletedBy         INT            NULL,
    IsDeleted         BIT            NOT NULL CONSTRAINT DF_QuestionImportBatches_IsDeleted DEFAULT(0),
    RowVersion        ROWVERSION     NOT NULL,
    CONSTRAINT FK_QuestionImportBatches_Users_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_QuestionImportBatches_Users_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_QuestionImportBatches_Users_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId)
);

-- Answers
CREATE TABLE dbo.Answers (
    AnswerId          INT            IDENTITY(1,1) NOT NULL PRIMARY KEY, -- Answer identifier
    QuestionId        INT            NOT NULL,                           -- FK to Questions
    Text              NVARCHAR(500) NOT NULL,                           -- Answer text
    IsCorrect         BIT            NOT NULL CONSTRAINT DF_Answers_IsCorrect DEFAULT(0), -- Correct answer flag
    CreatedAt         DATETIME2      NOT NULL CONSTRAINT DF_A_CreatedAt DEFAULT(SYSUTCDATETIME()), -- Creation timestamp
    CreatedBy         INT            NULL,                               -- UserId creator
    ModifiedAt        DATETIME2      NULL,                               -- Last mod timestamp
    ModifiedBy        INT            NULL,                               -- UserId modifier
    DeletedAt         DATETIME2      NULL,                               -- Soft delete timestamp
    DeletedBy         INT            NULL,                               -- UserId deleter
    IsDeleted         BIT            NOT NULL CONSTRAINT DF_A_IsDeleted DEFAULT(0), -- Soft delete flag
    RowVersion        ROWVERSION     NOT NULL,                           -- Concurrency token
    CONSTRAINT FK_A_Questions          FOREIGN KEY (QuestionId)   REFERENCES dbo.Questions(QuestionId),
    CONSTRAINT FK_A_Users_CreatedBy    FOREIGN KEY (CreatedBy)    REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_A_Users_ModifiedBy   FOREIGN KEY (ModifiedBy)   REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_A_Users_DeletedBy    FOREIGN KEY (DeletedBy)    REFERENCES dbo.Users(UserId)
);

-- Media junctions
CREATE TABLE dbo.QuestionMedia (
    QuestionId        INT            NOT NULL ,-- 'FK to Questions',
    MediaId           INT            NOT NULL ,-- 'FK to Media',
    CONSTRAINT PK_QuestionMedia       PRIMARY KEY (QuestionId, MediaId),
    CONSTRAINT FK_QM_Questions         FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId),
    CONSTRAINT FK_QM_Media             FOREIGN KEY (MediaId)    REFERENCES dbo.Media(MediaId)
);

CREATE TABLE dbo.AnswerMedia (
    AnswerId          INT            NOT NULL ,-- 'FK to Answers',
    MediaId           INT            NOT NULL ,-- 'FK to Media',
    CONSTRAINT PK_AnswerMedia         PRIMARY KEY (AnswerId, MediaId),
    CONSTRAINT FK_AM_Answers           FOREIGN KEY (AnswerId)   REFERENCES dbo.Answers(AnswerId),
    CONSTRAINT FK_AM_Media             FOREIGN KEY (MediaId)    REFERENCES dbo.Media(MediaId)
);

-- ================= Authentication Tables =================
CREATE TABLE dbo.RefreshTokens (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TokenHash NVARCHAR(500) NOT NULL,
    ExpiresAt DATETIME2 NOT NULL,
    IssuedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy INT NULL,
    UserId INT NOT NULL,
    RevocationReason NVARCHAR(500) NULL,
    RevokedAt DATETIME2 NULL,
    ClientIpAddress NVARCHAR(45) NULL,
    UserAgent NVARCHAR(500) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    IsExpired BIT NOT NULL DEFAULT 0,
    IsRevoked BIT NOT NULL DEFAULT 0,
    ModifiedAt DATETIME2 NULL,
    ModifiedBy INT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    DeletedAt DATETIME2 NULL,
    DeletedBy INT NULL,
    CONSTRAINT FK_RefreshTokens_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_RefreshTokens_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_RefreshTokens_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_RefreshTokens_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId)
);

-- Create indexes for RefreshTokens table
CREATE INDEX IX_RefreshTokens_TokenHash ON dbo.RefreshTokens(TokenHash);
CREATE INDEX IX_RefreshTokens_UserId ON dbo.RefreshTokens(UserId);
CREATE INDEX IX_RefreshTokens_ExpiresAt ON dbo.RefreshTokens(ExpiresAt);
CREATE INDEX IX_RefreshTokens_IsActive ON dbo.RefreshTokens(IsActive);

-- Login attempts tracking table for security auditing
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

-- Add indexes for LoginAttempts
CREATE INDEX IX_LoginAttempts_Username ON dbo.LoginAttempts(Username);
CREATE INDEX IX_LoginAttempts_UserId ON dbo.LoginAttempts(UserId);
CREATE INDEX IX_LoginAttempts_Timestamp ON dbo.LoginAttempts(Timestamp);
CREATE INDEX IX_LoginAttempts_Success ON dbo.LoginAttempts(Success);

-- Add indexes for Media Management tables
CREATE INDEX IX_MediaFiles_MediaType ON dbo.MediaFiles(MediaType);
CREATE INDEX IX_MediaFiles_Status ON dbo.MediaFiles(Status);
CREATE INDEX IX_MediaFiles_CategoryId ON dbo.MediaFiles(CategoryId);
CREATE INDEX IX_MediaFiles_UploadedBy ON dbo.MediaFiles(UploadedBy);
CREATE INDEX IX_MediaFiles_CreatedAt ON dbo.MediaFiles(CreatedAt);

CREATE INDEX IX_MediaCategories_Parent ON dbo.MediaCategories(ParentCategoryId);
CREATE INDEX IX_MediaCategories_IsActive ON dbo.MediaCategories(IsActive);

CREATE INDEX IX_MediaThumbnails_MediaFile ON dbo.MediaThumbnails(MediaFileId);
CREATE INDEX IX_MediaThumbnails_Size ON dbo.MediaThumbnails(ThumbnailSize);

CREATE INDEX IX_MediaProcessingJobs_MediaFile ON dbo.MediaProcessingJobs(MediaFileId);
CREATE INDEX IX_MediaProcessingJobs_Status ON dbo.MediaProcessingJobs(Status);
CREATE INDEX IX_MediaProcessingJobs_JobType ON dbo.MediaProcessingJobs(JobType);

CREATE INDEX IX_MediaCollections_Type ON dbo.MediaCollections(CollectionType);
CREATE INDEX IX_MediaCollections_IsPublic ON dbo.MediaCollections(IsPublic);

CREATE INDEX IX_MediaAccessLogs_MediaFile ON dbo.MediaAccessLogs(MediaFileId);
CREATE INDEX IX_MediaAccessLogs_User ON dbo.MediaAccessLogs(UserId);
CREATE INDEX IX_MediaAccessLogs_Action ON dbo.MediaAccessLogs(AccessAction);
CREATE INDEX IX_MediaAccessLogs_Timestamp ON dbo.MediaAccessLogs(AccessTimestamp);

CREATE INDEX IX_MediaMetadata_MediaFile ON dbo.MediaMetadata(MediaFileId);
CREATE INDEX IX_MediaMetadata_Key ON dbo.MediaMetadata(MetadataKey);
CREATE INDEX IX_MediaMetadata_Searchable ON dbo.MediaMetadata(IsSearchable);

-- Add indexes for Question Management tables
CREATE INDEX IX_QuestionBanks_IsPublic ON dbo.QuestionBanks(IsPublic);
CREATE INDEX IX_QuestionBanks_IsActive ON dbo.QuestionBanks(IsActive);

CREATE INDEX IX_QuestionBankCategories_QuestionBankId ON dbo.QuestionBankCategories(QuestionBankId);
CREATE INDEX IX_QuestionBankCategories_ParentCategoryId ON dbo.QuestionBankCategories(ParentCategoryId);
CREATE INDEX IX_QuestionBankCategories_IsActive ON dbo.QuestionBankCategories(IsActive);

CREATE INDEX IX_QuestionBankHierarchy_Ancestor ON dbo.QuestionBankHierarchy(AncestorId);
CREATE INDEX IX_QuestionBankHierarchy_Descendant ON dbo.QuestionBankHierarchy(DescendantId);
CREATE INDEX IX_QuestionBankHierarchy_Depth ON dbo.QuestionBankHierarchy(Depth);

CREATE INDEX IX_QuestionCategorizations_Question ON dbo.QuestionCategorizations(QuestionId);
CREATE INDEX IX_QuestionCategorizations_Category ON dbo.QuestionCategorizations(CategoryId);
CREATE INDEX IX_QuestionCategorizations_IsPrimary ON dbo.QuestionCategorizations(IsPrimary);

CREATE INDEX IX_QuestionTags_Question ON dbo.QuestionTags(QuestionId);
CREATE INDEX IX_QuestionTags_Name ON dbo.QuestionTags(TagName);

CREATE INDEX IX_QuestionTemplates_IsActive ON dbo.QuestionTemplates(IsActive);

CREATE INDEX IX_QuestionVersions_Question ON dbo.QuestionVersions(QuestionId);
CREATE INDEX IX_QuestionVersions_Status ON dbo.QuestionVersions(VersionStatus);

CREATE INDEX IX_QuestionValidations_Question ON dbo.QuestionValidations(QuestionId);
CREATE INDEX IX_QuestionValidations_Validator ON dbo.QuestionValidations(ValidatorId);
CREATE INDEX IX_QuestionValidations_Status ON dbo.QuestionValidations(ValidationStatus);

CREATE INDEX IX_QuestionUsageHistory_Question ON dbo.QuestionUsageHistory(QuestionId);
CREATE INDEX IX_QuestionUsageHistory_UsageType ON dbo.QuestionUsageHistory(UsageType);
CREATE INDEX IX_QuestionUsageHistory_UsedAt ON dbo.QuestionUsageHistory(UsedAt);

CREATE INDEX IX_QuestionImportBatches_Status ON dbo.QuestionImportBatches(ImportStatus);
CREATE INDEX IX_QuestionImportBatches_CreatedBy ON dbo.QuestionImportBatches(CreatedBy);

-- Add indexes for Notification & Audit tables
CREATE INDEX IX_NotificationTemplates_Type ON dbo.NotificationTemplates(TemplateType);
CREATE INDEX IX_NotificationTemplates_IsActive ON dbo.NotificationTemplates(IsActive);

CREATE INDEX IX_NotificationPreferences_User ON dbo.NotificationPreferences(UserId);
CREATE INDEX IX_NotificationPreferences_Type ON dbo.NotificationPreferences(NotificationType);

CREATE INDEX IX_Notifications_User ON dbo.Notifications(UserId);
CREATE INDEX IX_Notifications_Type ON dbo.Notifications(NotificationType);
CREATE INDEX IX_Notifications_IsRead ON dbo.Notifications(IsRead);
CREATE INDEX IX_Notifications_SentAt ON dbo.Notifications(SentAt);

CREATE INDEX IX_NotificationHistory_Notification ON dbo.NotificationHistory(NotificationId);
CREATE INDEX IX_NotificationHistory_Action ON dbo.NotificationHistory(Action);
CREATE INDEX IX_NotificationHistory_Timestamp ON dbo.NotificationHistory(ActionTimestamp);

-- Fix AuditLogs table structure to match C# entity expectations
CREATE TABLE dbo.AuditLogs (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NULL,
    UserIdentifier NVARCHAR(100) NULL,
    Action NVARCHAR(100) NOT NULL,
    EntityType NVARCHAR(100) NULL,
    EntityId NVARCHAR(100) NULL,
    OldValues NVARCHAR(MAX) NULL,
    NewValues NVARCHAR(MAX) NULL,
    IpAddress NVARCHAR(45) NULL,
    UserAgent NVARCHAR(500) NULL,
    SessionId NVARCHAR(100) NULL,
    CorrelationId NVARCHAR(100) NULL,
    Severity NVARCHAR(50) NULL,
    Category NVARCHAR(100) NULL,
    Description NVARCHAR(500) NULL,
    Details NVARCHAR(MAX) NULL,
    Timestamp DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    IsSystemAction BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy INT NULL,
    ModifiedAt DATETIME2 NULL,
    ModifiedBy INT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    DeletedAt DATETIME2 NULL,
    DeletedBy INT NULL,
    CONSTRAINT FK_AuditLogs_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_AuditLogs_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_AuditLogs_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_AuditLogs_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId)
);

-- Create indexes for AuditLogs table
CREATE INDEX IX_AuditLogs_UserId ON dbo.AuditLogs(UserId);
CREATE INDEX IX_AuditLogs_Action ON dbo.AuditLogs(Action);
CREATE INDEX IX_AuditLogs_EntityType ON dbo.AuditLogs(EntityType);
CREATE INDEX IX_AuditLogs_Timestamp ON dbo.AuditLogs(Timestamp);
CREATE INDEX IX_AuditLogs_CorrelationId ON dbo.AuditLogs(CorrelationId);

-- Add foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Categories FOREIGN KEY (CategoryId) REFERENCES dbo.MediaCategories(Id);

-- Add foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_QuestionBanks FOREIGN KEY (QuestionBankId) REFERENCES dbo.QuestionBanks(Id);

-- Add foreign key constraints for Questions
ALTER TABLE dbo.Questions ADD CONSTRAINT FK_Questions_QuestionBanks FOREIGN KEY (PrimaryTreeNodeId) REFERENCES dbo.QuestionBankCategories(Id);

-- Add remaining foreign key constraints for new tables
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ParentCategory 
    FOREIGN KEY (ParentCategoryId) REFERENCES dbo.QuestionBankCategories(Id);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ParentCategory 
    FOREIGN KEY (ParentCategoryId) REFERENCES dbo.MediaCategories(Id);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_MediaFiles 
    FOREIGN KEY (MediaFileId) REFERENCES dbo.MediaFiles(Id);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_MediaFiles 
    FOREIGN KEY (MediaFileId) REFERENCES dbo.MediaFiles(Id);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CoverMediaFile 
    FOREIGN KEY (CoverMediaFileId) REFERENCES dbo.MediaFiles(Id);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_Collections 
    FOREIGN KEY (CollectionId) REFERENCES dbo.MediaCollections(Id);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_MediaFiles 
    FOREIGN KEY (MediaFileId) REFERENCES dbo.MediaFiles(Id);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_Users 
    FOREIGN KEY (AddedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_MediaFiles 
    FOREIGN KEY (MediaFileId) REFERENCES dbo.MediaFiles(Id);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_MediaFiles 
    FOREIGN KEY (MediaFileId) REFERENCES dbo.MediaFiles(Id);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_Categories 
    FOREIGN KEY (CategoryId) REFERENCES dbo.QuestionBankCategories(Id);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

ALTER TABLE dbo.QuestionTemplates ADD CONSTRAINT FK_QuestionTemplates_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTemplates ADD CONSTRAINT FK_QuestionTemplates_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTemplates ADD CONSTRAINT FK_QuestionTemplates_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_Validators 
    FOREIGN KEY (ValidatorId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_Exams 
    FOREIGN KEY (UsedInExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_Users 
    FOREIGN KEY (UsedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionImportBatches ADD CONSTRAINT FK_QuestionImportBatches_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionImportBatches ADD CONSTRAINT FK_QuestionImportBatches_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionImportBatches ADD CONSTRAINT FK_QuestionImportBatches_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraint for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_MediaCategories 
    FOREIGN KEY (CategoryId) REFERENCES dbo.MediaCategories(Id);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_UploadedBy 
    FOREIGN KEY (UploadedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraint for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_MediaFiles 
    FOREIGN KEY (MediaFileId) REFERENCES dbo.MediaFiles(Id);

-- Add missing foreign key constraint for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_MediaFiles 
    FOREIGN KEY (MediaFileId) REFERENCES dbo.MediaFiles(Id);

-- Add missing foreign key constraint for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CoverMediaFile 
    FOREIGN KEY (CoverMediaFileId) REFERENCES dbo.MediaFiles(Id);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_Collections 
    FOREIGN KEY (CollectionId) REFERENCES dbo.MediaCollections(Id);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_MediaFiles 
    FOREIGN KEY (MediaFileId) REFERENCES dbo.MediaFiles(Id);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_Users 
    FOREIGN KEY (AddedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_MediaFiles 
    FOREIGN KEY (MediaFileId) REFERENCES dbo.MediaFiles(Id);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraint for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_MediaFiles 
    FOREIGN KEY (MediaFileId) REFERENCES dbo.MediaFiles(Id);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_Categories 
    FOREIGN KEY (CategoryId) REFERENCES dbo.QuestionBankCategories(Id);

-- Add missing foreign key constraint for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for QuestionTemplates
ALTER TABLE dbo.QuestionTemplates ADD CONSTRAINT FK_QuestionTemplates_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTemplates ADD CONSTRAINT FK_QuestionTemplates_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTemplates ADD CONSTRAINT FK_QuestionTemplates_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraint for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_Validators 
    FOREIGN KEY (ValidatorId) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_Exams 
    FOREIGN KEY (UsedInExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_Users 
    FOREIGN KEY (UsedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionUsageHistory
ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionUsageHistory ADD CONSTRAINT FK_QuestionUsageHistory_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for ExamQuestions
ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Exams 
    FOREIGN KEY (ExamId) REFERENCES dbo.Exams(ExamId);

ALTER TABLE dbo.ExamQuestions ADD CONSTRAINT FK_EQ_Questions 
    FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(QuestionId);

-- Add missing foreign key constraints for Notifications
ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.Notifications ADD CONSTRAINT FK_Notifications_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for NotificationHistory
ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Notifications 
    FOREIGN KEY (NotificationId) REFERENCES dbo.Notifications(Id);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.NotificationHistory ADD CONSTRAINT FK_NotificationHistory_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for AuditLogs
ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_Users 
    FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.AuditLogs ADD CONSTRAINT FK_AuditLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaFiles
ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaFiles ADD CONSTRAINT FK_MediaFiles_Users_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBanks
ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBanks ADD CONSTRAINT FK_QuestionBanks_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionBankCategories
ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionBankCategories ADD CONSTRAINT FK_QuestionBankCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCategories
ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCategories ADD CONSTRAINT FK_MediaCategories_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaThumbnails
ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaThumbnails ADD CONSTRAINT FK_MediaThumbnails_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaProcessingJobs
ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaProcessingJobs ADD CONSTRAINT FK_MediaProcessingJobs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollections
ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollections ADD CONSTRAINT FK_MediaCollections_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaCollectionItems
ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaCollectionItems ADD CONSTRAINT FK_MediaCollectionItems_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaAccessLogs
ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaAccessLogs ADD CONSTRAINT FK_MediaAccessLogs_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for MediaMetadata
ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.MediaMetadata ADD CONSTRAINT FK_MediaMetadata_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionCategorizations
ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionCategorizations ADD CONSTRAINT FK_QuestionCategorizations_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionTags
ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionTags ADD CONSTRAINT FK_QuestionTags_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionVersions
ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_CreatedBy 
    FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_ModifiedBy 
    FOREIGN KEY (ModifiedBy) REFERENCES dbo.Users(UserId);

ALTER TABLE dbo.QuestionVersions ADD CONSTRAINT FK_QuestionVersions_DeletedBy 
    FOREIGN KEY (DeletedBy) REFERENCES dbo.Users(UserId);

-- Add missing foreign key constraints for QuestionValidations
ALTER TABLE dbo.QuestionValidations ADD CONSTRAINT FK_QuestionValidations_