---
description: "Backend architecture guidelines with clean architecture patterns and repository design"
applyTo: "**/*.cs,**/Services/**,**/Repositories/**"
---

# Backend Architecture Guidelines

## Project Structure (Folder-per-Feature)
```
backend/
├── src/
│   ├── Features/
│   │   ├── Users/
│   │   │   ├── Controllers/
│   │   │   │   └── UsersController.cs
│   │   │   ├── Services/
│   │   │   │   ├── IUserService.cs
│   │   │   │   └── UserService.cs
│   │   │   ├── Models/
│   │   │   │   ├── UserDto.cs
│   │   │   │   └── UserEntity.cs
│   │   │   ├── Repositories/
│   │   │   │   ├── IUserRepository.cs
│   │   │   │   └── UserRepository.cs
│   │   │   ├── Commands/
│   │   │   │   ├── CreateUserCommand.cs
│   │   │   │   └── UpdateUserCommand.cs
│   │   │   ├── Queries/
│   │   │   │   ├── GetUserQuery.cs
│   │   │   │   └── GetUsersQuery.cs
│   │   │   ├── Validators/
│   │   │   │   └── UserValidator.cs
│   │   │   └── Mappings/
│   │   │       └── UserMappingProfile.cs
│   │   └── Products/
│   │       └── [same structure as Users]
│   ├── Shared/
│   │   ├── Common/
│   │   │   ├── BaseEntity.cs
│   │   │   ├── BaseResponse.cs
│   │   │   └── PaginatedResult.cs
│   │   ├── Middleware/
│   │   │   ├── ExceptionMiddleware.cs
│   │   │   └── LoggingMiddleware.cs
│   │   ├── Services/
│   │   │   ├── ICurrentUserService.cs
│   │   │   └── CurrentUserService.cs
│   │   └── Extensions/
│   │       └── ServiceCollectionExtensions.cs
│   ├── Infrastructure/
│   │   ├── Data/
│   │   │   ├── DbConnectionFactory.cs
│   │   │   └── IDbConnectionFactory.cs
│   │   ├── Identity/
│   │   │   └── IdentityService.cs
│   │   └── Repositories/
│   │       └── BaseRepository.cs
│   └── Program.cs
├── tests/
│   ├── UnitTests/
│   └── IntegrationTests/
└── Ikhtibar.Backend.csproj
```

## Code Templates and Patterns

### Controller Template
```csharp
[ApiController]
[Route("api/[controller]")]
public class {Feature}Controller : ControllerBase
{
    private readonly I{Feature}Service _service;
    private readonly ILogger<{Feature}Controller> _logger;

    public {Feature}Controller(I{Feature}Service service, ILogger<{Feature}Controller> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<{Feature}Dto>>> GetAll()
    {
        try
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all {feature}s");
            return StatusCode(500, "Internal server error");
        }
    }
}
```

### Service Template
```csharp
public interface I{Feature}Service
{
    Task<IEnumerable<{Feature}Dto>> GetAllAsync();
    Task<{Feature}Dto?> GetByIdAsync(int id);
    Task<{Feature}Dto> CreateAsync(Create{Feature}Dto dto);
    Task<{Feature}Dto> UpdateAsync(int id, Update{Feature}Dto dto);
    Task DeleteAsync(int id);
}

public class {Feature}Service : I{Feature}Service
{
    private readonly I{Feature}Repository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<{Feature}Service> _logger;

    public {Feature}Service(I{Feature}Repository repository, IMapper mapper, ILogger<{Feature}Service> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<{Feature}Dto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<{Feature}Dto>>(entities);
    }
}
```

### Repository Template
```csharp
public interface I{Feature}Repository
{
    Task<IEnumerable<{Feature}Entity>> GetAllAsync();
    Task<{Feature}Entity?> GetByIdAsync(int id);
    Task<{Feature}Entity> CreateAsync({Feature}Entity entity);
    Task<{Feature}Entity> UpdateAsync({Feature}Entity entity);
    Task DeleteAsync(int id);
}

public class {Feature}Repository : BaseRepository<{Feature}Entity>, I{Feature}Repository
{
    public {Feature}Repository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public async Task<IEnumerable<{Feature}Entity>> GetAllAsync()
    {
        return await base.GetAllAsync();
    }
}
```

