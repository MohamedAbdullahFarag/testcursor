namespace Ikhtibar.Shared.Enums
{
    public enum QuestionType
    {
        MultipleChoice = 1,
        TrueFalse = 2,
        FillInTheBlank = 3,
        ShortAnswer = 4,
        Essay = 5,
        Matching = 6,
        Ordering = 7,
        Hotspot = 8,
        DragAndDrop = 9,
        Numeric = 10,
        MultipleResponse = 11
    }

    public enum DifficultyLevel
    {
        Beginner = 1,
        Easy = 2,
        Medium = 3,
        Hard = 4,
        Expert = 5
    }

    public enum QuestionStatus
    {
        Draft = 1,
        UnderReview = 2,
        Approved = 3,
        Published = 4,
        Archived = 5,
        Rejected = 6,
        NeedsRevision = 7
    }

    public enum VersionStatus
    {
        Draft = 1,
        UnderReview = 2,
        Approved = 3,
        Published = 4,
        Archived = 5,
        Deprecated = 6
    }

    public enum ValidationStatus
    {
        Pending = 1,
        InProgress = 2,
        Approved = 3,
        Rejected = 4,
        NeedsRevision = 5,
        Completed = 6
    }

    public enum ImportStatus
    {
        Pending = 1,
        InProgress = 2,
        Completed = 3,
        Failed = 4,
        PartiallyCompleted = 5,
        Cancelled = 6
    }

    public enum AnswerType
    {
        Correct = 1,
        Incorrect = 2,
        PartiallyCorrect = 3,
        Distractor = 4
    }

    public enum MediaType
    {
        Image = 1,
        Audio = 2,
        Video = 3,
        Document = 4,
        Interactive = 5
    }

    public enum UsageType
    {
        Practice = 1,
        Exam = 2,
        Quiz = 3,
        Assessment = 4,
        Review = 5,
        Study = 6
    }

    public enum PermissionLevel
    {
        Read = 1,
        Write = 2,
        Admin = 3,
        Owner = 4
    }

    public enum QualityLevel
    {
        Poor = 1,
        Fair = 2,
        Good = 3,
        Excellent = 4,
        Outstanding = 5
    }

    public enum SeverityLevel
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }

    public enum IssueType
    {
        Content = 1,
        Format = 2,
        Business = 3,
        Technical = 4,
        Accessibility = 5,
        Security = 6
    }
}
