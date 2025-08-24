# Question Bank Tree Management System - Comprehensive Implementation PRP

## 🎯 Executive Summary
Generate a comprehensive hierarchical tree management system for the Ikhtibar question bank module that enables organizing questions, categories, subjects, and topics in a nested tree structure. This system will provide efficient categorization, navigation, and management of educational content with support for curriculum alignment and question organization.

## 📋 What to Generate

### 1. Backend Tree Management System
```
backend/Ikhtibar.Core/Entities/
├── QuestionBankCategory.cs            # Main category entity
├── QuestionBankSubject.cs             # Subject organization entity
├── QuestionBankTopic.cs               # Topic/subtopic entity
├── QuestionBankHierarchy.cs           # Hierarchy relationship entity
└── QuestionCategorization.cs          # Question-category association

backend/Ikhtibar.Core/Services/Interfaces/
├── IQuestionBankTreeService.cs        # Core tree operations
├── IQuestionCategoryService.cs        # Category management
├── IQuestionBankOrganizationService.cs # Organization operations
└── IQuestionHierarchyService.cs       # Hierarchy management

backend/Ikhtibar.Core/Services/Implementations/
├── QuestionBankTreeService.cs         # Main tree logic
├── QuestionCategoryService.cs         # Category operations
├── QuestionBankOrganizationService.cs # Organization management
└── QuestionHierarchyService.cs        # Hierarchy operations

backend/Ikhtibar.Core/Repositories/Interfaces/
├── IQuestionBankCategoryRepository.cs # Category data access
├── IQuestionBankTreeRepository.cs     # Tree operations repository
├── IQuestionHierarchyRepository.cs    # Hierarchy data access
└── IQuestionOrganizationRepository.cs # Organization data access

backend/Ikhtibar.Infrastructure/Repositories/
├── QuestionBankCategoryRepository.cs  # Category repository implementation
├── QuestionBankTreeRepository.cs      # Tree repository implementation
├── QuestionHierarchyRepository.cs     # Hierarchy repository implementation
└── QuestionOrganizationRepository.cs  # Organization repository implementation

backend/Ikhtibar.API/Controllers/
├── QuestionBankTreeController.cs      # Tree management endpoints
├── QuestionCategoryController.cs      # Category management endpoints
└── QuestionHierarchyController.cs     # Hierarchy management endpoints

backend/Ikhtibar.API/DTOs/
├── QuestionBankCategoryDto.cs         # Category data transfer objects
├── QuestionBankTreeDto.cs             # Tree structure objects
├── QuestionHierarchyDto.cs            # Hierarchy data objects
├── CreateCategoryDto.cs               # Category creation objects
└── TreeOperationDto.cs                # Tree operation objects
```

### 2. Frontend Tree Management Interface
```
frontend/src/modules/question-bank/
├── components/
│   ├── QuestionBankTree.tsx           # Main tree component
│   ├── TreeNode.tsx                   # Individual tree node
│   ├── CategoryManager.tsx            # Category management
│   ├── CategoryCreator.tsx            # Category creation form
│   ├── CategoryEditor.tsx             # Category editing form
│   ├── TreeNavigator.tsx              # Tree navigation component
│   ├── TreeSearch.tsx                 # Tree search functionality
│   ├── TreeActions.tsx                # Tree action buttons
│   ├── BreadcrumbNavigation.tsx       # Breadcrumb trail
│   ├── TreeDragDrop.tsx               # Drag & drop functionality
│   └── CategoryFilters.tsx            # Category filtering
├── hooks/
│   ├── useQuestionBankTree.tsx        # Main tree management hook
│   ├── useTreeNavigation.tsx          # Navigation functionality
│   ├── useTreeOperations.tsx          # Tree operations
│   ├── useCategoryManager.tsx         # Category management
│   └── useTreeSearch.tsx              # Search functionality
├── services/
│   ├── questionBankTreeService.ts     # Tree API service
│   ├── categoryService.ts             # Category operations
│   └── hierarchyService.ts            # Hierarchy operations
├── types/
│   ├── questionBankTree.types.ts      # Tree type definitions
│   ├── category.types.ts              # Category types
│   └── hierarchy.types.ts             # Hierarchy types
├── locales/
│   ├── en.json                        # English translations
│   └── ar.json                        # Arabic translations
└── constants/
    ├── treeOperations.ts              # Tree operation constants
    └── categoryTypes.ts               # Category type constants
```

### 3. Integration Components
```
backend/Ikhtibar.Core/Integrations/
├── TreeMigrations/
│   ├── ExistingTreeNodeMigrator.cs    # Migrate existing tree structure
│   └── QuestionCategoryMigrator.cs    # Migrate question categories
└── TreeValidators/
    ├── TreeStructureValidator.cs      # Tree structure validation
    └── CategoryHierarchyValidator.cs   # Category hierarchy validation
```

## 🏗 Implementation Architecture

### Entity Design Patterns

#### Question Bank Category Entity
```csharp
[Table("QuestionBankCategories")]
public class QuestionBankCategory : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public CategoryType Type { get; set; }

    [Required]
    public CategoryLevel Level { get; set; }

    public int? ParentId { get; set; }

    [Required]
    public int SortOrder { get; set; } = 0;

    [Required]
    [MaxLength(500)]
    public string TreePath { get; set; } = string.Empty;

    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    public bool AllowQuestions { get; set; } = true;

    [MaxLength(2000)]
    public string? MetadataJson { get; set; }

    // Curriculum alignment
    [MaxLength(100)]
    public string? CurriculumCode { get; set; }

    [MaxLength(100)]
    public string? GradeLevel { get; set; }

    [MaxLength(100)]
    public string? Subject { get; set; }

    // Navigation properties
    public virtual QuestionBankCategory? Parent { get; set; }
    public virtual ICollection<QuestionBankCategory> Children { get; set; } = new List<QuestionBankCategory>();
    public virtual ICollection<QuestionCategorization> Questions { get; set; } = new List<QuestionCategorization>();
    public virtual ICollection<QuestionBankHierarchy> HierarchyRelations { get; set; } = new List<QuestionBankHierarchy>();
}

public enum CategoryType
{
    Subject = 1,
    Chapter = 2,
    Topic = 3,
    Subtopic = 4,
    Skill = 5,
    Objective = 6
}

public enum CategoryLevel
{
    Level1 = 1,
    Level2 = 2,
    Level3 = 3,
    Level4 = 4,
    Level5 = 5,
    Level6 = 6
}
```

