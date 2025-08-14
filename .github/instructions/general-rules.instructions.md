---
description: "Project-wide conventions and GitHub Copilot optimization guidelines"
applyTo: "**/*"
---

# GitHub Copilot Enhanced Instructions - Ikhtibar Project
*Optimized for AI-Powered Development with Context-Rich Workflows*

## 🎯 Project Overview
This is a full-stack application with:
- **Backend**: ASP.NET Core Web API (.NET 8+) with folder-per-feature architecture
- **Frontend**: React.js with TypeScript, Vite, and Tailwind CSS
- **AI Development**: GitHub Copilot-optimized workflows with validation loops

## 🧠 GitHub Copilot Optimization Principles

### **Context Engineering Over Prompt Engineering**
- Provide comprehensive context through comments and documentation
- Use descriptive variable names and function signatures
- Include examples and patterns directly in code
- Maintain COPILOT.md file with project-specific guidelines

### **Validation-First Development**
- Each feature includes executable validation commands
- Progressive enhancement through iterative refinement
- Self-documenting code with clear intent
- Anti-pattern warnings embedded in comments

## 🏗️ Architecture Principles

### Folder-per-Feature Structure
Both backend and frontend follow the folder-per-feature pattern:
- Each feature has its own folder containing all related components
- Features are self-contained and loosely coupled
- Shared functionality is placed in common/shared folders
- **Copilot Hint**: Use consistent naming conventions across features for better AI suggestions

### Backend (ASP.NET Core Web API)
- **Language**: C# (.NET 8+)
- **Architecture**: Clean Architecture with folder-per-feature
- **Patterns**: CQRS, Mediator, Repository Pattern, Unit of Work
- **Database**: Dapper (micro ORM)
- **Authentication**: JWT Bearer tokens
- **API Documentation**: Swagger/OpenAPI
- **Copilot Context**: Use XML documentation for all public methods to improve AI suggestions

### Frontend (React.js)
- **Language**: TypeScript with strict mode
- **Framework**: React 18+ with Vite
- **Styling**: Tailwind CSS with consistent class patterns
- **State Management**: Context API or Zustand
- **Routing**: React Router with type-safe routes
- **HTTP Client**: Axios with interceptors
- **Internationalization**: i18next (Arabic/English)
- **Copilot Context**: Comprehensive TypeScript interfaces for better AI understanding

## 🤖 GitHub Copilot Development Guidelines

### **Natural Language Comments for AI Context**
Use descriptive comments that help Copilot understand your intent:

```csharp
// Create a new user following our authentication and validation patterns
// This should validate input, hash password, assign default role, and send welcome email
public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
{
    // Implementation follows...
}
```

```typescript
// UserCard component that displays user information with edit capabilities
// Follows our design system patterns with loading and error states
// Supports both Arabic RTL and English LTR layouts
interface UserCardProps {
    user: User;
    onEdit: (userId: string) => void;
    isLoading?: boolean;
}
```

### **Validation-Driven Development Workflow**
Each development task includes validation commands that can be executed:

### **Level 1: Syntax & Style Validation**
```powershell
# Backend validation
dotnet build --configuration Release
dotnet format --verify-no-changes
dotnet test --logger "console;verbosity=detailed"

# Frontend validation  
npm run type-check
npm run lint
npm run test -- --coverage
```

### **Level 2: Architectural Validation**
```powershell
# SRP compliance check
# Each class should have exactly one reason to change
# Use descriptive comments to help Copilot understand boundaries

# Performance validation
# Async operations should use proper patterns
# Database queries should be optimized
```

### **Level 3: Integration Validation**
```powershell
# API integration tests
curl -X GET http://localhost:5000/api/health
# Expected: {"status": "healthy", "timestamp": "..."}

# Frontend integration tests
npm run test:e2e
# Expected: All user workflows pass
```

## 🎯 Copilot-Optimized Patterns

### **Context-Rich Code Structure**
Write code that provides maximum context to GitHub Copilot:

```csharp
namespace Ikhtibar.Features.Users
{
    /// <summary>
    /// User management service following clean architecture patterns.
    /// Handles CRUD operations, validation, and business rules for user entities.
    /// Integrates with authentication, authorization, and notification systems.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Creates a new user with validation, password hashing, and welcome email.
        /// Assigns default role and logs the operation for audit trail.
        /// </summary>
        /// <param name="dto">User creation data with validation attributes</param>
        /// <returns>Created user DTO without sensitive information</returns>
        /// <exception cref="EmailAlreadyExistsException">When email is already registered</exception>
        /// <exception cref="ValidationException">When input validation fails</exception>
        Task<UserDto> CreateUserAsync(CreateUserDto dto);
    }
}
```

```typescript
/**
 * Custom hook for user management operations
 * Provides CRUD operations with optimistic updates, error handling, and loading states
 * Integrates with our API client and global error handling system
 * Supports both Arabic RTL and English LTR text directions
 */
export const useUserManagement = () => {
    const [users, setUsers] = useState<User[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    
    // Create user with validation and optimistic updates
    const createUser = useCallback(async (userData: CreateUserDto): Promise<User> => {
        // Implementation follows our established patterns...
    }, []);
    
    return { users, loading, error, createUser };
};
```

### **Progressive Enhancement Patterns**
Start simple, validate, then enhance:

```csharp
// Phase 1: Basic structure with validation
public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
{
    // TODO: Add input validation
    // TODO: Check email uniqueness 
    // TODO: Hash password
    // TODO: Save to database
    // TODO: Return mapped DTO
    
    throw new NotImplementedException("Use Copilot to implement following our patterns");
}

// Phase 2: Enhanced with business rules (Copilot will suggest based on context)
// Phase 3: Add comprehensive error handling and logging
// Phase 4: Add unit tests and integration tests
```

## 🏆 Enhanced Code Generation Rules

### **Context-Driven Development**
1. **Always provide rich context** through comments and documentation
2. **Use TypeScript strict mode** for maximum type safety and Copilot accuracy
3. **Follow established patterns** that Copilot can learn from
4. **Include validation loops** in every feature implementation
5. **Write self-documenting code** with clear intent and purpose
6. **Add comprehensive error handling** with descriptive messages
7. **Include unit tests** as part of the implementation process
8. **Use meaningful names** that describe business intent
9. **Document architectural decisions** and patterns

### **Copilot-Enhanced Workflow**
1. **Start with intent comments** describing what you want to build
2. **Let Copilot suggest implementation** based on your context
3. **Validate suggestions** against project patterns and standards
4. **Iterate and refine** using validation commands
5. **Document lessons learned** for future Copilot interactions

## 🎯 Backend Development Rules (Copilot-Optimized)

### **Context-Rich Class Structure**
Each class should provide maximum context to help Copilot understand the domain:
```csharp
namespace Ikhtibar.Features.UserManagement
{
    /// <summary>
    /// User repository implementing data access patterns for user entities.
    /// Uses Dapper for efficient database operations with SQL Server.
    /// Follows repository pattern with async operations and proper error handling.
    /// Integrates with our audit trail and soft delete functionality.
    /// </summary>
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        // Copilot will understand the context and suggest appropriate implementations
        // following our established patterns from similar repositories
    }
}
```

