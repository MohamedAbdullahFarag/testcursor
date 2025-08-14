# PRP: Role Management System Implementation

## 🎯 Objective
Implement a comprehensive role-based access control (RBAC) system for the Ikhtibar educational assessment platform, including role management, user-role assignments, permission mapping, and hierarchical role structures with complete CRUD operations and authorization enforcement.

## 📋 Why
- **Business Value**: Granular access control for educational assessment platform based on user responsibilities
- **User Impact**: Proper role segregation ensures users only access appropriate features (System Admin, Question Creator, Reviewer, Exam Manager, Supervisor, Student, Grader, Quality Reviewer)
- **Integration**: Foundation for authorization across all platform features and workflows
- **Security**: Implements principle of least privilege with role-based permission enforcement

## 🎯 What (User-Visible Behavior)
1. **Role Management**: Create, update, delete, and view system roles with descriptions and permissions
2. **User-Role Assignment**: Assign/remove roles to/from users with audit trail
3. **Permission Mapping**: Associate specific permissions with roles for fine-grained control
4. **Role Validation**: Prevent unauthorized access based on user roles and permissions
5. **Role Discovery**: Query user roles, role users, and role existence checks

### Success Criteria
- [ ] System administrators can create and manage roles with appropriate permissions
- [ ] Users can be assigned multiple roles with proper validation
- [ ] Role-based authorization protects all API endpoints appropriately
- [ ] Permission changes are immediately reflected in user access
- [ ] Role assignment audit trail is maintained for compliance
- [ ] Frontend displays appropriate UI elements based on user roles

## 📚 All Needed Context

### 🏗️ Architecture Pattern References
```yaml
existing_implementations:
  - file: backend/Ikhtibar.API/Controllers/UserManagement/RolesController.cs
    why: Complete CRUD operations for role management with REST patterns
    
  - file: backend/Ikhtibar.API/Controllers/UserManagement/UserRolesController.cs
    why: User-role assignment operations and relationship management
    
  - file: backend/Ikhtibar.Core/Services/Implementations/RoleService.cs
    why: Business logic for role operations following SRP patterns
    
  - file: backend/Ikhtibar.Core/Services/Interfaces/IUserRoleService.cs
    why: Interface definitions for user-role relationship operations

database_schema:
  - file: .github/copilot/requirements/data.sql
    why: Role definitions, permissions, and default role assignments
    
  - file: backend/Ikhtibar.Core/Entities/Role.cs
    why: Role entity structure with relationships to users and permissions
    
  - file: backend/Ikhtibar.Core/Entities/RolePermission.cs
    why: Junction entity for role-permission many-to-many relationships

guidelines:
  - file: .github/copilot/backend-guidelines.md
    why: SRP enforcement rules and repository separation patterns
    
  - file: .github/copilot/api-guidelines.md
    why: Authorization attributes and API design standards
```

### 🔧 Technical Implementation Context
```yaml
core_entities:
  - Role: Core role entity with code, name, description, system flag
  - UserRole: Junction entity for user-role assignments with audit fields
  - RolePermission: Junction entity for role-permission mappings
  - Permission: Individual permissions for specific actions/resources

role_types:
  system_roles:
    - system-admin: Full system control and configuration
    - reviewer: Question bank review and approval
    - creator: Question and content creation
    - exam-manager: Exam lifecycle management
    - supervisor: Exam session oversight and monitoring
    - student: Exam participation and result viewing
    - grader: Manual grading and assessment
    - quality-reviewer: Quality standards and compliance

dtos_and_contracts:
  - RoleDto: Role data transfer with permissions
  - CreateRoleDto: Role creation request
  - UpdateRoleDto: Role modification request
  - AssignRoleDto: User-role assignment request
  - UserRoleDto: User-role relationship data

business_rules:
  - System roles cannot be deleted
  - Users must have at least one role
  - Role codes must be unique
  - Permission assignments are additive
  - Role assignments require appropriate permissions
```

## 📝 Implementation Blueprint