## Dependency Injection Configuration
```csharp
// In Program.cs or ServiceCollectionExtensions.cs
services.AddScoped<I{Feature}Service, {Feature}Service>();
services.AddScoped<I{Feature}Repository, {Feature}Repository>();
```

## Error Handling Guidelines
- Always wrap controller actions in try-catch blocks
- Use custom exception types for business logic errors
- Implement global exception middleware
- Return appropriate HTTP status codes
- Log all exceptions with context

## Database Guidelines
- Use Dapper for data access with repository pattern
- Implement proper database schema management using SQL scripts
- Use async methods for all database operations
- Implement proper indexing for performance
- Use transactions for complex operations

## 🎯 Single Responsibility Principle (SRP) Guidelines

### Core SRP Rules
Every class, method, and module should have **one reason to change**. This means:
- One responsibility per class
- One concern per method
- Clear separation of business logic, data access, and presentation

### Repository Layer SRP Rules

#### ✅ DO: Keep repositories focused on data access only
```csharp
// ✅ GOOD: UserRepository focuses only on User data operations
public class UserRepository : IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}

// ✅ GOOD: Separate repository for user-role relationships
public class UserRoleRepository : IUserRoleRepository
{
    Task<bool> AssignRoleAsync(Guid userId, Guid roleId);
    Task<bool> RemoveRoleAsync(Guid userId, Guid roleId);
    Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId);
}
```

#### ❌ DON'T: Mix different concerns in repositories
```csharp
// ❌ BAD: UserRepository handling user-role operations
public class UserRepository : IUserRepository
{
    // User operations - correct responsibility
    Task<User?> GetByIdAsync(Guid id);
    
    // ❌ Role operations - wrong repository!
    Task<bool> AssignRoleAsync(Guid userId, Guid roleId);
    Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId);
    
    // ❌ Authentication - wrong repository!
    Task<bool> ValidatePasswordAsync(string username, string password);
    
    // ❌ Email operations - wrong repository!
    Task<bool> SendWelcomeEmailAsync(User user);
}
```

### Service Layer SRP Rules

#### ✅ DO: Create focused service classes
```csharp
// ✅ GOOD: UserService handles user business logic only
public class UserService : IUserService
{
    Task<UserDto> CreateUserAsync(CreateUserDto dto);
    Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto dto);
    Task<bool> DeactivateUserAsync(Guid id);
}

// ✅ GOOD: Separate service for authentication
public class AuthenticationService : IAuthenticationService
{
    Task<LoginResult> LoginAsync(LoginDto dto);
    Task<bool> ValidateTokenAsync(string token);
    Task LogoutAsync(string token);
}

// ✅ GOOD: Separate service for notifications
public class NotificationService : INotificationService
{
    Task SendWelcomeEmailAsync(User user);
    Task SendPasswordResetEmailAsync(User user);
}
```

#### ❌ DON'T: Create god services
```csharp
// ❌ BAD: UserService trying to do everything
public class UserService : IUserService
{
    // User management - correct responsibility
    Task<UserDto> CreateUserAsync(CreateUserDto dto);
    
    // ❌ Authentication - should be AuthenticationService
    Task<LoginResult> LoginAsync(LoginDto dto);
    
    // ❌ Email - should be NotificationService
    Task SendWelcomeEmailAsync(User user);
    
    // ❌ File handling - should be FileService
    Task<string> UploadAvatarAsync(IFormFile file);
    
    // ❌ Reporting - should be ReportingService
    Task<UserReportDto> GenerateUserReportAsync();
}
```

### Controller SRP Rules

#### ✅ DO: Keep controllers thin and focused
```csharp
// ✅ GOOD: Controller only handles HTTP concerns
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto dto)
    {
        // Only HTTP validation and response handling
        var user = await _userService.CreateUserAsync(dto);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }
}
```