### **One Class Per File with Context**
1. **Filename must match class name** (e.g., `UserService.cs` for `UserService` class)
   - **Copilot Context**: Use consistent naming for better AI pattern recognition
2. **Interfaces** should be in their own files (e.g., `IUserService.cs`)
   - **Copilot Context**: Include comprehensive XML documentation for contract clarity
3. **DTOs** should be in separate files (e.g., `CreateUserDto.cs`, `UpdateUserDto.cs`)
   - **Copilot Context**: Use data annotations and validation attributes
4. **Entities** should be in separate files (e.g., `User.cs`, `Role.cs`)
   - **Copilot Context**: Include navigation properties and constraints
5. **Exception**: Small, tightly related classes (e.g., enums) can share files
   - **Copilot Context**: Document relationships and usage patterns

### **Feature Structure for Maximum AI Understanding**
Each feature should have a clear, consistent structure that Copilot can learn from:

```
Features/
├── UserManagement/
│   ├── Controllers/
│   │   └── UsersController.cs          # API endpoints with Swagger docs
│   ├── Services/
│   │   ├── IUserService.cs             # Service contract with XML docs
│   │   └── UserService.cs              # Business logic implementation
│   ├── Models/
│   │   ├── DTOs/
│   │   │   ├── CreateUserDto.cs        # Input validation models
│   │   │   ├── UpdateUserDto.cs        # Update operation models
│   │   │   └── UserDto.cs              # Response models
│   │   └── Entities/
│   │       └── User.cs                 # Database entity
│   ├── Repositories/
│   │   ├── IUserRepository.cs          # Data access contract
│   │   └── UserRepository.cs           # Dapper implementation
│   ├── Validators/
│   │   └── CreateUserValidator.cs      # FluentValidation rules
│   ├── Mappings/
│   │   └── UserMappingProfile.cs       # AutoMapper configuration
│   └── Tests/
│       ├── UserServiceTests.cs         # Unit tests for business logic
│       ├── UserRepositoryTests.cs      # Data access tests
│       └── UsersControllerTests.cs     # API integration tests
```

### **API Controllers with Rich Context**
```csharp
namespace Ikhtibar.Features.UserManagement.Controllers
{
    /// <summary>
    /// User management API controller following RESTful conventions.
    /// Implements CRUD operations with proper HTTP status codes and error handling.
    /// Includes authentication, authorization, and input validation.
    /// Supports both Arabic and English localization.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Copilot understands this requires authentication
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;
        
        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }
        
        /// <summary>
        /// Creates a new user with validation and proper error handling.
        /// Returns 201 Created with user details or appropriate error response.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto dto)
        {
            // Copilot will suggest implementation following our established patterns
            // for validation, error handling, and response formatting
        }
    }
}
```

### **Service Layer with Business Context**
```csharp
namespace Ikhtibar.Features.UserManagement.Services
{
    /// <summary>
    /// User business logic service implementing domain rules and workflows.
    /// Orchestrates between repositories, external services, and validation.
    /// Handles user lifecycle events, notifications, and audit logging.
    /// Implements transaction management for complex operations.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEmailService _emailService;
        private readonly ILogger<UserService> _logger;
        
        // Copilot understands the dependencies and their purposes
        // Will suggest appropriate implementation patterns
        
        /// <summary>
        /// Creates new user following our business rules:
        /// 1. Validate input data using FluentValidation
        /// 2. Check email uniqueness across system
        /// 3. Hash password using our security standards
        /// 4. Assign default role based on configuration
        /// 5. Send welcome email with activation link
        /// 6. Log operation for audit trail
        /// </summary>
        public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
        {
            // Copilot will implement following the documented workflow
            // and established patterns from similar methods
        }
    }
}
```

### **Repository Pattern with Data Context**
```csharp
namespace Ikhtibar.Features.UserManagement.Repositories
{
    /// <summary>
    /// User data access repository using Dapper for efficient SQL operations.
    /// Implements async patterns with proper connection management.
    /// Supports soft deletes, audit trails, and optimistic concurrency.
    /// Uses parameterized queries to prevent SQL injection.
    /// </summary>
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        // Copilot understands Dapper patterns and will suggest:
        // - Parameterized SQL queries
        // - Async operations with proper disposal
        // - Error handling for database operations
        // - Connection string management
        
        /// <summary>
        /// Retrieves user by email with case-insensitive comparison.
        /// Returns null if user not found or is soft deleted.
        /// Includes role information for authorization checks.
        /// </summary>
        public async Task<User?> GetByEmailAsync(string email)
        {
            // Copilot will suggest SQL query following our patterns:
            // - JOIN with Roles table for complete user info
            // - WHERE clause with LOWER() for case-insensitive search
            // - Filter out soft-deleted records
            // - Proper parameter binding for security
        }
    }
}
```

## 🎨 Frontend Development Rules (Copilot-Optimized)

### **Context-Rich Component Structure**
```typescript
// File: src/modules/user-management/components/UserCard.tsx
/**
 * UserCard component displaying user information in card format
 * 
 * Features:
 * - Supports both Arabic RTL and English LTR layouts
 * - Includes loading states and error handling
 * - Follows our design system patterns with Tailwind CSS
 * - Implements accessibility standards (ARIA labels, keyboard navigation)
 * - Optimized with React.memo for performance
 * 
 * Dependencies:
 * - Uses our custom useUser hook for data management
 * - Integrates with global notification system
 * - Follows i18next patterns for internationalization
 */

import React, { memo, useCallback } from 'react';
import { User } from '../types/User';
import { useTranslation } from 'react-i18next';

interface UserCardProps {
    /** User data object with all required fields */
    user: User;
    /** Callback fired when user clicks edit button */
    onEdit: (userId: string) => void;
    /** Loading state for async operations */
    isLoading?: boolean;
    /** Error state with localized message */
    error?: string | null;
}

/**
 * UserCard component following our established patterns.
 * Copilot will understand the context and suggest appropriate implementation.
 */
export const UserCard: React.FC<UserCardProps> = memo(({ 
    user, 
    onEdit, 
    isLoading = false, 
    error = null 
}) => {
    const { t } = useTranslation('user-management');
    
    // Copilot will suggest implementation following our patterns:
    // - Tailwind CSS classes for consistent styling
    // - Loading spinner from our component library
    // - Error display with proper ARIA attributes
    // - RTL-aware layout with appropriate text direction
    // - Keyboard accessibility for edit action
    
    const handleEdit = useCallback(() => {
        onEdit(user.id);
    }, [user.id, onEdit]);
    
    // Implementation will follow our established component patterns
    return (
        // Copilot suggestions will include:
        // - Proper semantic HTML structure
        // - Accessibility attributes
        // - Responsive design classes
        // - Loading and error state handling
        // - RTL/LTR layout support
    );
});

UserCard.displayName = 'UserCard';
```

