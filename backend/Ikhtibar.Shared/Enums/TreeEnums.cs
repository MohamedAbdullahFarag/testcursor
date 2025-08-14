namespace Ikhtibar.Shared.Enums;

/// <summary>
/// Strategy for handling child categories when deleting a parent category
/// </summary>
public enum ChildHandlingStrategy
{
    /// <summary>
    /// Delete all child categories recursively
    /// </summary>
    Delete = 0,
    
    /// <summary>
    /// Move child categories to the parent's parent
    /// </summary>
    MoveToParent = 1,
    
    /// <summary>
    /// Move child categories to root level
    /// </summary>
    MoveToRoot = 2,
    
    /// <summary>
    /// Prevent deletion if category has children
    /// </summary>
    Prevent = 3,
    MoveWithParent = 4
}

/// <summary>
/// Strategy for merging imported tree data
/// </summary>
public enum MergeStrategy
{
    /// <summary>
    /// Skip categories that already exist
    /// </summary>
    Skip = 0,
    
    /// <summary>
    /// Overwrite existing categories with imported data
    /// </summary>
    Overwrite = 1,
    
    /// <summary>
    /// Merge data where possible
    /// </summary>
    Merge = 2,
    
    /// <summary>
    /// Create new items with modified names
    /// </summary>
    CreateNew = 3
}

/// <summary>
/// Type of question bank category
/// </summary>
public enum CategoryType
{
    /// <summary>
    /// Top-level subject (Mathematics, Science)
    /// </summary>
    Subject = 1,
    
    /// <summary>
    /// Chapter or major topic
    /// </summary>
    Chapter = 2,
    
    /// <summary>
    /// Specific topic within chapter
    /// </summary>
    Topic = 3,
    
    /// <summary>
    /// Detailed subtopic
    /// </summary>
    Subtopic = 4,
    
    /// <summary>
    /// Specific skill or competency
    /// </summary>
    Skill = 5,
    
    /// <summary>
    /// Learning objective or outcome
    /// </summary>
    Objective = 6
}

/// <summary>
/// Level of category in the hierarchy
/// </summary>
public enum CategoryLevel
{
    /// <summary>
    /// Root level categories
    /// </summary>
    Level1 = 1,
    
    /// <summary>
    /// Second level
    /// </summary>
    Level2 = 2,
    
    /// <summary>
    /// Third level
    /// </summary>
    Level3 = 3,
    
    /// <summary>
    /// Fourth level
    /// </summary>
    Level4 = 4,
    
    /// <summary>
    /// Fifth level
    /// </summary>
    Level5 = 5,
    
    /// <summary>
    /// Maximum depth level
    /// </summary>
    Level6 = 6
}
