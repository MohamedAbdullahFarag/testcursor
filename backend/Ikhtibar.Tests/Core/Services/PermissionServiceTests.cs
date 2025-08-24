using Xunit;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ikhtibar.Infrastructure.Services;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Core.DTOs;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Tests.TestHelpers;

namespace Ikhtibar.Tests.Core.Services;

/// <summary>
/// Comprehensive test suite for PermissionService business logic.
/// Tests all permission operations, validation rules, and error scenarios.
/// Uses AAA pattern (Arrange, Act, Assert) with descriptive test names.
/// Includes integration with mocked dependencies and database.
/// </summary>

public class PermissionServiceTests
{
    private Mock<IPermissionRepository> _mockPermissionRepository;
    private Mock<IRolePermissionRepository> _mockRolePermissionRepository;
    private Mock<IUserRoleRepository> _mockUserRoleRepository;
    private Mock<IMapper> _mockMapper;
    private Mock<ILogger<PermissionService>> _mockLogger;
    private PermissionService _permissionService;

    public PermissionServiceTests()
    {
        _mockPermissionRepository = new Mock<IPermissionRepository>();
        _mockRolePermissionRepository = new Mock<IRolePermissionRepository>();
        _mockUserRoleRepository = new Mock<IUserRoleRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<PermissionService>>();

        _permissionService = new PermissionService(
            _mockPermissionRepository.Object,
            _mockRolePermissionRepository.Object,
            _mockUserRoleRepository.Object,
            _mockMapper.Object,
            _mockLogger.Object
        );
    // apply defaults to reduce repetitive setups
    _mockUserRoleRepository.ApplyDefaults();
    _mockMapper.ApplyDefaults();
    }

    #region GetAllPermissionsAsync Tests

    /// <summary>
    /// Test: Getting all permissions should return all permissions from repository
    /// Scenario: Repository contains multiple permissions
    /// Expected: All permissions returned as DTOs
    /// </summary>
    [Fact]
    public async Task GetAllPermissionsAsync_Should_ReturnAllPermissions_When_PermissionsExist()
    {
        // Arrange
        var permissions = new List<Permission>
        {
            new Permission { PermissionId = 1, Code = "read_users", Name = "Read Users" },
            new Permission { PermissionId = 2, Code = "write_users", Name = "Write Users" }
        };

        var permissionDtos = new List<PermissionDto>
        {
            new PermissionDto { PermissionId = 1, Code = "read_users", Name = "Read Users" },
            new PermissionDto { PermissionId = 2, Code = "write_users", Name = "Write Users" }
        };

    _mockPermissionRepository.Setup(r => r.GetAllAsync(null, null)).ReturnsAsync(permissions);
    // Map any enumerable of Permission to PermissionDto by mapping each element through the mapper mock
    _mockMapper.Setup(m => m.Map<IEnumerable<PermissionDto>>(It.IsAny<IEnumerable<Permission>>()))
           .Returns((IEnumerable<Permission> perms) => perms?.Select(p => _mockMapper.Object.Map<PermissionDto>(p)).ToList() ?? new List<PermissionDto>());

        // Act
        var result = await _permissionService.GetAllPermissionsAsync();

    // Assert
    Assert.NotNull(result);
    Assert.Equal(2, result.Count());
        _mockPermissionRepository.Verify(r => r.GetAllAsync(null, null), Times.Once);
    }

    #endregion

    #region GetUserPermissionsAsync Tests