### 🏛️ Backend Implementation (Following SRP)

#### 1. Role Management Service Layer
```csharp
// PATTERN: Service layer following SRP - ONLY role business logic
public interface IRoleService
{
    Task<RoleDto?> GetRoleAsync(int roleId);
    Task<RoleDto?> GetRoleByCodeAsync(string code);
    Task<IEnumerable<RoleDto>> GetAllRolesAsync();
    Task<IEnumerable<RoleDto>> GetSystemRolesAsync();
    Task<IEnumerable<RoleDto>> GetCustomRolesAsync();
    Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto);
    Task<RoleDto?> UpdateRoleAsync(int roleId, UpdateRoleDto updateRoleDto);
    Task<bool> DeleteRoleAsync(int roleId);
    Task<bool> RoleExistsAsync(int roleId);
    Task<bool> IsRoleCodeInUseAsync(string code, int? excludeRoleId = null);
}

// ✅ CORRECT: Focused role service
public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionService _permissionService;
    private readonly IMapper _mapper;
    private readonly ILogger<RoleService> _logger;
    
    // ONLY role-related business operations
    // NO user operations, NO data access logic, NO HTTP concerns
    
    public async Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto)
    {
        // ✅ Business validation and logic only
        await ValidateRoleCreation(createRoleDto);
        var role = _mapper.Map<Role>(createRoleDto);
        var createdRole = await _roleRepository.CreateAsync(role);
        return _mapper.Map<RoleDto>(createdRole);
    }
}
```

#### 2. User-Role Assignment Service
```csharp
// PATTERN: User-Role service following SRP - ONLY user-role relationship logic
public interface IUserRoleService
{
    Task<IEnumerable<RoleDto>> GetUserRolesAsync(int userId);
    Task<IEnumerable<UserDto>> GetRoleUsersAsync(int roleId);
    Task<bool> AssignRoleToUserAsync(AssignRoleDto assignRoleDto);
    Task<bool> RemoveRoleFromUserAsync(int userId, int roleId);
    Task<bool> UserHasRoleAsync(int userId, int roleId);
    Task<bool> UserHasRoleAsync(int userId, string roleCode);
    Task<int> RemoveAllUserRolesAsync(int userId);
    Task<bool> UpdateUserRolesAsync(int userId, IEnumerable<int> roleIds);
}

// ✅ CORRECT: Focused user-role service
public class UserRoleService : IUserRoleService
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserService _userService;
    
    // ONLY user-role relationship operations
    // NO role management, NO user management, NO permission logic
}
```

#### 3. Separated Repository Pattern
```csharp
// PATTERN: Repository separation following SRP
public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(int roleId);
    Task<Role?> GetByCodeAsync(string code);
    Task<IEnumerable<Role>> GetAllAsync();
    Task<IEnumerable<Role>> GetSystemRolesAsync();
    Task<Role> CreateAsync(Role role);
    Task<Role?> UpdateAsync(Role role);
    Task<bool> DeleteAsync(int roleId);
    Task<bool> ExistsAsync(int roleId);
    Task<bool> CodeExistsAsync(string code, int? excludeRoleId = null);
}

// ✅ CORRECT: Separate repository for user-role relationships
public interface IUserRoleRepository
{
    Task<IEnumerable<Role>> GetUserRolesAsync(int userId);
    Task<IEnumerable<User>> GetRoleUsersAsync(int roleId);
    Task<UserRole> AssignRoleAsync(int userId, int roleId, int? assignedBy = null);
    Task<bool> RemoveRoleAsync(int userId, int roleId);
    Task<bool> UserHasRoleAsync(int userId, int roleId);
    Task<bool> UserHasRoleAsync(int userId, string roleCode);
    Task<int> RemoveAllUserRolesAsync(int userId);
}

// ❌ NEVER: Mixed responsibilities in repositories
public interface IBadUserRepository
{
    Task<User> CreateAsync(User user);
    Task<bool> AssignRoleAsync(int userId, int roleId); // ❌ Role responsibility
    Task<IEnumerable<Permission>> GetUserPermissionsAsync(int userId); // ❌ Permission responsibility
}
```

