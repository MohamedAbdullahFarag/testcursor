---
mode: agent
description: "Comprehensive code review focusing on quality, security, architecture, and best practices"
---

# General Code Review - Comprehensive Quality Analysis

## Purpose
This prompt performs a thorough code review of the current changes, specified files, or entire codebase, focusing on code quality, security, architecture adherence, and best practices following the Ikhtibar project's standards.

## Review Scope
The review can target:
- Staged/unstaged changes (`git diff --staged`)
- Specific files or directories
- Pull request changes
- Entire codebase comparison
- Feature branches

## Review Process

### Step 1: Change Analysis
```markdown
I'll first understand what needs to be reviewed:

**For Staged Changes:**
- Analyze `git diff --staged` output
- Review both new and modified files
- Check file additions and deletions

**For Specific Files:**
- Read specified files completely
- Analyze context and dependencies
- Check integration points

**For Pull Requests:**
- Review PR description and scope
- Analyze changed files and their relationships
- Check for breaking changes

**For Codebase Review:**
- Compare against main branch
- Focus on recent changes and patterns
- Identify architectural consistency
```

### Step 2: Comprehensive Analysis
```markdown
I'll perform multi-layered analysis covering:

**Technical Quality:**
- Code structure and organization
- Performance and optimization
- Error handling and resilience
- Testing coverage and quality

**Security Assessment:**
- Input validation and sanitization
- Authentication and authorization
- Data protection and privacy
- Vulnerability prevention

**Architecture Compliance:**
- Single Responsibility Principle adherence
- Folder-per-feature organization
- Layer separation and boundaries
- Integration pattern consistency
```

## Review Focus Areas

### 1. **Code Quality Standards**

#### Backend (C# / ASP.NET Core)
```markdown
✅ **Quality Checklist:**
- [ ] **Type Safety**: All methods have explicit return types
- [ ] **Error Handling**: Proper try-catch blocks with specific exceptions
- [ ] **Logging**: Structured logging with correlation IDs and scopes
- [ ] **Validation**: Input validation using data annotations
- [ ] **Documentation**: XML comments for public APIs
- [ ] **Async Patterns**: Proper async/await usage for I/O operations
- [ ] **Disposal**: IDisposable implementation where needed
- [ ] **Configuration**: No hardcoded values, use IOptions pattern

**Code Patterns to Enforce:**
```csharp
// ✅ Proper error handling
public async Task<UserDto> GetUserAsync(Guid id)
{
    using var scope = _logger.BeginScope("Getting user {UserId}", id);
    try
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found", id);
            throw new UserNotFoundException($"User with ID {id} not found");
        }
        
        return _mapper.Map<UserDto>(user);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error retrieving user {UserId}", id);
        throw;
    }
}

// ❌ Poor patterns to flag
public UserDto GetUser(Guid id)  // Missing async
{
    var user = _userRepository.GetById(id);  // Sync call
    return new UserDto { ... };  // Manual mapping
}
```
```

#### Frontend (TypeScript / React)
```markdown
✅ **Quality Checklist:**
- [ ] **TypeScript Strict**: Explicit typing, no `any` usage
- [ ] **Component Props**: Proper interface definitions
- [ ] **Error Boundaries**: Error handling for component failures
- [ ] **Performance**: Memoization with React.memo, useCallback, useMemo
- [ ] **Accessibility**: ARIA attributes and semantic HTML
- [ ] **Internationalization**: i18next integration for all text
- [ ] **State Management**: Proper hook usage and state lifting
- [ ] **Testing**: Component and hook tests with good coverage

**Code Patterns to Enforce:**
```typescript
// ✅ Proper component structure
interface UserCardProps {
  user: User;
  onEdit: (user: User) => void;
  className?: string;
}

const UserCard: React.FC<UserCardProps> = memo(({ user, onEdit, className }) => {
  const { t } = useTranslation('users');
  
  const handleEditClick = useCallback(() => {
    onEdit(user);
  }, [user, onEdit]);

  return (
    <div className={`user-card ${className}`} role="article">
      <h3>{user.name}</h3>
      <button 
        onClick={handleEditClick}
        aria-label={t('edit.button.label', { name: user.name })}
      >
        {t('edit.button.text')}
      </button>
    </div>
  );
});

// ❌ Poor patterns to flag
const UserCard = ({ user, onEdit }) => {  // No types
  return (
    <div onClick={() => onEdit(user)}>  // No accessibility
      <h3>{user.name}</h3>
      <button>Edit</button>  // Hardcoded text
    </div>
  );
};
```
```

### 2. **Security Assessment**

