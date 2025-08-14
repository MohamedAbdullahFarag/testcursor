---
mode: agent
description: "Review staged and unstaged changes focusing on quality, security, and architectural compliance"
---

# Staged/Unstaged Changes Review - Focused Change Analysis

## Purpose
This prompt performs a detailed review of files in the staging area (both staged and unstaged), analyzing the specific changes made and ensuring they meet the Ikhtibar project's quality, security, and architectural standards.

## Review Scope
The review targets:
- All files in the git staging area (staged changes)
- Modified files not yet staged (unstaged changes)
- New files added to the repository
- Deleted files and their impact analysis
- Previous review context if provided

## Review Process

### Step 1: Change Discovery
```markdown
I'll systematically identify and analyze all changes:

**Change Detection:**
- Use `git status` to identify staged and unstaged files
- Use `git diff --staged` for staged changes analysis
- Use `git diff` for unstaged changes analysis
- Check `git diff --name-status` for file operation types (added, modified, deleted)
- Analyze both content changes and file structure modifications

**Change Categorization:**
- **New Features**: Completely new functionality
- **Bug Fixes**: Corrections to existing behavior
- **Refactoring**: Code improvements without behavior changes
- **Configuration**: Settings, environment, or build changes
- **Documentation**: README, comments, or documentation updates
```

### Step 2: Differential Analysis
```markdown
For each changed file, I'll analyze:

**Content Changes:**
- Line-by-line diff analysis
- Context understanding around changes
- Impact on existing functionality
- Integration with surrounding code

**Quality Assessment:**
- Code quality improvements or regressions
- Adherence to established patterns
- Consistency with project conventions
- Potential breaking changes
```

## Review Focus Areas

### 1. **Code Quality Standards**

#### Backend Changes (C# / ASP.NET Core)
```markdown
✅ **Quality Validation for Changes:**
- [ ] **Type Safety**: New methods have explicit return types and parameters
- [ ] **Error Handling**: Added proper try-catch blocks with specific exceptions
- [ ] **Logging**: Structured logging added for new operations
- [ ] **Async Patterns**: New I/O operations use async/await correctly
- [ ] **Validation**: Input validation added for new endpoints
- [ ] **Documentation**: XML comments added for new public APIs
- [ ] **Testing**: Unit tests included for new functionality
- [ ] **Configuration**: No hardcoded values in new code

**Change Pattern Analysis:**
```csharp
// ✅ Good change example
// BEFORE (if applicable)
public User GetUser(int id) 
{
    return _repository.GetById(id);
}

// AFTER - Improved with async, logging, and error handling
public async Task<UserDto> GetUserAsync(Guid id)
{
    using var scope = _logger.BeginScope("Getting user {UserId}", id);
    try
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found", id);
            return null;
        }
        
        _logger.LogInformation("Successfully retrieved user {UserId}", id);
        return _mapper.Map<UserDto>(user);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error retrieving user {UserId}", id);
        throw;
    }
}

// ❌ Poor change to flag
public async Task<object> DoSomething(dynamic input)  // Weak typing
{
    var result = await SomeOperation(input);  // No error handling
    return result;  // No logging
}
```
```

#### Frontend Changes (TypeScript / React)
```markdown
✅ **Quality Validation for Changes:**
- [ ] **TypeScript Strict**: New code uses explicit typing, no `any`
- [ ] **Component Props**: New components have proper interface definitions
- [ ] **Hooks Usage**: New hooks follow single responsibility principle
- [ ] **Performance**: Added memoization where appropriate (memo, useCallback, useMemo)
- [ ] **Accessibility**: New UI elements have proper ARIA attributes
- [ ] **Internationalization**: New text uses i18next keys, not hardcoded strings
- [ ] **Error Handling**: New async operations include error states
- [ ] **Testing**: Component tests added for new UI elements

**Change Pattern Analysis:**
```typescript
// ✅ Good change example
// BEFORE (if applicable)
const UserCard = ({ user, onEdit }) => {
  return (
    <div onClick={() => onEdit(user)}>
      <h3>{user.name}</h3>
      <button>Edit</button>
    </div>
  );
};

