---
description: "REST API design guidelines with validation patterns and HTTP status codes"
applyTo: "**/*.cs,**/Controllers/**,**/DTOs/**"
---

# API Design Guidelines

## RESTful API Standards

### URL Structure
```
GET    /api/users              # Get all users
GET    /api/users/{id}         # Get user by ID
POST   /api/users              # Create new user
PUT    /api/users/{id}         # Update user
DELETE /api/users/{id}         # Delete user

# For nested resources
GET    /api/users/{id}/orders  # Get orders for user
POST   /api/users/{id}/orders  # Create order for user
```

### HTTP Status Codes
- **200 OK** - Successful GET, PUT
- **201 Created** - Successful POST
- **204 No Content** - Successful DELETE
- **400 Bad Request** - Invalid request data
- **401 Unauthorized** - Authentication required
- **403 Forbidden** - Access denied
- **404 Not Found** - Resource not found
- **409 Conflict** - Resource conflict
- **422 Unprocessable Entity** - Validation errors
- **500 Internal Server Error** - Server error

### Request/Response Format

#### Standard Response Wrapper
```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string Message { get; set; }
    public List<string> Errors { get; set; } = new();
    public int StatusCode { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
```

#### Pagination Response
```csharp
public class PaginatedResponse<T>
{
    public List<T> Data { get; set; }
    public int TotalRecords { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}
```

#### Error Response
```csharp
public class ErrorResponse
{
    public string Type { get; set; }
    public string Title { get; set; }
    public string Detail { get; set; }
    public string Instance { get; set; }
    public Dictionary<string, string[]> Errors { get; set; }
}
```

### DTO Naming Conventions
```csharp
// Request DTOs
public class CreateUserRequest { }
public class UpdateUserRequest { }
public class GetUsersRequest { }

// Response DTOs
public class UserResponse { }
public class UsersListResponse { }

// Domain DTOs
public class UserDto { }
public class UserListItemDto { }
```

## Authentication & Authorization

### JWT Token Structure
```csharp
public class JwtClaims
{
    public const string UserId = "userId";
    public const string Email = "email";
    public const string Role = "role";
    public const string Permissions = "permissions";
}
```

### Authorization Attributes
```csharp
[Authorize(Roles = "Admin")]
[Authorize(Policy = "RequireUserRole")]
[AllowAnonymous]
```

## Validation Guidelines

### FluentValidation Example
```csharp
public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Valid email is required");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters");
    }
}
```

### Data Annotations
```csharp
public class CreateUserRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Valid email is required")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters")]
    public string Password { get; set; }
}
```

## Swagger/OpenAPI Configuration

### Controller Documentation
```csharp
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ApiVersion("1.0")]
public class UsersController : ControllerBase
{
    /// <summary>
    /// Get all users with pagination
    /// </summary>
    /// <param name="request">Pagination and filter parameters</param>
    /// <returns>List of users</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginatedResponse<UserDto>>> GetUsers([FromQuery] GetUsersRequest request)
    {
        // Implementation
    }
}
```

## Error Handling Patterns

### Global Exception Middleware
```csharp
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            ValidationException => new ErrorResponse
            {
                Type = "ValidationError",
                Title = "Validation Failed",
                Detail = exception.Message,
                Instance = context.Request.Path
            },
            NotFoundException => new ErrorResponse
            {
                Type = "NotFound",
                Title = "Resource Not Found",
                Detail = exception.Message,
                Instance = context.Request.Path
            },
            _ => new ErrorResponse
            {
                Type = "ServerError",
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred",
                Instance = context.Request.Path
            }
        };

        context.Response.StatusCode = exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
```

## Logging Guidelines

### Structured Logging
```csharp
public class UserService
{
    private readonly ILogger<UserService> _logger;

    public async Task<UserDto> CreateUserAsync(CreateUserRequest request)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["UserId"] = request.Email,
            ["Operation"] = "CreateUser"
        });

        _logger.LogInformation("Creating new user with email {Email}", request.Email);

        try
        {
            // Implementation
            _logger.LogInformation("User created successfully with ID {UserId}", user.Id);
            return userDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create user with email {Email}", request.Email);
            throw;
        }
    }
}
```

## Performance Guidelines

### Caching Strategy
```csharp
public class UserService
{
    private readonly IMemoryCache _cache;
    private readonly IDistributedCache _distributedCache;

    public async Task<UserDto> GetUserAsync(int id)
    {
        var cacheKey = $"user_{id}";
        
        if (_cache.TryGetValue(cacheKey, out UserDto cachedUser))
        {
            return cachedUser;
        }

        var user = await _repository.GetByIdAsync(id);
        var userDto = _mapper.Map<UserDto>(user);

        _cache.Set(cacheKey, userDto, TimeSpan.FromMinutes(15));

        return userDto;
    }
}
```

### Database Optimization
```csharp
// Use async methods
public async Task<List<UserDto>> GetUsersAsync()
{
    return await _context.Users
        .Where(u => u.IsActive)
        .Select(u => new UserDto
        {
            Id = u.Id,
            Email = u.Email,
            Name = u.Name
        })
        .ToListAsync();
}

// Use projections to avoid loading unnecessary data
// Use Include() for related data
// Implement proper indexing
```

## Security Guidelines

### Input Sanitization
```csharp
public class SecurityHelper
{
    public static string SanitizeInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Remove potentially dangerous characters
        return Regex.Replace(input, @"[<>""'%;()&+]", "");
    }
}
```

### CORS Configuration
```csharp
services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:5173", "https://your-frontend-domain.com")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```