#### Input Validation
```markdown
✅ **Security Checklist:**
- [ ] **API Endpoints**: All inputs validated with DTOs and attributes
- [ ] **SQL Injection**: Parameterized queries used exclusively
- [ ] **XSS Prevention**: Input sanitization and output encoding
- [ ] **Authentication**: JWT validation and proper authorization
- [ ] **HTTPS**: All connections encrypted and secure headers set
- [ ] **Secrets Management**: No hardcoded secrets or credentials
- [ ] **Rate Limiting**: API endpoints protected from abuse
- [ ] **CORS**: Proper cross-origin policy configuration

**Security Patterns to Enforce:**
```csharp
// ✅ Proper input validation
[HttpPost]
[Authorize(Policy = "UserManagement")]
public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto dto)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }
    
    // Additional business validation
    if (await _userService.EmailExistsAsync(dto.Email))
    {
        return Conflict("Email already in use");
    }
    
    var user = await _userService.CreateUserAsync(dto);
    return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
}

// ❌ Security issues to flag
[HttpPost]
public ActionResult CreateUser(dynamic userData)  // No validation
{
    var sql = $"INSERT INTO Users VALUES ('{userData.name}')";  // SQL injection
    // ... execute raw SQL
}
```
```

### 3. **Architecture Compliance**

#### Single Responsibility Principle (SRP)
```markdown
✅ **SRP Validation:**
- [ ] **Controllers**: Only handle HTTP concerns, no business logic
- [ ] **Services**: Single business domain per service
- [ ] **Repositories**: One entity per repository, data access only
- [ ] **Components**: Single UI responsibility
- [ ] **Hooks**: Single state management concern
- [ ] **DTOs**: Pure data transfer, no behavior

**SRP Compliance Matrix:**
| Layer | Responsibility | Prohibited |
|-------|---------------|------------|
| Controllers | HTTP request/response | Business logic, data access |
| Services | Business logic | Data access, HTTP concerns |
| Repositories | Data persistence | Business rules, validation |
| Components | UI rendering | API calls, business logic |
| Hooks | State management | UI rendering |
| DTOs | Data structure | Business behavior |
```

#### Folder-per-Feature Organization
```markdown
✅ **Architecture Validation:**
- [ ] **Feature Boundaries**: No cross-feature dependencies
- [ ] **Layer Separation**: Clear separation between data, business, and presentation
- [ ] **Shared Components**: Only truly reusable components in shared
- [ ] **Integration Points**: Proper service registration and routing
- [ ] **Testing Structure**: Tests organized alongside features

**Expected Structure:**
```
backend/
├── Controllers/
│   └── FeatureController.cs (HTTP only)
├── Services/
│   └── FeatureService.cs (Business logic only)
├── Repositories/
│   └── FeatureRepository.cs (Data access only)

frontend/src/modules/feature/
├── components/ (UI only)
├── hooks/ (State management only)
├── services/ (API integration only)
├── types/ (Type definitions only)
└── views/ (Page composition only)
```
```

### 4. **Performance and Optimization**

#### Backend Performance
```markdown
✅ **Performance Checklist:**
- [ ] **Async Operations**: All I/O operations use async/await
- [ ] **Database Queries**: Efficient queries with proper indexing
- [ ] **Caching**: Appropriate use of memory and distributed caching
- [ ] **Connection Management**: Proper DbContext lifecycle
- [ ] **Memory Management**: IDisposable patterns implemented
- [ ] **API Response Size**: Pagination for large datasets
- [ ] **Exception Handling**: Efficient error processing

**Performance Patterns:**
```csharp
// ✅ Efficient data access
public async Task<PagedResult<UserDto>> GetUsersAsync(
    int page, int pageSize, CancellationToken cancellationToken)
{
    var users = await _repository.GetPagedAsync(page, pageSize, cancellationToken);
    return new PagedResult<UserDto>
    {
        Items = _mapper.Map<List<UserDto>>(users.Items),
        TotalCount = users.TotalCount,
        Page = page,
        PageSize = pageSize
    };
}
```
```

#### Frontend Performance
```markdown
✅ **Performance Checklist:**
- [ ] **React Optimization**: Proper use of memo, useCallback, useMemo
- [ ] **Bundle Size**: Code splitting and lazy loading
- [ ] **API Calls**: Efficient data fetching with TanStack Query
- [ ] **Image Optimization**: Proper image loading and sizing
- [ ] **Memory Leaks**: Cleanup in useEffect hooks
- [ ] **Render Optimization**: Minimize unnecessary re-renders

**Performance Patterns:**
```typescript
// ✅ Optimized component
const UserList: React.FC<UserListProps> = memo(({ filters, onUserSelect }) => {
  const { data: users, isLoading, error } = useUsers(filters);
  
  const handleUserClick = useCallback((user: User) => {
    onUserSelect(user);
  }, [onUserSelect]);
  
  const filteredUsers = useMemo(() => {
    return users?.filter(user => user.isActive) ?? [];
  }, [users]);
  
  if (isLoading) return <LoadingSpinner />;
  if (error) return <ErrorMessage error={error} />;
  
  return (
    <div className="user-list">
      {filteredUsers.map(user => (
        <UserCard 
          key={user.id} 
          user={user} 
          onClick={handleUserClick}
        />
      ))}
    </div>
  );
});
```
```