#### ❌ DON'T: Put business logic in controllers
```csharp
// ❌ BAD: Controller with business logic
[ApiController]
public class UsersController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto dto)
    {
        // ❌ Business validation - should be in service
        if (dto.Age < 18)
            return BadRequest("User must be 18 or older");
            
        // ❌ Data access - should be in repository
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (existingUser != null)
            return Conflict("Email already exists");
            
        // ❌ Complex business logic - should be in service
        var user = new User
        {
            // ... complex mapping logic
        };
        
        // ❌ Direct database operations - should be in repository
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        return Ok(user);
    }
}
```

### Method-Level SRP Rules

#### ✅ DO: Keep methods focused on one task
```csharp
// ✅ GOOD: Each method has one clear purpose
public class UserService
{
    public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
    {
        // Only handles user creation logic
        await ValidateUserCreationAsync(dto);
        var user = _mapper.Map<User>(dto);
        var createdUser = await _userRepository.CreateAsync(user);
        return _mapper.Map<UserDto>(createdUser);
    }
    
    private async Task ValidateUserCreationAsync(CreateUserDto dto)
    {
        // Only handles validation logic
        if (await _userRepository.EmailExistsAsync(dto.Email))
            throw new ValidationException("Email already exists");
    }
}
```

#### ❌ DON'T: Create methods that do multiple things
```csharp
// ❌ BAD: Method doing too many things
public async Task<UserDto> CreateUserAndSendWelcomeEmailAndLogActivity(CreateUserDto dto)
{
    // ❌ Multiple responsibilities in one method
    
    // 1. Validation
    if (await _userRepository.EmailExistsAsync(dto.Email))
        throw new ValidationException("Email already exists");
    
    // 2. User creation
    var user = _mapper.Map<User>(dto);
    var createdUser = await _userRepository.CreateAsync(user);
    
    // 3. Email sending
    var emailContent = GenerateWelcomeEmail(user);
    await _emailService.SendAsync(emailContent);
    
    // 4. Activity logging
    await _activityLogger.LogAsync($"User {user.Email} created");
    
    // 5. Cache invalidation
    await _cache.RemoveAsync("users-list");
    
    return _mapper.Map<UserDto>(createdUser);
}
```

### Class Design SRP Rules

#### ✅ DO: Design classes with single purpose
```csharp
// ✅ GOOD: Focused classes
public class PasswordHasher
{
    public string HashPassword(string password) { }
    public bool VerifyPassword(string password, string hash) { }
}

public class EmailValidator
{
    public bool IsValid(string email) { }
    public string GetDomain(string email) { }
}

public class UserCreationValidator
{
    public ValidationResult Validate(CreateUserDto dto) { }
}
```

#### ❌ DON'T: Create utility classes that do everything
```csharp
// ❌ BAD: God class with multiple responsibilities
public class UserHelper
{
    // Password operations
    public string HashPassword(string password) { }
    
    // Email operations
    public bool IsValidEmail(string email) { }
    public void SendEmail(string to, string subject, string body) { }
    
    // File operations
    public string SaveAvatar(IFormFile file) { }
    
    // Validation operations
    public bool ValidateUser(User user) { }
    
    // Logging operations
    public void LogUserActivity(string activity) { }
    
    // Cache operations
    public void ClearUserCache() { }
}
```

### Interface Segregation with SRP

#### ✅ DO: Create focused interfaces
```csharp
// ✅ GOOD: Specific interfaces for specific responsibilities
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<bool> DeleteAsync(Guid id);
}

public interface IUserRoleRepository
{
    Task<bool> AssignRoleAsync(Guid userId, Guid roleId);
    Task<bool> RemoveRoleAsync(Guid userId, Guid roleId);
    Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId);
}

public interface IUserPermissionRepository
{
    Task<IEnumerable<Permission>> GetUserPermissionsAsync(Guid userId);
    Task<bool> HasPermissionAsync(Guid userId, string permission);
}
```

