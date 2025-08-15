---
feature: "backend-hierarchy"
module: "tree-structure"
order: "01"
description: |
  Implement a hierarchical tree structure for organizing content in the Ikhtibar system,
  including entities, repositories, services, and controllers with support for CRUD operations,
  nested node relationships, path enumeration, and tree traversal.
---

## Goal
Design and implement a robust hierarchical tree structure backend system for Ikhtibar that supports organizing content (questions, categories, topics) in a nested tree with efficient querying capabilities.

## Why
- **Business value:** Enables organized content categorization and navigation across the platform.
- **Integration:** Provides foundation for question bank organization and exam creation.
- **Problem solved:** Creates reusable hierarchical structure needed by multiple system features.

## What
- TreeNode entity with path enumeration pattern for efficient tree operations.
- CRUD operations for tree nodes with proper parent-child relationship management.
- Services for node creation, movement, reordering, and subtree operations.
- APIs for retrieving tree views, subtrees, ancestors, and descendants.
- Support for different node types (categories, subjects, topics).
- Maintain materialized path for efficient traversal.

### Success Criteria
- [ ] Create, update, delete operations maintain proper tree structure.
- [ ] Path field maintains correct enumeration (e.g., `-1-4-9-`).
- [ ] Moving nodes correctly updates paths for all descendants.
- [ ] Tree traversal operations are efficient (O(1) for ancestry checks).
- [ ] Support pagination and filtering when retrieving nodes.
- [ ] All code passes type-checking, tests, and follows SRP.

## All Needed Context

```yaml
- file: .cursor/copilot/requirements/schema.sql
  why: Contains TreeNodes and TreeNodeTypes table definitions
- file: .cursor/copilot/examples/backend/repositories/BaseRepository.cs
  why: Repository pattern to follow
- file: .cursor/copilot/examples/backend/controllers/UsersController.cs
  why: API controller pattern
- file: Ikhtibar.Core/Entities/BaseEntity.cs
  why: Base entity implementation
- file: .cursor/copilot/examples/backend/services/UserService.cs
  why: Service layer pattern
```

## Implementation Blueprint

### 1. Core Entities

Create `Ikhtibar.Core/Entities/TreeNodeType.cs`:
```csharp
using System.ComponentModel.DataAnnotations;

namespace Ikhtibar.Shared.Entities
{
    public class TreeNodeType : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        // Navigation properties
        public virtual ICollection<TreeNode> TreeNodes { get; set; }
    }
}
```

Create `Ikhtibar.Core/Entities/TreeNode.cs`:
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ikhtibar.Shared.Entities
{
    public class TreeNode : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }
        
        [MaxLength(500)]
        public string Description { get; set; }
        
        [Required]
        public Guid TreeNodeTypeId { get; set; }
        
        public Guid? ParentId { get; set; }
        
        public int OrderIndex { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Path { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        [ForeignKey("TreeNodeTypeId")]
        public virtual TreeNodeType TreeNodeType { get; set; }
        
        [ForeignKey("ParentId")]
        public virtual TreeNode Parent { get; set; }
        
        public virtual ICollection<TreeNode> Children { get; set; }
        
        // For curriculum alignments
        public virtual ICollection<CurriculumAlignment> CurriculumAlignments { get; set; }
        
        // For questions with this as primary node
        public virtual ICollection<Question> Questions { get; set; }
    }
}
```

Create `Ikhtibar.Core/Entities/CurriculumAlignment.cs` (referenced by TreeNode):
```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ikhtibar.Shared.Entities
{
    public class CurriculumAlignment : BaseEntity
    {
        [Required]
        public Guid TreeNodeId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string StandardCode { get; set; }
        
        [MaxLength(20)]
        public string CurriculumVersion { get; set; }
        
        [MaxLength(50)]
        public string EducationLevel { get; set; }
        
        public int? GradeLevel { get; set; }
        
        [MaxLength(100)]
        public string SubjectArea { get; set; }
        
        [MaxLength(200)]
        public string Strand { get; set; }
        
        [MaxLength(500)]
        public string StandardUrl { get; set; }
        
        // Navigation properties
        [ForeignKey("TreeNodeId")]
        public virtual TreeNode TreeNode { get; set; }
    }
}
```

### 2. DTOs

Create `Ikhtibar.API/DTOs/TreeNodeTypeDto.cs`:
```csharp
namespace Ikhtibar.API.DTOs
{
    public class TreeNodeTypeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
    
    public class CreateTreeNodeTypeDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    }
    
    public class UpdateTreeNodeTypeDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
