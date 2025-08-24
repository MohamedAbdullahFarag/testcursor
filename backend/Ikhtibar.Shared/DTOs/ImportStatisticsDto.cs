using System;

namespace Ikhtibar.Shared.DTOs
{
    public class ImportStatisticsDto
    {
        public int TotalBatches { get; set; }
        public int SuccessfulImports { get; set; }
        public int FailedImports { get; set; }
        public DateTime? LastImportAt { get; set; }
    }
}