#### Question Categorization Entity
```csharp
[Table("QuestionCategorizations")]
public class QuestionCategorization : BaseEntity
{
    [Required]
    public int QuestionId { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public bool IsPrimary { get; set; } = false;

    [Required]
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public int AssignedBy { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    // Navigation properties
    [ForeignKey("QuestionId")]
    public virtual Question Question { get; set; } = null!;

    [ForeignKey("CategoryId")]
    public virtual QuestionBankCategory Category { get; set; } = null!;

    [ForeignKey("AssignedBy")]
    public virtual User AssignedByUser { get; set; } = null!;
}
```

#### Question Bank Hierarchy Entity
```csharp
[Table("QuestionBankHierarchies")]
public class QuestionBankHierarchy : BaseEntity
{
    [Required]
    public int AncestorId { get; set; }

    [Required]
    public int DescendantId { get; set; }

    [Required]
    public int Depth { get; set; }

    [Required]
    [MaxLength(500)]
    public string Path { get; set; } = string.Empty;

    // Navigation properties
    [ForeignKey("AncestorId")]
    public virtual QuestionBankCategory Ancestor { get; set; } = null!;

    [ForeignKey("DescendantId")]
    public virtual QuestionBankCategory Descendant { get; set; } = null!;
}
```

### Service Implementation Patterns