#### 4. Controller Layer (Thin Controllers)
```csharp
// PATTERN: Thin controllers - ONLY HTTP concerns
[ApiController]
[Route("api/roles")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;
    
    [HttpGet]
    [Authorize(Roles = "system-admin,exam-manager")]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetAllRoles()
    {
        // ✅ CORRECT: Delegate to service immediately
        var roles = await _roleService.GetAllRolesAsync();
        return Ok(roles);
    }
    
    [HttpPost]
    [Authorize(Roles = "system-admin")]
    public async Task<ActionResult<RoleDto>> CreateRole(CreateRoleDto createRoleDto)
    {
        // ✅ CORRECT: No business logic, just HTTP handling
        var role = await _roleService.CreateRoleAsync(createRoleDto);
        return CreatedAtAction(nameof(GetRole), new { id = role.RoleId }, role);
    }
}

[ApiController]
[Route("api/user-roles")]
[Authorize]
public class UserRolesController : ControllerBase
{
    private readonly IUserRoleService _userRoleService;
    
    [HttpPost("assign")]
    [Authorize(Roles = "system-admin,exam-manager")]
    public async Task<ActionResult> AssignRole(AssignRoleDto assignRoleDto)
    {
        await _userRoleService.AssignRoleToUserAsync(assignRoleDto);
        return Ok(new { Message = "Role assigned successfully" });
    }
}
```

### 🎨 Frontend Implementation (Role-Based UI)

#### 1. Role Management Components
```typescript
// PATTERN: Component with role-based rendering
export const RoleManagementPage: React.FC = () => {
    const { user, hasRole } = useAuth();
    const {
        roles,
        loading,
        error,
        createRole,
        updateRole,
        deleteRole
    } = useRoles();

    // ✅ CORRECT: Role-based UI rendering
    if (!hasRole('system-admin')) {
        return <AccessDenied message="Only system administrators can manage roles" />;
    }

    return (
        <div className="role-management">
            <RoleList roles={roles} onEdit={updateRole} onDelete={deleteRole} />
            <CreateRoleForm onSubmit={createRole} />
        </div>
    );
};
```

#### 2. Role Assignment Components
```typescript
// PATTERN: User-role assignment interface
export const UserRoleAssignment: React.FC<{ userId: number }> = ({ userId }) => {
    const { assignRole, removeRole, getUserRoles } = useUserRoles();
    const { availableRoles } = useRoles();
    const [userRoles, setUserRoles] = useState<Role[]>([]);

    const handleAssignRole = async (roleId: number) => {
        await assignRole({ userId, roleId });
        // Refresh user roles
        const updated = await getUserRoles(userId);
        setUserRoles(updated);
    };

    return (
        <div className="user-role-assignment">
            <CurrentRoles roles={userRoles} onRemove={(roleId) => removeRole(userId, roleId)} />
            <AvailableRoles roles={availableRoles} onAssign={handleAssignRole} />
        </div>
    );
};
```

#### 3. Permission-Based Route Protection
```typescript
// PATTERN: Role-based route protection
export const ProtectedRoute: React.FC<ProtectedRouteProps> = ({
    children,
    requiredRoles = [],
    requiredPermissions = [],
    fallbackPath = '/unauthorized'
}) => {
    const { user, hasRole, hasPermission } = useAuth();

    if (!user) {
        return <Navigate to="/login" replace />;
    }

    const hasRequiredRole = requiredRoles.length === 0 || 
        requiredRoles.some(role => hasRole(role));
    
    const hasRequiredPermission = requiredPermissions.length === 0 || 
        requiredPermissions.some(permission => hasPermission(permission));

    if (!hasRequiredRole || !hasRequiredPermission) {
        return <Navigate to={fallbackPath} replace />;
    }

    return <>{children}</>;
};
```

## 🔄 Validation Loops (Execute These to Verify Implementation)

