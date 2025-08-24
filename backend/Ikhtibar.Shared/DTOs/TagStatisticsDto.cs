using System;
using System.Collections.Generic;

namespace Ikhtibar.Shared.DTOs
{
    public class TagStatisticsDto
    {
        public int TotalTags { get; set; }
        public Dictionary<string,int>? TopTags { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
