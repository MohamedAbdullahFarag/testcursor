using Xunit;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ikhtibar.Core.Services.Implementations;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Core.DTOs;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Tests.TestHelpers;

namespace Ikhtibar.Tests.Core.Services;

public class UserRoleServiceTests
{
    private readonly Mock<IUserRoleRepository> _mockUserRoleRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IRoleRepository> _mockRoleRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<UserRoleService>> _mockLogger;
    private readonly UserRoleService _userRoleService;

    public UserRoleServiceTests()
    {
        _mockUserRoleRepository = new Mock<IUserRoleRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockRoleRepository = new Mock<IRoleRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<UserRoleService>>();

        _userRoleService = new UserRoleService(
            _mockUserRoleRepository.Object,
            _mockUserRepository.Object,
            _mockRoleRepository.Object,
            _mockMapper.Object,
            _mockLogger.Object
        );

    // Apply shared defaults
    _mockUserRepository.ApplyDefaults(preSeed: false);
    _mockRoleRepository.ApplyDefaults();
    _mockUserRoleRepository.ApplyDefaults();
    _mockMapper.ApplyDefaults();
    // Per-test setups should control UserExists/GetById behavior. Rely on ApplyDefaults in-memory store by default.
    }

    [Fact]
    public async Task AssignRoleAsync_Should_AssignRole_When_UserAndRoleExist()
    {
        var userId = 1;
        var roleId = 2;

    _mockUserRepository.Setup(x => x.UserExistsAsync(userId)).ReturnsAsync(true);
    _mockRoleRepository.Setup(x => x.GetByIdAsync(roleId)).ReturnsAsync(new Role { RoleId = roleId, Code = "user", Name = "User" });
        _mockUserRoleRepository.Setup(x => x.UserHasRoleAsync(userId, roleId)).ReturnsAsync(false);
        _mockUserRoleRepository.Setup(x => x.AssignRoleAsync(userId, roleId)).Returns(Task.CompletedTask);

        await _userRoleService.AssignRoleAsync(userId, roleId);
    }

    [Fact]
    public async Task AssignRoleAsync_Should_ThrowException_When_UserNotExists()
    {
        var userId = 999;
        var roleId = 1;

    _mockUserRepository.Setup(x => x.UserExistsAsync(userId)).ReturnsAsync(false);

    await Assert.ThrowsAsync<System.InvalidOperationException>(() => _userRoleService.AssignRoleAsync(userId, roleId));
    }

    [Fact]
    public async Task AssignRoleAsync_Should_ThrowException_When_RoleNotExists()
    {
        var userId = 1;
        var roleId = 999;

    _mockUserRepository.Setup(x => x.UserExistsAsync(userId)).ReturnsAsync(true);
    _mockRoleRepository.Setup(x => x.GetByIdAsync(roleId)).ReturnsAsync((Role?)null);

    await Assert.ThrowsAsync<System.InvalidOperationException>(() => _userRoleService.AssignRoleAsync(userId, roleId));
    }

    [Fact]
    public async Task AssignRoleAsync_Should_BeIdempotent_When_UserAlreadyHasRole()
    {
        var userId = 1;
        var roleId = 2;

    _mockUserRepository.Setup(x => x.UserExistsAsync(userId)).ReturnsAsync(true);
    _mockRoleRepository.Setup(x => x.GetByIdAsync(roleId)).ReturnsAsync(new Role { RoleId = roleId, Code = "role", Name = "Role" });
        _mockUserRoleRepository.Setup(x => x.UserHasRoleAsync(userId, roleId)).ReturnsAsync(true);

        await _userRoleService.AssignRoleAsync(userId, roleId);
    }

    [Fact]
    public async Task RemoveRoleAsync_Should_RemoveRole_When_UserHasRole()
    {
        var userId = 1;
        var roleId = 2;

    _mockUserRepository.Setup(x => x.UserExistsAsync(userId)).ReturnsAsync(true);
    _mockUserRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(new User { UserId = userId, Username = $"user{userId}", Email = $"user{userId}@example.com" });
    _mockUserRoleRepository.Setup(x => x.RemoveRoleAsync(userId, roleId)).Returns(Task.CompletedTask);

    await _userRoleService.RemoveRoleAsync(userId, roleId);
    }

    [Fact]
    public async Task RemoveRoleAsync_Should_BeIdempotent_When_UserDoesNotHaveRole()
    {
        var userId = 1;
        var roleId = 2;

    _mockUserRepository.Setup(x => x.UserExistsAsync(userId)).ReturnsAsync(true);
    _mockUserRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(new User { UserId = userId });
    _mockUserRoleRepository.Setup(x => x.RemoveRoleAsync(userId, roleId)).Returns(Task.CompletedTask);

    await _userRoleService.RemoveRoleAsync(userId, roleId);
    }

