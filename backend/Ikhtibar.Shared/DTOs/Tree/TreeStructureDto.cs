namespace Ikhtibar.Shared.DTOs.Tree;

/// <summary>
/// Data transfer object for complete tree structure responses.
/// </summary>
public class TreeStructureDto
{
    /// <summary>
    /// Collection of root nodes forming the tree structure.
    /// </summary>
    public List<TreeNodeDto> RootNodes { get; set; } = new List<TreeNodeDto>();

    /// <summary>
    /// Total number of nodes in the tree.
    /// </summary>
    public int TotalNodeCount { get; set; }

    /// <summary>
    /// Maximum depth of the tree.
    /// </summary>
    public int MaxDepth { get; set; }

    /// <summary>
    /// Total number of leaf nodes.
    /// </summary>
    public int LeafNodeCount { get; set; }

    /// <summary>
    /// Total number of internal nodes (nodes with children).
    /// </summary>
    public int InternalNodeCount { get; set; }

    /// <summary>
    /// When the tree structure was last updated.
    /// </summary>
    public DateTime LastUpdated { get; set; }

    /// <summary>
    /// Gets the total number of nodes in the tree.
    /// </summary>
    /// <returns>Total node count</returns>
    public int GetTotalNodeCount() => RootNodes.Sum(r => 1 + r.GetDescendantCount());

    /// <summary>
    /// Gets the maximum depth of the tree.
    /// </summary>
    /// <returns>Maximum depth</returns>
    public int GetMaxDepth() => RootNodes.Any() ? RootNodes.Max(r => r.Level) + 1 : 0;

    /// <summary>
    /// Gets the count of leaf nodes.
    /// </summary>
    /// <returns>Leaf node count</returns>
    public int GetLeafNodeCount() => RootNodes.Sum(r => GetLeafCountRecursive(r));

    /// <summary>
    /// Gets the count of internal nodes.
    /// </summary>
    /// <returns>Internal node count</returns>
    public int GetInternalNodeCount() => GetTotalNodeCount() - GetLeafNodeCount();

    /// <summary>
    /// Recursively counts leaf nodes in a subtree.
    /// </summary>
    /// <param name="node">The root node of the subtree</param>
    /// <returns>Leaf node count</returns>
    private static int GetLeafCountRecursive(TreeNodeDto node)
    {
        if (node.IsLeaf())
            return 1;
        
        return node.Children.Sum(GetLeafCountRecursive);
    }

    /// <summary>
    /// Finds a node by ID in the tree structure.
    /// </summary>
    /// <param name="nodeId">The node ID to find</param>
    /// <returns>The node if found, null otherwise</returns>
    public TreeNodeDto? FindNodeById(int nodeId)
    {
        foreach (var rootNode in RootNodes)
        {
            var found = FindNodeRecursive(rootNode, nodeId);
            if (found != null)
                return found;
        }
        return null;
    }

    /// <summary>
    /// Recursively searches for a node by ID in a subtree.
    /// </summary>
    /// <param name="node">The root node of the subtree</param>
    /// <param name="nodeId">The node ID to find</param>
    /// <returns>The node if found, null otherwise</returns>
    private static TreeNodeDto? FindNodeRecursive(TreeNodeDto node, int nodeId)
    {
        if (node.Id == nodeId)
            return node;
        
        foreach (var child in node.Children)
        {
            var found = FindNodeRecursive(child, nodeId);
            if (found != null)
                return found;
        }
        return null;
    }

    /// <summary>
    /// Gets all nodes at a specific depth.
    /// </summary>
    /// <param name="depth">The depth level</param>
    /// <returns>Collection of nodes at the specified depth</returns>
    public List<TreeNodeDto> GetNodesAtDepth(int depth)
    {
        var result = new List<TreeNodeDto>();
        foreach (var rootNode in RootNodes)
        {
            GetNodesAtDepthRecursive(rootNode, depth, 0, result);
        }
        return result;
    }

    /// <summary>
    /// Recursively collects nodes at a specific depth.
    /// </summary>
    /// <param name="node">The current node</param>
    /// <param name="targetDepth">The target depth</param>
    /// <param name="currentDepth">The current depth</param>
    /// <param name="result">The result collection</param>
    private static void GetNodesAtDepthRecursive(TreeNodeDto node, int targetDepth, int currentDepth, List<TreeNodeDto> result)
    {
        if (currentDepth == targetDepth)
        {
            result.Add(node);
            return;
        }
        
        foreach (var child in node.Children)
        {
            GetNodesAtDepthRecursive(child, targetDepth, currentDepth + 1, result);
        }
    }
}