### **Custom Hooks with Rich Context**
```typescript
// File: src/modules/user-management/hooks/useUserManagement.ts
/**
 * Custom hook for comprehensive user management operations
 * 
 * Provides:
 * - CRUD operations with optimistic updates
 * - Loading states for better UX
 * - Error handling with localized messages
 * - Caching and invalidation strategies
 * - Integration with global state management
 * 
 * Usage patterns:
 * - Follows our established hook conventions
 * - Returns consistent interface across all management hooks
 * - Implements proper cleanup on component unmount
 * - Supports real-time updates via WebSocket integration
 */

import { useState, useCallback, useEffect, useMemo } from 'react';
import { User, CreateUserDto, UpdateUserDto } from '../types';
import { userService } from '../services/userService';
import { useNotification } from '@/shared/hooks/useNotification';
import { useTranslation } from 'react-i18next';

interface UseUserManagementReturn {
    // State
    users: User[];
    loading: boolean;
    error: string | null;
    
    // Operations
    createUser: (userData: CreateUserDto) => Promise<User>;
    updateUser: (id: string, userData: UpdateUserDto) => Promise<User>;
    deleteUser: (id: string) => Promise<boolean>;
    refreshUsers: () => Promise<void>;
    
    // Derived state
    activeUsers: User[];
    totalCount: number;
}

export const useUserManagement = (): UseUserManagementReturn => {
    // Copilot will understand our state management patterns and suggest:
    // - Proper initial state setup
    // - Loading state management for each operation
    // - Error handling with user-friendly messages
    // - Optimistic updates for better UX
    // - Proper cleanup in useEffect
    
    // Implementation follows our established patterns for:
    // - API integration with error handling
    // - Notification system integration
    // - Internationalization support
    // - Performance optimization with useMemo/useCallback
};
```

### **Service Layer with API Context**
```typescript
// File: src/modules/user-management/services/userService.ts
/**
 * User API service implementing our standardized HTTP client patterns
 * 
 * Features:
 * - Type-safe API calls with comprehensive error handling
 * - Request/response interceptors for auth and logging
 * - Retry logic for network failures
 * - Request cancellation support
 * - Integration with our global loading and notification systems
 * 
 * Patterns:
 * - Uses our custom API client with authentication
 * - Implements consistent error mapping
 * - Supports both Arabic and English error messages
 * - Includes request/response logging for debugging
 */

import { ApiClient } from '@/shared/services/apiClient';
import { User, CreateUserDto, UpdateUserDto, UserFilters } from '../types';

class UserService {
    private readonly apiClient: ApiClient;
    
    constructor() {
        this.apiClient = new ApiClient('/api/users');
    }
    
    /**
     * Creates new user with comprehensive validation and error handling.
     * Implements optimistic updates and proper error recovery.
     */
    async createUser(userData: CreateUserDto): Promise<User> {
        // Copilot will suggest implementation following our patterns:
        // - Input validation before API call
        // - Proper error handling with translated messages
        // - Response validation and type checking
        // - Integration with notification system
        // - Proper async/await error handling
    }
    
    /**
     * Retrieves paginated user list with filtering and sorting.
     * Supports server-side pagination and search functionality.
     */
    async getUsers(filters?: UserFilters): Promise<{ users: User[]; total: number }> {
        // Implementation will follow our established API patterns
    }
}

export const userService = new UserService();
```

## 🧪 Testing Patterns (Copilot-Enhanced)

### **Context-Rich Test Structure**
```csharp
// File: tests/Features/UserManagement/UserServiceTests.cs
namespace Ikhtibar.Tests.Features.UserManagement
{
    /// <summary>
    /// Comprehensive test suite for UserService business logic.
    /// Tests all CRUD operations, validation rules, and error scenarios.
    /// Uses AAA pattern (Arrange, Act, Assert) with descriptive test names.
    /// Includes integration with mocked dependencies and database.
    /// </summary>
    public class UserServiceTests : TestBase
    {
        // Copilot understands testing patterns and will suggest:
        // - Proper test setup with mocked dependencies
        // - Comprehensive test scenarios including edge cases
        // - Descriptive test names following our conventions
        // - Proper assertion patterns with meaningful error messages
        
        /// <summary>
        /// Test: Creating user with valid data should return user DTO with generated ID
        /// Scenario: Happy path with all required fields provided
        /// Expected: User created successfully with hashed password and welcome email sent
        /// </summary>
        [Test]
        public async Task CreateUserAsync_Should_ReturnUserDto_When_ValidDataProvided()
        {
            // Arrange - Copilot will suggest proper test data setup
            // Act - Call method under test
            // Assert - Verify expected behavior and side effects
        }
        
        /// <summary>
        /// Test: Creating user with existing email should throw EmailAlreadyExistsException
        /// Scenario: Duplicate email validation
        /// Expected: Exception with appropriate message and no database changes
        /// </summary>
        [Test]
        public async Task CreateUserAsync_Should_ThrowEmailAlreadyExistsException_When_EmailExists()
        {
            // Copilot will implement following our exception testing patterns
        }
    }
}
```

## 🌐 Internationalization Context (Copilot-Enhanced)

### **i18n Patterns for AI Understanding**
```typescript
// File: src/modules/user-management/locales/en.ts
/**
 * English translations for user management module
 * 
 * Structure:
 * - Nested keys for logical grouping
 * - Consistent naming conventions across modules
 * - Support for interpolation and pluralization
 * - Context-aware translations for better UX
 */
export const userManagementTranslations = {
    // User interface labels
    labels: {
        name: 'Name',
        email: 'Email Address',
        role: 'User Role',
        status: 'Account Status',
        createdAt: 'Created Date',
        lastLogin: 'Last Login'
    },
    
    // Action buttons and links
    actions: {
        create: 'Create User',
        edit: 'Edit User',
        delete: 'Delete User',
        activate: 'Activate Account',
        deactivate: 'Deactivate Account'
    },
    
    // Validation and error messages
    validation: {
        nameRequired: 'Name is required',
        emailRequired: 'Email address is required',
        emailInvalid: 'Please enter a valid email address',
        passwordMinLength: 'Password must be at least 8 characters long'
    },
    
    // Success messages
    success: {
        userCreated: 'User created successfully',
        userUpdated: 'User updated successfully',
        userDeleted: 'User deleted successfully'
    }
};
```

```typescript
// File: src/modules/user-management/locales/ar.ts
/**
 * Arabic translations with RTL considerations
 * Includes cultural adaptations and proper Arabic grammar
 */
export const userManagementTranslationsAr = {
    // Copilot will understand the structure and suggest appropriate Arabic translations
    // following our established RTL and cultural adaptation patterns
};
```

## 🚀 Performance Optimization (Copilot Context)

### **React Performance Patterns**
```typescript
/**
 * Performance-optimized component following our established patterns
 * 
 * Optimizations:
 * - React.memo for preventing unnecessary re-renders
 * - useCallback for stable function references
 * - useMemo for expensive calculations
 * - Lazy loading for large datasets
 * - Virtual scrolling for lists
 */

import React, { memo, useMemo, useCallback } from 'react';

export const OptimizedUserList: React.FC<UserListProps> = memo(({ 
    users, 
    onUserSelect, 
    filters 
}) => {
    // Expensive filtering operation memoized
    const filteredUsers = useMemo(() => {
        return users.filter(user => matchesFilters(user, filters));
    }, [users, filters]);
    
    // Stable callback reference to prevent child re-renders
    const handleUserSelect = useCallback((userId: string) => {
        onUserSelect(userId);
    }, [onUserSelect]);
    
    // Copilot will suggest implementation following our performance patterns
});
```