    [Fact]
    public async Task GetUserRolesAsync_Should_ReturnRoles_When_UserHasRoles()
    {
        var userId = 1;
        var userRoles = new List<UserRole>
        {
            new UserRole { UserId = userId, RoleId = 1 },
            new UserRole { UserId = userId, RoleId = 2 }
        };

        var roles = new List<RoleDto>
        {
            new RoleDto { RoleId = 1, Name = "Admin", Code = "admin" },
            new RoleDto { RoleId = 2, Name = "User", Code = "user" }
        };

    _mockUserRepository.Setup(x => x.UserExistsAsync(userId)).ReturnsAsync(true);
    _mockUserRoleRepository.Setup(r => r.GetUserRolesAsync(userId)).ReturnsAsync(userRoles);
    _mockMapper.Setup(m => m.Map<IEnumerable<RoleDto>>(It.IsAny<IEnumerable<UserRole>>())).Returns(roles);

        var result = await _userRoleService.GetUserRolesAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetUserRolesAsync_Should_ThrowException_When_UserNotExists()
    {
        var userId = 999;
        _mockUserRepository.Setup(x => x.ExistsAsync(userId)).ReturnsAsync(false);

        await Assert.ThrowsAsync<System.InvalidOperationException>(() => _userRoleService.GetUserRolesAsync(userId));
    }

    [Fact]
    public async Task GetRoleUsersAsync_Should_ReturnUsers_When_RoleHasUsers()
    {
        var roleId = 1;
        var userRoles = new List<UserRole>
        {
            new UserRole { UserId = 1, RoleId = roleId },
            new UserRole { UserId = 2, RoleId = roleId }
        };

        var users = new List<UserDto>
        {
            new UserDto { UserId = 1, Username = "admin", FirstName = "Administrator", Roles = new List<string> { "Admin" } },
            new UserDto { UserId = 2, Username = "user1", FirstName = "User", LastName = "One", Roles = new List<string> { "User" } }
        };

    _mockRoleRepository.Setup(x => x.ExistsAsync(roleId)).ReturnsAsync(true);
    // Ensure the user repository returns user entities for the user ids used by the mapper
    _mockUserRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new User { UserId = 1, Username = "admin", FirstName = "Administrator" });
    _mockUserRepository.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(new User { UserId = 2, Username = "user1", FirstName = "User", LastName = "One" });
        _mockUserRoleRepository.Setup(x => x.GetRoleUsersAsync(roleId)).ReturnsAsync(userRoles);
        _mockMapper.Setup(m => m.Map<IEnumerable<UserDto>>(It.IsAny<IEnumerable<UserRole>>())).Returns(users);

        var result = await _userRoleService.GetRoleUsersAsync(roleId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        var resultList = result.ToList();
        Assert.Equal("admin", resultList[0].Username);
    }

    [Fact]
    public async Task UserHasRoleAsync_Should_ReturnTrue_When_UserHasRole()
    {
        var userId = 1;
        var roleId = 2;

        _mockUserRoleRepository.Setup(x => x.UserHasRoleAsync(userId, roleId)).ReturnsAsync(true);

        var result = await _userRoleService.UserHasRoleAsync(userId, roleId);

        Assert.True(result);
    }

    [Fact]
    public async Task UserHasRoleAsync_Should_ReturnFalse_When_UserDoesNotHaveRole()
    {
        var userId = 1;
        var roleId = 2;

        _mockUserRoleRepository.Setup(x => x.UserHasRoleAsync(userId, roleId)).ReturnsAsync(false);

        var result = await _userRoleService.UserHasRoleAsync(userId, roleId);

        Assert.False(result);
    }

    [Fact]
    public async Task RemoveAllUserRolesAsync_Should_RemoveAllRoles_When_UserHasRoles()
    {
        var userId = 1;

    _mockUserRepository.Setup(x => x.UserExistsAsync(userId)).ReturnsAsync(true);
    _mockUserRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(new User { UserId = userId });
    _mockUserRoleRepository.Setup(x => x.RemoveAllUserRolesAsync(userId)).Returns(Task.CompletedTask);

    await _userRoleService.RemoveAllUserRolesAsync(userId);
    }

    [Fact]
    public async Task AssignRolesAsync_Should_AssignAllRoles_When_AllRolesExist()
    {
        var userId = 1;
        var roleIds = new List<int> { 1, 2, 3 };

    _mockUserRepository.Setup(x => x.UserExistsAsync(userId)).ReturnsAsync(true);
    _mockUserRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(new User { UserId = userId });

        foreach (var roleId in roleIds)
        {
            _mockRoleRepository.Setup(x => x.ExistsAsync(roleId)).ReturnsAsync(true);
            _mockUserRoleRepository.Setup(x => x.UserHasRoleAsync(userId, roleId)).ReturnsAsync(false);
            _mockUserRoleRepository.Setup(x => x.AssignRoleAsync(userId, roleId)).Returns(Task.CompletedTask);
        }

        await _userRoleService.AssignRolesAsync(userId, roleIds);
    }
}