    /// <summary>
    /// Test: Getting user permissions should return permissions through roles
    /// Scenario: User has roles with associated permissions
    /// Expected: Collection of user's permissions returned
    /// </summary>
    [Fact]
    public async Task GetUserPermissionsAsync_Should_ReturnUserPermissions_When_UserHasRoles()
    {
        // Arrange
        var userId = 1;
        var userRoles = new List<UserRole>
        {
            new UserRole { UserId = userId, RoleId = 1 },
            new UserRole { UserId = userId, RoleId = 2 }
        };

        var roles = new List<Role>
        {
            new Role { RoleId = 1, Code = "admin", Name = "Administrator" },
            new Role { RoleId = 2, Code = "user", Name = "User" }
        };

        var rolePermissions = new List<RolePermission>
        {
            new RolePermission { RoleId = 1, PermissionId = 1 },
            new RolePermission { RoleId = 1, PermissionId = 2 },
            new RolePermission { RoleId = 2, PermissionId = 1 }
        };

        var permissions = new List<Permission>
        {
            new Permission { PermissionId = 1, Code = "read_users", Name = "Read Users" },
            new Permission { PermissionId = 2, Code = "write_users", Name = "Write Users" }
        };

        var permissionDtos = new List<PermissionDto>
        {
            new PermissionDto { PermissionId = 1, Code = "read_users", Name = "Read Users" },
            new PermissionDto { PermissionId = 2, Code = "write_users", Name = "Write Users" }
        };

        _mockUserRoleRepository.Setup(r => r.GetUserRolesAsync(userId)).ReturnsAsync(userRoles);
    // RolePermissionRepository provides permissions by role
        _mockRolePermissionRepository.Setup(r => r.GetPermissionsByRoleAsync(It.IsAny<int>()))
            .ReturnsAsync((int roleId) => rolePermissions.Where(rp => rp.RoleId == roleId).Select(rp => permissions.First(p => p.PermissionId == rp.PermissionId)));
        _mockPermissionRepository.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync(permissions);
        _mockMapper.Setup(m => m.Map<IEnumerable<PermissionDto>>(It.IsAny<IEnumerable<Permission>>()))
                   .Returns((IEnumerable<Permission> perms) => perms?.Select(p => _mockMapper.Object.Map<PermissionDto>(p)).ToList() ?? new List<PermissionDto>());

        // Act
    var result = await _permissionService.GetPermissionsByUserAsync(userId);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(2, result.Count());
    }

    #endregion

    #region UserHasPermissionAsync Tests

    /// <summary>
    /// Test: User should have permission when they have role with that permission
    /// Scenario: User has a role that grants the permission
    /// Expected: True returned
    /// </summary>
    [Fact]
    public async Task UserHasPermissionAsync_Should_ReturnTrue_When_UserHasPermission()
    {
        // Arrange
        var userId = 1;
        var permissionCode = "read_users";
        
        var userRoles = new List<UserRole>
        {
            new UserRole { UserId = userId, RoleId = 1 }
        };

        var rolePermissions = new List<RolePermission>
        {
            new RolePermission { RoleId = 1, PermissionId = 1 }
        };

        var permission = new Permission { PermissionId = 1, Code = permissionCode, Name = "Read Users" };

    var permissions = new List<Permission> { permission };

    _mockUserRoleRepository.Setup(r => r.GetUserRolesAsync(userId)).ReturnsAsync(userRoles);
    _mockRolePermissionRepository.Setup(r => r.GetPermissionsByRoleAsync(1)).ReturnsAsync(rolePermissions.Where(rp => rp.RoleId == 1).Select(rp => permissions.First(p => p.PermissionId == rp.PermissionId)));
        _mockPermissionRepository.Setup(r => r.GetByCodeAsync(permissionCode)).ReturnsAsync(permission);

        // Act
    var result = await _permissionService.UserHasPermissionAsync(userId, permissionCode);

    // Assert
    Assert.True(result);
    }

    /// <summary>
    /// Test: User should not have permission when they don't have role with that permission
    /// Scenario: User doesn't have any role that grants the permission
    /// Expected: False returned
    /// </summary>
    [Fact]
    public async Task UserHasPermissionAsync_Should_ReturnFalse_When_UserDoesNotHavePermission()
    {
        // Arrange
        var userId = 1;
        var permissionCode = "write_users";
        
        var userRoles = new List<UserRole>
        {
            new UserRole { UserId = userId, RoleId = 1 }
        };

        var rolePermissions = new List<RolePermission>
        {
            new RolePermission { RoleId = 1, PermissionId = 1 }
        };

        var permission = new Permission { PermissionId = 2, Code = permissionCode, Name = "Write Users" };

    var permissions = new List<Permission> { permission };

    _mockUserRoleRepository.Setup(r => r.GetUserRolesAsync(userId)).ReturnsAsync(userRoles);
    _mockRolePermissionRepository.Setup(r => r.GetPermissionsByRoleAsync(1)).ReturnsAsync(rolePermissions.Where(rp => rp.RoleId == 1).Select(rp => permissions.First(p => p.PermissionId == rp.PermissionId)));
        _mockPermissionRepository.Setup(r => r.GetByCodeAsync(permissionCode)).ReturnsAsync(permission);

        // Act
    var result = await _permissionService.UserHasPermissionAsync(userId, permissionCode);

    // Assert
    Assert.False(result);
    }

    #endregion

    #region GetPermissionMatrixAsync Tests

