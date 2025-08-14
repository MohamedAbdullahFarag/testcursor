using System.ComponentModel.DataAnnotations;


namespace Ikhtibar.Shared.Entities;

/// <summary>
/// Base entity with common audit fields and soft delete functionality
/// Provides audit trails and soft delete for all entities following schema requirements
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Primary key - virtual to allow override in specific entities
    /// For entities with different primary key names (UserId, RoleId, etc.)
    /// Schema uses INT IDENTITY primary keys, not GUIDs
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Creation timestamp
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// User ID who created the record
    /// </summary>
    public int? CreatedBy { get; set; }

    /// <summary>
    /// Last modification timestamp
    /// </summary>
    public DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// User ID who last modified the record
    /// </summary>
    public int? ModifiedBy { get; set; }

    /// <summary>
    /// Soft delete timestamp
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// User ID who deleted the record (soft delete)
    /// </summary>
    public int? DeletedBy { get; set; }

    /// <summary>
    /// Soft delete flag
    /// </summary>
    [Required]
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// Optimistic concurrency token (SQL Server ROWVERSION)
    /// </summary>
    [Timestamp]
    public byte[]? RowVersion { get; set; }
}