```

Create `Ikhtibar.API/DTOs/TreeNodeDto.cs`:
```csharp
namespace Ikhtibar.API.DTOs
{
    public class TreeNodeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public Guid TreeNodeTypeId { get; set; }
        public string TreeNodeTypeName { get; set; }
        public Guid? ParentId { get; set; }
        public int OrderIndex { get; set; }
        public string Path { get; set; }
        public bool IsActive { get; set; }
        public int Level { get; set; } // Derived from path
        public bool HasChildren { get; set; } // Flag indicating if this node has children
        public DateTime CreatedAt { get; set; }
    }
    
    public class TreeNodeDetailDto : TreeNodeDto
    {
        public IEnumerable<TreeNodeDto> Children { get; set; }
        public IEnumerable<CurriculumAlignmentDto> CurriculumAlignments { get; set; }
    }
    
    public class CreateTreeNodeDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }
        
        [MaxLength(500)]
        public string Description { get; set; }
        
        [Required]
        public Guid TreeNodeTypeId { get; set; }
        
        public Guid? ParentId { get; set; }
        
        public int OrderIndex { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
    
    public class UpdateTreeNodeDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        
        [MaxLength(50)]
        public string Code { get; set; }
        
        [MaxLength(500)]
        public string Description { get; set; }
        
        public Guid TreeNodeTypeId { get; set; }
        
        public int OrderIndex { get; set; }
        
        public bool IsActive { get; set; }
    }
    
    public class MoveTreeNodeDto
    {
        [Required]
        public Guid NewParentId { get; set; }
    }
}
```

### 3. Repositories

Create `Ikhtibar.Infrastructure/Repositories/ITreeNodeTypeRepository.cs`:
```csharp
using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Infrastructure.Repositories
{
    public interface ITreeNodeTypeRepository : IBaseRepository<TreeNodeType>
    {
        Task<TreeNodeType> GetByNameAsync(string name);
    }
}
```

Create `Ikhtibar.Infrastructure/Repositories/TreeNodeTypeRepository.cs`:
```csharp
using Ikhtibar.Shared.Entities;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.Infrastructure.Repositories
{
    public class TreeNodeTypeRepository : BaseRepository<TreeNodeType>, ITreeNodeTypeRepository
    {
        public TreeNodeTypeRepository(IDbContext dbContext, ILogger<TreeNodeTypeRepository> logger) 
            : base(dbContext, logger)
        {
        }

        public async Task<TreeNodeType> GetByNameAsync(string name)
        {
            var query = @"SELECT * FROM TreeNodeTypes WHERE Name = @Name AND IsDeleted = 0";
            
            return await _dbContext.QueryFirstOrDefaultAsync<TreeNodeType>(
                query,
                new { Name = name }
            );
        }
    }
}
```

Create `Ikhtibar.Infrastructure/Repositories/ITreeNodeRepository.cs`:
```csharp
using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Infrastructure.Repositories
{
    public interface ITreeNodeRepository : IBaseRepository<TreeNode>
    {
        Task<IEnumerable<TreeNode>> GetChildrenAsync(Guid parentId);
        Task<TreeNode> GetByCodeAsync(string code);
        Task<IEnumerable<TreeNode>> GetByPathAsync(string pathQuery);
        Task<IEnumerable<TreeNode>> GetAncestorsAsync(Guid nodeId);
        Task<IEnumerable<TreeNode>> GetDescendantsAsync(Guid nodeId);
        Task<bool> UpdatePathsAsync(string oldPath, string newPath);
        Task<int> GetMaxOrderIndexAsync(Guid? parentId);
        Task<bool> ReorderNodesAsync(Guid parentId, IDictionary<Guid, int> nodeOrders);
    }
}
```

Create `Ikhtibar.Infrastructure/Repositories/TreeNodeRepository.cs`:
```csharp
using Ikhtibar.Shared.Entities;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.Infrastructure.Repositories
{
    public class TreeNodeRepository : BaseRepository<TreeNode>, ITreeNodeRepository
    {
        public TreeNodeRepository(IDbContext dbContext, ILogger<TreeNodeRepository> logger) 
            : base(dbContext, logger)
        {
        }

        public async Task<IEnumerable<TreeNode>> GetChildrenAsync(Guid parentId)
        {
            var query = @"
                SELECT t.*, tt.Name as TreeNodeTypeName 
                FROM TreeNodes t
                INNER JOIN TreeNodeTypes tt ON t.TreeNodeTypeId = tt.Id
                WHERE t.ParentId = @ParentId 
                AND t.IsDeleted = 0
                ORDER BY t.OrderIndex";
            
            return await _dbContext.QueryAsync<TreeNode>(
                query,
                new { ParentId = parentId }
            );
        }

        public async Task<TreeNode> GetByCodeAsync(string code)
        {
            var query = @"
                SELECT t.*, tt.Name as TreeNodeTypeName 
                FROM TreeNodes t
                INNER JOIN TreeNodeTypes tt ON t.TreeNodeTypeId = tt.Id
                WHERE t.Code = @Code 
                AND t.IsDeleted = 0";
            
            return await _dbContext.QueryFirstOrDefaultAsync<TreeNode>(
                query,
                new { Code = code }
            );
        }

        public async Task<IEnumerable<TreeNode>> GetByPathAsync(string pathQuery)
        {
            var query = @"
                SELECT t.*, tt.Name as TreeNodeTypeName 
                FROM TreeNodes t
                INNER JOIN TreeNodeTypes tt ON t.TreeNodeTypeId = tt.Id
                WHERE t.Path LIKE @PathQuery 
                AND t.IsDeleted = 0
                ORDER BY t.Path, t.OrderIndex";
            
            return await _dbContext.QueryAsync<TreeNode>(
                query,
                new { PathQuery = $"{pathQuery}%" }
            );
        }

        public async Task<IEnumerable<TreeNode>> GetAncestorsAsync(Guid nodeId)
        {
            // First get the node to extract its path
            var node = await GetByIdAsync(nodeId);
            if (node == null) return new List<TreeNode>();
            
            // Then use the path to extract all ancestors
            var path = node.Path;
            var nodeIds = path.Trim('-').Split('-')
                .Where(id => !string.IsNullOrEmpty(id))
                .Select(id => Guid.Parse(id));
            
            if (!nodeIds.Any()) return new List<TreeNode>();
            
            var query = @"
                SELECT t.*, tt.Name as TreeNodeTypeName 
                FROM TreeNodes t
                INNER JOIN TreeNodeTypes tt ON t.TreeNodeTypeId = tt.Id
                WHERE t.Id IN @NodeIds 
                AND t.IsDeleted = 0
                ORDER BY t.Path";
            
            return await _dbContext.QueryAsync<TreeNode>(
                query,
                new { NodeIds = nodeIds }
            );
        }

        public async Task<IEnumerable<TreeNode>> GetDescendantsAsync(Guid nodeId)
        {
            // First get the node to extract its path
            var node = await GetByIdAsync(nodeId);
            if (node == null) return new List<TreeNode>();
            
            return await GetByPathAsync(node.Path);
        }

        public async Task<bool> UpdatePathsAsync(string oldPath, string newPath)
        {
            var query = @"
                UPDATE TreeNodes
                SET Path = REPLACE(Path, @OldPath, @NewPath)
                WHERE Path LIKE @PathQuery";
            
            var result = await _dbContext.ExecuteAsync(
                query,
                new { OldPath = oldPath, NewPath = newPath, PathQuery = $"{oldPath}%" }
            );
            
            return result > 0;
        }

        public async Task<int> GetMaxOrderIndexAsync(Guid? parentId)
        {
            var query = @"
                SELECT ISNULL(MAX(OrderIndex), 0)
                FROM TreeNodes
                WHERE ParentId = @ParentId
                AND IsDeleted = 0";
            
            return await _dbContext.ExecuteScalarAsync<int>(
                query,
                new { ParentId = parentId }
            );
        }

        public async Task<bool> ReorderNodesAsync(Guid parentId, IDictionary<Guid, int> nodeOrders)
        {
            using (var transaction = _dbContext.BeginTransaction())
            {
                try
                {
                    foreach (var order in nodeOrders)
                    {
                        var updateQuery = @"
                            UPDATE TreeNodes
                            SET OrderIndex = @OrderIndex
                            WHERE Id = @Id AND ParentId = @ParentId";
                        
                        await _dbContext.ExecuteAsync(
                            updateQuery,
                            new { Id = order.Key, OrderIndex = order.Value, ParentId = parentId }
                        );
                    }
                    
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error reordering nodes for parent {ParentId}", parentId);
                    transaction.Rollback();
                    return false;
                }
            }
        }
    }
}
```

### 4. Services

Create `Ikhtibar.API/Services/ITreeNodeTypeService.cs`:
```csharp
using Ikhtibar.API.DTOs;

