using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ikhtibar.API.Controllers.Base;
using Ikhtibar.Shared.DTOs;
using Ikhtibar.Core.Services.Interfaces;

namespace Ikhtibar.API.Controllers.UserManagement;

/// <summary>
/// API Controller for User management operations
/// Provides CRUD operations for users following REST conventions
/// </summary>
[Authorize]
[ApiController]
[Route("api/users")]
[Produces("application/json")]
public class UsersController : ApiControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserService userService,
        ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all users with optional pagination and sorting
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Number of items per page (default: 10)</param>
    /// <param name="sortBy">Field to sort by (default: createdAt)</param>
    /// <param name="sortDirection">Sort direction: asc or desc (default: desc)</param>
    /// <returns>List of users with pagination info</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllUsersAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = "createdAt",
        [FromQuery] string sortDirection = "desc")
    {
        try
        {
            _logger.LogInformation("Retrieving users - Page: {Page}, PageSize: {PageSize}, SortBy: {SortBy}, SortDirection: {SortDirection}", 
                page, pageSize, sortBy, sortDirection);
            
            var users = await _userService.GetAllUsersAsync(page, pageSize);
            
            // Return standardized API response format
            return SuccessResponse(users, "Users retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users");
            return ErrorResponse("Failed to retrieve users", StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Retrieves a specific user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserAsync(int id)
    {
        try
        {
            _logger.LogInformation("Retrieving user with ID: {UserId}", id);
            var user = await _userService.GetUserAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", id);
                return ErrorResponse($"User with ID {id} not found", StatusCodes.Status404NotFound);
            }

            return SuccessResponse(user, "User retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with ID: {UserId}", id);
            return ErrorResponse("Failed to retrieve user", StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Creates a new user
    /// </summary>
    /// <param name="createUserDto">User creation data</param>
    /// <returns>Created user details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserDto>> CreateUserAsync([FromBody] CreateUserDto createUserDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for user creation: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating new user with email: {Email}", createUserDto.Email);
            var createdUser = await _userService.CreateUserAsync(createUserDto);

            return CreatedAtAction(
                nameof(GetUserAsync),
                new { id = createdUser.UserId },
                createdUser);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "User creation failed - business rule violation");
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user with email: {Email}", createUserDto.Email);
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create user");
        }
    }

    /// <summary>
    /// Updates an existing user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="updateUserDto">User update data</param>
    /// <returns>Updated user details</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserDto>> UpdateUserAsync(int id, [FromBody] UpdateUserDto updateUserDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for user update: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Updating user with ID: {UserId}", id);
            var updatedUser = await _userService.UpdateUserAsync(id, updateUserDto);

            if (updatedUser == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for update", id);
                return NotFound($"User with ID {id} not found");
            }

            return Ok(updatedUser);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "User update failed - business rule violation for ID: {UserId}", id);
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID: {UserId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update user");
        }
    }

    /// <summary>
    /// Deletes a user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Deletion confirmation</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUserAsync(int id)
    {
        try
        {
            _logger.LogInformation("Deleting user with ID: {UserId}", id);
            var deleted = await _userService.DeleteUserAsync(id);

            if (!deleted)
            {
                _logger.LogWarning("User with ID {UserId} not found for deletion", id);
                return NotFound($"User with ID {id} not found");
            }

            _logger.LogInformation("User with ID {UserId} successfully deleted", id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "User deletion failed - business rule violation for ID: {UserId}", id);
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID: {UserId}", id);
            return ErrorResponse("Failed to delete user", StatusCodes.Status500InternalServerError);
        }
    }
}