### Level 1: Build and Syntax Validation
```bash
# Backend validation
cd backend
dotnet build
dotnet format --verify-no-changes

# Frontend validation  
cd frontend
npm run type-check
npm run lint
npm run build
```

### Level 2: Unit Testing (Comprehensive Coverage)
```csharp
// Backend: Role service tests
[Test]
public async Task CreateRoleAsync_Should_CreateRole_When_ValidData()
{
    // Arrange
    var mockRoleRepository = new Mock<IRoleRepository>();
    var mockMapper = new Mock<IMapper>();
    var createRoleDto = new CreateRoleDto 
    { 
        Code = "test-role",
        Name = "Test Role",
        Description = "Test role description"
    };
    
    var role = new Role { RoleId = 1, Code = "test-role" };
    var roleDto = new RoleDto { RoleId = 1, Code = "test-role" };
    
    mockMapper.Setup(x => x.Map<Role>(createRoleDto)).Returns(role);
    mockRoleRepository.Setup(x => x.CreateAsync(role)).ReturnsAsync(role);
    mockMapper.Setup(x => x.Map<RoleDto>(role)).Returns(roleDto);

    var roleService = new RoleService(mockRoleRepository.Object, mockMapper.Object, /* other deps */);

    // Act
    var result = await roleService.CreateRoleAsync(createRoleDto);

    // Assert
    Assert.IsNotNull(result);
    Assert.AreEqual("test-role", result.Code);
    mockRoleRepository.Verify(x => x.CreateAsync(It.IsAny<Role>()), Times.Once);
}

[Test]
public async Task CreateRoleAsync_Should_ThrowException_When_CodeExists()
{
    // Test duplicate role code scenario
}

[Test]
public async Task DeleteRoleAsync_Should_PreventDeletion_When_SystemRole()
{
    // Test system role protection
}
```

```csharp
// Backend: User-role service tests
[Test]
public async Task AssignRoleToUserAsync_Should_AssignRole_When_ValidUserAndRole()
{
    // Arrange
    var mockUserRoleRepository = new Mock<IUserRoleRepository>();
    var assignRoleDto = new AssignRoleDto { UserId = 1, RoleId = 2 };
    var userRole = new UserRole { UserId = 1, RoleId = 2 };
    
    mockUserRoleRepository.Setup(x => x.AssignRoleAsync(1, 2, null))
                          .ReturnsAsync(userRole);

    var userRoleService = new UserRoleService(mockUserRoleRepository.Object, /* other deps */);

    // Act
    var result = await userRoleService.AssignRoleToUserAsync(assignRoleDto);

    // Assert
    Assert.IsTrue(result);
    mockUserRoleRepository.Verify(x => x.AssignRoleAsync(1, 2, null), Times.Once);
}

[Test]
public async Task UserHasRoleAsync_Should_ReturnTrue_When_UserHasRole()
{
    // Test role verification logic
}
```

```typescript
// Frontend: Role management tests
describe('RoleManagementPage', () => {
  it('should display role management interface for system admin', () => {
    // Mock authenticated system admin
    const mockUseAuth = vi.mocked(useAuth);
    mockUseAuth.mockReturnValue({
      user: { roles: ['system-admin'] },
      hasRole: vi.fn().mockReturnValue(true),
      // ... other auth properties
    });

    render(<RoleManagementPage />);
    
    expect(screen.getByText('Role Management')).toBeInTheDocument();
    expect(screen.getByText('Create New Role')).toBeInTheDocument();
  });

  it('should show access denied for non-admin users', () => {
    const mockUseAuth = vi.mocked(useAuth);
    mockUseAuth.mockReturnValue({
      user: { roles: ['student'] },
      hasRole: vi.fn().mockReturnValue(false),
    });

    render(<RoleManagementPage />);
    
    expect(screen.getByText('Access Denied')).toBeInTheDocument();
  });
});
```