#### ❌ DON'T: Create fat interfaces
```csharp
// ❌ BAD: Interface with too many responsibilities
public interface IUserRepository
{
    // User CRUD - correct
    Task<User?> GetByIdAsync(Guid id);
    Task<User> CreateAsync(User user);
    
    // ❌ Role management - should be separate interface
    Task<bool> AssignRoleAsync(Guid userId, Guid roleId);
    Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId);
    
    // ❌ Permission management - should be separate interface
    Task<IEnumerable<Permission>> GetUserPermissionsAsync(Guid userId);
    
    // ❌ Authentication - should be separate interface
    Task<bool> ValidatePasswordAsync(string username, string password);
    
    // ❌ Statistics - should be separate interface
    Task<UserStatistics> GetUserStatisticsAsync();
}
```

### SRP Violation Detection

#### Warning Signs of SRP Violations:
1. **Class names with "And"**: `UserAndRoleService`, `CreateAndValidateUser`
2. **Large classes**: >300 lines usually indicate multiple responsibilities
3. **Multiple reasons to change**: If changing email logic affects user creation
4. **God objects**: Classes that import many unrelated dependencies
5. **Mixed abstraction levels**: Low-level DB operations mixed with high-level business logic

#### SRP Refactoring Checklist:
- [ ] Each class has one primary responsibility
- [ ] Methods are focused on single tasks
- [ ] Interfaces are specific and cohesive
- [ ] Dependencies are minimal and related
- [ ] Business logic is separated from infrastructure concerns
- [ ] Data access is separated from business logic
- [ ] HTTP concerns are separated from business logic

### SRP Benefits in Our Architecture:
1. **Easier Testing**: Mock specific interfaces, test single responsibilities
2. **Better Maintainability**: Changes affect minimal code surface
3. **Improved Reusability**: Focused classes can be reused in different contexts
4. **Clearer Code**: Purpose is immediately obvious
5. **Reduced Coupling**: Classes depend only on what they need
6. **Simplified Debugging**: Issues isolated to specific responsibilities

## 🚨 SRP Enforcement Rules (MANDATORY)

### ⚠️ Critical SRP Violations to Fix Immediately
The following are **MANDATORY** fixes that must be applied to existing code:

#### 1. Remove User-Role Operations from IUserRepository
```csharp
// ❌ VIOLATION: These methods belong in IUserRoleRepository
public interface IUserRepository
{
    Task<bool> AssignRoleAsync(Guid userId, Guid roleId, Guid assignedBy);     // ❌ MOVE to IUserRoleRepository
    Task<bool> RemoveRoleAsync(Guid userId, Guid roleId);                     // ❌ MOVE to IUserRoleRepository
    Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId);                   // ❌ MOVE to IUserRoleRepository
}
```

#### 2. Correct Interface Segregation
```csharp
// ✅ CORRECT: Clean separation of concerns
public interface IUserRepository
{
    // ONLY User entity operations
    Task<User?> GetByIdAsync(Guid id);
    Task<User> CreateAsync(User user);
    Task<User?> UpdateAsync(User user);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> UserExistsAsync(Guid id);
    Task<bool> UsernameExistsAsync(string username, Guid? excludeUserId = null);
    Task<bool> EmailExistsAsync(string email, Guid? excludeUserId = null);
    // Statistics and filtering methods for Users only
}

public interface IUserRoleRepository
{
    // ONLY User-Role relationship operations
    Task<bool> AssignRoleAsync(Guid userId, Guid roleId, Guid assignedBy);
    Task<bool> RemoveRoleAsync(Guid userId, Guid roleId);
    Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId);
    Task<IEnumerable<User>> GetRoleUsersAsync(Guid roleId);
    Task<bool> HasRoleAsync(Guid userId, Guid roleId);
}

public interface IUserPermissionService
{
    // ONLY User-Permission business logic
    Task<IEnumerable<Permission>> GetUserPermissionsAsync(Guid userId);
    Task<bool> HasPermissionAsync(Guid userId, string resource, string action);
}
```

### 🎯 SRP Validation Checklist (Apply to ALL Code)
Before any code is considered complete, verify:

#### Class-Level SRP Validation:
- [ ] **One Sentence Test**: Can you describe the class purpose without using "and"?
- [ ] **Reason to Change Test**: Would changes to authentication affect user creation?
- [ ] **Dependency Relevance**: Are all injected dependencies directly related to the class purpose?
- [ ] **Method Cohesion**: Do all methods operate on the same domain concept?
- [ ] **Abstraction Level**: Are all methods at the same level of abstraction?