#### Core Question Bank Tree Service
```csharp
public interface IQuestionBankTreeService
{
    // Tree structure operations
    Task<QuestionBankTreeDto> GetCompleteTreeAsync();
    Task<QuestionBankTreeDto> GetTreeByRootAsync(int rootId, int maxDepth = 5);
    Task<QuestionBankCategoryDto> GetCategoryAsync(int categoryId);
    Task<PagedResult<QuestionBankCategoryDto>> GetCategoriesAsync(CategoryFilterDto filter);
    
    // Category operations
    Task<QuestionBankCategoryDto> CreateCategoryAsync(CreateCategoryDto dto);
    Task<QuestionBankCategoryDto> UpdateCategoryAsync(int categoryId, UpdateCategoryDto dto);
    Task<bool> DeleteCategoryAsync(int categoryId);
    Task<bool> MoveCategoryAsync(int categoryId, MoveCategoryDto dto);
    
    // Tree navigation
    Task<IEnumerable<QuestionBankCategoryDto>> GetCategoryPathAsync(int categoryId);
    Task<IEnumerable<QuestionBankCategoryDto>> GetChildrenAsync(int parentId);
    Task<IEnumerable<QuestionBankCategoryDto>> GetSiblingsAsync(int categoryId);
    Task<IEnumerable<QuestionBankCategoryDto>> GetAncestorsAsync(int categoryId);
    Task<IEnumerable<QuestionBankCategoryDto>> GetDescendantsAsync(int categoryId);
    
    // Question categorization
    Task<bool> AssignQuestionToCategoryAsync(int questionId, int categoryId, bool isPrimary = false);
    Task<bool> RemoveQuestionFromCategoryAsync(int questionId, int categoryId);
    Task<IEnumerable<QuestionBankCategoryDto>> GetQuestionCategoriesAsync(int questionId);
    Task<PagedResult<QuestionDto>> GetCategoryQuestionsAsync(int categoryId, QuestionFilterDto filter);
    
    // Tree validation
    Task<bool> ValidateTreeStructureAsync();
    Task<bool> ValidateCategoryHierarchyAsync(int categoryId);
    Task<TreeValidationResult> GetTreeValidationReportAsync();
    
    // Search and filter
    Task<IEnumerable<QuestionBankCategoryDto>> SearchCategoriesAsync(CategorySearchDto searchDto);
    Task<PagedResult<QuestionDto>> SearchQuestionsInTreeAsync(TreeQuestionSearchDto searchDto);
    
    // Bulk operations
    Task<bool> BulkMoveCategoriesAsync(BulkMoveCategoryDto dto);
    Task<bool> BulkAssignQuestionsAsync(BulkAssignQuestionsDto dto);
    Task<bool> BulkUpdateCategoriesAsync(BulkUpdateCategoryDto dto);
}

public class QuestionBankTreeService : IQuestionBankTreeService
{
    private readonly IQuestionBankCategoryRepository _categoryRepository;
    private readonly IQuestionBankTreeRepository _treeRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IQuestionCategoryRepository _questionCategoryRepository;
    private readonly ILogger<QuestionBankTreeService> _logger;
    private readonly IMapper _mapper;

    public QuestionBankTreeService(
        IQuestionBankCategoryRepository categoryRepository,
        IQuestionBankTreeRepository treeRepository,
        IQuestionRepository questionRepository,
        IQuestionCategoryRepository questionCategoryRepository,
        ILogger<QuestionBankTreeService> logger,
        IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _treeRepository = treeRepository;
        _questionRepository = questionRepository;
        _questionCategoryRepository = questionCategoryRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<QuestionBankTreeDto> GetCompleteTreeAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving complete question bank tree structure");

            var rootCategories = await _categoryRepository.GetRootCategoriesAsync();
            var allCategories = await _categoryRepository.GetAllActiveAsync();
            
            var tree = new QuestionBankTreeDto
            {
                RootNodes = new List<QuestionBankCategoryDto>()
            };

            foreach (var rootCategory in rootCategories)
            {
                var categoryDto = _mapper.Map<QuestionBankCategoryDto>(rootCategory);
                await LoadCategoryChildrenRecursively(categoryDto, allCategories.ToList());
                tree.RootNodes.Add(categoryDto);
            }

            tree.TotalCategories = allCategories.Count();
            tree.MaxDepth = await _treeRepository.GetMaxDepthAsync();
            tree.LastUpdated = DateTime.UtcNow;

            _logger.LogInformation("Complete tree structure retrieved with {Count} categories", tree.TotalCategories);
            return tree;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving complete question bank tree");
            throw;
        }
    }

    public async Task<QuestionBankCategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
    {
        try
        {
            _logger.LogInformation("Creating new category: {Name} under parent: {ParentId}", dto.Name, dto.ParentId);

            // Validate parent category if specified
            QuestionBankCategory? parent = null;
            if (dto.ParentId.HasValue)
            {
                parent = await _categoryRepository.GetByIdAsync(dto.ParentId.Value);
                if (parent == null)
                {
                    throw new ArgumentException($"Parent category with ID {dto.ParentId.Value} not found");
                }
            }

            // Validate code uniqueness
            var existingWithCode = await _categoryRepository.GetByCodeAsync(dto.Code);
            if (existingWithCode != null)
            {
                throw new InvalidOperationException($"Category with code '{dto.Code}' already exists");
            }

            // Calculate tree path
            var treePath = await CalculateTreePathAsync(dto.ParentId);
            
            // Determine level
            var level = parent?.Level + 1 ?? CategoryLevel.Level1;
            if ((int)level > 6)
            {
                throw new InvalidOperationException("Maximum tree depth exceeded (6 levels)");
            }

            // Get sort order
            var sortOrder = await _categoryRepository.GetNextSortOrderAsync(dto.ParentId);

            var category = new QuestionBankCategory
            {
                Name = dto.Name,
                Code = dto.Code,
                Description = dto.Description,
                Type = dto.Type,
                Level = level,
                ParentId = dto.ParentId,
                SortOrder = sortOrder,
                TreePath = treePath,
                IsActive = dto.IsActive,
                AllowQuestions = dto.AllowQuestions,
                MetadataJson = dto.MetadataJson,
                CurriculumCode = dto.CurriculumCode,
                GradeLevel = dto.GradeLevel,
                Subject = dto.Subject
            };

            var savedCategory = await _categoryRepository.AddAsync(category);
            
            // Update hierarchy table
            await _treeRepository.UpdateHierarchyAsync(savedCategory.Id);

            _logger.LogInformation("Category created successfully with ID: {CategoryId}", savedCategory.Id);
            return _mapper.Map<QuestionBankCategoryDto>(savedCategory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category: {Name}", dto.Name);
            throw;
        }
    }

    public async Task<bool> AssignQuestionToCategoryAsync(int questionId, int categoryId, bool isPrimary = false)
    {
        try
        {
            _logger.LogInformation("Assigning question {QuestionId} to category {CategoryId}, Primary: {IsPrimary}", 
                questionId, categoryId, isPrimary);

            // Validate question exists
            var question = await _questionRepository.GetByIdAsync(questionId);
            if (question == null)
            {
                throw new ArgumentException($"Question with ID {questionId} not found");
            }

            // Validate category exists and allows questions
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                throw new ArgumentException($"Category with ID {categoryId} not found");
            }

            if (!category.AllowQuestions)
            {
                throw new InvalidOperationException("Selected category does not allow direct question assignment");
            }

            // Check if assignment already exists
            var existingAssignment = await _questionCategoryRepository.GetByQuestionAndCategoryAsync(questionId, categoryId);
            if (existingAssignment != null)
            {
                _logger.LogWarning("Question {QuestionId} is already assigned to category {CategoryId}", questionId, categoryId);
                return false;
            }

            // If this is primary, remove primary flag from other assignments
            if (isPrimary)
            {
                await _questionCategoryRepository.RemovePrimaryFlagAsync(questionId);
            }

            var categorization = new QuestionCategorization
            {
                QuestionId = questionId,
                CategoryId = categoryId,
                IsPrimary = isPrimary,
                AssignedAt = DateTime.UtcNow,
                AssignedBy = 1 // TODO: Get current user ID
            };

            await _questionCategoryRepository.AddAsync(categorization);

            _logger.LogInformation("Question {QuestionId} successfully assigned to category {CategoryId}", questionId, categoryId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning question {QuestionId} to category {CategoryId}", questionId, categoryId);
            throw;
        }
    }

    public async Task<bool> MoveCategoryAsync(int categoryId, MoveCategoryDto dto)
    {
        try
        {
            _logger.LogInformation("Moving category {CategoryId} to new parent {NewParentId}", categoryId, dto.NewParentId);

            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                throw new ArgumentException($"Category with ID {categoryId} not found");
            }

            // Validate new parent (if specified)
            QuestionBankCategory? newParent = null;
            if (dto.NewParentId.HasValue)
            {
                newParent = await _categoryRepository.GetByIdAsync(dto.NewParentId.Value);
                if (newParent == null)
                {
                    throw new ArgumentException($"New parent category with ID {dto.NewParentId.Value} not found");
                }

                // Check for circular reference
                if (await _treeRepository.IsDescendantAsync(dto.NewParentId.Value, categoryId))
                {
                    throw new InvalidOperationException("Cannot move category to its own descendant");
                }

                // Check depth limit
                var newDepth = (int)newParent.Level + 1;
                var subtreeDepth = await _treeRepository.GetSubtreeDepthAsync(categoryId);
                if (newDepth + subtreeDepth > 6)
                {
                    throw new InvalidOperationException("Move operation would exceed maximum tree depth");
                }
            }

            // Calculate new tree path
            var newTreePath = await CalculateTreePathAsync(dto.NewParentId);
            var oldTreePath = category.TreePath;

            // Update category
            category.ParentId = dto.NewParentId;
            category.TreePath = newTreePath;
            category.Level = newParent?.Level + 1 ?? CategoryLevel.Level1;
            category.SortOrder = await _categoryRepository.GetNextSortOrderAsync(dto.NewParentId);

            await _categoryRepository.UpdateAsync(category);

            // Update all descendant paths
            await _treeRepository.UpdateDescendantPathsAsync(categoryId, oldTreePath, newTreePath);

            // Rebuild hierarchy table for affected subtree
            await _treeRepository.RebuildHierarchyForSubtreeAsync(categoryId);

            _logger.LogInformation("Category {CategoryId} moved successfully to parent {NewParentId}", categoryId, dto.NewParentId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving category {CategoryId} to parent {NewParentId}", categoryId, dto.NewParentId);
            throw;
        }
    }

    private async Task LoadCategoryChildrenRecursively(QuestionBankCategoryDto categoryDto, List<QuestionBankCategory> allCategories)
    {
        var children = allCategories
            .Where(c => c.ParentId == categoryDto.Id)
            .OrderBy(c => c.SortOrder)
            .ToList();

        categoryDto.Children = new List<QuestionBankCategoryDto>();
        categoryDto.HasChildren = children.Any();
        categoryDto.QuestionCount = await _questionCategoryRepository.GetQuestionCountAsync(categoryDto.Id);

        foreach (var child in children)
        {
            var childDto = _mapper.Map<QuestionBankCategoryDto>(child);
            await LoadCategoryChildrenRecursively(childDto, allCategories);
            categoryDto.Children.Add(childDto);
        }
    }

    private async Task<string> CalculateTreePathAsync(int? parentId)
    {
        if (!parentId.HasValue)
            return "/";

        var parent = await _categoryRepository.GetByIdAsync(parentId.Value);
        return parent == null ? "/" : $"{parent.TreePath}{parentId}/";
    }
}
```