### 5. **Testing Quality**

#### Test Coverage and Quality
```markdown
✅ **Testing Checklist:**
- [ ] **Unit Tests**: All business logic covered
- [ ] **Integration Tests**: API endpoints and database interactions
- [ ] **Component Tests**: React components with user interactions
- [ ] **Hook Tests**: Custom hooks with state management
- [ ] **Edge Cases**: Error conditions and boundary cases
- [ ] **Mocking**: Proper isolation of dependencies
- [ ] **Performance Tests**: Load testing for critical paths

**Testing Patterns:**
```csharp
// ✅ Comprehensive unit test
[Test]
public async Task CreateUserAsync_WithValidData_ShouldReturnUserDto()
{
    // Arrange
    var createDto = new CreateUserDto { Email = "test@example.com", Name = "Test User" };
    var expectedUser = new User { Id = Guid.NewGuid(), Email = createDto.Email, Name = createDto.Name };
    
    _mockRepository.Setup(r => r.EmailExistsAsync(createDto.Email))
                   .ReturnsAsync(false);
    _mockRepository.Setup(r => r.CreateAsync(It.IsAny<User>()))
                   .ReturnsAsync(expectedUser);
    
    // Act
    var result = await _userService.CreateUserAsync(createDto);
    
    // Assert
    result.Should().NotBeNull();
    result.Email.Should().Be(createDto.Email);
    result.Name.Should().Be(createDto.Name);
    
    _mockRepository.Verify(r => r.CreateAsync(It.Is<User>(u => 
        u.Email == createDto.Email && u.Name == createDto.Name)), Times.Once);
}
```
```

## Implementation Process

### Review Execution
```markdown
I'll systematically review the codebase using this comprehensive approach:

**Phase 1: Scope Identification**
- Determine what needs to be reviewed (changes, files, or entire codebase)
- Use appropriate git commands or file analysis
- Establish review boundaries and priorities

**Phase 2: Technical Analysis**
- Code quality assessment using established patterns
- Security vulnerability scanning
- Architecture compliance validation
- Performance bottleneck identification

**Phase 3: Issue Documentation**
- Categorize findings by severity and type
- Provide specific examples and fix recommendations
- Include code snippets showing before/after improvements
- Estimate effort required for each fix

**Phase 4: Report Generation**
- Create comprehensive review report
- Prioritize issues by impact and effort
- Provide actionable recommendations
- Include validation steps for fixes
```

### Review Report Format
```markdown
# Code Review Report

## Executive Summary
- **Review Scope**: [Description of what was reviewed]
- **Total Issues**: X (Critical: X, High: X, Medium: X, Low: X)
- **Overall Quality Score**: X/10
- **Estimated Fix Effort**: X hours

## Critical Issues (Fix Immediately)
### 🚨 Security Vulnerability: SQL Injection in UserController
- **File**: `Controllers/UserController.cs` (Line 45)
- **Issue**: Raw SQL query with user input
- **Risk**: Complete database compromise
- **Fix**: Use parameterized queries or ORM
```csharp
// ❌ Current vulnerable code
var sql = $"SELECT * FROM Users WHERE Email = '{email}'";

// ✅ Fixed secure code
var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
```

## High Priority Issues
[Similar format for high priority items]

## Architecture Violations
[SRP violations, layer mixing, etc.]

## Performance Issues
[Inefficient queries, missing async, etc.]

## Code Quality Issues
[Naming, complexity, documentation, etc.]

## Recommendations
### Immediate Actions (This Sprint)
- [ ] Fix all critical security issues
- [ ] Resolve architecture violations
- [ ] Add missing error handling

### Short-term Improvements (Next Sprint)
- [ ] Improve test coverage to >80%
- [ ] Add missing documentation
- [ ] Optimize database queries

### Long-term Enhancements (Future Sprints)
- [ ] Implement advanced caching
- [ ] Add performance monitoring
- [ ] Enhance error reporting
```

## Validation Commands

### Pre-Review Validation
```powershell
# Ensure clean build state
dotnet clean && dotnet build --configuration Release

# Run all existing tests
dotnet test --logger "console;verbosity=detailed"

# Check code formatting
dotnet format --verify-no-changes
```

```bash
# Frontend validation
npm run type-check && npm run lint && npm run test
```

### Post-Fix Validation
```powershell
# Verify fixes don't break functionality
dotnet build && dotnet test

# Run security analysis
dotnet list package --vulnerable
```

## Success Criteria
- All critical and high-priority issues identified
- Specific, actionable fixes provided with code examples
- Architecture compliance validated
- Security vulnerabilities highlighted with fixes
- Performance bottlenecks identified with solutions
- Testing gaps documented with recommendations
- Clear prioritization and effort estimates provided

This comprehensive review ensures code quality, security, and architectural consistency while maintaining the project's established patterns and principles.
