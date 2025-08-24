using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    public class QuestionVersionMetadataDto
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string VersionNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // Draft, Review, Approved, Rejected
        public string? ChangeDescription { get; set; }
        public string? ChangeReason { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModified { get; set; }
        public int? ReviewedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewComments { get; set; }
    }

    public class UpdateVersionMetadataDto
    {
        [MaxLength(500)]
        public string? ChangeDescription { get; set; }
        
        [MaxLength(500)]
        public string? ChangeReason { get; set; }
        
        [MaxLength(50)]
        public string? Status { get; set; }
        
        [MaxLength(1000)]
        public string? ReviewComments { get; set; }
        
        public int? ReviewedBy { get; set; }
    }


}
