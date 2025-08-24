using System;
using System.Collections.Generic;

namespace Ikhtibar.Shared.DTOs
{
    public class SaveSearchQueryDto
    {
        public string Name { get; set; } = string.Empty;
        public string Query { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsPublic { get; set; } = false;
    }

    public class SavedSearchQueryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Query { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
    }

    public class PopularSearchTermDto
    {
        public string Term { get; set; } = string.Empty;
        public int Count { get; set; }
        public DateTime LastSearched { get; set; }
    }
}
