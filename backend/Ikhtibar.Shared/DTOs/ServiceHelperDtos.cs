using System.Collections.Generic;

namespace Ikhtibar.Shared.DTOs
{
    public class CloneQuestionBankDto
    {
        public string? Name { get; set; }
        public bool CloneQuestions { get; set; } = true;
        public int ClonedBy { get; set; }
    }

    public class GrantQuestionBankAccessDto
    {
        public int UserId { get; set; }
        public string Permission { get; set; } = string.Empty;
        public DateTime? ExpiresAt { get; set; }
    }

    public class TagSearchDto
    {
        public string? Query { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    // TagStatisticsDto removed (defined in QuestionAnalyticsDtos.cs)

    public class BulkValidationDto
    {
        public IEnumerable<int> QuestionIds { get; set; } = new List<int>();
        public int RequestedBy { get; set; }
    }

    public class TemplateSearchDto
    {
        public string? Query { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class TemplateStatisticsDto
    {
        public int TotalTemplates { get; set; }
    }
}