## 📝 Documentation Standards (Copilot-Enhanced)

### **Self-Documenting Code Patterns**
- Use XML documentation for all public APIs
- Include usage examples in complex methods
- Document business rules and constraints
- Explain non-obvious implementation decisions
- Provide troubleshooting guidance for common issues

### **COPILOT.md Integration**
Create a `COPILOT.md` file in repository root with:
- Project architecture overview
- Common patterns and conventions
- Code style guidelines
- Testing strategies
- Deployment procedures
- Troubleshooting guides
- Support both English and Arabic languages
- Use resource files for backend localization
- Handle RTL (Right-to-Left) layout for Arabic

## Common Patterns to Follow
1. **Error Handling**: Always implement proper error handling with meaningful error messages
2. **Validation**: Validate input at both frontend and backend levels
3. **Logging**: Add comprehensive logging for debugging and monitoring
4. **Documentation**: Document APIs and complex business logic
5. **Consistency**: Follow established patterns and conventions throughout the project

## Cross-Platform Validation Scripts

Ikhtibar uses both PowerShell and Bash scripts for validation to ensure proper project setup across all developer environments.

### PowerShell and Bash Script Equivalence

These scripts are designed to be functionally equivalent across platforms. Each script set performs the same operations but is optimized for Windows (PowerShell) or macOS/Linux (Bash) environments.

#### Project Validation Script

```powershell
# File: scripts/validate-project.ps1
<#
.SYNOPSIS
    Validates the Ikhtibar project setup and configuration.
.DESCRIPTION
    Checks for required dependencies, configuration files, and environment setup.
    Runs validation tests to ensure the project is correctly configured.
.EXAMPLE
    ./scripts/validate-project.ps1
#>

# Configure script behavior
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# Define color outputs for consistent messaging
function Write-Success($Message) {
    Write-Host "✅ $Message" -ForegroundColor Green
}

function Write-Error($Message) {
    Write-Host "❌ $Message" -ForegroundColor Red
}

function Write-Info($Message) {
    Write-Host "ℹ️ $Message" -ForegroundColor Cyan
}

function Write-Warning($Message) {
    Write-Host "⚠️ $Message" -ForegroundColor Yellow
}

# Display header
Write-Host "=================================================" -ForegroundColor Blue
Write-Host "          Ikhtibar Project Validation             " -ForegroundColor Blue
Write-Host "=================================================" -ForegroundColor Blue

# Track validation status
$ValidationPassed = $true

# Validate .NET SDK
Write-Info "Checking .NET SDK..."
try {
    $dotnetVersion = (dotnet --version)
    if (-not $dotnetVersion) {
        Write-Error "Failed to detect .NET SDK version."
        $ValidationPassed = $false
    } elseif ([version]$dotnetVersion -lt [version]"6.0.0") {
        Write-Warning ".NET SDK version $dotnetVersion detected. Version 6.0 or higher is recommended."
    } else {
        Write-Success ".NET SDK version $dotnetVersion detected."
    }
} catch {
    Write-Error "Failed to detect .NET SDK: $_"
    $ValidationPassed = $false
}

# Validate Node.js
Write-Info "Checking Node.js..."
try {
    $nodeVersion = (node --version).TrimStart("v")
    if (-not $nodeVersion) {
        Write-Error "Failed to detect Node.js version."
        $ValidationPassed = $false
    } elseif ([version]$nodeVersion -lt [version]"16.0.0") {
        Write-Warning "Node.js version $nodeVersion detected. Version 16.0 or higher is recommended."
    } else {
        Write-Success "Node.js version $nodeVersion detected."
    }
} catch {
    Write-Error "Failed to detect Node.js: $_"
    $ValidationPassed = $false
}

# Validate pnpm
Write-Info "Checking pnpm..."
try {
    $pnpmVersion = (pnpm --version)
    if (-not $pnpmVersion) {
        Write-Error "Failed to detect pnpm version."
        $ValidationPassed = $false
    } else {
        Write-Success "pnpm version $pnpmVersion detected."
    }
} catch {
    Write-Error "pnpm not installed. Please install pnpm using: npm install -g pnpm"
    $ValidationPassed = $false
}

# Check required configuration files
Write-Info "Checking required configuration files..."

$RequiredFiles = @(
    "./backend/src/Ikhtibar.Api/appsettings.json",
    "./backend/src/Ikhtibar.Api/appsettings.Development.json",
    "./frontend/.env",
    "./frontend/tsconfig.json"
)

foreach ($file in $RequiredFiles) {
    if (Test-Path $file) {
        Write-Success "Found configuration file: $file"
    } else {
        Write-Error "Missing required configuration file: $file"
        $ValidationPassed = $false
    }
}

# Validate database connection
Write-Info "Validating database connection settings..."
if (Test-Path "./backend/src/Ikhtibar.Api/appsettings.Development.json") {
    try {
        $appsettings = Get-Content "./backend/src/Ikhtibar.Api/appsettings.Development.json" | ConvertFrom-Json
        $connectionString = $appsettings.ConnectionStrings.DefaultConnection
        if (-not $connectionString) {
            Write-Warning "Database connection string not found in appsettings.Development.json"
        } else {
            if ($connectionString -match "Server=(.*);Database=(.*);") {
                Write-Success "Database connection string configured: Server=$($Matches[1]), Database=$($Matches[2])"
            } else {
                Write-Warning "Database connection string format may be incorrect."
            }
        }
    } catch {
        Write-Error "Failed to parse appsettings.Development.json: $_"
    }
} else {
    Write-Warning "Skipping database connection check (appsettings.Development.json not found)"
}

# Check frontend environment variables
Write-Info "Checking frontend environment variables..."
if (Test-Path "./frontend/.env") {
    $envContent = Get-Content "./frontend/.env"
    $requiredVars = @("VITE_API_URL", "VITE_APP_TITLE")
    $missingVars = @()
    
    foreach ($var in $requiredVars) {
        if (-not ($envContent -match "$var=")) {
            $missingVars += $var
        }
    }
    
    if ($missingVars.Count -gt 0) {
        Write-Warning "Missing environment variables in .env file: $($missingVars -join ', ')"
    } else {
        Write-Success "All required environment variables are set."
    }
} else {
    Write-Warning "Skipping frontend environment check (.env not found)"
}

# Display validation summary
Write-Host "`n-------------------------------------------------" -ForegroundColor Blue
if ($ValidationPassed) {
    Write-Success "All validation checks passed! Your development environment is ready."
} else {
    Write-Error "Some validation checks failed. Please fix the issues above before proceeding."
    exit 1
}

Write-Host "`nNext steps:" -ForegroundColor Cyan
Write-Host "1. Start backend: cd backend && dotnet run --project src/Ikhtibar.Api" -ForegroundColor White
Write-Host "2. Start frontend: cd frontend && pnpm dev" -ForegroundColor White
```

```bash
#!/bin/bash
# File: scripts/validate-project.sh
#
# Validates the Ikhtibar project setup and configuration.
# Checks for required dependencies, configuration files, and environment setup.
# Runs validation tests to ensure the project is correctly configured.
#
# Usage: ./scripts/validate-project.sh