#### Method-Level SRP Validation:
- [ ] **Single Action**: Does the method perform exactly one business action?
- [ ] **Command/Query Separation**: Does the method either change state OR return data, not both?
- [ ] **Parameter Cohesion**: Are all parameters related to the same operation?
- [ ] **Side Effects**: Does the method have any unexpected side effects?

#### Interface-Level SRP Validation:
- [ ] **Cohesive Methods**: Do all interface methods relate to the same responsibility?
- [ ] **Client Segmentation**: Would different clients use different subsets of methods?
- [ ] **Implementation Burden**: Would implementers need unrelated dependencies?

### 🔧 SRP Refactoring Patterns

#### Pattern 1: Extract Service Classes
```csharp
// ❌ BEFORE: God service
public class UserService
{
    Task<User> CreateUserAsync(CreateUserDto dto);
    Task<bool> AuthenticateAsync(string username, string password);  // ❌ Auth responsibility
    Task SendWelcomeEmailAsync(User user);                          // ❌ Email responsibility
    Task<byte[]> GenerateReportAsync();                             // ❌ Reporting responsibility
}

// ✅ AFTER: Focused services
public class UserService
{
    Task<User> CreateUserAsync(CreateUserDto dto);
    Task<User> UpdateUserAsync(Guid id, UpdateUserDto dto);
    Task<bool> DeleteUserAsync(Guid id);
}

public class AuthenticationService
{
    Task<AuthResult> AuthenticateAsync(string username, string password);
    Task<bool> ValidateTokenAsync(string token);
}

public class NotificationService
{
    Task SendWelcomeEmailAsync(User user);
    Task SendPasswordResetEmailAsync(User user);
}
```

#### Pattern 2: Extract Repository Classes
```csharp
// ❌ BEFORE: Mixed repository
public class UserRepository
{
    Task<User> CreateAsync(User user);
    Task<bool> AssignRoleAsync(Guid userId, Guid roleId);  // ❌ Role responsibility
    Task<bool> HasPermissionAsync(Guid userId, string permission);  // ❌ Permission responsibility
}

// ✅ AFTER: Separated repositories
public class UserRepository
{
    Task<User> CreateAsync(User user);
    Task<User?> GetByIdAsync(Guid id);
    Task<bool> DeleteAsync(Guid id);
}

public class UserRoleRepository
{
    Task<bool> AssignRoleAsync(Guid userId, Guid roleId);
    Task<bool> RemoveRoleAsync(Guid userId, Guid roleId);
    Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId);
}
```

#### Pattern 3: Extract Helper Classes
```csharp
// ❌ BEFORE: Mixed concerns
public class UserService
{
    public async Task<User> CreateUserAsync(CreateUserDto dto)
    {
        // Validation logic
        if (string.IsNullOrEmpty(dto.Email)) throw new ValidationException("Email required");
        if (!IsValidEmail(dto.Email)) throw new ValidationException("Invalid email format");
        
        // Password hashing
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        
        // User creation
        var user = new User { Email = dto.Email, PasswordHash = hashedPassword };
        return await _repository.CreateAsync(user);
    }
}

// ✅ AFTER: Extracted helpers
public class UserService
{
    private readonly IUserValidator _validator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRepository _repository;
    
    public async Task<User> CreateUserAsync(CreateUserDto dto)
    {
        _validator.ValidateCreateUser(dto);
        var hashedPassword = _passwordHasher.HashPassword(dto.Password);
        
        var user = new User { Email = dto.Email, PasswordHash = hashedPassword };
        return await _repository.CreateAsync(user);
    }
}

public class UserValidator
{
    public void ValidateCreateUser(CreateUserDto dto)
    {
        if (string.IsNullOrEmpty(dto.Email)) throw new ValidationException("Email required");
        if (!IsValidEmail(dto.Email)) throw new ValidationException("Invalid email format");
    }
}

public class PasswordHasher
{
    public string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    public bool VerifyPassword(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}
```

### 🚀 Dapper ORM Best Practices