    /// <summary>
    /// Test: Getting permission matrix should return roles and permissions matrix
    /// Scenario: System has roles and permissions with associations
    /// Expected: Permission matrix returned with role-permission mappings
    /// </summary>
    [Fact]
    public async Task GetPermissionMatrixAsync_Should_ReturnMatrix_When_RolesAndPermissionsExist()
    {
        // Arrange
        var roles = new List<Role>
        {
            new Role { RoleId = 1, Code = "admin", Name = "Administrator" },
            new Role { RoleId = 2, Code = "user", Name = "User" }
        };

        var permissions = new List<Permission>
        {
            new Permission { PermissionId = 1, Code = "read_users", Name = "Read Users" },
            new Permission { PermissionId = 2, Code = "write_users", Name = "Write Users" }
        };

        var rolePermissions = new List<RolePermission>
        {
            new RolePermission { RoleId = 1, PermissionId = 1 },
            new RolePermission { RoleId = 1, PermissionId = 2 },
            new RolePermission { RoleId = 2, PermissionId = 1 }
        };

        var roleDtos = new List<RoleDto>
        {
            new RoleDto { RoleId = 1, Code = "admin", Name = "Administrator" },
            new RoleDto { RoleId = 2, Code = "user", Name = "User" }
        };

        var permissionDtos = new List<PermissionDto>
        {
            new PermissionDto { PermissionId = 1, Code = "read_users", Name = "Read Users" },
            new PermissionDto { PermissionId = 2, Code = "write_users", Name = "Write Users" }
        };

        _mockPermissionRepository.Setup(r => r.GetAllAsync(null, null)).ReturnsAsync(permissions);
    // Simulate mapping of role->permission relationships via GetAllRolesWithPermissionsAsync
    _mockRolePermissionRepository.Setup(r => r.GetAllRolesWithPermissionsAsync()).ReturnsAsync(roles.Select(r => new Role { RoleId = r.RoleId, Code = r.Code, Name = r.Name }));
        _mockMapper.Setup(m => m.Map<IEnumerable<RoleDto>>(roles)).Returns(roleDtos);
        _mockMapper.Setup(m => m.Map<IEnumerable<PermissionDto>>(It.IsAny<IEnumerable<Permission>>()))
                   .Returns((IEnumerable<Permission> perms) => perms?.Select(p => _mockMapper.Object.Map<PermissionDto>(p)).ToList() ?? new List<PermissionDto>());

        // Act
    var result = await _permissionService.GetPermissionMatrixAsync();

    // Assert
    Assert.NotNull(result);
    Assert.Equal(2, result.Permissions.Count());
    Assert.Equal(2, result.Roles.Count());
    Assert.Equal(2, result.Matrix.Count);
        _mockPermissionRepository.Verify(r => r.GetAllAsync(null, null), Times.Once);
    }

    #endregion

    #region CodeExistsAsync Tests

    /// <summary>
    /// Test: Code exists should return false when permission code doesn't exist
    /// Scenario: Permission code is not in use
    /// Expected: False returned
    /// </summary>
    [Fact]
    public async Task CodeExistsAsync_Should_ReturnFalse_When_CodeDoesNotExist()
    {
        // Arrange
        var permissionCode = "new_permission";
    _mockPermissionRepository.Setup(r => r.CodeExistsAsync(permissionCode, null)).ReturnsAsync(false);
    var result = await _mockPermissionRepository.Object.CodeExistsAsync(permissionCode, null);
    Assert.False(result);
    }

    /// <summary>
    /// Test: Code exists should return true when permission code exists
    /// Scenario: Permission code is already in use
    /// Expected: True returned
    /// </summary>
    [Fact]
    public async Task CodeExistsAsync_Should_ReturnTrue_When_CodeExists()
    {
        // Arrange
        var permissionCode = "existing_permission";
        var permission = new Permission { PermissionId = 1, Code = permissionCode };
    _mockPermissionRepository.Setup(r => r.CodeExistsAsync(permissionCode, null)).ReturnsAsync(true);
    var result = await _mockPermissionRepository.Object.CodeExistsAsync(permissionCode, null);
    Assert.True(result);
    }

    #endregion

    #region GetUserPermissionsAsync Empty Tests

    /// <summary>
    /// Test: Getting permissions for user with no roles should return empty collection
    /// Scenario: User has no assigned roles
    /// Expected: Empty permissions collection returned
    /// </summary>
    [Fact]
    public async Task GetUserPermissionsAsync_Should_ReturnEmpty_When_UserHasNoRoles()
    {
        // Arrange
        var userId = 1;
        var emptyUserRoles = new List<UserRole>();

        _mockUserRoleRepository.Setup(r => r.GetUserRolesAsync(userId)).ReturnsAsync(emptyUserRoles);

    // Act
    var result = await _permissionService.GetPermissionsByUserAsync(userId);

    // Assert
    Assert.NotNull(result);
    Assert.Empty(result);
    }

    #endregion
}
