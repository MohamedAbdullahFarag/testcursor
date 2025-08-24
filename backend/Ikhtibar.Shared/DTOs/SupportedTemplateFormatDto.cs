using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.DTOs
{
    public class SupportedTemplateFormatDto
    {
        public int Id { get; set; }
        public string FormatName { get; set; } = string.Empty;
        public string FileExtension { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsSupported { get; set; }
        public int MaxFileSize { get; set; } // in bytes
        public string? ValidationRules { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
