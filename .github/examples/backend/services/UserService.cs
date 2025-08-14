using AutoMapper;
using Ikhtibar.Backend.Features.Users.Models;
using Ikhtibar.Backend.Features.Users.Repositories;
using Ikhtibar.Backend.Shared.Common;
using Ikhtibar.Backend.Shared.Export;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Ikhtibar.Backend.Shared.Localization;
using Ikhtibar.Backend.Features.Users.Constants;
using System.Security.Claims;

namespace Ikhtibar.Backend.Features.Users.Services;

/// <summary>
/// Service interface for user operations
/// </summary>
public interface IUserService
{
    Task<PaginatedResult<UserDto>> GetUsersAsync(GetUsersRequest request);
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<UserDto> CreateUserAsync(CreateUserRequest request);
    Task<UserDto?> UpdateUserAsync(int id, UpdateUserRequest request);
    Task<bool> DeleteUserAsync(int id);
    Task<BulkDeleteResult> DeleteUsersAsync(IEnumerable<int> ids);
    Task<ExportResult> ExportUsersAsync(string format);
    Task<bool> IsEmailInUseAsync(string email, int? excludeUserId = null);
    Task<UserDto?> ChangeUserStatusAsync(int id, bool isActive);
}

/// <summary>
/// Implementation of user service operations following PRP Methodology:
/// - Context is King: Comprehensive context in documentation and error handling
/// - Validation Loops: Built-in validation for all operations 
/// - Information Dense: Clear method signatures and type safety
/// - Progressive Success: Operations maintain data integrity at each step
/// - One-Pass Implementation: Complete user management with all required features
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;
    private readonly IExportService _exportService;
    private readonly ILocalizationService _localizationService;
    private readonly ICurrentUserService _currentUserService;

    public UserService(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<UserService> logger,
        IExportService exportService,
        ILocalizationService localizationService,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _exportService = exportService;
        _localizationService = localizationService;
        _currentUserService = currentUserService;
    }

    /// <inheritdoc />
    /// <remarks>
    /// PRP Context Engineering: Includes comprehensive context via logging and locale-aware adjustments
    /// Supports RTL languages by applying special sorting and text direction markers
    /// </remarks>
    public async Task<PaginatedResult<UserDto>> GetUsersAsync(GetUsersRequest request)
    {
        using var logScope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["Page"] = request.Page,
            ["PageSize"] = request.PageSize,
            ["SearchTerm"] = request.SearchTerm ?? "None",
            ["Status"] = request.Status?.ToString() ?? "All",
            ["Role"] = request.Role?.ToString() ?? "All",
            ["SortBy"] = request.SortBy ?? "CreatedAt",
            ["SortOrder"] = request.SortOrder ?? "Desc",
            ["CurrentUser"] = _currentUserService.GetCurrentUserId(),
            ["LanguageCode"] = _localizationService.GetCurrentLanguage()
        });
        
        _logger.LogInformation("Getting users with pagination");

        // Get current language to determine sort order for names
        var currentLanguage = _localizationService.GetCurrentLanguage();
        var isRtl = currentLanguage == "ar";
        
        // Apply special sorting for RTL text if needed
        if (isRtl && (request.SortBy?.ToLower() == "name" || request.SortBy?.ToLower() == "fullname"))
        {
            _logger.LogDebug("Applying RTL-specific name sorting");
            // Use special RTL-aware sorting provided by repository
            request.SortBy = "NameRtl";
        }
        
        var result = await _userRepository.GetUsersAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm,
            request.Status,
            request.Role,
            request.SortBy,
            request.SortOrder);
        
        var userDtos = _mapper.Map<List<UserDto>>(result.Items);

        // Set direction hints for proper text display based on current language
        if (isRtl)
        {
            foreach (var user in userDtos)
            {
                // Mark names with RTL hint for proper display
                user.Name = $"\u200F{user.Name}";
                if (!string.IsNullOrEmpty(user.Country))
                {
                    user.Country = $"\u200F{user.Country}";
                }
            }
        }
        
        return new PaginatedResult<UserDto>
        {
            Items = userDtos,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages,
            TotalRecords = result.TotalRecords,
            HasNextPage = result.HasNextPage,
            HasPreviousPage = result.HasPreviousPage
        };
    }

    /// <inheritdoc />
    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        _logger.LogInformation("Getting user with ID: {UserId}", id);
        
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return null;
        
        var userDto = _mapper.Map<UserDto>(user);
        
        // Add RTL mark for proper display if needed
        if (_localizationService.GetCurrentLanguage() == "ar")
        {
            userDto.Name = $"\u200F{userDto.Name}";
            if (!string.IsNullOrEmpty(userDto.Country))
            {
                userDto.Country = $"\u200F{userDto.Country}";
            }
        }
        
        return userDto;
    }

    /// <inheritdoc />
    public async Task<UserDto> CreateUserAsync(CreateUserRequest request)
    {
        using var logScope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["Email"] = request.Email,
            ["Role"] = request.Role,
            ["CurrentUser"] = _currentUserService.GetCurrentUserId()
        });
        
        _logger.LogInformation("Creating new user with email: {Email}", request.Email);
        
        // Validate input
        ValidateUserRequest(request);
        
        // Check if email is already in use
        if (await IsEmailInUseAsync(request.Email))
        {
            var errorMessage = _localizationService.GetLocalizedString(
                UserLocalizationKeys.EmailAlreadyInUse, 
                new { Email = request.Email }
            );
            
            _logger.LogWarning("Email already in use: {Email}", request.Email);
            throw new DuplicateResourceException(errorMessage);
        }
        
        // Enforce security policies based on current user role
        await EnforceUserCreationSecurityPolicies(request);
        
        // Create user entity
        var userEntity = _mapper.Map<UserEntity>(request);
        
        // Set default values
        userEntity.IsActive = request.IsActive ?? true;
        userEntity.CreatedAt = DateTime.UtcNow;
        userEntity.UpdatedAt = DateTime.UtcNow;
        userEntity.CreatedById = _currentUserService.GetCurrentUserId();
        userEntity.PreferredLanguage = request.PreferredLanguage ?? _localizationService.GetCurrentLanguage();
        
        // Save user to database
        var createdUser = await _userRepository.CreateAsync(userEntity);
        
        _logger.LogInformation("User created successfully with ID: {UserId}", createdUser.Id);
        
        return _mapper.Map<UserDto>(createdUser);
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<UserDto>> CreateUsersAsync(IEnumerable<CreateUserRequest> requests)
    {
        _logger.LogInformation("Creating {Count} users in bulk", requests.Count());
        
        var results = new List<UserDto>();
        var errors = new List<Exception>();
        
        foreach (var request in requests)
        {
            try
            {
                var user = await CreateUserAsync(request);
                results.Add(user);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to create user with email: {Email}", request.Email);
                errors.Add(ex);
            }
        }
        
        if (errors.Any())
        {
            _logger.LogWarning(
                "Bulk user creation completed with {SuccessCount} successes and {ErrorCount} failures",
                results.Count,
                errors.Count);
                
            // If no users were created successfully, throw the first error
            if (results.Count == 0 && errors.Any())
            {
                throw errors.First();
            }
        }
        else
        {
            _logger.LogInformation("All {Count} users created successfully", results.Count);
        }
        
        return results;
    }

    /// <inheritdoc />
    public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserRequest request)
    {
        using var logScope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["UserId"] = id,
            ["CurrentUser"] = _currentUserService.GetCurrentUserId()
        });
        
        _logger.LogInformation("Updating user with ID: {UserId}", id);
        
        // Validate input
        ValidateUserUpdateRequest(request);
        
        // Get existing user
        var userEntity = await _userRepository.GetByIdAsync(id);
        if (userEntity == null)
        {
            _logger.LogWarning("User not found with ID: {UserId}", id);
            return null;
        }
        
        // Check if email is changed and already in use
        if (request.Email != null && 
            request.Email != userEntity.Email && 
            await IsEmailInUseAsync(request.Email, id))
        {
            var errorMessage = _localizationService.GetLocalizedString(
                UserLocalizationKeys.EmailAlreadyInUse, 
                new { Email = request.Email }
            );
            
            _logger.LogWarning("Email already in use: {Email}", request.Email);
            throw new DuplicateResourceException(errorMessage);
        }
        
        // Enforce security policies based on current user role
        await EnforceUserUpdateSecurityPolicies(userEntity, request);
        
        // Update user properties
        if (request.Name != null) userEntity.Name = request.Name;
        if (request.Email != null) userEntity.Email = request.Email;
        if (request.PhoneNumber != null) userEntity.PhoneNumber = request.PhoneNumber;
        if (request.Role != null) userEntity.Role = request.Role;
        if (request.AvatarUrl != null) userEntity.AvatarUrl = request.AvatarUrl;
        if (request.IsActive.HasValue) userEntity.IsActive = request.IsActive.Value;
        if (request.PreferredLanguage != null) userEntity.PreferredLanguage = request.PreferredLanguage;
        if (request.Country != null) userEntity.Country = request.Country;
        
        // Update audit fields
        userEntity.UpdatedAt = DateTime.UtcNow;
        userEntity.UpdatedById = _currentUserService.GetCurrentUserId();
        
        // Save changes
        var updatedUser = await _userRepository.UpdateAsync(userEntity);
        
        _logger.LogInformation("User updated successfully with ID: {UserId}", updatedUser.Id);
        
        return _mapper.Map<UserDto>(updatedUser);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteUserAsync(int id)
    {
        using var logScope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["UserId"] = id,
            ["CurrentUser"] = _currentUserService.GetCurrentUserId()
        });
        
        _logger.LogInformation("Deleting user with ID: {UserId}", id);
        
        // Enforce security policies for deletion
        await EnforceUserDeletionSecurityPolicies(id);
        
        // Delete user
        var result = await _userRepository.DeleteAsync(id);
        
        if (result)
        {
            _logger.LogInformation("User deleted successfully with ID: {UserId}", id);
        }
        else
        {
            _logger.LogWarning("User not found for deletion with ID: {UserId}", id);
        }
        
        return result;
    }
    
    /// <inheritdoc />
    public async Task<BulkDeleteResult> DeleteUsersAsync(IEnumerable<int> ids)
    {
        _logger.LogInformation("Deleting multiple users. Count: {Count}", ids.Count());
        
        var result = new BulkDeleteResult
        {
            SuccessCount = 0,
            FailedCount = 0,
            FailedIds = new List<int>()
        };
        
        foreach (var id in ids)
        {
            try
            {
                var success = await DeleteUserAsync(id);
                if (success)
                {
                    result.SuccessCount++;
                }
                else
                {
                    result.FailedCount++;
                    result.FailedIds.Add(id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete user with ID: {UserId}", id);
                result.FailedCount++;
                result.FailedIds.Add(id);
            }
        }
        
        _logger.LogInformation(
            "Bulk delete completed with {SuccessCount} successes and {FailedCount} failures",
            result.SuccessCount,
            result.FailedCount);
            
        return result;
    }
    
    /// <inheritdoc />
    public async Task<ExportResult> ExportUsersAsync(string format)
    {
        _logger.LogInformation("Exporting users in {Format} format", format);
        
        // Validate export format
        if (!_exportService.IsSupportedFormat(format))
        {
            var supportedFormats = string.Join(", ", _exportService.GetSupportedFormats());
            var errorMessage = _localizationService.GetLocalizedString(
                UserLocalizationKeys.UnsupportedExportFormat, 
                new { Format = format, SupportedFormats = supportedFormats }
            );
            
            _logger.LogWarning("Unsupported export format: {Format}", format);
            throw new ArgumentException(errorMessage);
        }
        
        // Get all users for export (with pagination to handle large datasets)
        const int pageSize = 1000;
        var page = 1;
        var allUsers = new List<UserDto>();
        PaginatedResult<UserEntity> result;
        
        do
        {
            result = await _userRepository.GetUsersAsync(
                page,
                pageSize,
                null,
                null,
                null,
                "Id",
                "Asc");
            
            allUsers.AddRange(_mapper.Map<List<UserDto>>(result.Items));
            page++;
        } 
        while (result.HasNextPage);
        
        _logger.LogInformation("Retrieved {Count} users for export", allUsers.Count);
        
        // Prepare export data
        var exportData = allUsers.Select(u => new
        {
            u.Id,
            u.Name,
            u.Email,
            u.PhoneNumber,
            u.Role,
            Status = u.IsActive ? 
                _localizationService.GetLocalizedString(UserLocalizationKeys.Active) : 
                _localizationService.GetLocalizedString(UserLocalizationKeys.Inactive),
            u.Country,
            PreferredLanguage = u.PreferredLanguage == "ar" ? 
                _localizationService.GetLocalizedString(UserLocalizationKeys.Arabic) : 
                _localizationService.GetLocalizedString(UserLocalizationKeys.English),
            CreatedAt = u.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
            LastLogin = u.LastLoginDate?.ToString("yyyy-MM-dd HH:mm")
        });
        
        // Define column headers with localization
        var headers = new Dictionary<string, string>
        {
            ["Id"] = _localizationService.GetLocalizedString(CommonLocalizationKeys.Id),
            ["Name"] = _localizationService.GetLocalizedString(UserLocalizationKeys.Name),
            ["Email"] = _localizationService.GetLocalizedString(UserLocalizationKeys.Email),
            ["PhoneNumber"] = _localizationService.GetLocalizedString(UserLocalizationKeys.PhoneNumber),
            ["Role"] = _localizationService.GetLocalizedString(UserLocalizationKeys.Role),
            ["Status"] = _localizationService.GetLocalizedString(UserLocalizationKeys.Status),
            ["Country"] = _localizationService.GetLocalizedString(UserLocalizationKeys.Country),
            ["PreferredLanguage"] = _localizationService.GetLocalizedString(UserLocalizationKeys.PreferredLanguage),
            ["CreatedAt"] = _localizationService.GetLocalizedString(UserLocalizationKeys.CreatedAt),
            ["LastLogin"] = _localizationService.GetLocalizedString(UserLocalizationKeys.LastLogin)
        };
        
        // Set right-to-left direction for Arabic exports
        var isRtl = _localizationService.GetCurrentLanguage() == "ar";
        
        // Generate export file
        var result = await _exportService.ExportDataAsync(
            exportData, 
            headers,
            format,
            "Users",
            isRtl);
            
        _logger.LogInformation(
            "Users exported successfully in {Format} format. File size: {FileSize} bytes",
            format,
            result.FileContents.Length);
            
        return result;
    }
    
    /// <inheritdoc />
    public async Task<bool> IsEmailInUseAsync(string email, int? excludeUserId = null)
    {
        _logger.LogDebug("Checking if email is in use: {Email}", email);
        return await _userRepository.IsEmailInUseAsync(email, excludeUserId);
    }
    
    /// <inheritdoc />
    public async Task<UserDto?> ChangeUserStatusAsync(int id, bool isActive)
    {
        using var logScope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["UserId"] = id,
            ["NewStatus"] = isActive ? "Active" : "Inactive",
            ["CurrentUser"] = _currentUserService.GetCurrentUserId()
        });
        
        _logger.LogInformation(
            "Changing status for user with ID: {UserId} to {Status}", 
            id, 
            isActive ? "Active" : "Inactive");
        
        // Get existing user
        var userEntity = await _userRepository.GetByIdAsync(id);
        if (userEntity == null)
        {
            _logger.LogWarning("User not found with ID: {UserId}", id);
            return null;
        }
        
        // Enforce security policy - don't allow self-deactivation
        if (!isActive && userEntity.Id == _currentUserService.GetCurrentUserId())
        {
            var errorMessage = _localizationService.GetLocalizedString(
                UserLocalizationKeys.CannotDeactivateSelf
            );
            
            _logger.LogWarning("Attempt to deactivate own account: {UserId}", id);
            throw new InvalidOperationException(errorMessage);
        }
        
        // Update status
        userEntity.IsActive = isActive;
        userEntity.UpdatedAt = DateTime.UtcNow;
        userEntity.UpdatedById = _currentUserService.GetCurrentUserId();
        
        // Save changes
        var updatedUser = await _userRepository.UpdateAsync(userEntity);
        
        _logger.LogInformation(
            "User status changed successfully. User ID: {UserId}, New status: {Status}",
            updatedUser.Id,
            isActive ? "Active" : "Inactive");
            
        return _mapper.Map<UserDto>(updatedUser);
    }
    
    /// <inheritdoc />
    public async Task<UserStatisticsDto> GetUserStatisticsAsync(bool includeInactive = false)
    {
        _logger.LogInformation("Getting user statistics. Include inactive: {IncludeInactive}", includeInactive);
        
        var stats = await _userRepository.GetUserStatisticsAsync(includeInactive);
        return _mapper.Map<UserStatisticsDto>(stats);
    }
    
    /// <inheritdoc />
    public async Task<UserDto?> SetUserPreferredLanguageAsync(int id, string languageCode)
    {
        _logger.LogInformation(
            "Setting preferred language for user with ID: {UserId} to {Language}", 
            id, 
            languageCode);
        
        // Validate language code
        if (languageCode != "en" && languageCode != "ar")
        {
            var errorMessage = _localizationService.GetLocalizedString(
                UserLocalizationKeys.UnsupportedLanguage, 
                new { Language = languageCode }
            );
            
            _logger.LogWarning("Unsupported language code: {Language}", languageCode);
            throw new ArgumentException(errorMessage);
        }
        
        // Get existing user
        var userEntity = await _userRepository.GetByIdAsync(id);
        if (userEntity == null)
        {
            _logger.LogWarning("User not found with ID: {UserId}", id);
            return null;
        }
        
        // Update language
        userEntity.PreferredLanguage = languageCode;
        userEntity.UpdatedAt = DateTime.UtcNow;
        userEntity.UpdatedById = _currentUserService.GetCurrentUserId();
        
        // Save changes
        var updatedUser = await _userRepository.UpdateAsync(userEntity);
        
        _logger.LogInformation(
            "User preferred language updated successfully. User ID: {UserId}, Language: {Language}",
            updatedUser.Id,
            languageCode);
            
        return _mapper.Map<UserDto>(updatedUser);
    }
    
    #region Private Methods
    
    /// <summary>
    /// Validates user creation request
    /// </summary>
    private void ValidateUserRequest(CreateUserRequest request)
    {
        // Name validation
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException(_localizationService.GetLocalizedString(UserLocalizationKeys.NameRequired));
        }
        
        // Email validation
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ArgumentException(_localizationService.GetLocalizedString(UserLocalizationKeys.EmailRequired));
        }
        
        // Use regex for more thorough email validation
        if (!IsValidEmail(request.Email))
        {
            throw new ArgumentException(_localizationService.GetLocalizedString(UserLocalizationKeys.InvalidEmailFormat));
        }
        
        // Password validation
        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException(_localizationService.GetLocalizedString(UserLocalizationKeys.PasswordRequired));
        }
        
        if (request.Password.Length < 8)
        {
            throw new ArgumentException(_localizationService.GetLocalizedString(UserLocalizationKeys.PasswordTooShort));
        }
        
        if (!IsPasswordComplex(request.Password))
        {
            throw new ArgumentException(_localizationService.GetLocalizedString(UserLocalizationKeys.PasswordNotComplex));
        }
        
        // Password confirmation validation
        if (request.Password != request.ConfirmPassword)
        {
            throw new ArgumentException(_localizationService.GetLocalizedString(UserLocalizationKeys.PasswordsDoNotMatch));
        }
        
        // Phone validation if provided
        if (request.PhoneNumber != null && !IsValidPhoneNumber(request.PhoneNumber))
        {
            throw new ArgumentException(_localizationService.GetLocalizedString(UserLocalizationKeys.InvalidPhoneFormat));
        }
        
        // Role validation
        if (string.IsNullOrWhiteSpace(request.Role))
        {
            throw new ArgumentException(_localizationService.GetLocalizedString(UserLocalizationKeys.RoleRequired));
        }
    }
    
    /// <summary>
    /// Validates user update request
    /// </summary>
    private void ValidateUserUpdateRequest(UpdateUserRequest request)
    {
        // Name validation
        if (request.Name != null && string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException(_localizationService.GetLocalizedString(UserLocalizationKeys.NameRequired));
        }
        
        // Email validation
        if (request.Email != null)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                throw new ArgumentException(_localizationService.GetLocalizedString(UserLocalizationKeys.EmailRequired));
            }
            
            if (!IsValidEmail(request.Email))
            {
                throw new ArgumentException(_localizationService.GetLocalizedString(UserLocalizationKeys.InvalidEmailFormat));
            }
        }
        
        // Phone validation if provided
        if (request.PhoneNumber != null && !string.IsNullOrWhiteSpace(request.PhoneNumber) && !IsValidPhoneNumber(request.PhoneNumber))
        {
            throw new ArgumentException(_localizationService.GetLocalizedString(UserLocalizationKeys.InvalidPhoneFormat));
        }
        
        // Language validation if provided
        if (request.PreferredLanguage != null && request.PreferredLanguage != "en" && request.PreferredLanguage != "ar")
        {
            throw new ArgumentException(_localizationService.GetLocalizedString(
                UserLocalizationKeys.UnsupportedLanguage, 
                new { Language = request.PreferredLanguage }
            ));
        }
    }
    
    /// <summary>
    /// Checks if email format is valid
    /// </summary>
    private bool IsValidEmail(string email)
    {
        try
        {
            // Simple regex for basic email validation
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(email);
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Checks if password is complex enough (has uppercase, lowercase, digit, and special character)
    /// </summary>
    private bool IsPasswordComplex(string password)
    {
        var hasUppercase = false;
        var hasLowercase = false;
        var hasDigit = false;
        var hasSpecialChar = false;
        
        foreach (var c in password)
        {
            if (char.IsUpper(c)) hasUppercase = true;
            else if (char.IsLower(c)) hasLowercase = true;
            else if (char.IsDigit(c)) hasDigit = true;
            else hasSpecialChar = true;
            
            // If all requirements are met, return true
            if (hasUppercase && hasLowercase && hasDigit && hasSpecialChar)
            {
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Checks if phone number format is valid
    /// </summary>
    private bool IsValidPhoneNumber(string phoneNumber)
    {
        try
        {
            // Basic phone validation allowing for international format
            var regex = new Regex(@"^\+?[0-9\s\-\(\)]{8,20}$");
            return regex.IsMatch(phoneNumber);
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Enforces security policies for user creation
    /// </summary>
    private async Task EnforceUserCreationSecurityPolicies(CreateUserRequest request)
    {
        var currentUserRole = _currentUserService.GetCurrentUserRole();
        
        // Only admins can create admin users
        if (request.Role.ToLower() == "admin" && currentUserRole.ToLower() != "admin")
        {
            var errorMessage = _localizationService.GetLocalizedString(
                UserLocalizationKeys.OnlyAdminsCanCreateAdminUsers
            );
            
            _logger.LogWarning(
                "Unauthorized attempt to create admin user by user with role: {Role}", 
                currentUserRole);
                
            throw new UnauthorizedAccessException(errorMessage);
        }
        
        // Additional security policies can be added here
    }
    
    /// <summary>
    /// Enforces security policies for user updates
    /// </summary>
    private async Task EnforceUserUpdateSecurityPolicies(UserEntity user, UpdateUserRequest request)
    {
        var currentUserId = _currentUserService.GetCurrentUserId();
        var currentUserRole = _currentUserService.GetCurrentUserRole();
        
        // Only admins can change user roles
        if (request.Role != null && 
            request.Role != user.Role && 
            currentUserRole.ToLower() != "admin")
        {
            var errorMessage = _localizationService.GetLocalizedString(
                UserLocalizationKeys.OnlyAdminsCanChangeRoles
            );
            
            _logger.LogWarning(
                "Unauthorized attempt to change user role by user with role: {Role}", 
                currentUserRole);
                
            throw new UnauthorizedAccessException(errorMessage);
        }
        
        // Users can't demote themselves from admin
        if (currentUserId == user.Id && 
            request.Role != null && 
            user.Role.ToLower() == "admin" &&
            request.Role.ToLower() != "admin")
        {
            var errorMessage = _localizationService.GetLocalizedString(
                UserLocalizationKeys.CannotDemoteSelf
            );
            
            _logger.LogWarning("User attempted to demote themselves from admin: {UserId}", currentUserId);
            throw new InvalidOperationException(errorMessage);
        }
        
        // Additional security policies can be added here
    }
    
    /// <summary>
    /// Enforces security policies for user deletion
    /// </summary>
    private async Task EnforceUserDeletionSecurityPolicies(int userId)
    {
        var currentUserId = _currentUserService.GetCurrentUserId();
        var currentUserRole = _currentUserService.GetCurrentUserRole();
        
        // Check if trying to delete self
        if (userId == currentUserId)
        {
            var errorMessage = _localizationService.GetLocalizedString(
                UserLocalizationKeys.CannotDeleteSelf
            );
            
            _logger.LogWarning("User attempted to delete themselves: {UserId}", currentUserId);
            throw new InvalidOperationException(errorMessage);
        }
        
        // Only admins can delete users
        if (currentUserRole.ToLower() != "admin")
        {
            var errorMessage = _localizationService.GetLocalizedString(
                UserLocalizationKeys.OnlyAdminsCanDeleteUsers
            );
            
            _logger.LogWarning(
                "Unauthorized attempt to delete user by user with role: {Role}", 
                currentUserRole);
                
            throw new UnauthorizedAccessException(errorMessage);
        }
        
        // Check if trying to delete the last admin
        var userToDelete = await _userRepository.GetByIdAsync(userId);
        if (userToDelete != null && 
            userToDelete.Role.ToLower() == "admin")
        {
            // Count active admins
            var adminCount = await _userRepository.CountUsersByRoleAsync("admin", true);
            if (adminCount <= 1)
            {
                var errorMessage = _localizationService.GetLocalizedString(
                    UserLocalizationKeys.CannotDeleteLastAdmin
                );
                
                _logger.LogWarning("Attempt to delete the last admin user: {UserId}", userId);
                throw new InvalidOperationException(errorMessage);
            }
        }
    }
    #endregion
}