// AFTER - Improved with types, i18n, accessibility
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
    <article className={`user-card ${className}`}>
      <h3>{user.name}</h3>
      <button 
        onClick={handleEditClick}
        aria-label={t('edit.button.label', { name: user.name })}
        type="button"
      >
        {t('edit.button.text')}
      </button>
    </article>
  );
});

// ❌ Poor change to flag
const UserCard = ({ user, onEdit }: any) => {  // Any usage
  return (
    <div onClick={onEdit}>  // No accessibility, direct prop usage
      <h3>{user.name}</h3>
      <button>Edit User</button>  // Hardcoded text
    </div>
  );
};
```
```

### 2. **Security Assessment**

#### Security-Critical Changes
```markdown
✅ **Security Validation for Changes:**
- [ ] **Input Validation**: New endpoints validate all inputs
- [ ] **Authentication**: New endpoints have proper authorization attributes
- [ ] **SQL Injection**: New database queries use parameterized queries
- [ ] **XSS Prevention**: New UI inputs sanitize user data
- [ ] **Secret Management**: No hardcoded secrets in changed files
- [ ] **Error Information**: New error messages don't expose internal details
- [ ] **Rate Limiting**: New endpoints consider abuse prevention
- [ ] **HTTPS**: New configurations enforce secure connections

**Security Change Analysis:**
```csharp
// ✅ Secure change example
[HttpPost]
[Authorize(Policy = "UserManagement")]
[ValidateAntiForgeryToken]
public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto dto)
{
    if (!ModelState.IsValid)
    {
        _logger.LogWarning("Invalid user creation attempt with validation errors");
        return BadRequest(ModelState);
    }
    
    try
    {
        var user = await _userService.CreateUserAsync(dto);
        _logger.LogInformation("User created successfully with ID {UserId}", user.Id);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }
    catch (UserAlreadyExistsException ex)
    {
        _logger.LogWarning(ex, "Attempt to create user with existing email");
        return Conflict("User with this email already exists");
    }
}

// ❌ Security issue to flag
[HttpPost]
public ActionResult CreateUser(string userData)  // No authorization
{
    var sql = $"INSERT INTO Users VALUES ('{userData}')";  // SQL injection
    // Direct SQL execution without validation
}
```
```

### 3. **Architecture Compliance**

#### Single Responsibility Principle (SRP) Changes
```markdown
✅ **SRP Validation for Changes:**
- [ ] **Controller Changes**: Only handle HTTP concerns, no business logic added
- [ ] **Service Changes**: Business logic additions stay within single domain
- [ ] **Repository Changes**: Data access only, no business rules added
- [ ] **Component Changes**: UI responsibility only, no business logic
- [ ] **Hook Changes**: Single state concern, no mixed responsibilities
- [ ] **DTO Changes**: Pure data structure, no behavior added

**SRP Change Analysis:**
```csharp
// ✅ Good SRP change
// UserController - only HTTP concerns
[HttpPut("{id}")]
public async Task<ActionResult<UserDto>> UpdateUser(Guid id, [FromBody] UpdateUserDto dto)
{
    var result = await _userService.UpdateUserAsync(id, dto);  // Delegates to service
    return Ok(result);
}

// UserService - only business logic
public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto dto)
{
    var existingUser = await _userRepository.GetByIdAsync(id);
    if (existingUser == null)
        throw new UserNotFoundException($"User {id} not found");
    
    // Business validation
    if (dto.Email != existingUser.Email && await _userRepository.EmailExistsAsync(dto.Email))
        throw new EmailAlreadyExistsException("Email is already in use");
    
    var updatedUser = await _userRepository.UpdateAsync(id, dto);
    return _mapper.Map<UserDto>(updatedUser);
}