Ikhtibar uses Dapper as a micro-ORM for efficient database access. The following guidelines ensure consistent and efficient database operations.

### Repository Structure with Dapper

```csharp
// Base repository pattern with Dapper
public abstract class BaseRepository<T> where T : BaseEntity
{
    protected readonly IDbConnection _connection;
    protected readonly ILogger<BaseRepository<T>> _logger;
    protected readonly string _tableName;

    protected BaseRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<BaseRepository<T>> logger,
        string tableName)
    {
        _connection = connectionFactory.CreateConnection();
        _logger = logger;
        _tableName = tableName;
    }

    // Common operations that can be reused across repositories
    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        try
        {
            const string sql = "SELECT * FROM @TableName WHERE Id = @Id";
            
            return await _connection.QueryFirstOrDefaultAsync<T>(
                sql.Replace("@TableName", _tableName),
                new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving {EntityType} with ID {Id}", typeof(T).Name, id);
            throw new RepositoryException($"Failed to retrieve {typeof(T).Name}", ex);
        }
    }
    
    // Additional common operations...
}

// Feature-specific repository implementation
public class UserRepository : BaseRepository<UserEntity>, IUserRepository
{
    public UserRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<UserRepository> logger)
        : base(connectionFactory, logger, "Users")
    {
    }

    // Feature-specific operations that require custom SQL
    public async Task<IEnumerable<UserEntity>> GetUsersByRoleAsync(string roleName)
    {
        try
        {
            const string sql = @"
                SELECT u.*
                FROM Users u
                INNER JOIN UserRoles ur ON u.Id = ur.UserId
                INNER JOIN Roles r ON ur.RoleId = r.Id
                WHERE r.Name = @RoleName";
            
            return await _connection.QueryAsync<UserEntity>(sql, new { RoleName = roleName });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users with role {RoleName}", roleName);
            throw new RepositoryException($"Failed to retrieve users with role {roleName}", ex);
        }
    }
}
```

### Dapper Query Best Practices

#### ✅ DO: Use parameterized queries for all SQL operations

```csharp
// ✅ GOOD: Use parameters to prevent SQL injection
public async Task<UserEntity> GetByEmailAsync(string email)
{
    const string sql = "SELECT * FROM Users WHERE Email = @Email";
    return await _connection.QuerySingleOrDefaultAsync<UserEntity>(sql, new { Email = email });
}
```

#### ❌ DON'T: Use string concatenation for SQL queries

```csharp
// ❌ BAD: SQL injection vulnerability!
public async Task<UserEntity> GetByEmailAsync(string email)
{
    // NEVER DO THIS - SQL injection risk
    string sql = $"SELECT * FROM Users WHERE Email = '{email}'"; 
    return await _connection.QuerySingleOrDefaultAsync<UserEntity>(sql);
}
```

#### ✅ DO: Use multi-mapping for joins

```csharp
// ✅ GOOD: Efficient multi-mapping for related data
public async Task<IEnumerable<User>> GetUsersWithRolesAsync()
{
    const string sql = @"
        SELECT u.*, r.*
        FROM Users u
        LEFT JOIN UserRoles ur ON u.Id = ur.UserId
        LEFT JOIN Roles r ON ur.RoleId = r.Id";
    
    var userDictionary = new Dictionary<Guid, User>();
    
    await _connection.QueryAsync<User, Role, User>(
        sql,
        (user, role) => {
            if (!userDictionary.TryGetValue(user.Id, out var existingUser))
            {
                existingUser = user;
                existingUser.Roles = new List<Role>();
                userDictionary.Add(user.Id, existingUser);
            }
            
            if (role != null)
            {
                existingUser.Roles.Add(role);
            }
            
            return existingUser;
        },
        splitOn: "Id");
    
    return userDictionary.Values;
}
```

#### ✅ DO: Use dynamic parameters for complex filtering

