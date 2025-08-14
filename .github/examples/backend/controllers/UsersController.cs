using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ikhtibar.Backend.Features.Users.Services;
using Ikhtibar.Backend.Features.Users.Models;
using Ikhtibar.Backend.Shared.Common;
using Microsoft.AspNetCore.JsonPatch;
using System.ComponentModel.DataAnnotations;
using Ikhtibar.Backend.Shared.Attributes;
using System.Net;
using Ikhtibar.Backend.Features.Users.Constants;
using Ikhtibar.Backend.Shared.Localization;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Ikhtibar.Backend.Features.Users.Controllers;

/// <summary>
/// Controller for managing user operations with full CRUD support and additional features
/// like bulk operations, partial updates, and exports.
/// 
/// This controller demonstrates PRP (Product Requirements Prompt) methodology:
/// - Context is King: Comprehensive documentation and error handling with i18n
/// - Validation Loops: Every action has proper validation and error states
/// - Information Dense: Rich type safety with detailed parameter documentation
/// - Progressive Success: Actions build on each other logically (GET, POST, PUT, PATCH, DELETE)
/// - One-Pass Implementation: Complete API surface with all standard operations plus specialized endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ApiVersion("1.0")]
[ServiceFilter(typeof(ApiExceptionFilter))]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;
    private readonly IMapper _mapper;
    private readonly ILocalizationService _localizationService;

    public UsersController(
        IUserService userService, 
        ILogger<UsersController> logger, 
        IMapper mapper,
        ILocalizationService localizationService)
    {
        _userService = userService;
        _logger = logger;
        _mapper = mapper;
        _localizationService = localizationService;
    }

    /// <summary>
    /// Get all users with pagination, filtering, sorting and search capabilities
    /// </summary>
    /// <remarks>
    /// PRP Context Engineering: Rich logging context captures all request parameters
    /// PRP Validation Loop: Input validation + structured error responses
    /// Information Density: Uses strongly-typed request/response models
    /// </remarks>
    /// <param name="request">Pagination and filter parameters</param>
    /// <returns>Paginated list of users</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<UserDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [RateLimit(PerSecond = 10, PerMinute = 100)]
    public async Task<ActionResult<ApiResponse<PaginatedResult<UserDto>>>> GetUsers([FromQuery] GetUsersRequest request)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["Action"] = "GetUsers",
            ["Page"] = request.Page,
            ["PageSize"] = request.PageSize,
            ["SearchTerm"] = request.SearchTerm ?? "None",
            ["LanguageCode"] = _localizationService.GetCurrentLanguage()
        });
        
        try
        {
            _logger.LogInformation("Getting users with pagination. Page: {Page}, PageSize: {PageSize}", 
                request.Page, request.PageSize);

            var result = await _userService.GetUsersAsync(request);

            return Ok(new ApiResponse<PaginatedResult<UserDto>>
            {
                Success = true,
                Data = result,
                Message = _localizationService.GetLocalizedString(UserLocalizationKeys.UsersRetrievedSuccessfully)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users with request {@Request}", request);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = _localizationService.GetLocalizedString(CommonLocalizationKeys.InternalServerError),
                Errors = new List<string> { _localizationService.GetLocalizedString(UserLocalizationKeys.ErrorRetrievingUsers) }
            });
        }
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User details</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "id" })]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetUser(int id)
    {
        try
        {
            _logger.LogInformation("Getting user with ID: {UserId}", id);

            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = _localizationService.GetLocalizedString(UserLocalizationKeys.UserNotFound),
                    Errors = new List<string> { 
                        _localizationService.GetLocalizedString(UserLocalizationKeys.UserWithIdNotFound, new { Id = id }) 
                    }
                });
            }

            return Ok(new ApiResponse<UserDto>
            {
                Success = true,
                Data = user,
                Message = _localizationService.GetLocalizedString(UserLocalizationKeys.UserRetrievedSuccessfully)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user with ID: {UserId}", id);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = _localizationService.GetLocalizedString(CommonLocalizationKeys.InternalServerError)
            });
        }
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="request">User creation data</param>
    /// <returns>Created user</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [RateLimit(PerMinute = 30)]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            _logger.LogInformation("Creating new user with email: {Email}", request.Email);

            var user = await _userService.CreateUserAsync(request);

            // Return 201 Created with Location header pointing to the GetUser endpoint
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new ApiResponse<UserDto>
            {
                Success = true,
                Data = user,
                Message = _localizationService.GetLocalizedString(UserLocalizationKeys.UserCreatedSuccessfully)
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid user data provided: {Message}", ex.Message);
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = _localizationService.GetLocalizedString(UserLocalizationKeys.InvalidUserData),
                Errors = new List<string> { ex.Message }
            });
        }
        catch (DuplicateResourceException ex)
        {
            _logger.LogWarning(ex, "Duplicate user data: {Message}", ex.Message);
            return Conflict(new ApiResponse<object>
            {
                Success = false,
                Message = _localizationService.GetLocalizedString(UserLocalizationKeys.DuplicateUser),
                Errors = new List<string> { ex.Message }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user with email: {Email}", request.Email);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = _localizationService.GetLocalizedString(CommonLocalizationKeys.InternalServerError)
            });
        }
    }

    /// <summary>
    /// Create multiple users in a single request
    /// </summary>
    /// <param name="requests">List of user creation requests</param>
    /// <returns>List of created users</returns>
    [HttpPost("bulk")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserDto>>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [RateLimit(PerHour = 5)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> CreateUsers(
        [FromBody] IEnumerable<CreateUserRequest> requests)
    {
        try
        {
            if (requests == null || !requests.Any())
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = _localizationService.GetLocalizedString(UserLocalizationKeys.NoUsersProvided),
                    Errors = new List<string> { _localizationService.GetLocalizedString(UserLocalizationKeys.RequestMustContainAtLeastOneUser) }
                });
            }

            _logger.LogInformation("Creating {Count} users in bulk", requests.Count());

            var users = await _userService.CreateUsersAsync(requests);

            return StatusCode(StatusCodes.Status201Created, new ApiResponse<IEnumerable<UserDto>>
            {
                Success = true,
                Data = users,
                Message = _localizationService.GetLocalizedString(UserLocalizationKeys.UsersCreatedSuccessfully, new { Count = users.Count() })
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid user data provided for bulk creation: {Message}", ex.Message);
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = _localizationService.GetLocalizedString(UserLocalizationKeys.InvalidUserData),
                Errors = new List<string> { ex.Message }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating users in bulk");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = _localizationService.GetLocalizedString(CommonLocalizationKeys.InternalServerError)
            });
        }
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">User update data</param>
    /// <returns>Updated user</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
        try
        {
            _logger.LogInformation("Updating user with ID: {UserId}", id);

            var user = await _userService.UpdateUserAsync(id, request);

            if (user == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = _localizationService.GetLocalizedString(UserLocalizationKeys.UserNotFound),
                    Errors = new List<string> { 
                        _localizationService.GetLocalizedString(UserLocalizationKeys.UserWithIdNotFound, new { Id = id }) 
                    }
                });
            }

            return Ok(new ApiResponse<UserDto>
            {
                Success = true,
                Data = user,
                Message = _localizationService.GetLocalizedString(UserLocalizationKeys.UserUpdatedSuccessfully)
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid user data provided for update: {Message}", ex.Message);
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = _localizationService.GetLocalizedString(UserLocalizationKeys.InvalidUserData),
                Errors = new List<string> { ex.Message }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID: {UserId}", id);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = _localizationService.GetLocalizedString(CommonLocalizationKeys.InternalServerError)
            });
        }
    }

    /// <summary>
    /// Partially update a user with JSON Patch
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="patchDocument">The JSON Patch document with partial updates</param>
    /// <returns>Updated user</returns>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [Consumes("application/json-patch+json")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<UserDto>>> PatchUser(
        int id, 
        [FromBody] JsonPatchDocument<UpdateUserRequest> patchDocument)
    {
        try
        {
            _logger.LogInformation("Partially updating user with ID: {UserId}", id);

            // First get the existing user
            var existingUser = await _userService.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = _localizationService.GetLocalizedString(UserLocalizationKeys.UserNotFound),
                    Errors = new List<string> { 
                        _localizationService.GetLocalizedString(UserLocalizationKeys.UserWithIdNotFound, new { Id = id }) 
                    }
                });
            }

            // Create update request from existing user
            var updateRequest = _mapper.Map<UpdateUserRequest>(existingUser);

            // Apply patch operations to the update request
            patchDocument.ApplyTo(updateRequest, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = _localizationService.GetLocalizedString(UserLocalizationKeys.InvalidPatchDocument),
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            // Process the update with the patched request
            var updatedUser = await _userService.UpdateUserAsync(id, updateRequest);

            return Ok(new ApiResponse<UserDto>
            {
                Success = true,
                Data = updatedUser,
                Message = _localizationService.GetLocalizedString(UserLocalizationKeys.UserUpdatedSuccessfully)
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid patch data provided: {Message}", ex.Message);
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = _localizationService.GetLocalizedString(UserLocalizationKeys.InvalidUserData),
                Errors = new List<string> { ex.Message }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error patching user with ID: {UserId}", id);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = _localizationService.GetLocalizedString(CommonLocalizationKeys.InternalServerError)
            });
        }
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteUser(int id)
    {
        try
        {
            _logger.LogInformation("Deleting user with ID: {UserId}", id);

            var result = await _userService.DeleteUserAsync(id);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = _localizationService.GetLocalizedString(UserLocalizationKeys.UserNotFound),
                    Errors = new List<string> { 
                        _localizationService.GetLocalizedString(UserLocalizationKeys.UserWithIdNotFound, new { Id = id }) 
                    }
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = _localizationService.GetLocalizedString(UserLocalizationKeys.UserDeletedSuccessfully)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID: {UserId}", id);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = _localizationService.GetLocalizedString(CommonLocalizationKeys.InternalServerError)
            });
        }
    }

    /// <summary>
    /// Delete multiple users in a single request
    /// </summary>
    /// <param name="ids">List of user IDs to delete</param>
    /// <returns>Bulk delete result</returns>
    [HttpDelete("bulk")]
    [ProducesResponseType(typeof(ApiResponse<BulkDeleteResult>), StatusCodes.Status200OK)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<BulkDeleteResult>>> DeleteUsers([FromBody] IEnumerable<int> ids)
    {
        try
        {
            _logger.LogInformation("Deleting multiple users. Count: {Count}", ids.Count());

            var result = await _userService.DeleteUsersAsync(ids);

            return Ok(new ApiResponse<BulkDeleteResult>
            {
                Success = true,
                Data = result,
                Message = _localizationService.GetLocalizedString(
                    UserLocalizationKeys.UsersDeletedWithResult, 
                    new { 
                        Deleted = result.SuccessCount, 
                        Failed = result.FailedCount 
                    }
                )
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting multiple users");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = _localizationService.GetLocalizedString(CommonLocalizationKeys.InternalServerError)
            });
        }
    }

    /// <summary>
    /// Export users to a specified format (CSV, Excel, PDF)
    /// </summary>
    /// <param name="format">Export format (csv, excel, pdf)</param>
    /// <returns>File download</returns>
    [HttpGet("export")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> ExportUsers([FromQuery] string format = "csv")
    {
        try
        {
            _logger.LogInformation("Exporting users in {Format} format", format);

            var result = await _userService.ExportUsersAsync(format);

            // Set culture-specific filename based on current language
            var currentLanguage = _localizationService.GetCurrentLanguage();
            var fileNameBase = currentLanguage == "ar" ? "المستخدمين" : "Users";
            var fileName = $"{fileNameBase}_{DateTime.UtcNow:yyyyMMddHHmmss}.{result.FileExtension}";

            // Set content disposition with proper handling of Arabic characters if needed
            if (currentLanguage == "ar")
            {
                // RFC 5987 encoding for non-ASCII characters in headers
                var encodedFileName = Uri.EscapeDataString(fileName);
                Response.Headers.Add("Content-Disposition", $"attachment; filename*=UTF-8''{encodedFileName}");
            }
            else
            {
                Response.Headers.Add("Content-Disposition", $"attachment; filename={fileName}");
            }

            // Add RTL marker for Arabic content in CSV or other text formats
            if (currentLanguage == "ar" && (format == "csv" || format == "txt"))
            {
                // Add RTL mark at the beginning of the file for proper RTL display
                var rtlBytes = Encoding.UTF8.GetBytes("\u200F");
                var combinedBytes = rtlBytes.Concat(result.FileContents).ToArray();
                return File(combinedBytes, result.ContentType);
            }

            return File(result.FileContents, result.ContentType);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid export format: {Message}", ex.Message);
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = _localizationService.GetLocalizedString(UserLocalizationKeys.InvalidExportFormat),
                Errors = new List<string> { ex.Message }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting users in {Format} format", format);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = _localizationService.GetLocalizedString(CommonLocalizationKeys.InternalServerError)
            });
        }
    }

    /// <summary>
    /// Change user active status
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="isActive">New active status</param>
    /// <returns>Updated user</returns>
    [HttpPut("{id:int}/status")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<UserDto>>> ChangeUserStatus(int id, [FromQuery] bool isActive)
    {
        try
        {
            _logger.LogInformation("Changing status for user with ID: {UserId} to {Status}", 
                id, isActive ? "Active" : "Inactive");

            var user = await _userService.ChangeUserStatusAsync(id, isActive);

            if (user == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = _localizationService.GetLocalizedString(UserLocalizationKeys.UserNotFound),
                    Errors = new List<string> { 
                        _localizationService.GetLocalizedString(UserLocalizationKeys.UserWithIdNotFound, new { Id = id }) 
                    }
                });
            }

            var messageKey = isActive 
                ? UserLocalizationKeys.UserActivatedSuccessfully 
                : UserLocalizationKeys.UserDeactivatedSuccessfully;

            return Ok(new ApiResponse<UserDto>
            {
                Success = true,
                Data = user,
                Message = _localizationService.GetLocalizedString(messageKey)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing status for user with ID: {UserId}", id);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = _localizationService.GetLocalizedString(CommonLocalizationKeys.InternalServerError)
            });
        }
    }
}