### Level 3: Integration Testing
```bash
# Start backend API
cd backend
dotnet run --project Ikhtibar.API

# Test role management endpoints
curl -X GET http://localhost:5000/api/roles \
  -H "Authorization: Bearer admin-jwt-token"
# Expected: 200 OK with list of roles

curl -X POST http://localhost:5000/api/roles \
  -H "Authorization: Bearer admin-jwt-token" \
  -H "Content-Type: application/json" \
  -d '{"code": "test-role", "name": "Test Role", "description": "Test Description"}'
# Expected: 201 Created with role details

curl -X POST http://localhost:5000/api/user-roles/assign \
  -H "Authorization: Bearer admin-jwt-token" \
  -H "Content-Type: application/json" \
  -d '{"userId": 1, "roleId": 2}'
# Expected: 200 OK with assignment confirmation

curl -X GET http://localhost:5000/api/user-roles/user/1 \
  -H "Authorization: Bearer admin-jwt-token"
# Expected: 200 OK with user's roles
```

### Level 4: Authorization Testing
```typescript
// E2E test for role-based access control
describe('Role-Based Access Control', () => {
  it('should allow system admin to manage roles', async () => {
    // Login as system admin
    await page.goto('/login');
    await page.fill('[data-testid="email-input"]', 'admin@ikhtibar.com');
    await page.fill('[data-testid="password-input"]', 'Admin123!');
    await page.click('[data-testid="login-button"]');
    
    // Navigate to role management
    await page.goto('/admin/roles');
    await expect(page.locator('[data-testid="create-role-button"]')).toBeVisible();
    
    // Create new role
    await page.click('[data-testid="create-role-button"]');
    await page.fill('[data-testid="role-code-input"]', 'test-role');
    await page.fill('[data-testid="role-name-input"]', 'Test Role');
    await page.click('[data-testid="save-role-button"]');
    
    // Verify role created
    await expect(page.locator('text=Test Role')).toBeVisible();
  });

  it('should deny access to role management for students', async () => {
    // Login as student
    await page.goto('/login');
    await page.fill('[data-testid="email-input"]', 'student@ikhtibar.com');
    await page.fill('[data-testid="password-input"]', 'Student123!');
    await page.click('[data-testid="login-button"]');
    
    // Try to access role management
    await page.goto('/admin/roles');
    await expect(page.locator('text=Access Denied')).toBeVisible();
  });
});
```

## 🚨 Anti-Patterns to Avoid (Critical SRP Violations)

### ❌ NEVER Generate These Patterns:

#### Mixed Repository Responsibilities
```csharp
// ❌ BAD: Repository with mixed concerns
public class UserRepository
{
    Task<User> CreateAsync(User user);
    Task<bool> AssignRoleAsync(int userId, int roleId); // ❌ Role responsibility
    Task<IEnumerable<Permission>> GetUserPermissionsAsync(int userId); // ❌ Permission responsibility
    Task<bool> SendEmailAsync(User user); // ❌ Email responsibility
}

// ✅ CORRECT: Separated repositories
public class UserRepository
{
    Task<User> CreateAsync(User user);
    Task<User?> GetByIdAsync(int userId);
    Task<bool> DeleteAsync(int userId);
}

public class UserRoleRepository
{
    Task<bool> AssignRoleAsync(int userId, int roleId);
    Task<bool> RemoveRoleAsync(int userId, int roleId);
    Task<IEnumerable<Role>> GetUserRolesAsync(int userId);
}
```

#### Service with Multiple Concerns
```csharp
// ❌ BAD: Service with mixed responsibilities
public class RoleService
{
    public async Task<Role> CreateRoleAsync(CreateRoleDto dto)
    {
        // ❌ Data access - belongs in repository
        var role = await _dbContext.Roles.AddAsync(new Role());
        
        // ❌ User operations - belongs in UserService
        await NotifyUsersOfNewRole(role);
        
        // ❌ Email operations - belongs in EmailService
        await SendRoleCreatedEmail(role);
        
        return role;
    }
}

// ✅ CORRECT: Focused service
public class RoleService
{
    public async Task<RoleDto> CreateRoleAsync(CreateRoleDto dto)
    {
        // Business validation only
        await ValidateRoleCreation(dto);
        
        // Delegate data access to repository
        var role = await _roleRepository.CreateAsync(_mapper.Map<Role>(dto));
        
        // Return data, let other services handle notifications
        return _mapper.Map<RoleDto>(role);
    }
}
```