# Exit on error
set -e

# Define color outputs for consistent messaging
GREEN='\033[0;32m'
RED='\033[0;31m'
CYAN='\033[0;36m'
YELLOW='\033[0;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

function echo_success() {
  echo -e "${GREEN}✅ $1${NC}"
}

function echo_error() {
  echo -e "${RED}❌ $1${NC}"
}

function echo_info() {
  echo -e "${CYAN}ℹ️ $1${NC}"
}

function echo_warning() {
  echo -e "${YELLOW}⚠️ $1${NC}"
}

# Display header
echo -e "${BLUE}==================================================${NC}"
echo -e "${BLUE}          Ikhtibar Project Validation             ${NC}"
echo -e "${BLUE}==================================================${NC}"

# Track validation status
VALIDATION_PASSED=true

# Validate .NET SDK
echo_info "Checking .NET SDK..."
if command -v dotnet &> /dev/null; then
  DOTNET_VERSION=$(dotnet --version)
  DOTNET_VERSION_MAJOR=$(echo "$DOTNET_VERSION" | cut -d. -f1)
  if [ "$DOTNET_VERSION_MAJOR" -lt 6 ]; then
    echo_warning ".NET SDK version $DOTNET_VERSION detected. Version 6.0 or higher is recommended."
  else
    echo_success ".NET SDK version $DOTNET_VERSION detected."
  fi
else
  echo_error "Failed to detect .NET SDK."
  VALIDATION_PASSED=false
fi

# Validate Node.js
echo_info "Checking Node.js..."
if command -v node &> /dev/null; then
  NODE_VERSION=$(node --version | cut -d'v' -f2)
  NODE_VERSION_MAJOR=$(echo "$NODE_VERSION" | cut -d. -f1)
  if [ "$NODE_VERSION_MAJOR" -lt 16 ]; then
    echo_warning "Node.js version $NODE_VERSION detected. Version 16.0 or higher is recommended."
  else
    echo_success "Node.js version $NODE_VERSION detected."
  fi
else
  echo_error "Failed to detect Node.js."
  VALIDATION_PASSED=false
fi

# Validate pnpm
echo_info "Checking pnpm..."
if command -v pnpm &> /dev/null; then
  PNPM_VERSION=$(pnpm --version)
  echo_success "pnpm version $PNPM_VERSION detected."
else
  echo_error "pnpm not installed. Please install pnpm using: npm install -g pnpm"
  VALIDATION_PASSED=false
fi

# Check required configuration files
echo_info "Checking required configuration files..."

REQUIRED_FILES=(
  "./backend/src/Ikhtibar.Api/appsettings.json"
  "./backend/src/Ikhtibar.Api/appsettings.Development.json"
  "./frontend/.env"
  "./frontend/tsconfig.json"
)

for file in "${REQUIRED_FILES[@]}"; do
  if [ -f "$file" ]; then
    echo_success "Found configuration file: $file"
  else
    echo_error "Missing required configuration file: $file"
    VALIDATION_PASSED=false
  fi
done

# Validate database connection
echo_info "Validating database connection settings..."
if [ -f "./backend/src/Ikhtibar.Api/appsettings.Development.json" ]; then
  if command -v jq &> /dev/null; then
    CONNECTION_STRING=$(jq -r '.ConnectionStrings.DefaultConnection' "./backend/src/Ikhtibar.Api/appsettings.Development.json")
    if [ "$CONNECTION_STRING" = "null" ]; then
      echo_warning "Database connection string not found in appsettings.Development.json"
    else
      SERVER=$(echo "$CONNECTION_STRING" | grep -o 'Server=[^;]*' | cut -d= -f2)
      DATABASE=$(echo "$CONNECTION_STRING" | grep -o 'Database=[^;]*' | cut -d= -f2)
      if [ -n "$SERVER" ] && [ -n "$DATABASE" ]; then
        echo_success "Database connection string configured: Server=$SERVER, Database=$DATABASE"
      else
        echo_warning "Database connection string format may be incorrect."
      fi
    fi
  else
    echo_warning "jq not installed. Skipping detailed database connection check."
    grep -q "ConnectionStrings" "./backend/src/Ikhtibar.Api/appsettings.Development.json" && echo_success "ConnectionStrings section found in appsettings.Development.json" || echo_warning "ConnectionStrings section might be missing in appsettings.Development.json"
  fi
else
  echo_warning "Skipping database connection check (appsettings.Development.json not found)"
fi

