using System;
using System.Collections.Generic;

namespace Ikhtibar.Shared.DTOs
{
    public class CustomAnalyticsRequestDto
    {
        public string ReportType { get; set; } = string.Empty;
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public IEnumerable<string> Metrics { get; set; } = new List<string>();
        public IEnumerable<string> Filters { get; set; } = new List<string>();
        public string? GroupBy { get; set; }
        public string? SortBy { get; set; }
        public bool Ascending { get; set; } = true;
        public int? Limit { get; set; }
    }

    public class CustomAnalyticsReportDto
    {
        public string ReportType { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
        public IEnumerable<string> Metrics { get; set; } = new List<string>();
        public IEnumerable<string> Filters { get; set; } = new List<string>();
        public object Data { get; set; } = new object();
        public string? Summary { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }

    public class AnalyticsExportRequestDto
    {
        public string ExportType { get; set; } = string.Empty;
        public string Format { get; set; } = "excel";
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public IEnumerable<string> Metrics { get; set; } = new List<string>();
        public IEnumerable<string> Filters { get; set; } = new List<string>();
        public bool IncludeCharts { get; set; } = true;
        public bool IncludeSummary { get; set; } = true;
    }

    public class AnalyticsExportDto
    {
        public string ExportType { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime ExportedAt { get; set; }
        public string? DownloadUrl { get; set; }
    }
}