## 🔄 Integration Points (Configuration Required)

### Database Seeding
```sql
-- Role and permission seeding (already exists in data.sql)
-- Verify these roles are properly created:
INSERT INTO Roles (Code, Name, Description, IsSystemRole) VALUES
('system-admin', 'System Administrator', 'Full system control', 1),
('reviewer', 'Reviewer', 'Question review and approval', 1),
('creator', 'Question Creator', 'Content creation', 1),
('exam-manager', 'Exam Manager', 'Exam lifecycle management', 1),
('supervisor', 'Supervisor', 'Exam oversight', 1),
('student', 'Student', 'Exam participation', 1),
('grader', 'Grader', 'Manual assessment', 1),
('quality-reviewer', 'Quality Reviewer', 'Standards compliance', 1);
```

### Authorization Configuration
```csharp
// Program.cs additions for role-based authorization
builder.Services.AddAuthorization(options =>
{
    // Role-based policies
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("system-admin"));
    
    options.AddPolicy("ContentManager", policy =>
        policy.RequireRole("system-admin", "exam-manager", "creator"));
    
    options.AddPolicy("ExamStaff", policy =>
        policy.RequireRole("system-admin", "exam-manager", "supervisor", "grader"));
        
    options.AddPolicy("QualityControl", policy =>
        policy.RequireRole("system-admin", "reviewer", "quality-reviewer"));
});
```

### Frontend Route Configuration
```typescript
// App routing with role protection
const App: React.FC = () => {
    return (
        <Router>
            <Routes>
                {/* Admin routes */}
                <Route path="/admin/roles" element={
                    <ProtectedRoute requiredRoles={['system-admin']}>
                        <RoleManagementPage />
                    </ProtectedRoute>
                } />
                
                {/* Content management routes */}
                <Route path="/questions/*" element={
                    <ProtectedRoute requiredRoles={['system-admin', 'creator', 'reviewer']}>
                        <QuestionBankRoutes />
                    </ProtectedRoute>
                } />
                
                {/* Exam management routes */}
                <Route path="/exams/*" element={
                    <ProtectedRoute requiredRoles={['system-admin', 'exam-manager']}>
                        <ExamManagementRoutes />
                    </ProtectedRoute>
                } />
            </Routes>
        </Router>
    );
};
```

### Dependency Injection Registration
```csharp
// Register role-related services
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
```

## 🧪 Testing Strategy Checklist

- [ ] **Unit Tests**: Role service logic, validation rules, user-role assignments
- [ ] **Integration Tests**: API endpoints, database operations, role queries
- [ ] **Authorization Tests**: Role-based access control, permission enforcement
- [ ] **Component Tests**: Role management UI, user assignment interfaces
- [ ] **E2E Tests**: Complete role management workflows, access control scenarios

## 🛡️ Security Validation Checklist

- [ ] Role assignments require appropriate permissions
- [ ] System roles cannot be deleted or modified inappropriately
- [ ] Role-based authorization is enforced at API level
- [ ] Frontend UI respects user roles and permissions
- [ ] Audit trail is maintained for role assignments
- [ ] Role escalation is prevented (users cannot grant higher privileges)

## 📊 Performance Considerations

- [ ] Role queries are optimized with proper indexing
- [ ] User role checks are cached for frequent operations
- [ ] Bulk role assignments are supported for efficiency
- [ ] Permission checks are optimized for authorization middleware

## 🌐 Internationalization

- [ ] Role names and descriptions support localization
- [ ] Role management UI text is translatable
- [ ] Error messages for role operations are localized
- [ ] Role-based access denied messages support i18n

Remember: **Roles are the foundation of authorization** - implement with comprehensive validation to ensure secure, scalable access control for the educational assessment platform.
