using Ikhtibar.API.Controllers.Base;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Core.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ikhtibar.API.Controllers;

/// <summary>
/// Sample controller demonstrating dependency injection and repository patterns
/// This controller serves as a template for implementing feature-specific controllers
/// </summary>
public class SampleController : ApiControllerBase
{
    private readonly IRepository<User> _userRepository;
    private readonly ILogger<SampleController> _logger;

    /// <summary>
    /// Initializes a new instance of the SampleController
    /// </summary>
    /// <param name="userRepository">User repository for data access</param>
    /// <param name="logger">Logger for this controller</param>
    public SampleController(IRepository<User> userRepository, ILogger<SampleController> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets all users (demonstration endpoint)
    /// </summary>
    /// <returns>List of users</returns>
    [HttpGet("users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            _logger.LogInformation("Fetching all users");

            var users = await _userRepository.GetAllAsync();

            _logger.LogInformation("Retrieved {UserCount} users", users.Count());

            return SuccessResponse(users, "Users retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching users");
            throw; // Let the global error handler process this
        }
    }

    /// <summary>
    /// Gets a user by ID (demonstration endpoint)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User details</returns>
    [HttpGet("users/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUser(int id)
    {
        try
        {
            _logger.LogInformation("Fetching user with ID: {UserId}", id);

            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", id);
                return NotFound($"User with ID {id} was not found");
            }

            _logger.LogInformation("User {UserId} retrieved successfully", id);

            return SuccessResponse(user, "User retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching user {UserId}", id);
            throw; // Let the global error handler process this
        }
    }

    /// <summary>
    /// Creates a new user (demonstration endpoint)
    /// </summary>
    /// <param name="request">User creation request</param>
    /// <returns>Created user</returns>
    [HttpPost("users")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating new user with email: {Email}", request.Email);

            var user = new User
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Username = request.Email, // Use email as username for demo
                PasswordHash = "demo_hash", // This would be properly hashed in real implementation
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            var createdUser = await _userRepository.AddAsync(user);

            _logger.LogInformation("User created successfully with ID: {UserId}", createdUser.Id);

            return CreatedAtAction(
                nameof(GetUser),
                new { id = createdUser.Id },
                SuccessResponse(createdUser, "User created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating user");
            throw; // Let the global error handler process this
        }
    }
}

/// <summary>
/// Request model for creating a user (demonstration)
/// </summary>
public class CreateUserRequest
{
    /// <summary>
    /// User's email address
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// User's first name
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// User's last name
    /// </summary>
    public required string LastName { get; set; }
}