```csharp
// ✅ GOOD: Using DynamicParameters for flexible filtering
public async Task<IEnumerable<UserEntity>> SearchUsersAsync(UserSearchCriteria criteria)
{
    var parameters = new DynamicParameters();
    var conditions = new List<string>();
    
    if (!string.IsNullOrEmpty(criteria.Name))
    {
        conditions.Add("(FirstName LIKE @Name OR LastName LIKE @Name)");
        parameters.Add("@Name", $"%{criteria.Name}%");
    }
    
    if (!string.IsNullOrEmpty(criteria.Email))
    {
        conditions.Add("Email LIKE @Email");
        parameters.Add("@Email", $"%{criteria.Email}%");
    }
    
    if (criteria.Status.HasValue)
    {
        conditions.Add("Status = @Status");
        parameters.Add("@Status", criteria.Status.Value);
    }
    
    if (criteria.FromDate.HasValue)
    {
        conditions.Add("CreatedAt >= @FromDate");
        parameters.Add("@FromDate", criteria.FromDate.Value);
    }
    
    if (criteria.ToDate.HasValue)
    {
        conditions.Add("CreatedAt <= @ToDate");
        parameters.Add("@ToDate", criteria.ToDate.Value);
    }
    
    string whereClause = conditions.Any() ? "WHERE " + string.Join(" AND ", conditions) : "";
    string orderClause = !string.IsNullOrEmpty(criteria.SortBy) 
        ? $"ORDER BY {criteria.SortBy} {(criteria.SortDescending ? "DESC" : "ASC")}" 
        : "ORDER BY CreatedAt DESC";
    
    string sql = $@"
        SELECT * FROM Users
        {whereClause}
        {orderClause}
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY";
    
    parameters.Add("@Offset", (criteria.Page - 1) * criteria.PageSize);
    parameters.Add("@PageSize", criteria.PageSize);
    
    return await _connection.QueryAsync<UserEntity>(sql, parameters);
}
```

#### ✅ DO: Use transactions for multi-statement operations

```csharp
// ✅ GOOD: Using transactions for operations that must succeed or fail as a unit
public async Task<bool> CreateUserWithRolesAsync(User user, IEnumerable<Guid> roleIds)
{
    using var transaction = _connection.BeginTransaction();
    try
    {
        // Insert the user
        const string insertUserSql = @"
            INSERT INTO Users (Id, FirstName, LastName, Email, PasswordHash, CreatedAt)
            VALUES (@Id, @FirstName, @LastName, @Email, @PasswordHash, @CreatedAt);";
            
        await _connection.ExecuteAsync(insertUserSql, user, transaction);
        
        // Assign roles to the user
        const string insertUserRoleSql = @"
            INSERT INTO UserRoles (UserId, RoleId)
            VALUES (@UserId, @RoleId);";
            
        foreach (var roleId in roleIds)
        {
            await _connection.ExecuteAsync(insertUserRoleSql, 
                new { UserId = user.Id, RoleId = roleId }, 
                transaction);
        }
        
        transaction.Commit();
        return true;
    }
    catch (Exception ex)
    {
        transaction.Rollback();
        _logger.LogError(ex, "Error creating user with roles: {User}", user.Email);
        throw new RepositoryException("Failed to create user with roles", ex);
    }
}
```

#### ✅ DO: Use stored procedures for complex operations

```csharp
// ✅ GOOD: Using stored procedures for complex database operations
public async Task<IEnumerable<UserActivity>> GetUserActivityReportAsync(DateTime startDate, DateTime endDate)
{
    try
    {
        return await _connection.QueryAsync<UserActivity>(
            "sp_GetUserActivityReport",
            new { StartDate = startDate, EndDate = endDate },
            commandType: CommandType.StoredProcedure);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error generating user activity report");
        throw new RepositoryException("Failed to generate user activity report", ex);
    }
}
```

### Performance Optimization

#### ✅ DO: Cache frequently accessed, rarely changing data