### Frontend Implementation Patterns

#### Question Bank Tree Component
```typescript
// frontend/src/modules/question-bank/components/QuestionBankTree.tsx
import React, { useState, useEffect, useCallback } from 'react';
import { TreeView, TreeItem } from '@mui/x-tree-view';
import { ExpandMore, ChevronRight, Folder, FolderOpen, Assignment } from '@mui/icons-material';
import { useQuestionBankTree } from '../hooks/useQuestionBankTree';
import { TreeNode } from './TreeNode';
import { TreeSearch } from './TreeSearch';
import { CategoryManager } from './CategoryManager';
import { BreadcrumbNavigation } from './BreadcrumbNavigation';
import { TreeActions } from './TreeActions';
import { QuestionBankCategoryDto, TreeOperationDto } from '../types/questionBankTree.types';

interface QuestionBankTreeProps {
  onCategorySelect?: (category: QuestionBankCategoryDto) => void;
  onQuestionSelect?: (questionId: number) => void;
  selectedCategoryId?: number;
  readonly?: boolean;
  showQuestions?: boolean;
  enableDragDrop?: boolean;
  maxDepth?: number;
  className?: string;
}

export const QuestionBankTree: React.FC<QuestionBankTreeProps> = ({
  onCategorySelect,
  onQuestionSelect,
  selectedCategoryId,
  readonly = false,
  showQuestions = true,
  enableDragDrop = false,
  maxDepth = 6,
  className
}) => {
  const [expandedNodes, setExpandedNodes] = useState<string[]>([]);
  const [selectedNodeId, setSelectedNodeId] = useState<string | null>(null);
  const [searchQuery, setSearchQuery] = useState('');
  const [showCategoryManager, setShowCategoryManager] = useState(false);

  const {
    treeData,
    isLoading,
    error,
    refreshTree,
    createCategory,
    updateCategory,
    deleteCategory,
    moveCategory,
    searchCategories,
    assignQuestionToCategory,
    removeQuestionFromCategory
  } = useQuestionBankTree();

  useEffect(() => {
    if (selectedCategoryId) {
      setSelectedNodeId(`category-${selectedCategoryId}`);
      // Expand path to selected category
      expandPathToCategory(selectedCategoryId);
    }
  }, [selectedCategoryId]);

  const expandPathToCategory = useCallback(async (categoryId: number) => {
    try {
      // Get ancestors and expand them
      const ancestors = await getAncestors(categoryId);
      const ancestorIds = ancestors.map(a => `category-${a.id}`);
      setExpandedNodes(prev => [...new Set([...prev, ...ancestorIds])]);
    } catch (error) {
      console.error('Error expanding path to category:', error);
    }
  }, []);

  const handleNodeToggle = useCallback((event: React.SyntheticEvent, nodeIds: string[]) => {
    setExpandedNodes(nodeIds);
  }, []);

  const handleNodeSelect = useCallback((event: React.SyntheticEvent, nodeIds: string | string[] | null) => {
    if (typeof nodeIds === 'string') {
      setSelectedNodeId(nodeIds);
      
      if (nodeIds.startsWith('category-')) {
        const categoryId = parseInt(nodeIds.replace('category-', ''));
        const category = findCategoryById(categoryId);
        if (category && onCategorySelect) {
          onCategorySelect(category);
        }
      } else if (nodeIds.startsWith('question-')) {
        const questionId = parseInt(nodeIds.replace('question-', ''));
        if (onQuestionSelect) {
          onQuestionSelect(questionId);
        }
      }
    }
  }, [onCategorySelect, onQuestionSelect]);

  const handleCategoryCreate = useCallback(async (parentId: number | null, categoryData: CreateCategoryDto) => {
    try {
      const newCategory = await createCategory(categoryData);
      await refreshTree();
      
      // Expand parent and select new category
      if (parentId) {
        setExpandedNodes(prev => [...prev, `category-${parentId}`]);
      }
      setSelectedNodeId(`category-${newCategory.id}`);
      
      if (onCategorySelect) {
        onCategorySelect(newCategory);
      }
    } catch (error) {
      console.error('Error creating category:', error);
    }
  }, [createCategory, refreshTree, onCategorySelect]);

  const handleCategoryMove = useCallback(async (categoryId: number, newParentId: number | null) => {
    try {
      await moveCategory(categoryId, { newParentId });
      await refreshTree();
    } catch (error) {
      console.error('Error moving category:', error);
    }
  }, [moveCategory, refreshTree]);

  const handleSearch = useCallback(async (query: string) => {
    setSearchQuery(query);
    if (query.trim()) {
      try {
        const searchResults = await searchCategories({ query, includeQuestions: showQuestions });
        // Expand nodes containing search results
        const resultIds = searchResults.flatMap(r => 
          r.categories.map(c => `category-${c.id}`)
        );
        setExpandedNodes(prev => [...new Set([...prev, ...resultIds])]);
      } catch (error) {
        console.error('Error searching categories:', error);
      }
    }
  }, [searchCategories, showQuestions]);

  const renderTreeNode = useCallback((category: QuestionBankCategoryDto): React.ReactNode => {
    return (
      <TreeNode
        key={`category-${category.id}`}
        category={category}
        isSelected={selectedNodeId === `category-${category.id}`}
        isExpanded={expandedNodes.includes(`category-${category.id}`)}
        readonly={readonly}
        showQuestions={showQuestions}
        enableDragDrop={enableDragDrop}
        onCategoryCreate={handleCategoryCreate}
        onCategoryUpdate={updateCategory}
        onCategoryDelete={deleteCategory}
        onCategoryMove={handleCategoryMove}
        onQuestionAssign={assignQuestionToCategory}
        onQuestionRemove={removeQuestionFromCategory}
        searchQuery={searchQuery}
      />
    );
  }, [
    selectedNodeId,
    expandedNodes,
    readonly,
    showQuestions,
    enableDragDrop,
    handleCategoryCreate,
    updateCategory,
    deleteCategory,
    handleCategoryMove,
    assignQuestionToCategory,
    removeQuestionFromCategory,
    searchQuery
  ]);

  const findCategoryById = useCallback((categoryId: number): QuestionBankCategoryDto | null => {
    const findInTree = (nodes: QuestionBankCategoryDto[]): QuestionBankCategoryDto | null => {
      for (const node of nodes) {
        if (node.id === categoryId) return node;
        if (node.children && node.children.length > 0) {
          const found = findInTree(node.children);
          if (found) return found;
        }
      }
      return null;
    };

    return treeData ? findInTree(treeData.rootNodes) : null;
  }, [treeData]);

  if (isLoading) {
    return (
      <div className="flex items-center justify-center p-8">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
        <span className="ml-2">Loading question bank tree...</span>
      </div>
    );
  }

  if (error) {
    return (
      <div className="p-4 border border-red-200 rounded-md bg-red-50">
        <p className="text-red-800">Error loading tree: {error}</p>
        <button 
          onClick={refreshTree}
          className="mt-2 px-4 py-2 bg-red-100 text-red-800 rounded hover:bg-red-200"
        >
          Retry
        </button>
      </div>
    );
  }

  return (
    <div className={`question-bank-tree ${className || ''}`}>
      {/* Header */}
      <div className="tree-header border-b pb-4 mb-4">
        <div className="flex items-center justify-between">
          <h2 className="text-lg font-semibold">Question Bank</h2>
          {!readonly && (
            <TreeActions
              onCategoryCreate={() => setShowCategoryManager(true)}
              onRefresh={refreshTree}
              selectedCategoryId={selectedCategoryId}
            />
          )}
        </div>
        
        <TreeSearch
          onSearch={handleSearch}
          placeholder="Search categories and topics..."
          className="mt-2"
        />
      </div>

      {/* Breadcrumb Navigation */}
      {selectedCategoryId && (
        <BreadcrumbNavigation
          categoryId={selectedCategoryId}
          onCategorySelect={onCategorySelect}
          className="mb-4"
        />
      )}

      {/* Tree View */}
      <div className="tree-content">
        {treeData && treeData.rootNodes.length > 0 ? (
          <TreeView
            defaultCollapseIcon={<ExpandMore />}
            defaultExpandIcon={<ChevronRight />}
            expanded={expandedNodes}
            selected={selectedNodeId || undefined}
            onNodeToggle={handleNodeToggle}
            onNodeSelect={handleNodeSelect}
            multiSelect={false}
            className="tree-view"
          >
            {treeData.rootNodes.map(renderTreeNode)}
          </TreeView>
        ) : (
          <div className="text-center py-8 text-gray-500">
            <Folder className="w-12 h-12 mx-auto mb-2 opacity-50" />
            <p>No categories found</p>
            {!readonly && (
              <button
                onClick={() => setShowCategoryManager(true)}
                className="mt-2 px-4 py-2 bg-primary text-white rounded hover:bg-primary-dark"
              >
                Create First Category
              </button>
            )}
          </div>
        )}
      </div>

      {/* Category Manager Modal */}
      {showCategoryManager && (
        <CategoryManager
          parentCategoryId={selectedCategoryId}
          onClose={() => setShowCategoryManager(false)}
          onCategoryCreated={handleCategoryCreate}
        />
      )}

      {/* Tree Statistics */}
      {treeData && (
        <div className="tree-stats mt-4 p-3 bg-gray-50 rounded text-sm text-gray-600">
          <div className="flex justify-between">
            <span>Categories: {treeData.totalCategories}</span>
            <span>Max Depth: {treeData.maxDepth}</span>
          </div>
        </div>
      )}
    </div>
  );
};
```