# Check frontend environment variables
echo_info "Checking frontend environment variables..."
if [ -f "./frontend/.env" ]; then
  REQUIRED_VARS=("VITE_API_URL" "VITE_APP_TITLE")
  MISSING_VARS=()
  
  for var in "${REQUIRED_VARS[@]}"; do
    if ! grep -q "^$var=" "./frontend/.env"; then
      MISSING_VARS+=("$var")
    fi
  done
  
  if [ ${#MISSING_VARS[@]} -gt 0 ]; then
    echo_warning "Missing environment variables in .env file: ${MISSING_VARS[*]}"
  else
    echo_success "All required environment variables are set."
  fi
else
  echo_warning "Skipping frontend environment check (.env not found)"
fi

# Display validation summary
echo -e "\n${BLUE}-------------------------------------------------${NC}"
if [ "$VALIDATION_PASSED" = true ]; then
  echo_success "All validation checks passed! Your development environment is ready."
else
  echo_error "Some validation checks failed. Please fix the issues above before proceeding."
  exit 1
fi

echo -e "\n${CYAN}Next steps:${NC}"
echo -e "1. Start backend: cd backend && dotnet run --project src/Ikhtibar.Api"
echo -e "2. Start frontend: cd frontend && pnpm dev"
```

#### Build Validation Script

```powershell
# File: scripts/build-validate.ps1
<#
.SYNOPSIS
    Validates build artifacts for the Ikhtibar project.
.DESCRIPTION
    Checks that all required build artifacts are present and correctly structured.
    Verifies frontend bundle size, backend DLLs, and database migration scripts.
.EXAMPLE
    ./scripts/build-validate.ps1
#>

# Configure script behavior
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# Define color outputs for consistent messaging
function Write-Success($Message) {
    Write-Host "✅ $Message" -ForegroundColor Green
}

function Write-Error($Message) {
    Write-Host "❌ $Message" -ForegroundColor Red
}

function Write-Info($Message) {
    Write-Host "ℹ️ $Message" -ForegroundColor Cyan
}

function Write-Warning($Message) {
    Write-Host "⚠️ $Message" -ForegroundColor Yellow
}

# Display header
Write-Host "=================================================" -ForegroundColor Blue
Write-Host "          Ikhtibar Build Validation              " -ForegroundColor Blue
Write-Host "=================================================" -ForegroundColor Blue

# Track validation status
$ValidationPassed = $true

# Check if builds exist
$BackendBuildPath = "./backend/src/Ikhtibar.Api/bin/Release/net6.0/publish"
$FrontendBuildPath = "./frontend/dist"

# Validate backend build
Write-Info "Checking backend build artifacts..."
if (-not (Test-Path $BackendBuildPath)) {
    Write-Error "Backend build artifacts not found at: $BackendBuildPath"
    Write-Info "Run: dotnet publish ./backend/src/Ikhtibar.Api -c Release"
    $ValidationPassed = $false
} else {
    # Check critical backend files
    $RequiredBackendFiles = @(
        "Ikhtibar.Api.dll",
        "Ikhtibar.Core.dll",
        "Ikhtibar.Infrastructure.dll",
        "appsettings.json",
        "web.config"
    )
    
    $MissingBackendFiles = @()
    foreach ($file in $RequiredBackendFiles) {
        $filePath = Join-Path -Path $BackendBuildPath -ChildPath $file
        if (-not (Test-Path $filePath)) {
            $MissingBackendFiles += $file
        }
    }
    
    if ($MissingBackendFiles.Count -gt 0) {
        Write-Error "Missing required backend files: $($MissingBackendFiles -join ', ')"
        $ValidationPassed = $false
    } else {
        Write-Success "All required backend build files are present."
        
        # Check file sizes for suspicious builds
        $apiDllPath = Join-Path -Path $BackendBuildPath -ChildPath "Ikhtibar.Api.dll"
        $apiDllSize = (Get-Item $apiDllPath).Length / 1MB
        
        if ($apiDllSize -lt 0.1) {
            Write-Warning "Ikhtibar.Api.dll size is suspiciously small ($($apiDllSize.ToString('0.00')) MB)"
        } else {
            Write-Success "Ikhtibar.Api.dll size looks good ($($apiDllSize.ToString('0.00')) MB)"
        }
    }
}

# Validate frontend build
Write-Info "Checking frontend build artifacts..."
if (-not (Test-Path $FrontendBuildPath)) {
    Write-Error "Frontend build artifacts not found at: $FrontendBuildPath"
    Write-Info "Run: cd frontend && pnpm build"
    $ValidationPassed = $false
} else {
    # Check critical frontend files
    $RequiredFrontendFiles = @(
        "index.html",
        "assets"
    )
    
    $MissingFrontendFiles = @()
    foreach ($file in $RequiredFrontendFiles) {
        $filePath = Join-Path -Path $FrontendBuildPath -ChildPath $file
        if (-not (Test-Path $filePath)) {
            $MissingFrontendFiles += $file
        }
    }
    
    if ($MissingFrontendFiles.Count -gt 0) {
        Write-Error "Missing required frontend files: $($MissingFrontendFiles -join ', ')"
        $ValidationPassed = $false
    } else {
        Write-Success "All required frontend build files are present."
        
        # Check for JavaScript files
        $JsFiles = Get-ChildItem -Path "$FrontendBuildPath/assets" -Filter "*.js" -Recurse
        if ($JsFiles.Count -eq 0) {
            Write-Error "No JavaScript files found in frontend build."
            $ValidationPassed = $false
        } else {
            $TotalJsSize = ($JsFiles | Measure-Object -Property Length -Sum).Sum / 1MB
            
            Write-Success "Frontend JS bundle size: $($TotalJsSize.ToString('0.00')) MB"
            
            if ($TotalJsSize -gt 5) {
                Write-Warning "Frontend bundle size is large (>5MB). Consider code splitting or optimization."
            }
        }
    }
}

# Check for database migrations
Write-Info "Checking database migrations..."
$MigrationPath = "./backend/src/Ikhtibar.Infrastructure/Migrations"
if (-not (Test-Path $MigrationPath)) {
    Write-Warning "Migrations folder not found at: $MigrationPath"
} else {
    $MigrationFiles = Get-ChildItem -Path $MigrationPath -Filter "*.cs" | Where-Object { $_.Name -notlike "*ModelSnapshot.cs" }
    if ($MigrationFiles.Count -eq 0) {
        Write-Warning "No migration files found. Database schema may not be versioned."
    } else {
        Write-Success "Found $($MigrationFiles.Count) database migrations."
    }
}

# Display validation summary
Write-Host "`n-------------------------------------------------" -ForegroundColor Blue
if ($ValidationPassed) {
    Write-Success "All build validation checks passed! Your application is ready for deployment."
} else {
    Write-Error "Some build validation checks failed. Please fix the issues above before deploying."
    exit 1
}
```

```bash
#!/bin/bash
# File: scripts/build-validate.sh
#
# Validates build artifacts for the Ikhtibar project.
# Checks that all required build artifacts are present and correctly structured.
# Verifies frontend bundle size, backend DLLs, and database migration scripts.
#
# Usage: ./scripts/build-validate.sh

# Exit on error
set -e

# Define color outputs for consistent messaging
GREEN='\033[0;32m'
RED='\033[0;31m'
CYAN='\033[0;36m'
YELLOW='\033[0;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

function echo_success() {
  echo -e "${GREEN}✅ $1${NC}"
}

function echo_error() {
  echo -e "${RED}❌ $1${NC}"
}

function echo_info() {
  echo -e "${CYAN}ℹ️ $1${NC}"
}

function echo_warning() {
  echo -e "${YELLOW}⚠️ $1${NC}"
}

# Display header
echo -e "${BLUE}==================================================${NC}"
echo -e "${BLUE}          Ikhtibar Build Validation              ${NC}"
echo -e "${BLUE}==================================================${NC}"

# Track validation status
VALIDATION_PASSED=true

# Check if builds exist
BACKEND_BUILD_PATH="./backend/src/Ikhtibar.Api/bin/Release/net6.0/publish"
FRONTEND_BUILD_PATH="./frontend/dist"

# Validate backend build
echo_info "Checking backend build artifacts..."
if [ ! -d "$BACKEND_BUILD_PATH" ]; then
  echo_error "Backend build artifacts not found at: $BACKEND_BUILD_PATH"
  echo_info "Run: dotnet publish ./backend/src/Ikhtibar.Api -c Release"
  VALIDATION_PASSED=false
else
  # Check critical backend files
  REQUIRED_BACKEND_FILES=(
    "Ikhtibar.Api.dll"
    "Ikhtibar.Core.dll"
    "Ikhtibar.Infrastructure.dll"
    "appsettings.json"
    "web.config"
  )
  
  MISSING_BACKEND_FILES=()
  for file in "${REQUIRED_BACKEND_FILES[@]}"; do
    if [ ! -f "$BACKEND_BUILD_PATH/$file" ]; then
      MISSING_BACKEND_FILES+=("$file")
    fi
  done
  
  if [ ${#MISSING_BACKEND_FILES[@]} -gt 0 ]; then
    echo_error "Missing required backend files: ${MISSING_BACKEND_FILES[*]}"
    VALIDATION_PASSED=false
  else
    echo_success "All required backend build files are present."
    
    # Check file sizes for suspicious builds
    API_DLL_PATH="$BACKEND_BUILD_PATH/Ikhtibar.Api.dll"
    API_DLL_SIZE=$(du -m "$API_DLL_PATH" | cut -f1)
    
    if (( $(echo "$API_DLL_SIZE < 0.1" | bc -l) )); then
      echo_warning "Ikhtibar.Api.dll size is suspiciously small (${API_DLL_SIZE} MB)"
    else
      echo_success "Ikhtibar.Api.dll size looks good (${API_DLL_SIZE} MB)"
    fi
  fi
fi

# Validate frontend build
echo_info "Checking frontend build artifacts..."
if [ ! -d "$FRONTEND_BUILD_PATH" ]; then
  echo_error "Frontend build artifacts not found at: $FRONTEND_BUILD_PATH"
  echo_info "Run: cd frontend && pnpm build"
  VALIDATION_PASSED=false
else
  # Check critical frontend files
  REQUIRED_FRONTEND_FILES=(
    "index.html"
    "assets"
  )
  
  MISSING_FRONTEND_FILES=()
  for file in "${REQUIRED_FRONTEND_FILES[@]}"; do
    if [ ! -e "$FRONTEND_BUILD_PATH/$file" ]; then
      MISSING_FRONTEND_FILES+=("$file")
    fi
  done
  
  if [ ${#MISSING_FRONTEND_FILES[@]} -gt 0 ]; then
    echo_error "Missing required frontend files: ${MISSING_FRONTEND_FILES[*]}"
    VALIDATION_PASSED=false
  else
    echo_success "All required frontend build files are present."
    
    # Check for JavaScript files
    JS_FILES=$(find "$FRONTEND_BUILD_PATH/assets" -name "*.js" | wc -l)
    if [ "$JS_FILES" -eq 0 ]; then
      echo_error "No JavaScript files found in frontend build."
      VALIDATION_PASSED=false
    else
      # Calculate total JS size in MB
      if command -v du &> /dev/null; then
        TOTAL_JS_SIZE=$(du -c "$FRONTEND_BUILD_PATH/assets"/*.js 2>/dev/null | grep total | awk '{print $1/1024}')
        
        echo_success "Frontend JS bundle size: ${TOTAL_JS_SIZE} MB"
        
        if (( $(echo "$TOTAL_JS_SIZE > 5" | bc -l) )); then
          echo_warning "Frontend bundle size is large (>5MB). Consider code splitting or optimization."
        fi
      else
        echo_success "Found $JS_FILES JavaScript files in the build."
      fi
    fi
  fi
fi

# Check for database migrations
echo_info "Checking database migrations..."
MIGRATION_PATH="./backend/src/Ikhtibar.Infrastructure/Migrations"
if [ ! -d "$MIGRATION_PATH" ]; then
  echo_warning "Migrations folder not found at: $MIGRATION_PATH"
else
  MIGRATION_FILES=$(find "$MIGRATION_PATH" -name "*.cs" | grep -v "ModelSnapshot.cs" | wc -l)
  if [ "$MIGRATION_FILES" -eq 0 ]; then
    echo_warning "No migration files found. Database schema may not be versioned."
  else
    echo_success "Found $MIGRATION_FILES database migrations."
  fi
fi

# Display validation summary
echo -e "\n${BLUE}-------------------------------------------------${NC}"
if [ "$VALIDATION_PASSED" = true ]; then
  echo_success "All build validation checks passed! Your application is ready for deployment."
else
  echo_error "Some build validation checks failed. Please fix the issues above before deploying."
  exit 1
fi
```

#### Development Environment Validation Script

```powershell
# File: scripts/check-environment.ps1
<#
.SYNOPSIS
    Validates the development environment for Ikhtibar project contributors.
.DESCRIPTION
    Checks for all required tools, dependencies, and environment variables.
    Suggests fixes for common configuration issues.
.EXAMPLE
    ./scripts/check-environment.ps1
#>

# Configure script behavior
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# Define color outputs for consistent messaging
function Write-Success($Message) {
    Write-Host "✅ $Message" -ForegroundColor Green
}

function Write-Error($Message) {
    Write-Host "❌ $Message" -ForegroundColor Red
}

function Write-Info($Message) {
    Write-Host "ℹ️ $Message" -ForegroundColor Cyan
}

function Write-Warning($Message) {
    Write-Host "⚠️ $Message" -ForegroundColor Yellow
}

# Check if command exists
function Test-CommandExists {
    param ($command)
    
    $oldPreference = $ErrorActionPreference
    $ErrorActionPreference = 'stop'
    
    try {
        if (Get-Command $command) { return $true }
    } catch {
        return $false
    } finally {
        $ErrorActionPreference = $oldPreference
    }
}

# Display header
Write-Host "=================================================" -ForegroundColor Blue
Write-Host "      Ikhtibar Development Environment Check      " -ForegroundColor Blue
Write-Host "=================================================" -ForegroundColor Blue

# Required tools and versions
$requiredTools = @(
    @{ Name = "dotnet"; MinVersion = "6.0.0"; VersionCommand = "dotnet --version"; VersionRegex = "^(.+)$" },
    @{ Name = "node"; MinVersion = "16.0.0"; VersionCommand = "node --version"; VersionRegex = "v(.+)$" },
    @{ Name = "pnpm"; MinVersion = "6.0.0"; VersionCommand = "pnpm --version"; VersionRegex = "^(.+)$" },
    @{ Name = "git"; MinVersion = "2.0.0"; VersionCommand = "git --version"; VersionRegex = "git version (.+?)(?: |$)" },
    @{ Name = "docker"; MinVersion = "20.0.0"; VersionCommand = "docker --version"; VersionRegex = "Docker version (.+?)," }
)

$allRequirementsMet = $true

# Check each required tool
foreach ($tool in $requiredTools) {
    Write-Info "Checking $($tool.Name)..."
    
    if (-not (Test-CommandExists $tool.Name)) {
        Write-Error "$($tool.Name) is not installed or not in PATH"
        $allRequirementsMet = $false
        continue
    }
    
    try {
        $versionOutput = Invoke-Expression $tool.VersionCommand
        if ($versionOutput -match $tool.VersionRegex) {
            $version = $matches[1]
            
            if ([version]$version -lt [version]$tool.MinVersion) {
                Write-Warning "$($tool.Name) version $version is installed, but version $($tool.MinVersion) or higher is recommended"
            } else {
                Write-Success "$($tool.Name) version $version is installed"
            }
        } else {
            Write-Warning "Could not determine $($tool.Name) version"
        }
    } catch {
        Write-Error "Error checking $($tool.Name) version: $_"
        $allRequirementsMet = $false
    }
}

# Check SQL Server
Write-Info "Checking SQL Server connectivity..."
try {
    if (Test-CommandExists "sqlcmd") {
        Write-Success "SQL Server command line tools are installed"
        
        # Check if SQL Server is running
        if ((Get-Service -Name MSSQLSERVER -ErrorAction SilentlyContinue).Status -eq 'Running') {
            Write-Success "SQL Server service is running"
        } else {
            Write-Warning "SQL Server service is not running or not installed"
        }
    } else {
        Write-Warning "SQL Server command line tools not found. This may be needed for database operations."
    }
} catch {
    Write-Warning "Could not check SQL Server status"
}

# Check .NET SDK tools
Write-Info "Checking .NET SDK tools..."
$dotnetTools = @(
    "dotnet-ef"
)

foreach ($tool in $dotnetTools) {
    try {
        $toolOutput = & dotnet tool list --global | Select-String -Pattern $tool -SimpleMatch
        if ($toolOutput) {
            Write-Success "$tool is installed globally"
        } else {
            Write-Warning "$tool is not installed. Consider installing with: dotnet tool install -g $tool"
        }
    } catch {
        Write-Warning "Could not check if $tool is installed: $_"
    }
}

# Check environment variables
Write-Info "Checking environment variables..."
$requiredEnvVars = @(
    "ASPNETCORE_ENVIRONMENT"
)

foreach ($var in $requiredEnvVars) {
    if (Test-Path "env:$var") {
        Write-Success "$var is set to: $([Environment]::GetEnvironmentVariable($var))"
    } else {
        Write-Warning "$var environment variable is not set"
    }
}

# Check Git configuration
Write-Info "Checking Git configuration..."
try {
    $gitUser = git config --get user.name
    $gitEmail = git config --get user.email
    
    if ($gitUser) {
        Write-Success "Git user name is configured as: $gitUser"
    } else {
        Write-Warning "Git user name is not configured. Use: git config --global user.name 'Your Name'"
    }
    
    if ($gitEmail) {
        Write-Success "Git email is configured as: $gitEmail"
    } else {
        Write-Warning "Git email is not configured. Use: git config --global user.email 'your.email@example.com'"
    }
} catch {
    Write-Warning "Could not check Git configuration: $_"
}

# Display summary
Write-Host "`n-------------------------------------------------" -ForegroundColor Blue
if ($allRequirementsMet) {
    Write-Success "Your development environment meets all the basic requirements!"
} else {
    Write-Warning "Some requirements are missing. Please address the issues above."
}

# Show next steps
Write-Host "`nNext steps:" -ForegroundColor Cyan
Write-Host "1. Clone the repository (if not already done): git clone https://github.com/your-org/Ikhtibar.git"
Write-Host "2. Install backend dependencies: cd backend && dotnet restore"
Write-Host "3. Install frontend dependencies: cd frontend && pnpm install"
Write-Host "4. Run project validation: ./scripts/validate-project.ps1"
```

```bash
#!/bin/bash
# File: scripts/check-environment.sh
#
# Validates the development environment for Ikhtibar project contributors.
# Checks for all required tools, dependencies, and environment variables.
# Suggests fixes for common configuration issues.
#
# Usage: ./scripts/check-environment.sh

# Exit on error
set -e

# Define color outputs for consistent messaging
GREEN='\033[0;32m'
RED='\033[0;31m'
CYAN='\033[0;36m'
YELLOW='\033[0;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

function echo_success() {
  echo -e "${GREEN}✅ $1${NC}"
}

function echo_error() {
  echo -e "${RED}❌ $1${NC}"
}

function echo_info() {
  echo -e "${CYAN}ℹ️ $1${NC}"
}

function echo_warning() {
  echo -e "${YELLOW}⚠️ $1${NC}"
}

# Check if command exists
function command_exists() {
  command -v "$1" &> /dev/null
}

# Compare versions
function version_lt() {
  [ "$(printf '%s\n' "$1" "$2" | sort -V | head -n1)" = "$1" ] && [ "$1" != "$2" ]
}

# Display header
echo -e "${BLUE}==================================================${NC}"
echo -e "${BLUE}      Ikhtibar Development Environment Check      ${NC}"
echo -e "${BLUE}==================================================${NC}"

# Required tools and versions
declare -A REQUIRED_TOOLS=(
  ["dotnet"]="6.0.0"
  ["node"]="16.0.0"
  ["pnpm"]="6.0.0"
  ["git"]="2.0.0"
  ["docker"]="20.0.0"
)

ALL_REQUIREMENTS_MET=true

# Check each required tool
for tool in "${!REQUIRED_TOOLS[@]}"; do
  echo_info "Checking $tool..."
  
  if ! command_exists "$tool"; then
    echo_error "$tool is not installed or not in PATH"
    ALL_REQUIREMENTS_MET=false
    continue
  fi
  
  case "$tool" in
    dotnet)
      VERSION=$($tool --version)
      ;;
    node)
      VERSION=$($tool --version | cut -c2-)
      ;;
    pnpm)
      VERSION=$($tool --version)
      ;;
    git)
      VERSION=$($tool --version | sed -E 's/git version ([0-9]+(\.[0-9]+)*).*/\1/')
      ;;
    docker)
      VERSION=$($tool --version | sed -E 's/Docker version ([0-9]+(\.[0-9]+)*).*/\1/')
      ;;
    *)
      VERSION="unknown"
      ;;
  esac
  
  MIN_VERSION="${REQUIRED_TOOLS[$tool]}"
  
  if version_lt "$VERSION" "$MIN_VERSION"; then
    echo_warning "$tool version $VERSION is installed, but version $MIN_VERSION or higher is recommended"
  else
    echo_success "$tool version $VERSION is installed"
  fi
done

# Check database access
echo_info "Checking database connectivity..."

if command_exists "psql"; then
  echo_success "PostgreSQL client tools are installed"
elif command_exists "sqlcmd"; then
  echo_success "SQL Server command line tools are installed"
else
  echo_warning "No SQL client tools found. This may be needed for database operations."
fi

# Check Docker status
if command_exists "docker"; then
  echo_info "Checking Docker status..."
  if docker info &>/dev/null; then
    echo_success "Docker daemon is running"
  else
    echo_warning "Docker daemon is not running"
  fi
fi

# Check .NET SDK tools
echo_info "Checking .NET SDK tools..."
DOTNET_TOOLS=(
  "dotnet-ef"
)

for tool in "${DOTNET_TOOLS[@]}"; do
  if dotnet tool list --global | grep -q "$tool"; then
    echo_success "$tool is installed globally"
  else
    echo_warning "$tool is not installed. Consider installing with: dotnet tool install -g $tool"
  fi
done

# Check environment variables
echo_info "Checking environment variables..."
REQUIRED_ENV_VARS=(
  "ASPNETCORE_ENVIRONMENT"
)

for var in "${REQUIRED_ENV_VARS[@]}"; do
  if [ -n "${!var}" ]; then
    echo_success "$var is set to: ${!var}"
  else
    echo_warning "$var environment variable is not set"
  fi
done

# Check Git configuration
echo_info "Checking Git configuration..."
GIT_USER=$(git config --get user.name 2>/dev/null || echo "")
GIT_EMAIL=$(git config --get user.email 2>/dev/null || echo "")

if [ -n "$GIT_USER" ]; then
  echo_success "Git user name is configured as: $GIT_USER"
else
  echo_warning "Git user name is not configured. Use: git config --global user.name 'Your Name'"
fi

if [ -n "$GIT_EMAIL" ]; then
  echo_success "Git email is configured as: $GIT_EMAIL"
else
  echo_warning "Git email is not configured. Use: git config --global user.email 'your.email@example.com'"
fi

# Display summary
echo -e "\n${BLUE}-------------------------------------------------${NC}"
if [ "$ALL_REQUIREMENTS_MET" = true ]; then
  echo_success "Your development environment meets all the basic requirements!"
else
  echo_warning "Some requirements are missing. Please address the issues above."
fi

# Show next steps
echo -e "\n${CYAN}Next steps:${NC}"
echo "1. Clone the repository (if not already done): git clone https://github.com/your-org/Ikhtibar.git"
echo "2. Install backend dependencies: cd backend && dotnet restore"
echo "3. Install frontend dependencies: cd frontend && pnpm install"
echo "4. Run project validation: ./scripts/validate-project.sh"
```