// ❌ SRP violation to flag
[HttpPut("{id}")]
public async Task<ActionResult> UpdateUser(Guid id, UpdateUserDto dto)
{
    // ❌ Business logic in controller
    var existingUser = await _userRepository.GetByIdAsync(id);
    if (dto.Email != existingUser.Email)
    {
        var emailExists = await _userRepository.EmailExistsAsync(dto.Email);
        if (emailExists) return Conflict("Email exists");
    }
    
    // ❌ Data access in controller
    await _userRepository.UpdateAsync(id, dto);
    return Ok();
}
```
```

#### Folder-per-Feature Compliance
```markdown
✅ **Architecture Validation for Changes:**
- [ ] **Feature Boundaries**: No new cross-feature dependencies
- [ ] **Layer Organization**: Changes maintain proper layer separation
- [ ] **Shared Components**: Only truly reusable code in shared folders
- [ ] **Import Patterns**: Relative imports within features, absolute for shared
- [ ] **File Placement**: New files follow established folder structure

**Architectural Change Analysis:**
```
✅ Good file organization change:
backend/Ikhtibar.Core/
├── Services/
│   └── UserService.cs (business logic only)
├── Repositories/
│   └── IUserRepository.cs (data access interface)

frontend/src/modules/users/
├── components/
│   └── UserCard.tsx (UI component only)
├── hooks/
│   └── useUsers.ts (state management only)
├── services/
│   └── userService.ts (API integration only)

❌ Poor organization to flag:
src/shared/
└── UserBusinessLogic.tsx (feature-specific logic in shared)
```
```

### 4. **Change Impact Analysis**

#### Breaking Change Detection
```markdown
✅ **Breaking Change Validation:**
- [ ] **API Changes**: No breaking changes to public interfaces
- [ ] **Database Schema**: Migrations provided for schema changes
- [ ] **Configuration**: New settings have default values
- [ ] **Dependencies**: Package updates don't break existing functionality
- [ ] **Component Props**: Interface changes maintain backward compatibility
- [ ] **Hook Signatures**: Custom hook changes don't break consumers

**Impact Assessment:**
```csharp
// ✅ Non-breaking change
// BEFORE
public async Task<UserDto> GetUserAsync(Guid id)

// AFTER - Added optional parameter
public async Task<UserDto> GetUserAsync(Guid id, bool includeRoles = false)

// ❌ Breaking change to flag
// BEFORE
public async Task<UserDto> GetUserAsync(Guid id)

// AFTER - Changed return type without versioning
public async Task<UserDetailDto> GetUserAsync(Guid id)
```
```

### 5. **Testing Coverage for Changes**

#### Test Requirements for Changes
```markdown
✅ **Testing Validation for Changes:**
- [ ] **New Methods**: Unit tests added for new business logic
- [ ] **New Endpoints**: Integration tests for new API endpoints
- [ ] **New Components**: Component tests with user interactions
- [ ] **New Hooks**: Hook tests with state management scenarios
- [ ] **Bug Fixes**: Tests added to prevent regression
- [ ] **Edge Cases**: Error conditions and boundary cases tested

**Testing Change Analysis:**
```csharp
// ✅ Good test addition
[Test]
public async Task UpdateUserAsync_WithDuplicateEmail_ShouldThrowEmailAlreadyExistsException()
{
    // Arrange
    var userId = Guid.NewGuid();
    var dto = new UpdateUserDto { Email = "existing@example.com" };
    
    _mockRepository.Setup(r => r.GetByIdAsync(userId))
                   .ReturnsAsync(new User { Id = userId, Email = "original@example.com" });
    _mockRepository.Setup(r => r.EmailExistsAsync(dto.Email))
                   .ReturnsAsync(true);
    
    // Act & Assert
    await _userService.Invoking(s => s.UpdateUserAsync(userId, dto))
                      .Should().ThrowAsync<EmailAlreadyExistsException>();
}
```
```

## Implementation Process

