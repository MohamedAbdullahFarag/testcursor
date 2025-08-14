using AutoMapper;
using Microsoft.Extensions.Logging;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Core.Services.Interfaces;

using Ikhtibar.Shared.Models;
namespace Ikhtibar.Core.Services.Implementations;

/// <summary>
/// Service implementation for TreeNodeType management operations
/// Following SRP: ONLY tree node type business logic
/// </summary>
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
        _treeNodeTypeRepository = treeNodeTypeRepository ?? throw new ArgumentNullException(nameof(treeNodeTypeRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<TreeNodeTypeDto>> GetAllAsync()
    {
        using var scope = _logger.BeginScope("Getting all tree node types");

        try
        {
            _logger.LogInformation("Retrieving all tree node types");

            var entities = await _treeNodeTypeRepository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<TreeNodeTypeDto>>(entities);

            _logger.LogInformation("Retrieved {Count} tree node types", dtos.Count());
            return dtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all tree node types");
            throw;
        }
    }

    public async Task<IEnumerable<TreeNodeTypeDto>> GetActiveAsync()
    {
        using var scope = _logger.BeginScope("Getting active tree node types");

        try
        {
            _logger.LogInformation("Retrieving active tree node types");

            var entities = await _treeNodeTypeRepository.GetActiveAsync();
            var dtos = _mapper.Map<IEnumerable<TreeNodeTypeDto>>(entities);

            _logger.LogInformation("Retrieved {Count} active tree node types", dtos.Count());
            return dtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active tree node types");
            throw;
        }
    }

    public async Task<TreeNodeTypeDto?> GetTreeNodeTypeAsync(int treeNodeTypeId)
    {
        using var scope = _logger.BeginScope("Getting tree node type {TreeNodeTypeId}", treeNodeTypeId);

        try
        {
            _logger.LogInformation("Retrieving tree node type");

            var entity = await _treeNodeTypeRepository.GetByIdAsync(treeNodeTypeId);
            if (entity == null)
            {
                _logger.LogWarning("Tree node type not found");
                return null;
            }

            var dto = _mapper.Map<TreeNodeTypeDto>(entity);
            _logger.LogInformation("Tree node type retrieved successfully");
            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tree node type");
            throw;
        }
    }

    public async Task<TreeNodeTypeDto?> GetTreeNodeTypeByNameAsync(string name)
    {
        using var scope = _logger.BeginScope("Getting tree node type by name {Name}", name);

        try
        {
            _logger.LogInformation("Retrieving tree node type by name");

            var entity = await _treeNodeTypeRepository.GetByNameAsync(name);
            if (entity == null)
            {
                _logger.LogWarning("Tree node type not found");
                return null;
            }

            var dto = _mapper.Map<TreeNodeTypeDto>(entity);
            _logger.LogInformation("Tree node type retrieved successfully");
            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tree node type by name");
            throw;
        }
    }

    public async Task<TreeNodeTypeDto> CreateTreeNodeTypeAsync(CreateTreeNodeTypeDto createDto)
    {
        using var scope = _logger.BeginScope("Creating tree node type {Name}", createDto.Name);

        try
        {
            _logger.LogInformation("Starting tree node type creation process");

            // Business validation: Check if name already exists
            var existingType = await _treeNodeTypeRepository.GetByNameAsync(createDto.Name);
            if (existingType != null)
            {
                _logger.LogWarning("Tree node type with name already exists");
                throw new InvalidOperationException($"Tree node type with name '{createDto.Name}' already exists");
            }

            // Map and create entity
            var entity = _mapper.Map<TreeNodeType>(createDto);
            var createdEntity = await _treeNodeTypeRepository.CreateAsync(entity);
            var resultDto = _mapper.Map<TreeNodeTypeDto>(createdEntity);

            _logger.LogInformation("Tree node type created successfully with ID {TreeNodeTypeId}", resultDto.TreeNodeTypeId);
            return resultDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tree node type");
            throw;
        }
    }

    public async Task<TreeNodeTypeDto> UpdateTreeNodeTypeAsync(int treeNodeTypeId, UpdateTreeNodeTypeDto updateDto)
    {
        using var scope = _logger.BeginScope("Updating tree node type {TreeNodeTypeId}", treeNodeTypeId);

        try
        {
            _logger.LogInformation("Starting tree node type update process");

            // Check if entity exists
            var existingEntity = await _treeNodeTypeRepository.GetByIdAsync(treeNodeTypeId);
            if (existingEntity == null)
            {
                _logger.LogWarning("Tree node type not found for update");
                throw new KeyNotFoundException($"Tree node type with ID {treeNodeTypeId} not found");
            }

            // Business validation: Check if new name conflicts with another type
            if (existingEntity.Name != updateDto.Name)
            {
                var conflictingType = await _treeNodeTypeRepository.GetByNameAsync(updateDto.Name);
                if (conflictingType != null && conflictingType.TreeNodeTypeId != treeNodeTypeId)
                {
                    _logger.LogWarning("Tree node type name conflicts with existing type");
                    throw new InvalidOperationException($"Tree node type with name '{updateDto.Name}' already exists");
                }
            }

            // Update entity
            _mapper.Map(updateDto, existingEntity);
            var updatedEntity = await _treeNodeTypeRepository.UpdateAsync(existingEntity);
            var resultDto = _mapper.Map<TreeNodeTypeDto>(updatedEntity);

            _logger.LogInformation("Tree node type updated successfully");
            return resultDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tree node type");
            throw;
        }
    }

    public async Task<bool> DeleteTreeNodeTypeAsync(int treeNodeTypeId)
    {
        using var scope = _logger.BeginScope("Deleting tree node type {TreeNodeTypeId}", treeNodeTypeId);

        try
        {
            _logger.LogInformation("Starting tree node type deletion process");

            // Check if entity exists
            var existingEntity = await _treeNodeTypeRepository.GetByIdAsync(treeNodeTypeId);
            if (existingEntity == null)
            {
                _logger.LogWarning("Tree node type not found for deletion");
                return false;
            }

            // Business validation: Check if type is in use
            var isInUse = await _treeNodeTypeRepository.IsInUseAsync(treeNodeTypeId);
            if (isInUse)
            {
                _logger.LogWarning("Cannot delete tree node type as it is in use");
                throw new InvalidOperationException($"Cannot delete tree node type '{existingEntity.Name}' as it is in use by existing nodes");
            }

            // Delete entity
            var result = await _treeNodeTypeRepository.DeleteAsync(treeNodeTypeId);

            _logger.LogInformation("Tree node type deletion result: {Result}", result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tree node type");
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int treeNodeTypeId)
    {
        try
        {
            var result = await _treeNodeTypeRepository.ExistsAsync(treeNodeTypeId);
            _logger.LogDebug("Tree node type {TreeNodeTypeId} exists: {Exists}", treeNodeTypeId, result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if tree node type exists");
            throw;
        }
    }

    public async Task<bool> IsInUseAsync(int treeNodeTypeId)
    {
        try
        {
            var result = await _treeNodeTypeRepository.IsInUseAsync(treeNodeTypeId);
            _logger.LogDebug("Tree node type {TreeNodeTypeId} is in use: {InUse}", treeNodeTypeId, result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if tree node type is in use");
            throw;
        }
    }
}
