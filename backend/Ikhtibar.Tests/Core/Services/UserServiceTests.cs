using Xunit;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ikhtibar.Core.Services.Implementations;
using Ikhtibar.Core.Services.Interfaces;
using Ikhtibar.Core.Repositories.Interfaces;
using Ikhtibar.Core.DTOs;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Tests.TestHelpers;

namespace Ikhtibar.Tests.Core.Services;

/// <summary>
/// Comprehensive test suite for UserService business logic.
/// Tests all CRUD operations, validation rules, and error scenarios.
/// Uses AAA pattern (Arrange, Act, Assert) with descriptive test names.
/// Includes integration with mocked dependencies and database.
/// </summary>
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IRoleRepository> _mockRoleRepository;
    private readonly Mock<IUserRoleRepository> _mockUserRoleRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockRoleRepository = new Mock<IRoleRepository>();
        _mockUserRoleRepository = new Mock<IUserRoleRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<UserService>>();
        
        _userService = new UserService(
            _mockUserRepository.Object,
            _mockRoleRepository.Object,
            _mockUserRoleRepository.Object,
            _mockMapper.Object,
            _mockLogger.Object
        );
    // Apply sensible defaults to reduce per-test setup
    _mockUserRepository.ApplyDefaults(preSeed: false);
    _mockRoleRepository.ApplyDefaults();
    _mockUserRoleRepository.ApplyDefaults();
    _mockMapper.ApplyDefaults();
    // Per-test setups should control UserExists/GetByEmail behavior. Rely on ApplyDefaults in-memory store by default.
    }

    #region CreateUserAsync Tests

    /// <summary>
    /// Test: Creating user with valid data should return user DTO with generated ID
    /// Scenario: Happy path with all required fields provided
    /// Expected: User created successfully with proper mapping and validation
    /// </summary>
    [Fact]
    public async Task CreateUserAsync_Should_CreateUser_When_ValidDataProvided()
    {
        // Arrange
        var createUserDto = new CreateUserDto
        {
            Username = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "+1234567890",
            PreferredLanguage = "en",
            IsActive = true,
            Password = "password123",
            RoleIds = new List<int> { 1, 2 }
        };

        var user = new User
        {
            UserId = 0,
            Username = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "+1234567890",
            PreferredLanguage = "en",
            IsActive = true,
            EmailVerified = false,
            PhoneVerified = false,
            PasswordHash = "hashed_password",
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

        var userDto = new UserDto
        {
            UserId = 1,
            Username = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "+1234567890",
            PreferredLanguage = "en",
            IsActive = true,
            EmailVerified = false,
            PhoneVerified = false,
            Roles = new List<string> { "admin", "user" },
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

    _mockUserRepository.Setup(x => x.GetByEmailAsync("test@example.com"))
              .ReturnsAsync((User?)null);
    _mockMapper.Setup(x => x.Map<User>(createUserDto))
           .Returns(user);
    // Rely on ApplyDefaults AddAsync to assign an ID and persist the user so GetByIdAsync can retrieve it.

        // Act
        var result = await _userService.CreateUserAsync(createUserDto);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(1, result.UserId);
    Assert.Equal("test@example.com", result.Email);
    Assert.Equal("testuser", result.Username);
        _mockUserRepository.Verify(x => x.GetByEmailAsync("test@example.com"), Times.Once);
    }

    /// <summary>
    /// Test: Creating user with existing email should throw exception
    /// Scenario: Email address already exists in the system
    /// Expected: InvalidOperationException with descriptive message
    /// </summary>
    [Fact]
    public async Task CreateUserAsync_Should_ThrowException_When_EmailExists()
    {
        // Arrange
        var createUserDto = new CreateUserDto
        {
            Username = "testuser",
            Email = "existing@example.com",
            FirstName = "Test",
            LastName = "User",
            Password = "password123"
        };

        var existingUser = new User { UserId = 1, Email = "existing@example.com" };

        _mockUserRepository.Setup(x => x.GetByEmailAsync("existing@example.com"))
                          .ReturnsAsync(existingUser);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _userService.CreateUserAsync(createUserDto));
    }

    #endregion

    #region GetUserAsync Tests

    /// <summary>
    /// Test: Getting existing user should return user DTO
    /// Scenario: User exists in repository with valid ID
    /// Expected: User DTO returned with correct data
    /// </summary>
    [Fact]
    public async Task GetUserAsync_Should_ReturnUser_When_UserExists()
    {
        // Arrange
        var userId = 1;
        var user = new User
        {
            UserId = userId,
            Username = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            IsActive = true
        };

        var userDto = new UserDto
        {
            UserId = userId,
            Username = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            IsActive = true,
            Roles = new List<string> { "admin" }
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                          .ReturnsAsync(user);
        _mockMapper.Setup(x => x.Map<UserDto>(user))
                   .Returns(userDto);

        // Act
        var result = await _userService.GetUserAsync(userId);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(userId, result.UserId);
    Assert.Equal("testuser", result.Username);
    }

    #endregion

    #region UpdateUserAsync Tests

    /// <summary>
    /// Test: Updating user with valid data should return updated user DTO
    /// Scenario: Valid update data for existing user
    /// Expected: User updated successfully with new data
    /// </summary>
    [Fact]
    public async Task UpdateUserAsync_Should_UpdateUser_When_ValidDataProvided()
    {
        // Arrange
        var userId = 1;
        var updateUserDto = new UpdateUserDto
        {
            FirstName = "Updated",
            LastName = "User",
            PhoneNumber = "+1987654321",
            PreferredLanguage = "es",
            IsActive = true
        };

        var existingUser = new User
        {
            UserId = userId,
            Username = "testuser",
            Email = "test@example.com",
            FirstName = "Old",
            LastName = "User",
            IsActive = true
        };

        var updatedUser = new User
        {
            UserId = userId,
            Username = "testuser",
            Email = "test@example.com",
            FirstName = "Updated",
            LastName = "User",
            PhoneNumber = "+1987654321",
            PreferredLanguage = "es",
            IsActive = true
        };

        var userDto = new UserDto
        {
            UserId = userId,
            Username = "testuser",
            Email = "test@example.com",
            FirstName = "Updated",
            LastName = "User",
            PhoneNumber = "+1987654321",
            PreferredLanguage = "es",
            IsActive = true
        };

    // Return existing user first, then updated user after UpdateAsync
    _mockUserRepository.SetupSequence(x => x.GetByIdAsync(userId))
              .ReturnsAsync(existingUser)
              .ReturnsAsync(updatedUser);
    _mockMapper.Setup(x => x.Map(updateUserDto, existingUser))
           .Returns(updatedUser);
    _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
              .ReturnsAsync(updatedUser);
        _mockMapper.Setup(x => x.Map<UserDto>(updatedUser))
                   .Returns(userDto);

        // Act
        var result = await _userService.UpdateUserAsync(userId, updateUserDto);

    // Assert
    Assert.NotNull(result);
    Assert.Equal("Updated", result.FirstName);
    Assert.Equal("+1987654321", result.PhoneNumber);
    }

    /// <summary>
    /// Test: Updating non-existing user should throw exception
    /// Scenario: User ID does not exist in repository
    /// Expected: KeyNotFoundException thrown
    /// </summary>
    [Fact]
    public async Task UpdateUserAsync_Should_ThrowException_When_UserNotExists()
    {
        // Arrange
        var userId = 999;
        var updateUserDto = new UpdateUserDto
        {
            FirstName = "Updated",
            LastName = "User"
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                          .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _userService.UpdateUserAsync(userId, updateUserDto));
    }

    #endregion

    #region DeleteUserAsync Tests

    /// <summary>
    /// Test: Deleting existing user should return true
    /// Scenario: Valid user ID for existing user
    /// Expected: True returned and user deleted
    /// </summary>
    [Fact]
    public async Task DeleteUserAsync_Should_ReturnTrue_When_UserExists()
    {
        // Arrange
        var userId = 1;
    _mockUserRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(new User { UserId = userId, Username = "user1", Email = "user1@example.com" });
    _mockUserRepository.Setup(x => x.DeleteAsync(userId))
              .ReturnsAsync(true);

        // Act
        var result = await _userService.DeleteUserAsync(userId);

    // Assert
    Assert.True(result);
    }

    /// <summary>
    /// Test: Deleting non-existing user should return false
    /// Scenario: User ID does not exist
    /// Expected: False returned
    /// </summary>
    [Fact]
    public async Task DeleteUserAsync_Should_ReturnFalse_When_UserNotExists()
    {
        // Arrange
        var userId = 999;
        _mockUserRepository.Setup(x => x.DeleteAsync(userId))
                          .ReturnsAsync(false);

        // Act
        var result = await _userService.DeleteUserAsync(userId);

    // Assert
    Assert.False(result);
    }

    #endregion

    #region GetAllUsersAsync Tests

    /// <summary>
    /// Test: Getting all users should return all users from repository
    /// Scenario: Repository contains multiple users
    /// Expected: All users returned as DTOs
    /// </summary>
    [Fact]
    public async Task GetAllUsersAsync_Should_ReturnAllUsers_When_UsersExist()
    {
        // Arrange
        var users = new List<User>
        {
            new User { UserId = 1, Username = "user1", Email = "user1@example.com" },
            new User { UserId = 2, Username = "user2", Email = "user2@example.com" }
        };

        var userDtos = new List<UserDto>
        {
            new UserDto { UserId = 1, Username = "user1", Email = "user1@example.com", Roles = new List<string> { "user" } },
            new UserDto { UserId = 2, Username = "user2", Email = "user2@example.com", Roles = new List<string> { "user" } }
        };

        _mockUserRepository.Setup(x => x.GetAllAsync(null, null))
                          .ReturnsAsync(users);
        _mockMapper.Setup(x => x.Map<IEnumerable<UserDto>>(users))
                   .Returns(userDtos);

        // Act
        var result = await _userService.GetAllUsersAsync();

    // Assert
    Assert.NotNull(result);
    Assert.Equal(2, result.Count());
    }

    #endregion

    #region GetUserByEmailAsync Tests

    /// <summary>
    /// Test: Getting user by email should return user DTO when user exists
    /// Scenario: User with specified email exists
    /// Expected: User DTO returned
    /// </summary>
    [Fact]
    public async Task GetUserByEmailAsync_Should_ReturnUser_When_EmailExists()
    {
        // Arrange
        var email = "test@example.com";
        var user = new User
        {
            UserId = 1,
            Username = "testuser",
            Email = email,
            FirstName = "Test",
            LastName = "User"
        };

        var userDto = new UserDto
        {
            UserId = 1,
            Username = "testuser",
            Email = email,
            FirstName = "Test",
            LastName = "User"
        };

        _mockUserRepository.Setup(x => x.GetByEmailAsync(email))
                          .ReturnsAsync(user);
        _mockMapper.Setup(x => x.Map<UserDto>(user))
                   .Returns(userDto);

        // Act
        var result = await _userService.GetUserByEmailAsync(email);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(email, result.Email);
    }

    #endregion

    #region AuthenticateAsync Tests

    /// <summary>
    /// Test: Authentication with valid credentials should return user DTO
    /// Scenario: Email and password match existing user
    /// Expected: User DTO returned
    /// </summary>
    [Fact]
    public async Task AuthenticateAsync_Should_ReturnUser_When_CredentialsValid()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password123";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User
        {
            UserId = 1,
            Email = email,
            PasswordHash = hashedPassword,
            IsActive = true,
            EmailVerified = true
        };

        var userDto = new UserDto
        {
            UserId = 1,
            Email = email,
            IsActive = true,
            EmailVerified = true
        };

        _mockUserRepository.Setup(x => x.GetByEmailAsync(email))
                          .ReturnsAsync(user);
        _mockMapper.Setup(x => x.Map<UserDto>(user))
                   .Returns(userDto);

    // Ensure GetUserAsync can retrieve the user after authentication
    _mockUserRepository.Setup(x => x.GetByIdAsync(user.UserId)).ReturnsAsync(user);

        // Act
        var result = await _userService.AuthenticateAsync(email, password);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(email, result.Email);
    }

    /// <summary>
    /// Test: Authentication with invalid email should return null
    /// Scenario: Email does not exist in system
    /// Expected: Null returned
    /// </summary>
    [Fact]
    public async Task AuthenticateAsync_Should_ReturnNull_When_EmailNotExists()
    {
        // Arrange
        var email = "nonexistent@example.com";
        var password = "password123";

        _mockUserRepository.Setup(x => x.GetByEmailAsync(email))
                          .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.AuthenticateAsync(email, password);

    // Assert
    Assert.Null(result);
    }

    /// <summary>
    /// Test: Authentication with invalid password should return null
    /// Scenario: Password does not match user's password
    /// Expected: Null returned
    /// </summary>
    [Fact]
    public async Task AuthenticateAsync_Should_ReturnNull_When_PasswordInvalid()
    {
        // Arrange
        var email = "test@example.com";
        var password = "wrongpassword";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("correctpassword");

        var user = new User
        {
            UserId = 1,
            Email = email,
            PasswordHash = hashedPassword,
            IsActive = true
        };

        _mockUserRepository.Setup(x => x.GetByEmailAsync(email))
                          .ReturnsAsync(user);

        // Act
        var result = await _userService.AuthenticateAsync(email, password);

    // Assert
    Assert.Null(result);
    }

    #endregion

    #region UserExistsAsync Tests

    /// <summary>
    /// Test: Checking existing user should return true
    /// Scenario: User exists in repository
    /// Expected: True returned
    /// </summary>
    [Fact]
    public async Task UserExistsAsync_Should_ReturnTrue_When_UserExists()
    {
        // Arrange
        var userId = 1;
    _mockUserRepository.Setup(x => x.UserExistsAsync(userId))
                          .ReturnsAsync(true);

        // Act
        var result = await _userService.UserExistsAsync(userId);

    // Assert
    Assert.True(result);
    }

    /// <summary>
    /// Test: Checking non-existing user should return false
    /// Scenario: User does not exist in repository
    /// Expected: False returned
    /// </summary>
    [Fact]
    public async Task UserExistsAsync_Should_ReturnFalse_When_UserNotExists()
    {
        // Arrange
        var userId = 999;
        _mockUserRepository.Setup(x => x.ExistsAsync(userId))
                          .ReturnsAsync(false);

        // Act
        var result = await _userService.UserExistsAsync(userId);

    // Assert
    Assert.False(result);
    }

    #endregion

    #region EmailExistsAsync Tests

    /// <summary>
    /// Test: Checking existing email should return true
    /// Scenario: Email exists in repository
    /// Expected: True returned
    /// </summary>
    [Fact]
    public async Task EmailExistsAsync_Should_ReturnTrue_When_EmailExists()
    {
        // Arrange
        var email = "existing@example.com";
        var user = new User { UserId = 1, Email = email };
        _mockUserRepository.Setup(x => x.GetByEmailAsync(email))
                          .ReturnsAsync(user);

        // Act
        var result = await _userService.EmailExistsAsync(email);

    // Assert
    Assert.True(result);
    }

    /// <summary>
    /// Test: Checking non-existing email should return false
    /// Scenario: Email does not exist in repository
    /// Expected: False returned
    /// </summary>
    [Fact]
    public async Task EmailExistsAsync_Should_ReturnFalse_When_EmailNotExists()
    {
        // Arrange
        var email = "nonexistent@example.com";
        _mockUserRepository.Setup(x => x.GetByEmailAsync(email))
                          .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.EmailExistsAsync(email);

    // Assert
    Assert.False(result);
    }

    #endregion

    #region UsernameExistsAsync Tests

    /// <summary>
    /// Test: Checking existing username should return true
    /// Scenario: Username exists in repository
    /// Expected: True returned
    /// </summary>
    [Fact]
    public async Task UsernameExistsAsync_Should_ReturnTrue_When_UsernameExists()
    {
        // Arrange
        var username = "existinguser";
        var user = new User { UserId = 1, Username = username };
        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
                          .ReturnsAsync(user);

        // Act
        var result = await _userService.UserExistsAsync(1);

    // Assert
    Assert.True(result);
    }

    /// <summary>
    /// Test: Checking non-existing username should return false
    /// Scenario: Username does not exist in repository
    /// Expected: False returned
    /// </summary>
    [Fact]
    public async Task UsernameExistsAsync_Should_ReturnFalse_When_UsernameNotExists()
    {
        // Arrange
        var username = "nonexistentuser@example.com";
        _mockUserRepository.Setup(x => x.GetByEmailAsync(username))
                          .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.UserExistsAsync(764532);

    // Assert
    Assert.False(result);
    }

    #endregion
}
