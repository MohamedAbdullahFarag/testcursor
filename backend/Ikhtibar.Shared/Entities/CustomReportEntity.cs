using System;
using System.Collections.Generic;

namespace Ikhtibar.Shared.Entities
{
    public class CustomReportEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        // Note: CreatedAt and UpdatedAt are inherited from BaseEntity
    }
}