namespace Ikhtibar.API.Services
{
    public interface ITreeNodeTypeService
    {
        Task<IEnumerable<TreeNodeTypeDto>> GetAllAsync();
        Task<TreeNodeTypeDto> GetByIdAsync(Guid id);
        Task<TreeNodeTypeDto> CreateAsync(CreateTreeNodeTypeDto dto);
        Task<TreeNodeTypeDto> UpdateAsync(Guid id, UpdateTreeNodeTypeDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
```

Create `Ikhtibar.API/Services/TreeNodeTypeService.cs`:
```csharp
using AutoMapper;
using Ikhtibar.API.DTOs;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.API.Services
{
    public class TreeNodeTypeService : ITreeNodeTypeService
    {
        private readonly ITreeNodeTypeRepository _treeNodeTypeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<TreeNodeTypeService> _logger;

        public TreeNodeTypeService(
            ITreeNodeTypeRepository treeNodeTypeRepository,
            IMapper mapper,
            ILogger<TreeNodeTypeService> logger)
        {
            _treeNodeTypeRepository = treeNodeTypeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<TreeNodeTypeDto>> GetAllAsync()
        {
            try
            {
                var types = await _treeNodeTypeRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<TreeNodeTypeDto>>(types);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tree node types");
                throw;
            }
        }

        public async Task<TreeNodeTypeDto> GetByIdAsync(Guid id)
        {
            try
            {
                var type = await _treeNodeTypeRepository.GetByIdAsync(id);
                return _mapper.Map<TreeNodeTypeDto>(type);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tree node type {Id}", id);
                throw;
            }
        }

        public async Task<TreeNodeTypeDto> CreateAsync(CreateTreeNodeTypeDto dto)
        {
            try
            {
                // Check if name is unique
                var existing = await _treeNodeTypeRepository.GetByNameAsync(dto.Name);
                if (existing != null)
                {
                    throw new InvalidOperationException($"Tree node type with name '{dto.Name}' already exists");
                }

                var entity = _mapper.Map<TreeNodeType>(dto);
                var created = await _treeNodeTypeRepository.CreateAsync(entity);
                return _mapper.Map<TreeNodeTypeDto>(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tree node type {Name}", dto.Name);
                throw;
            }
        }

        public async Task<TreeNodeTypeDto> UpdateAsync(Guid id, UpdateTreeNodeTypeDto dto)
        {
            try
            {
                var entity = await _treeNodeTypeRepository.GetByIdAsync(id);
                if (entity == null)
                {
                    throw new KeyNotFoundException($"Tree node type with ID {id} not found");
                }

                // Check if new name is unique (if changed)
                if (entity.Name != dto.Name)
                {
                    var existing = await _treeNodeTypeRepository.GetByNameAsync(dto.Name);
                    if (existing != null && existing.Id != id)
                    {
                        throw new InvalidOperationException($"Tree node type with name '{dto.Name}' already exists");
                    }
                }

                _mapper.Map(dto, entity);
                var updated = await _treeNodeTypeRepository.UpdateAsync(entity);
                return _mapper.Map<TreeNodeTypeDto>(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tree node type {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                // Check if type is in use by any nodes
                // (This would be better in a separate method, but keeping it simple)
                // TODO: Add check if type is in use

                return await _treeNodeTypeRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tree node type {Id}", id);
                throw;
            }
        }
    }
}
```

Create `Ikhtibar.API/Services/ITreeNodeService.cs`:
```csharp
using Ikhtibar.API.DTOs;

namespace Ikhtibar.API.Services
{
    public interface ITreeNodeService
    {
        Task<IEnumerable<TreeNodeDto>> GetAllAsync();
        Task<IEnumerable<TreeNodeDto>> GetRootNodesAsync();
        Task<IEnumerable<TreeNodeDto>> GetChildrenAsync(Guid parentId);
        Task<TreeNodeDetailDto> GetByIdAsync(Guid id);
        Task<TreeNodeDto> GetByCodeAsync(string code);
        Task<TreeNodeDetailDto> GetTreeAsync(Guid rootId, int levels = 1);
        Task<IEnumerable<TreeNodeDto>> GetAncestorsAsync(Guid nodeId);
        Task<IEnumerable<TreeNodeDto>> GetDescendantsAsync(Guid nodeId);
        Task<TreeNodeDto> CreateAsync(CreateTreeNodeDto dto);
        Task<TreeNodeDto> UpdateAsync(Guid id, UpdateTreeNodeDto dto);
        Task<TreeNodeDto> MoveAsync(Guid id, MoveTreeNodeDto dto);
        Task<bool> ReorderAsync(Guid parentId, IDictionary<Guid, int> nodeOrders);
        Task<bool> DeleteAsync(Guid id);
    }
}
```

Create `Ikhtibar.API/Services/TreeNodeService.cs`:
```csharp
using AutoMapper;
using Ikhtibar.API.DTOs;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace Ikhtibar.API.Services
{
    public class TreeNodeService : ITreeNodeService
    {
        private readonly ITreeNodeRepository _treeNodeRepository;
        private readonly ITreeNodeTypeRepository _treeNodeTypeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<TreeNodeService> _logger;

        public TreeNodeService(
            ITreeNodeRepository treeNodeRepository,
            ITreeNodeTypeRepository treeNodeTypeRepository,
            IMapper mapper,
            ILogger<TreeNodeService> logger)
        {
            _treeNodeRepository = treeNodeRepository;
            _treeNodeTypeRepository = treeNodeTypeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<TreeNodeDto>> GetAllAsync()
        {
            try
            {
                var nodes = await _treeNodeRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<TreeNodeDto>>(nodes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tree nodes");
                throw;
            }
        }

        public async Task<IEnumerable<TreeNodeDto>> GetRootNodesAsync()
        {
            try
            {
                var nodes = (await _treeNodeRepository.GetAllAsync())
                    .Where(n => n.ParentId == null)
                    .OrderBy(n => n.OrderIndex);
                
                return _mapper.Map<IEnumerable<TreeNodeDto>>(nodes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving root tree nodes");
                throw;
            }
        }

        public async Task<IEnumerable<TreeNodeDto>> GetChildrenAsync(Guid parentId)
        {
            try
            {
                var nodes = await _treeNodeRepository.GetChildrenAsync(parentId);
                return _mapper.Map<IEnumerable<TreeNodeDto>>(nodes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving children of tree node {ParentId}", parentId);
                throw;
            }
        }

        public async Task<TreeNodeDetailDto> GetByIdAsync(Guid id)
        {
            try
            {
                var node = await _treeNodeRepository.GetByIdAsync(id);
                if (node == null) return null;
                
                var dto = _mapper.Map<TreeNodeDetailDto>(node);
                
                // Load children
                var children = await _treeNodeRepository.GetChildrenAsync(id);
                dto.Children = _mapper.Map<IEnumerable<TreeNodeDto>>(children);
                
                // Load curriculum alignments if available
                // (This would typically be in a separate repository, but keeping it simple)
                // TODO: Add curriculum alignments
                
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tree node {Id}", id);
                throw;
            }
        }

        public async Task<TreeNodeDto> GetByCodeAsync(string code)
        {
            try
            {
                var node = await _treeNodeRepository.GetByCodeAsync(code);
                return _mapper.Map<TreeNodeDto>(node);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tree node with code {Code}", code);
                throw;
            }
        }

        public async Task<TreeNodeDetailDto> GetTreeAsync(Guid rootId, int levels = 1)
        {
            try
            {
                var root = await _treeNodeRepository.GetByIdAsync(rootId);
                if (root == null) return null;
                
                var result = _mapper.Map<TreeNodeDetailDto>(root);
                
                if (levels <= 0) return result;
                
                // Recursively load children up to specified depth
                await LoadChildrenRecursively(result, rootId, levels, 1);
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tree from node {RootId}", rootId);
                throw;
            }
        }

        private async Task LoadChildrenRecursively(TreeNodeDetailDto parentDto, Guid parentId, int maxLevels, int currentLevel)
        {
            var children = await _treeNodeRepository.GetChildrenAsync(parentId);
            var childrenDtos = _mapper.Map<IEnumerable<TreeNodeDetailDto>>(children);
            parentDto.Children = childrenDtos;
            
            if (currentLevel < maxLevels)
            {
                foreach (var childDto in childrenDtos)
                {
                    await LoadChildrenRecursively(childDto, Guid.Parse(childDto.Id.ToString()), maxLevels, currentLevel + 1);
                }
            }
        }

        public async Task<IEnumerable<TreeNodeDto>> GetAncestorsAsync(Guid nodeId)
        {
            try
            {
                var ancestors = await _treeNodeRepository.GetAncestorsAsync(nodeId);
                return _mapper.Map<IEnumerable<TreeNodeDto>>(ancestors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ancestors of tree node {NodeId}", nodeId);
                throw;
            }
        }

        public async Task<IEnumerable<TreeNodeDto>> GetDescendantsAsync(Guid nodeId)
        {
            try
            {
                var descendants = await _treeNodeRepository.GetDescendantsAsync(nodeId);
                return _mapper.Map<IEnumerable<TreeNodeDto>>(descendants);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving descendants of tree node {NodeId}", nodeId);
                throw;
            }
        }

        public async Task<TreeNodeDto> CreateAsync(CreateTreeNodeDto dto)
        {
            try
            {
                // Validate tree node type
                var nodeType = await _treeNodeTypeRepository.GetByIdAsync(dto.TreeNodeTypeId);
                if (nodeType == null)
                {
                    throw new KeyNotFoundException($"Tree node type with ID {dto.TreeNodeTypeId} not found");
                }
                
                // Validate parent node if provided
                string path = "-";
                if (dto.ParentId.HasValue)
                {
                    var parent = await _treeNodeRepository.GetByIdAsync(dto.ParentId.Value);
                    if (parent == null)
                    {
                        throw new KeyNotFoundException($"Parent node with ID {dto.ParentId.Value} not found");
                    }
                    path = $"{parent.Path}{dto.ParentId.Value}-";
                }
                
                // Validate code uniqueness
                var existingWithCode = await _treeNodeRepository.GetByCodeAsync(dto.Code);
                if (existingWithCode != null)
                {
                    throw new InvalidOperationException($"Tree node with code '{dto.Code}' already exists");
                }
                
                // Get next order index if not specified
                if (dto.OrderIndex <= 0)
                {
                    dto.OrderIndex = await _treeNodeRepository.GetMaxOrderIndexAsync(dto.ParentId) + 1;
                }
                
                var entity = _mapper.Map<TreeNode>(dto);
                entity.Path = path;
                
                var created = await _treeNodeRepository.CreateAsync(entity);
                return _mapper.Map<TreeNodeDto>(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tree node {Name}", dto.Name);
                throw;
            }
        }

        public async Task<TreeNodeDto> UpdateAsync(Guid id, UpdateTreeNodeDto dto)
        {
            try
            {
                var entity = await _treeNodeRepository.GetByIdAsync(id);
                if (entity == null)
                {
                    throw new KeyNotFoundException($"Tree node with ID {id} not found");
                }
                
                // Validate tree node type if changed
                if (entity.TreeNodeTypeId != dto.TreeNodeTypeId)
                {
                    var nodeType = await _treeNodeTypeRepository.GetByIdAsync(dto.TreeNodeTypeId);
                    if (nodeType == null)
                    {
                        throw new KeyNotFoundException($"Tree node type with ID {dto.TreeNodeTypeId} not found");
                    }
                }
                
                // Validate code uniqueness if changed
                if (entity.Code != dto.Code)
                {
                    var existingWithCode = await _treeNodeRepository.GetByCodeAsync(dto.Code);
                    if (existingWithCode != null && existingWithCode.Id != id)
                    {
                        throw new InvalidOperationException($"Tree node with code '{dto.Code}' already exists");
                    }
                }
                
                // Update entity properties
                _mapper.Map(dto, entity);
                
                var updated = await _treeNodeRepository.UpdateAsync(entity);
                return _mapper.Map<TreeNodeDto>(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tree node {Id}", id);
                throw;
            }
        }

        public async Task<TreeNodeDto> MoveAsync(Guid id, MoveTreeNodeDto dto)
        {
            try
            {
                var node = await _treeNodeRepository.GetByIdAsync(id);
                if (node == null)
                {
                    throw new KeyNotFoundException($"Tree node with ID {id} not found");
                }
                
                var newParent = await _treeNodeRepository.GetByIdAsync(dto.NewParentId);
                if (newParent == null)
                {
                    throw new KeyNotFoundException($"New parent node with ID {dto.NewParentId} not found");
                }
                
                // Ensure we're not creating a circular reference
                if (newParent.Path.Contains($"-{id}-"))
                {
                    throw new InvalidOperationException("Cannot move a node to one of its descendants");
                }
                
                // Get the old path for all descendants
                var oldPath = node.Path;
                
                // Update node's parent and path
                node.ParentId = dto.NewParentId;
                node.Path = $"{newParent.Path}{dto.NewParentId}-";
                
                // Get next order index in new parent
                node.OrderIndex = await _treeNodeRepository.GetMaxOrderIndexAsync(dto.NewParentId) + 1;
                
                // Update the node itself
                await _treeNodeRepository.UpdateAsync(node);
                
                // Update all descendants' paths
                await _treeNodeRepository.UpdatePathsAsync(oldPath, node.Path);
                
                return _mapper.Map<TreeNodeDto>(node);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error moving tree node {Id} to parent {NewParentId}", id, dto.NewParentId);
                throw;
            }
        }

        public async Task<bool> ReorderAsync(Guid parentId, IDictionary<Guid, int> nodeOrders)
        {
            try
            {
                // Validate parent node
                if (parentId != Guid.Empty)
                {
                    var parent = await _treeNodeRepository.GetByIdAsync(parentId);
                    if (parent == null)
                    {
                        throw new KeyNotFoundException($"Parent node with ID {parentId} not found");
                    }
                }
                
                // Validate that all nodes belong to the parent
                var children = await _treeNodeRepository.GetChildrenAsync(parentId);
                var childIds = children.Select(c => c.Id).ToHashSet();
                
                foreach (var nodeId in nodeOrders.Keys)
                {
                    if (!childIds.Contains(nodeId))
                    {
                        throw new InvalidOperationException($"Node {nodeId} is not a child of parent {parentId}");
                    }
                }
                
                // Update order indexes
                return await _treeNodeRepository.ReorderNodesAsync(parentId, nodeOrders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reordering children of tree node {ParentId}", parentId);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var node = await _treeNodeRepository.GetByIdAsync(id);
                if (node == null) return false;
                
                // Check if node has children
                var children = await _treeNodeRepository.GetChildrenAsync(id);
                if (children.Any())
                {
                    throw new InvalidOperationException("Cannot delete a node with children");
                }
                
                // Check if node is referenced by other entities
                // TODO: Add checks for references from questions, etc.
                
                return await _treeNodeRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tree node {Id}", id);
                throw;
            }
        }
    }
}
```

### 5. Controllers

Create `Ikhtibar.API/Controllers/TreeNodeTypesController.cs`:
```csharp
using Ikhtibar.API.DTOs;
using Ikhtibar.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ikhtibar.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/tree-node-types")]
    public class TreeNodeTypesController : ControllerBase
    {
        private readonly ITreeNodeTypeService _treeNodeTypeService;
        private readonly ILogger<TreeNodeTypesController> _logger;

        public TreeNodeTypesController(
            ITreeNodeTypeService treeNodeTypeService,
            ILogger<TreeNodeTypesController> logger)
        {
            _treeNodeTypeService = treeNodeTypeService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TreeNodeTypeDto>>> GetAll()
        {
            try
            {
                var types = await _treeNodeTypeService.GetAllAsync();
                return Ok(types);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tree node types");
                return StatusCode(500, "An error occurred while retrieving tree node types");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TreeNodeTypeDto>> GetById(Guid id)
        {
            try
            {
                var type = await _treeNodeTypeService.GetByIdAsync(id);
                if (type == null) return NotFound();
                
                return Ok(type);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tree node type {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the tree node type");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TreeNodeTypeDto>> Create(CreateTreeNodeTypeDto dto)
        {
            try
            {
                var created = await _treeNodeTypeService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tree node type");
                return StatusCode(500, "An error occurred while creating the tree node type");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TreeNodeTypeDto>> Update(Guid id, UpdateTreeNodeTypeDto dto)
        {
            try
            {
                var updated = await _treeNodeTypeService.UpdateAsync(id, dto);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tree node type {Id}", id);
                return StatusCode(500, "An error occurred while updating the tree node type");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _treeNodeTypeService.DeleteAsync(id);
                if (!result) return NotFound();
                
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tree node type {Id}", id);
                return StatusCode(500, "An error occurred while deleting the tree node type");
            }
        }
    }
}
```

Create `Ikhtibar.API/Controllers/TreeNodesController.cs`:
```csharp
using Ikhtibar.API.DTOs;
using Ikhtibar.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ikhtibar.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/tree-nodes")]
    public class TreeNodesController : ControllerBase
    {
        private readonly ITreeNodeService _treeNodeService;
        private readonly ILogger<TreeNodesController> _logger;

        public TreeNodesController(
            ITreeNodeService treeNodeService,
            ILogger<TreeNodesController> logger)
        {
            _treeNodeService = treeNodeService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TreeNodeDto>>> GetAll()
        {
            try
            {
                var nodes = await _treeNodeService.GetAllAsync();
                return Ok(nodes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tree nodes");
                return StatusCode(500, "An error occurred while retrieving tree nodes");
            }
        }

        [HttpGet("roots")]
        public async Task<ActionResult<IEnumerable<TreeNodeDto>>> GetRoots()
        {
            try
            {
                var nodes = await _treeNodeService.GetRootNodesAsync();
                return Ok(nodes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving root tree nodes");
                return StatusCode(500, "An error occurred while retrieving root tree nodes");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TreeNodeDetailDto>> GetById(Guid id)
        {
            try
            {
                var node = await _treeNodeService.GetByIdAsync(id);
                if (node == null) return NotFound();
                
                return Ok(node);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tree node {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the tree node");
            }
        }

        [HttpGet("code/{code}")]
        public async Task<ActionResult<TreeNodeDto>> GetByCode(string code)
        {
            try
            {
                var node = await _treeNodeService.GetByCodeAsync(code);
                if (node == null) return NotFound();
                
                return Ok(node);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tree node with code {Code}", code);
                return StatusCode(500, "An error occurred while retrieving the tree node");
            }
        }

        [HttpGet("{id}/children")]
        public async Task<ActionResult<IEnumerable<TreeNodeDto>>> GetChildren(Guid id)
        {
            try
            {
                var children = await _treeNodeService.GetChildrenAsync(id);
                return Ok(children);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving children of tree node {Id}", id);
                return StatusCode(500, "An error occurred while retrieving tree node children");
            }
        }

        [HttpGet("{id}/tree")]
        public async Task<ActionResult<TreeNodeDetailDto>> GetTree(Guid id, [FromQuery] int levels = 1)
        {
            try
            {
                var tree = await _treeNodeService.GetTreeAsync(id, levels);
                if (tree == null) return NotFound();
                
                return Ok(tree);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tree from node {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the tree");
            }
        }

        [HttpGet("{id}/ancestors")]
        public async Task<ActionResult<IEnumerable<TreeNodeDto>>> GetAncestors(Guid id)
        {
            try
            {
                var ancestors = await _treeNodeService.GetAncestorsAsync(id);
                return Ok(ancestors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ancestors of tree node {Id}", id);
                return StatusCode(500, "An error occurred while retrieving tree node ancestors");
            }
        }

        [HttpGet("{id}/descendants")]
        public async Task<ActionResult<IEnumerable<TreeNodeDto>>> GetDescendants(Guid id)
        {
            try
            {
                var descendants = await _treeNodeService.GetDescendantsAsync(id);
                return Ok(descendants);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving descendants of tree node {Id}", id);
                return StatusCode(500, "An error occurred while retrieving tree node descendants");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TreeNodeDto>> Create(CreateTreeNodeDto dto)
        {
            try
            {
                var created = await _treeNodeService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tree node");
                return StatusCode(500, "An error occurred while creating the tree node");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TreeNodeDto>> Update(Guid id, UpdateTreeNodeDto dto)
        {
            try
            {
                var updated = await _treeNodeService.UpdateAsync(id, dto);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tree node {Id}", id);
                return StatusCode(500, "An error occurred while updating the tree node");
            }
        }

        [HttpPut("{id}/move")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TreeNodeDto>> Move(Guid id, MoveTreeNodeDto dto)
        {
            try
            {
                var moved = await _treeNodeService.MoveAsync(id, dto);
                return Ok(moved);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error moving tree node {Id}", id);
                return StatusCode(500, "An error occurred while moving the tree node");
            }
        }

        [HttpPut("{parentId}/reorder")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Reorder(Guid parentId, [FromBody] IDictionary<Guid, int> nodeOrders)
        {
            try
            {
                var result = await _treeNodeService.ReorderAsync(parentId, nodeOrders);
                if (!result) return BadRequest("Failed to reorder nodes");
                
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reordering children of tree node {ParentId}", parentId);
                return StatusCode(500, "An error occurred while reordering tree nodes");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _treeNodeService.DeleteAsync(id);
                if (!result) return NotFound();
                
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tree node {Id}", id);
                return StatusCode(500, "An error occurred while deleting the tree node");
            }
        }
    }
}
```

### 6. Dependency Registration

Add to `Startup.cs` or `Program.cs` service registration:

```csharp
// Register repositories
services.AddScoped<ITreeNodeTypeRepository, TreeNodeTypeRepository>();
services.AddScoped<ITreeNodeRepository, TreeNodeRepository>();

// Register services
services.AddScoped<ITreeNodeTypeService, TreeNodeTypeService>();
services.AddScoped<ITreeNodeService, TreeNodeService>();
```

### 7. API Model Mappings

Add AutoMapper profile in `Ikhtibar.API/MapperProfiles/TreeNodeMapperProfile.cs`:

```csharp
using AutoMapper;
using Ikhtibar.API.DTOs;
using Ikhtibar.Shared.Entities;

namespace Ikhtibar.API.MapperProfiles
{
    public class TreeNodeMapperProfile : Profile
    {
        public TreeNodeMapperProfile()
        {
            // TreeNodeType mappings
            CreateMap<TreeNodeType, TreeNodeTypeDto>();
            CreateMap<CreateTreeNodeTypeDto, TreeNodeType>();
            CreateMap<UpdateTreeNodeTypeDto, TreeNodeType>();
            
            // TreeNode mappings
            CreateMap<TreeNode, TreeNodeDto>()
                .ForMember(dest => dest.TreeNodeTypeName, opt => 
                    opt.MapFrom(src => src.TreeNodeType != null ? src.TreeNodeType.Name : null))
                .ForMember(dest => dest.Level, opt => 
                    opt.MapFrom(src => src.Path.Count(c => c == '-') - 1))
                .ForMember(dest => dest.HasChildren, opt => 
                    opt.Ignore()); // This is populated later
            
            CreateMap<TreeNode, TreeNodeDetailDto>()
                .IncludeBase<TreeNode, TreeNodeDto>()
                .ForMember(dest => dest.Children, opt => opt.Ignore())
                .ForMember(dest => dest.CurriculumAlignments, opt => opt.Ignore());
            
            CreateMap<CreateTreeNodeDto, TreeNode>();
            CreateMap<UpdateTreeNodeDto, TreeNode>();
            
            // CurriculumAlignment mappings
            CreateMap<CurriculumAlignment, CurriculumAlignmentDto>();
        }
    }
}
```

## Integration Points

```yaml
DATABASE:
  - tables: "TreeNodes, TreeNodeTypes, CurriculumAlignments"
  - indexes: "IX_TN_Parent, IX_TN_Path for traversal"
CONFIG:
  - backend: "Register repositories/services in DI"
ROUTES:
  - api: "/api/tree-node-types, /api/tree-nodes"
PERMISSIONS:
  - roles: "Admin can create/update/delete"
```

## Validation Loop

### Level 1: Syntax & Style
```powershell
# Backend validation
dotnet build
dotnet format --verify-no-changes
dotnet test
```

### Level 2: Unit Tests
Create `Ikhtibar.Tests/Services/TreeNodeServiceTests.cs`:
```csharp
public class TreeNodeServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldSetCorrectPath_WhenParentProvided()
    {
        // Arrange
        // Mock repositories, mapper
        var parentNode = new TreeNode { Id = Guid.NewGuid(), Path = "-1-" };
        var dto = new CreateTreeNodeDto { ParentId = parentNode.Id };
        
        // Act
        var result = await _service.CreateAsync(dto);
        
        // Assert
        Assert.Equal($"-1-{parentNode.Id}-", result.Path);
    }
    
    [Fact]
    public async Task MoveAsync_ShouldUpdatePaths_ForAllDescendants()
    {
        // Arrange, Act, Assert
    }
}
```

### Level 3: Integration
```bash
# Check API endpoints
curl -X GET http://localhost:5000/api/tree-nodes/roots
# Expect: 200 OK and JSON array of root nodes

curl -X GET "http://localhost:5000/api/tree-nodes/{id}/tree?levels=2"
# Expect: 200 OK and hierarchical JSON object
```