#### Question Bank Tree Hook
```typescript
// frontend/src/modules/question-bank/hooks/useQuestionBankTree.tsx
import { useState, useEffect, useCallback } from 'react';
import { questionBankTreeService } from '../services/questionBankTreeService';
import { 
  QuestionBankTreeDto, 
  QuestionBankCategoryDto,
  CreateCategoryDto,
  UpdateCategoryDto,
  MoveCategoryDto,
  CategorySearchDto,
  CategoryFilterDto
} from '../types/questionBankTree.types';

interface UseQuestionBankTreeResult {
  treeData: QuestionBankTreeDto | null;
  categories: QuestionBankCategoryDto[];
  isLoading: boolean;
  error: string | null;
  
  // Tree operations
  refreshTree: () => Promise<void>;
  getCategory: (categoryId: number) => Promise<QuestionBankCategoryDto | null>;
  getChildren: (parentId: number) => Promise<QuestionBankCategoryDto[]>;
  getAncestors: (categoryId: number) => Promise<QuestionBankCategoryDto[]>;
  
  // Category CRUD
  createCategory: (dto: CreateCategoryDto) => Promise<QuestionBankCategoryDto>;
  updateCategory: (categoryId: number, dto: UpdateCategoryDto) => Promise<QuestionBankCategoryDto>;
  deleteCategory: (categoryId: number) => Promise<boolean>;
  moveCategory: (categoryId: number, dto: MoveCategoryDto) => Promise<boolean>;
  
  // Question operations
  assignQuestionToCategory: (questionId: number, categoryId: number, isPrimary?: boolean) => Promise<boolean>;
  removeQuestionFromCategory: (questionId: number, categoryId: number) => Promise<boolean>;
  getQuestionCategories: (questionId: number) => Promise<QuestionBankCategoryDto[]>;
  getCategoryQuestions: (categoryId: number) => Promise<any[]>;
  
  // Search and filter
  searchCategories: (searchDto: CategorySearchDto) => Promise<any[]>;
  filterCategories: (filter: CategoryFilterDto) => Promise<QuestionBankCategoryDto[]>;
  
  // Validation
  validateTreeStructure: () => Promise<boolean>;
  getValidationReport: () => Promise<any>;
}

export const useQuestionBankTree = (): UseQuestionBankTreeResult => {
  const [treeData, setTreeData] = useState<QuestionBankTreeDto | null>(null);
  const [categories, setCategories] = useState<QuestionBankCategoryDto[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const refreshTree = useCallback(async () => {
    try {
      setIsLoading(true);
      setError(null);
      
      const tree = await questionBankTreeService.getCompleteTree();
      setTreeData(tree);
      
      // Flatten tree to categories array
      const flatCategories = flattenTreeToCategories(tree.rootNodes);
      setCategories(flatCategories);
      
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to load tree';
      setError(errorMessage);
      console.error('Error refreshing tree:', err);
    } finally {
      setIsLoading(false);
    }
  }, []);

  const getCategory = useCallback(async (categoryId: number): Promise<QuestionBankCategoryDto | null> => {
    try {
      return await questionBankTreeService.getCategory(categoryId);
    } catch (err) {
      console.error('Error getting category:', err);
      return null;
    }
  }, []);

  const getChildren = useCallback(async (parentId: number): Promise<QuestionBankCategoryDto[]> => {
    try {
      return await questionBankTreeService.getChildren(parentId);
    } catch (err) {
      console.error('Error getting children:', err);
      return [];
    }
  }, []);

  const getAncestors = useCallback(async (categoryId: number): Promise<QuestionBankCategoryDto[]> => {
    try {
      return await questionBankTreeService.getAncestors(categoryId);
    } catch (err) {
      console.error('Error getting ancestors:', err);
      return [];
    }
  }, []);

  const createCategory = useCallback(async (dto: CreateCategoryDto): Promise<QuestionBankCategoryDto> => {
    try {
      const newCategory = await questionBankTreeService.createCategory(dto);
      await refreshTree(); // Refresh tree to show new category
      return newCategory;
    } catch (err) {
      console.error('Error creating category:', err);
      throw err;
    }
  }, [refreshTree]);

  const updateCategory = useCallback(async (categoryId: number, dto: UpdateCategoryDto): Promise<QuestionBankCategoryDto> => {
    try {
      const updatedCategory = await questionBankTreeService.updateCategory(categoryId, dto);
      await refreshTree(); // Refresh tree to show updates
      return updatedCategory;
    } catch (err) {
      console.error('Error updating category:', err);
      throw err;
    }
  }, [refreshTree]);

  const deleteCategory = useCallback(async (categoryId: number): Promise<boolean> => {
    try {
      const success = await questionBankTreeService.deleteCategory(categoryId);
      if (success) {
        await refreshTree(); // Refresh tree to remove deleted category
      }
      return success;
    } catch (err) {
      console.error('Error deleting category:', err);
      throw err;
    }
  }, [refreshTree]);

  const moveCategory = useCallback(async (categoryId: number, dto: MoveCategoryDto): Promise<boolean> => {
    try {
      const success = await questionBankTreeService.moveCategory(categoryId, dto);
      if (success) {
        await refreshTree(); // Refresh tree to show new structure
      }
      return success;
    } catch (err) {
      console.error('Error moving category:', err);
      throw err;
    }
  }, [refreshTree]);

  const assignQuestionToCategory = useCallback(async (
    questionId: number, 
    categoryId: number, 
    isPrimary: boolean = false
  ): Promise<boolean> => {
    try {
      return await questionBankTreeService.assignQuestionToCategory(questionId, categoryId, isPrimary);
    } catch (err) {
      console.error('Error assigning question to category:', err);
      throw err;
    }
  }, []);

  const removeQuestionFromCategory = useCallback(async (questionId: number, categoryId: number): Promise<boolean> => {
    try {
      return await questionBankTreeService.removeQuestionFromCategory(questionId, categoryId);
    } catch (err) {
      console.error('Error removing question from category:', err);
      throw err;
    }
  }, []);

  const getQuestionCategories = useCallback(async (questionId: number): Promise<QuestionBankCategoryDto[]> => {
    try {
      return await questionBankTreeService.getQuestionCategories(questionId);
    } catch (err) {
      console.error('Error getting question categories:', err);
      return [];
    }
  }, []);

  const getCategoryQuestions = useCallback(async (categoryId: number): Promise<any[]> => {
    try {
      const result = await questionBankTreeService.getCategoryQuestions(categoryId, { page: 1, pageSize: 100 });
      return result.data;
    } catch (err) {
      console.error('Error getting category questions:', err);
      return [];
    }
  }, []);

  const searchCategories = useCallback(async (searchDto: CategorySearchDto): Promise<any[]> => {
    try {
      return await questionBankTreeService.searchCategories(searchDto);
    } catch (err) {
      console.error('Error searching categories:', err);
      return [];
    }
  }, []);

  const filterCategories = useCallback(async (filter: CategoryFilterDto): Promise<QuestionBankCategoryDto[]> => {
    try {
      const result = await questionBankTreeService.getCategories(filter);
      return result.data;
    } catch (err) {
      console.error('Error filtering categories:', err);
      return [];
    }
  }, []);

  const validateTreeStructure = useCallback(async (): Promise<boolean> => {
    try {
      return await questionBankTreeService.validateTreeStructure();
    } catch (err) {
      console.error('Error validating tree structure:', err);
      return false;
    }
  }, []);

  const getValidationReport = useCallback(async (): Promise<any> => {
    try {
      return await questionBankTreeService.getValidationReport();
    } catch (err) {
      console.error('Error getting validation report:', err);
      return null;
    }
  }, []);

  // Utility function to flatten tree structure
  const flattenTreeToCategories = useCallback((nodes: QuestionBankCategoryDto[]): QuestionBankCategoryDto[] => {
    const result: QuestionBankCategoryDto[] = [];
    
    const traverse = (categories: QuestionBankCategoryDto[]) => {
      for (const category of categories) {
        result.push(category);
        if (category.children && category.children.length > 0) {
          traverse(category.children);
        }
      }
    };
    
    traverse(nodes);
    return result;
  }, []);

  // Initial load
  useEffect(() => {
    refreshTree();
  }, [refreshTree]);

  return {
    treeData,
    categories,
    isLoading,
    error,
    
    refreshTree,
    getCategory,
    getChildren,
    getAncestors,
    
    createCategory,
    updateCategory,
    deleteCategory,
    moveCategory,
    
    assignQuestionToCategory,
    removeQuestionFromCategory,
    getQuestionCategories,
    getCategoryQuestions,
    
    searchCategories,
    filterCategories,
    
    validateTreeStructure,
    getValidationReport
  };
};
```

