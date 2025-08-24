using System;
using System.Collections.Generic;

namespace Ikhtibar.Shared.Entities
{
    public class AnalyticsDashboardEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        // Note: CreatedAt and ModifiedAt are inherited from BaseEntity
    }
}