### Change Review Execution
```markdown
I'll systematically review all staged and unstaged changes:

**Phase 1: Change Identification**
- Execute `git status` to get overview of all changes
- Use `git diff --staged` to analyze staged changes
- Use `git diff` to analyze unstaged changes
- Check `git diff --stat` for change volume analysis

**Phase 2: File-by-File Analysis**
- Review each changed file individually
- Analyze diff context and surrounding code
- Check for integration impact with existing code
- Validate against established patterns

**Phase 3: Cross-File Impact Analysis**
- Check dependencies between changed files
- Validate interface compatibility
- Ensure consistent patterns across changes
- Identify potential integration issues

**Phase 4: Quality and Security Assessment**
- Apply all quality checklists to changes
- Perform security analysis on new/modified code
- Validate architectural compliance
- Check testing coverage for changes
```

### Change Review Report Format
```markdown
# Staged/Unstaged Changes Review Report

## Change Summary
- **Total Files Changed**: X (Staged: X, Unstaged: X)
- **Lines Added**: +X
- **Lines Deleted**: -X
- **Change Types**: [New features, Bug fixes, Refactoring, etc.]

## File-by-File Analysis

### Staged Changes
#### ✅ backend/Controllers/UserController.cs (+15, -5)
**Changes Made:**
- Added input validation for CreateUser endpoint
- Improved error handling with specific exceptions
- Added structured logging

**Quality Assessment:** ✅ Excellent
- Follows SRP (HTTP concerns only)
- Proper error handling added
- Security improvements with validation

**Recommendations:** None - ready for commit

#### ⚠️ frontend/src/modules/users/UserCard.tsx (+25, -10)
**Changes Made:**
- Added TypeScript interfaces
- Improved accessibility with ARIA attributes
- Added internationalization support

**Quality Assessment:** ⚠️ Good with minor issues
- ✅ TypeScript typing improved
- ✅ Accessibility enhanced
- ❌ Missing performance optimization (memo wrapper)

**Recommendations:**
```typescript
// Add React.memo for performance
const UserCard: React.FC<UserCardProps> = memo(({ user, onEdit }) => {
  // ... existing code
});
```

### Unstaged Changes
#### 🚨 backend/Services/UserService.cs (+10, -2)
**Changes Made:**
- Added business logic for email validation

**Quality Assessment:** 🚨 Needs fixes
- ❌ Missing error handling for database operations
- ❌ No logging for business operations
- ⚠️ Could benefit from additional validation

**Required Fixes:**
```csharp
// Add proper error handling and logging
public async Task<bool> EmailExistsAsync(string email)
{
    using var scope = _logger.BeginScope("Checking email existence {Email}", email);
    try
    {
        var exists = await _userRepository.EmailExistsAsync(email);
        _logger.LogInformation("Email check completed for {Email}: {Exists}", email, exists);
        return exists;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error checking email existence for {Email}", email);
        throw;
    }
}
```

## Priority Actions

### Critical (Fix Before Commit)
- [ ] Add error handling to UserService.cs
- [ ] Fix security validation in AuthController.cs

### High Priority (Fix This Session)
- [ ] Add React.memo to UserCard component
- [ ] Add missing unit tests for new business logic

### Medium Priority (Next Sprint)
- [ ] Improve error messages with internationalization
- [ ] Add integration tests for new endpoints

## Overall Assessment
- **Quality Score**: 7/10
- **Security Score**: 8/10
- **Architecture Compliance**: 9/10
- **Ready for Commit**: ❌ (Critical issues need fixing)
```

## Validation Commands

### Pre-Review Validation
```powershell
# Check current git status
git status

# View staged changes
git diff --staged

# View unstaged changes  
git diff

# Check build status
dotnet build --configuration Release
```

### Post-Fix Validation
```powershell
# Verify all changes pass tests
dotnet test

# Check code formatting
dotnet format --verify-no-changes

# Frontend validation
npm run type-check && npm run lint && npm run test
```

## Success Criteria
- All staged and unstaged changes identified and analyzed
- Quality assessment provided for each changed file
- Security vulnerabilities highlighted with specific fixes
- Architecture compliance validated for all changes
- Breaking changes identified and documented
- Testing requirements specified for new functionality
- Clear prioritization of required fixes before commit
- Actionable recommendations with code examples provided

This focused change review ensures that only high-quality, secure, and architecturally compliant code progresses through the development workflow.