## 🔄 Integration Validation Loops

### Backend Validation Loop
```bash
# 1. Entity Validation
dotnet ef migrations add AddQuestionBankTreeEntities
dotnet ef database update

# 2. Repository Testing
dotnet test --filter Category=QuestionBankTreeRepository

# 3. Service Layer Testing
dotnet test --filter Category=QuestionBankTreeService

# 4. Controller Testing
dotnet test --filter Category=QuestionBankTreeController

# 5. Integration Testing
dotnet test --filter Category=QuestionBankTreeIntegration

# 6. Tree Structure Validation
dotnet test --filter Category=TreeStructureValidation
```

### Frontend Validation Loop
```bash
# 1. Component Testing
npm run test -- --testPathPattern=question-bank/tree

# 2. Hook Testing
npm run test -- --testPathPattern=useQuestionBankTree

# 3. Service Testing
npm run test -- --testPathPattern=questionBankTreeService

# 4. Type Checking
npx tsc --noEmit --project tsconfig.json

# 5. Integration Testing
npm run test:integration -- --testPathPattern=question-bank

# 6. E2E Tree Testing
npm run test:e2e -- --testPathPattern=tree-management
```

### Tree Structure Validation Loop
```bash
# 1. Hierarchy Validation
curl -X GET "https://localhost:7001/api/question-bank-tree/validate/structure"
# Expected: 200 OK with validation report

# 2. Path Integrity Check
curl -X GET "https://localhost:7001/api/question-bank-tree/validate/paths"
# Expected: 200 OK with path validation results

# 3. Category Operations
curl -X POST "https://localhost:7001/api/question-bank-tree/categories" \
  -H "Content-Type: application/json" \
  -d '{"name":"Mathematics","code":"MATH001","type":"Subject"}'
# Expected: 201 Created with category data
```

