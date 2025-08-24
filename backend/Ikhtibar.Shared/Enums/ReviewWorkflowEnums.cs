namespace Ikhtibar.Shared.Enums
{
    public enum ReviewStatus
    {
        Pending = 1,
        InProgress = 2,
        Approved = 3,
        Rejected = 4,
        NeedsRevision = 5,
        Cancelled = 6
    }

    public enum WorkflowStatus
    {
        Draft = 1,
        Active = 2,
        Paused = 3,
        Completed = 4,
        Cancelled = 5
    }
}