```csharp
// ✅ GOOD: Caching frequently used reference data
public class ReferenceDataRepository : BaseRepository<ReferenceData>, IReferenceDataRepository
{
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(24);
    
    public ReferenceDataRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<ReferenceDataRepository> logger,
        IMemoryCache cache)
        : base(connectionFactory, logger, "ReferenceData")
    {
        _cache = cache;
    }
    
    public async Task<IEnumerable<LookupItem>> GetLookupItemsAsync(string category)
    {
        string cacheKey = $"LookupItems_{category}";
        
        if (!_cache.TryGetValue(cacheKey, out IEnumerable<LookupItem> items))
        {
            const string sql = "SELECT Id, Name FROM LookupItems WHERE Category = @Category";
            items = await _connection.QueryAsync<LookupItem>(sql, new { Category = category });
            
            _cache.Set(cacheKey, items, _cacheDuration);
        }
        
        return items;
    }
}
```

#### ✅ DO: Use bulk operations for better performance

```csharp
// ✅ GOOD: Using bulk operations for better performance
public async Task<bool> BulkInsertUsersAsync(IEnumerable<UserEntity> users)
{
    if (!users.Any())
        return true;
        
    try
    {
        // Option 1: Using DataTable for SqlBulkCopy (for SQL Server)
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id", typeof(Guid));
        dataTable.Columns.Add("FirstName", typeof(string));
        dataTable.Columns.Add("LastName", typeof(string));
        dataTable.Columns.Add("Email", typeof(string));
        dataTable.Columns.Add("PasswordHash", typeof(string));
        dataTable.Columns.Add("CreatedAt", typeof(DateTime));
        
        foreach (var user in users)
        {
            dataTable.Rows.Add(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.PasswordHash,
                user.CreatedAt
            );
        }
        
        using (var bulkCopy = new SqlBulkCopy((SqlConnection)_connection))
        {
            bulkCopy.DestinationTableName = "Users";
            await bulkCopy.WriteToServerAsync(dataTable);
        }
        
        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error bulk inserting users");
        throw new RepositoryException("Failed to bulk insert users", ex);
    }
}

// Alternative approach for moderate batch sizes
public async Task<bool> BatchInsertUsersAsync(IEnumerable<UserEntity> users)
{
    if (!users.Any())
        return true;
        
    try
    {
        const string sql = @"
            INSERT INTO Users (Id, FirstName, LastName, Email, PasswordHash, CreatedAt)
            VALUES (@Id, @FirstName, @LastName, @Email, @PasswordHash, @CreatedAt)";
            
        await _connection.ExecuteAsync(sql, users);
        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error batch inserting users");
        throw new RepositoryException("Failed to batch insert users", ex);
    }
}
```

### Testing Dapper Repositories

```csharp
// Unit testing repositories with in-memory database
[TestFixture]
public class UserRepositoryTests
{
    private IDbConnection _connection;
    private UserRepository _repository;
    private ILogger<UserRepository> _logger;
    
    [SetUp]
    public void Setup()
    {
        // Use SQLite in-memory database for testing
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
        
        // Create test tables
        _connection.Execute(@"
            CREATE TABLE Users (
                Id TEXT PRIMARY KEY,
                FirstName TEXT NOT NULL,
                LastName TEXT NOT NULL,
                Email TEXT NOT NULL,
                PasswordHash TEXT NOT NULL,
                CreatedAt TEXT NOT NULL
            )");
            
        _logger = Substitute.For<ILogger<UserRepository>>();
        
        var connectionFactory = Substitute.For<IDbConnectionFactory>();
        connectionFactory.CreateConnection().Returns(_connection);
        
        _repository = new UserRepository(connectionFactory, _logger);
    }
    
    [Test]
    public async Task GetByIdAsync_Should_ReturnUser_When_UserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        await _connection.ExecuteAsync(
            "INSERT INTO Users (Id, FirstName, LastName, Email, PasswordHash, CreatedAt) " +
            "VALUES (@Id, @FirstName, @LastName, @Email, @PasswordHash, @CreatedAt)",
            new { 
                Id = userId, 
                FirstName = "John", 
                LastName = "Doe", 
                Email = "john.doe@example.com",
                PasswordHash = "hash",
                CreatedAt = DateTime.UtcNow
            });
            
        // Act
        var result = await _repository.GetByIdAsync(userId);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(userId));
        Assert.That(result.Email, Is.EqualTo("john.doe@example.com"));
    }
    
    [TearDown]
    public void TearDown()
    {
        _connection?.Dispose();
    }
}
```