## 🎯 Success Criteria

### Functional Requirements ✅
- [ ] Hierarchical tree structure with up to 6 levels depth
- [ ] Category CRUD operations with parent-child relationships
- [ ] Question assignment to multiple categories
- [ ] Tree navigation with breadcrumbs and search
- [ ] Drag & drop category reorganization
- [ ] Bulk operations for categories and questions
- [ ] Tree structure validation and integrity checks
- [ ] Path enumeration for efficient traversal
- [ ] Curriculum alignment support
- [ ] Multi-language category names and descriptions

### Performance Requirements ✅
- [ ] Tree loading < 2 seconds for 1000+ categories
- [ ] Category operations < 500ms response time
- [ ] Search functionality < 1 second response time
- [ ] Efficient tree traversal with materialized paths
- [ ] Optimized database queries with proper indexing
- [ ] Memory-efficient tree rendering in frontend
- [ ] Lazy loading for large tree branches

### Security Requirements ✅
- [ ] Role-based access control for tree operations
- [ ] Category modification permissions
- [ ] Audit logging for tree structure changes
- [ ] Input validation and sanitization
- [ ] SQL injection prevention
- [ ] XSS protection in tree rendering

### Integration Requirements ✅
- [ ] Seamless integration with existing TreeNode system
- [ ] Question Bank module compatibility
- [ ] User management system integration
- [ ] Audit logging system integration
- [ ] Frontend tree component reusability
- [ ] Mobile-responsive tree interface

## ⚠ Anti-Patterns to Avoid

### Backend Anti-Patterns ❌
```csharp
// ❌ DON'T: Mixed responsibilities in tree service
public class QuestionBankTreeService
{
    public async Task<CategoryDto> CreateCategoryAndSendEmail(CreateCategoryDto dto) { } // Email responsibility
    public async Task<bool> DeleteCategoryAndUpdateExams(int categoryId) { } // Exam responsibility
}

// ❌ DON'T: Direct database queries in controllers
public class QuestionBankTreeController
{
    public async Task<IActionResult> GetTree()
    {
        var categories = await _context.Categories.ToListAsync(); // Direct DB access
    }
}

// ❌ DON'T: Inefficient tree traversal
public async Task<IEnumerable<Category>> GetAllDescendants(int categoryId)
{
    var result = new List<Category>();
    var children = await GetChildren(categoryId); // N+1 queries
    foreach (var child in children)
    {
        result.AddRange(await GetAllDescendants(child.Id)); // Recursive N+1
    }
    return result;
}

// ❌ DON'T: Circular reference validation missing
public async Task<bool> MoveCategory(int categoryId, int newParentId)
{
    // Missing circular reference check
    category.ParentId = newParentId; // Could create infinite loops
}
```

### Frontend Anti-Patterns ❌
```typescript
// ❌ DON'T: Uncontrolled tree expansion
const QuestionBankTree = () => {
  const [expandedNodes, setExpandedNodes] = useState([]);
  
  useEffect(() => {
    // Expanding entire tree on load
    const allNodeIds = getAllNodeIds(); // Performance killer
    setExpandedNodes(allNodeIds);
  }, []);
};

// ❌ DON'T: Direct state mutation
const moveCategory = (categoryId, newParentId) => {
  const category = treeData.find(c => c.id === categoryId);
  category.parentId = newParentId; // Direct mutation
  setTreeData([...treeData]); // Shallow copy won't help
};

// ❌ DON'T: Missing error boundaries
const TreeNode = ({ category }) => {
  return (
    <div>
      {category.children.map(child => ( // Could crash if children is undefined
        <TreeNode key={child.id} category={child} />
      ))}
    </div>
  );
};

// ❌ DON'T: Inefficient search implementation
const searchTree = (query) => {
  return treeData.filter(category => 
    category.name.includes(query) || // Case-sensitive search
    category.children.some(child => searchTree(query)) // Infinite recursion risk
  );
};
```

### Architecture Anti-Patterns ❌
```csharp
// ❌ DON'T: Tight coupling to tree structure
public class QuestionService
{
    public async Task<Question> CreateQuestion(CreateQuestionDto dto)
    {
        var question = new Question(dto);
        
        // Tightly coupled to specific tree structure
        var mathCategory = await _categoryRepository.GetByCode("MATH-LEVEL1-ALGEBRA");
        question.CategoryId = mathCategory.Id; // Hard-coded assumption
    }
}

// ❌ DON'T: Inconsistent path formats
public class TreePathService
{
    public string BuildPath(int[] categoryIds)
    {
        return string.Join("-", categoryIds); // Inconsistent with other formats
    }
    
    public string BuildPath2(int[] categoryIds)
    {
        return "/" + string.Join("/", categoryIds) + "/"; // Different format
    }
}

// ❌ DON'T: Missing transaction boundaries
public class CategoryMoveService
{
    public async Task<bool> MoveCategory(int categoryId, int newParentId)
    {
        await UpdateCategory(categoryId, newParentId); // Could fail
        await UpdateAllDescendantPaths(categoryId); // Leaving inconsistent state
        await UpdateHierarchyTable(categoryId); // No rollback mechanism
    }
}
```

## 📚 Implementation Guide

### Phase 1: Core Backend Tree System (Week 1)
1. **Entity Implementation**
   - Create QuestionBankCategory, QuestionCategorization, QuestionBankHierarchy entities
   - Configure Dapper relationships and indexes

2. **Repository Layer**
   - Implement category and hierarchy repositories
   - Add efficient tree traversal methods

3. **Service Foundation**
   - Create core tree service with CRUD operations
   - Add path calculation and hierarchy management

### Phase 2: Advanced Tree Operations (Week 2)
1. **Tree Manipulation**
   - Implement move, copy, and bulk operations
   - Add tree validation and integrity checks

2. **Search and Navigation**
   - Add tree search functionality
   - Implement breadcrumb navigation support

3. **Question Integration**
   - Connect tree system with question entities
   - Add question categorization functionality

### Phase 3: Frontend Tree Interface (Week 3)
1. **Core Components**
   - Build main tree component with TreeView
   - Create tree node and navigation components

2. **Interactive Features**
   - Add drag & drop functionality
   - Implement tree search and filtering

3. **Management Interface**
   - Create category management forms
   - Add bulk operation interfaces

### Phase 4: Integration & Testing (Week 4)
1. **System Integration**
   - Integrate with existing TreeNode system
   - Connect with question bank module

2. **Performance Optimization**
   - Optimize tree loading and rendering
   - Add caching for frequently accessed paths

3. **Testing & Validation**
   - Comprehensive unit and integration tests
   - Tree structure validation tools

## 🛡️ Security Checklist
- [ ] Role-based access control implemented
- [ ] Category modification permissions enforced
- [ ] Input validation and sanitization
- [ ] SQL injection prevention with parameterized queries
- [ ] XSS protection in tree rendering
- [ ] Audit logging for tree operations
- [ ] Rate limiting for tree operations

## 📊 Performance Checklist
- [ ] Materialized path pattern for efficient traversal
- [ ] Database indexes on path and parent columns
- [ ] Lazy loading for tree branches
- [ ] Frontend virtualization for large trees
- [ ] Efficient search with proper indexing
- [ ] Memory optimization in tree rendering
- [ ] Caching for frequently accessed tree data

## 🌐 Integration Points

```yaml
DATABASE:
  - tables: "QuestionBankCategories, QuestionCategorizations, QuestionBankHierarchies"
  - indexes: "IX_Category_Path, IX_Category_Parent for traversal"
  - migration: "Migrate existing TreeNode data to new structure"

CONFIG:
  - backend: "Register tree repositories and services in DI"
  - frontend: "Configure tree component in question bank module"

ROUTES:
  - api: "/api/question-bank-tree, /api/question-categories"
  - frontend: "/question-bank/categories, /question-bank/tree"

NAVIGATION:
  - dashboard: "Add tree management menu items"
  - permissions: "Category management role-based access"

INTEGRATION:
  - existing: "Map TreeNode entities to new category structure"
  - questions: "Connect Question entities with categorization"
  - search: "Integrate tree search with global search"
```

This comprehensive PRP provides a complete implementation guide for the question bank tree management system that extends and enhances the existing tree structure while maintaining architectural consistency and following established patterns.
