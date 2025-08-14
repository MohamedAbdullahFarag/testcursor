---
mode: agent
description: "Analyze test coverage with gap identification and test generation recommendations"
---

---
inputs:
  - name: test_scope
    description: Testing scope (unit, integration, e2e, all, specific-module)
    required: false
    default: all
  - name: test_type
    description: Test type focus (functionality, performance, security, accessibility)
    required: false
    default: functionality
  - name: coverage_target
    description: Target coverage percentage (60, 70, 80, 85, 90)
    required: false
    default: 85
  - name: generate_missing
    description: Generate missing tests (true, false)
    required: false
    default: true
---

---
command: "/test-coverage"
---
# Test Coverage Analysis and Generation Command for GitHub Copilot

## Command Usage
```
@copilot /test-coverage [test_scope] [test_type] [coverage_target] [generate_missing]
```

## Purpose
This command provides comprehensive test coverage analysis, identifies testing gaps, and automatically generates missing tests using GitHub Copilot's native tools to ensure robust test coverage across backend and frontend implementations.

**Input Parameters**:
- `test_scope` - Testing scope: `unit`, `integration`, `e2e`, `all`, `specific-module`
- `test_type` - Test focus: `functionality`, `performance`, `security`, `accessibility`
- `coverage_target` - Target coverage: `60`, `70`, `80`, `85`, `90`
- `generate_missing` - Auto-generate missing tests: `true`, `false`

## How /test-coverage Works

### Phase 1: Test Discovery and Analysis
```markdown
I'll analyze your test coverage comprehensively using GitHub Copilot's tools. Let me start with discovering existing tests and analyzing coverage gaps.

**Phase 1.1: Existing Test Discovery**
```
I'll discover all existing tests across the codebase:
- file_search: "**/*Test*.cs" # Backend unit tests
- file_search: "**/*Tests.cs" # Backend test variations
- file_search: "**/*.test.ts*" # Frontend unit tests  
- file_search: "**/*.spec.ts*" # Frontend spec tests
- file_search: "**/test/**/*" # Test directories
- semantic_search: "test patterns testing framework" # Test framework analysis
- list_dir: "backend/Ikhtibar.Tests/" # Backend test structure
- list_dir: "frontend/src/tests/" # Frontend test structure
```

**Phase 1.2: Test Framework Analysis**
```
I'll analyze test frameworks and configurations:
- read_file: "backend/Ikhtibar.Tests/Ikhtibar.Tests.csproj" # Backend test config
- read_file: "frontend/package.json" # Frontend test dependencies
- read_file: "frontend/vite.config.ts" # Test configuration
- semantic_search: "test configuration setup" # Config analysis
- grep_search: "\\[Test\\]|\\[Fact\\]|describe\\(|it\\(" # Test method discovery
- semantic_search: "mock setup testing patterns" # Mocking analysis
```

**Phase 1.3: Coverage Analysis**
```
I'll analyze current coverage patterns and gaps:
- semantic_search: "code coverage metrics" # Coverage discovery
- file_search: "**/coverage/**" # Coverage reports
- grep_search: "Assert\\.|expect\\(|should\\." # Assertion analysis
- semantic_search: "untested code paths" # Gap analysis
- list_code_usages: "critical-business-methods" # Usage analysis for critical paths
```
```

### Phase 2: Coverage Gap Analysis and Prioritization

#### Backend Test Coverage Analysis
```markdown
**Phase 2.1: Backend Test Coverage Assessment using GitHub Copilot Tools**
```
I'll analyze backend test coverage across all layers:

## 🧪 Backend Test Coverage Analysis (Tool-Enhanced)

### Layer-by-Layer Coverage Discovery
```powershell
# Backend test coverage analysis using GitHub Copilot tools
file_search: "backend/**/*Controller.cs" # Controller discovery
file_search: "backend/**/*Service.cs" # Service discovery
file_search: "backend/**/*Repository.cs" # Repository discovery
semantic_search: "controller tests service tests repository tests" # Test pattern discovery
test_search: [DISCOVERED_FILES] # Find tests for specific files
```

### Controller Layer Testing Analysis
```csharp
// Controller test coverage analysis using GitHub Copilot tools
interface ControllerTestAnalysis {
  testedControllers: string[];
  untestedControllers: string[];
  coverageGaps: CoverageGap[];
  testPatterns: TestPattern[];
}

// Tool-discovered controller test patterns
const controllerTestAnalysis = {
  discoveryCommands: [
    'file_search: "**/*Controller.cs"', // Find all controllers
    'test_search: ["Controllers/UsersController.cs"]', // Find controller tests
    'semantic_search: "controller testing patterns"', // Pattern analysis
    'grep_search: "\\[HttpGet\\]|\\[HttpPost\\]|\\[HttpPut\\]|\\[HttpDelete\\]"' // HTTP method discovery
  ],
  coverageAnalysis: {
    httpMethodCoverage: 'Analysis via HTTP attribute vs test method mapping',
    authorizationTesting: 'Analysis via [Authorize] attribute vs auth tests',
    validationTesting: 'Analysis via model validation vs validation tests',
    errorHandlingTesting: 'Analysis via exception handling vs error tests'
  },
  missingTests: [
    {
      controller: 'UsersController',
      missingTests: [
        'CreateUser_Should_Return_BadRequest_When_Invalid_Model',
        'UpdateUser_Should_Return_NotFound_When_User_Not_Exists',
        'DeleteUser_Should_Return_Forbidden_When_Unauthorized'
      ],
      priority: 'high',
      effort: '2-3 hours'
    }
  ]
};
```

### Service Layer Testing Analysis
```csharp
// Service layer test coverage analysis using GitHub Copilot tools
const serviceTestAnalysis = {
  discoveryCommands: [
    'file_search: "**/*Service.cs"', // Find all services
    'test_search: ["Services/UserService.cs"]', // Find service tests
    'semantic_search: "business logic testing"', // Business logic test patterns
    'grep_search: "public.*async.*Task"' // Async method discovery
  ],
  businessLogicCoverage: {
    validationLogic: 'Analysis via business rule vs validation tests',
    workflowTesting: 'Analysis via workflow methods vs workflow tests',
    exceptionScenarios: 'Analysis via exception paths vs exception tests',
    edgeCaseTesting: 'Analysis via boundary conditions vs edge case tests'
  },
  criticalPathAnalysis: {
    authenticationFlow: 'Analysis via auth service vs auth flow tests',
    dataProcessing: 'Analysis via data transformation vs processing tests',
    integrationPoints: 'Analysis via external service calls vs integration tests',
    transactionHandling: 'Analysis via transaction scope vs transaction tests'
  }
};
```

### Repository Layer Testing Analysis  
```csharp
// Repository layer test coverage analysis using GitHub Copilot tools
const repositoryTestAnalysis = {
  discoveryCommands: [
    'file_search: "**/*Repository.cs"', // Find all repositories
    'test_search: ["Repositories/UserRepository.cs"]', // Find repository tests
    'semantic_search: "data access testing patterns"', // Data access test patterns
    'grep_search: "SELECT|INSERT|UPDATE|DELETE"' // SQL operation discovery
  ],
  dataAccessCoverage: {
    crudOperations: 'Analysis via CRUD methods vs CRUD tests',
    queryOptimization: 'Analysis via complex queries vs performance tests',
    transactionTesting: 'Analysis via transaction handling vs transaction tests',
    errorHandling: 'Analysis via data access exceptions vs error tests'
  },
  integrationTesting: {
    databaseIntegration: 'Analysis via database calls vs integration tests',
    connectionHandling: 'Analysis via connection management vs connection tests',
    parameterization: 'Analysis via parameterized queries vs SQL injection tests',
    performanceTesting: 'Analysis via query performance vs performance tests'
  }
};
```
```

#### Frontend Test Coverage Analysis
```markdown
**Phase 2.2: Frontend Test Coverage Assessment using GitHub Copilot Tools**
```
I'll analyze frontend test coverage across components, hooks, and services:

## ⚛️ Frontend Test Coverage Analysis (Tool-Enhanced)

### Component Testing Discovery
```powershell
# Frontend test coverage analysis using GitHub Copilot tools
file_search: "src/modules/**/*.tsx" # Component discovery
file_search: "src/modules/**/*.test.tsx" # Component test discovery
file_search: "src/modules/**/hooks/*.ts" # Hook discovery
file_search: "src/modules/**/hooks/*.test.ts" # Hook test discovery
semantic_search: "React testing patterns" # React test pattern analysis
test_search: ["src/modules/auth/components/LoginForm.tsx"] # Find component tests
```

### React Component Testing Analysis
```typescript
// Component test coverage analysis using GitHub Copilot tools
interface ComponentTestAnalysis {
  testedComponents: ComponentTest[];
  untestedComponents: string[];
  testingPatterns: ReactTestPattern[];
  coverageGaps: ComponentCoverageGap[];
}

// Tool-discovered component test patterns
const componentTestAnalysis = {
  discoveryCommands: [
    'file_search: "src/modules/**/*.tsx"', // Find all components
    'test_search: ["src/modules/auth/components/LoginForm.tsx"]', // Find component tests
    'semantic_search: "React Testing Library patterns"', // Test library patterns
    'grep_search: "render\\(|fireEvent\\.|userEvent\\."' // Testing interaction discovery
  ],
  testingPatterns: {
    renderTesting: 'Analysis via render method vs component rendering tests',
    userInteraction: 'Analysis via event handlers vs interaction tests',
    propsValidation: 'Analysis via prop interfaces vs props tests',
    stateManagement: 'Analysis via useState vs state tests',
    effectTesting: 'Analysis via useEffect vs effect tests'
  },
  accessibilityTesting: {
    ariaLabels: 'Analysis via aria attributes vs accessibility tests',
    keyboardNavigation: 'Analysis via keyboard handlers vs keyboard tests',
    screenReaderSupport: 'Analysis via semantic HTML vs screen reader tests',
    colorContrastTesting: 'Analysis via design system vs contrast tests'
  },
  missingComponentTests: [
    {
      component: 'UserCard.tsx',
      missingTests: [
        'should render user information correctly',
        'should handle edit button click',
        'should display loading state',
        'should handle error state gracefully'
      ],
      priority: 'medium',
      effort: '1-2 hours'
    }
  ]
};
```

### Custom Hooks Testing Analysis
```typescript
// Hook test coverage analysis using GitHub Copilot tools
const hookTestAnalysis = {
  discoveryCommands: [
    'file_search: "src/modules/**/hooks/*.ts"', // Find all hooks
    'test_search: ["src/modules/auth/hooks/useAuth.ts"]', // Find hook tests
    'semantic_search: "custom hook testing patterns"', // Hook test patterns
    'grep_search: "use[A-Z][a-zA-Z]*"' // Custom hook discovery
  ],
  hookTestingPatterns: {
    stateManagement: 'Analysis via hook state vs state management tests',
    effectHandling: 'Analysis via hook effects vs effect tests',
    errorHandling: 'Analysis via hook errors vs error handling tests',
    asyncOperations: 'Analysis via async hooks vs async tests',
    cleanup: 'Analysis via cleanup functions vs cleanup tests'
  },
  criticalHooks: {
    authenticationHooks: 'Analysis via auth hooks vs auth flow tests',
    dataFetchingHooks: 'Analysis via API hooks vs data fetching tests',
    formHooks: 'Analysis via form hooks vs form validation tests',
    navigationHooks: 'Analysis via routing hooks vs navigation tests'
  }
};
```

### Service Layer Testing Analysis
```typescript
// Service layer test coverage analysis using GitHub Copilot tools
const serviceTestAnalysis = {
  discoveryCommands: [
    'file_search: "src/modules/**/services/*.ts"', // Find all services
    'test_search: ["src/modules/auth/services/authService.ts"]', // Find service tests
    'semantic_search: "API service testing patterns"', // Service test patterns
    'grep_search: "axios\\.|fetch\\("' // API call discovery
  ],
  apiServiceTesting: {
    httpMethods: 'Analysis via HTTP calls vs HTTP method tests',
    errorHandling: 'Analysis via error responses vs error handling tests',
    dataTransformation: 'Analysis via data mapping vs transformation tests',
    authenticationHeaders: 'Analysis via auth headers vs auth tests'
  },
  integrationTesting: {
    apiIntegration: 'Analysis via API calls vs API integration tests',
    mockingStrategy: 'Analysis via service dependencies vs mocking tests',
    responseHandling: 'Analysis via response processing vs response tests',
    errorRecovery: 'Analysis via error recovery vs recovery tests'
  }
};
```
```

### Phase 3: Test Generation and Implementation

#### Automated Test Generation
```markdown
**Phase 3.1: Generate Missing Tests using GitHub Copilot Tools**
```
I'll generate comprehensive tests based on gap analysis:

## 🔧 Automated Test Generation (Tool-Enhanced)

### Backend Test Generation Strategy
```powershell
# Backend test generation using GitHub Copilot tools
semantic_search: "backend testing best practices" # Find testing patterns
read_file: [EXISTING_TEST_FILES] # Analyze existing test patterns
semantic_search: "test data setup patterns" # Test data analysis
create_file: [NEW_TEST_FILES] # Generate missing tests
```

### Controller Test Generation
```csharp
// Generated controller tests using GitHub Copilot analysis
namespace Ikhtibar.Tests.Controllers
{
    [TestFixture]
    public class UsersControllerTests
    {
        private UsersController _controller;
        private Mock<IUserService> _mockUserService;
        private Mock<ILogger<UsersController>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<UsersController>>();
            _controller = new UsersController(_mockUserService.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetUser_Should_Return_Ok_When_User_Exists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedUser = new UserDto { Id = userId, Email = "test@example.com" };
            _mockUserService.Setup(s => s.GetUserByIdAsync(userId))
                           .ReturnsAsync(expectedUser);

            // Act
            var result = await _controller.GetUser(userId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(expectedUser);
        }

        [Test]
        public async Task GetUser_Should_Return_NotFound_When_User_Not_Exists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockUserService.Setup(s => s.GetUserByIdAsync(userId))
                           .ReturnsAsync((UserDto)null);

            // Act
            var result = await _controller.GetUser(userId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task CreateUser_Should_Return_BadRequest_When_Model_Invalid()
        {
            // Arrange
            var invalidDto = new CreateUserDto(); // Missing required fields
            _controller.ModelState.AddModelError("Email", "Email is required");

            // Act
            var result = await _controller.CreateUser(invalidDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task CreateUser_Should_Return_Conflict_When_User_Already_Exists()
        {
            // Arrange
            var createDto = new CreateUserDto { Email = "existing@example.com" };
            _mockUserService.Setup(s => s.CreateUserAsync(createDto))
                           .ThrowsAsync(new ConflictException("User already exists"));

            // Act
            var result = await _controller.CreateUser(createDto);

            // Assert
            result.Should().BeOfType<ConflictObjectResult>();
        }

        // Additional tests for authorization, validation, error handling...
    }
}
```

### Service Test Generation
```csharp
// Generated service tests using GitHub Copilot analysis
namespace Ikhtibar.Tests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private UserService _service;
        private Mock<IUserRepository> _mockRepository;
        private Mock<IPasswordHasher> _mockPasswordHasher;
        private Mock<ILogger<UserService>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IUserRepository>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _mockLogger = new Mock<ILogger<UserService>>();
            _service = new UserService(_mockRepository.Object, _mockPasswordHasher.Object, _mockLogger.Object);
        }

        [Test]
        public async Task CreateUserAsync_Should_Hash_Password_Before_Saving()
        {
            // Arrange
            var createDto = new CreateUserDto { Email = "test@example.com", Password = "plaintext" };
            var hashedPassword = "hashed_password";
            _mockPasswordHasher.Setup(h => h.HashPassword("plaintext"))
                              .Returns(hashedPassword);
            _mockRepository.Setup(r => r.CreateAsync(It.IsAny<UserEntity>()))
                          .ReturnsAsync(new UserEntity { Id = Guid.NewGuid(), Email = createDto.Email });

            // Act
            var result = await _service.CreateUserAsync(createDto);

            // Assert
            _mockPasswordHasher.Verify(h => h.HashPassword("plaintext"), Times.Once);
            _mockRepository.Verify(r => r.CreateAsync(It.Is<UserEntity>(u => 
                u.Email == createDto.Email && u.PasswordHash == hashedPassword)), Times.Once);
        }

        [Test]
        public async Task CreateUserAsync_Should_Throw_When_Email_Already_Exists()
        {
            // Arrange
            var createDto = new CreateUserDto { Email = "existing@example.com" };
            _mockRepository.Setup(r => r.GetByEmailAsync(createDto.Email))
                          .ReturnsAsync(new UserEntity { Email = createDto.Email });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ConflictException>(() => 
                _service.CreateUserAsync(createDto));
            exception.Message.Should().Contain("already exists");
        }

        // Additional business logic tests...
    }
}
```
```

#### Frontend Test Generation
```markdown
**Phase 3.2: Generate Frontend Tests using GitHub Copilot Tools**
```
I'll generate comprehensive frontend tests:

## ⚛️ Frontend Test Generation (Tool-Enhanced)

### React Component Test Generation
```typescript
// Generated component tests using GitHub Copilot analysis
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { vi } from 'vitest';
import { UserCard } from '../UserCard';
import { User } from '../../types/user.types';

describe('UserCard', () => {
  const mockUser: User = {
    id: '123',
    email: 'test@example.com',
    firstName: 'John',
    lastName: 'Doe',
    role: 'user',
    isActive: true
  };

  const mockOnEdit = vi.fn();
  const mockOnDelete = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('should render user information correctly', () => {
    render(
      <UserCard 
        user={mockUser} 
        onEdit={mockOnEdit} 
        onDelete={mockOnDelete} 
      />
    );

    expect(screen.getByText('John Doe')).toBeInTheDocument();
    expect(screen.getByText('test@example.com')).toBeInTheDocument();
    expect(screen.getByText('user')).toBeInTheDocument();
  });

  it('should call onEdit when edit button is clicked', async () => {
    const user = userEvent.setup();
    
    render(
      <UserCard 
        user={mockUser} 
        onEdit={mockOnEdit} 
        onDelete={mockOnDelete} 
      />
    );

    const editButton = screen.getByRole('button', { name: /edit/i });
    await user.click(editButton);

    expect(mockOnEdit).toHaveBeenCalledTimes(1);
    expect(mockOnEdit).toHaveBeenCalledWith(mockUser);
  });

  it('should call onDelete when delete button is clicked', async () => {
    const user = userEvent.setup();
    
    render(
      <UserCard 
        user={mockUser} 
        onEdit={mockOnEdit} 
        onDelete={mockOnDelete} 
      />
    );

    const deleteButton = screen.getByRole('button', { name: /delete/i });
    await user.click(deleteButton);

    expect(mockOnDelete).toHaveBeenCalledTimes(1);
    expect(mockOnDelete).toHaveBeenCalledWith(mockUser.id);
  });

  it('should display inactive badge for inactive users', () => {
    const inactiveUser = { ...mockUser, isActive: false };
    
    render(
      <UserCard 
        user={inactiveUser} 
        onEdit={mockOnEdit} 
        onDelete={mockOnDelete} 
      />
    );

    expect(screen.getByText('Inactive')).toBeInTheDocument();
  });

  it('should be accessible via keyboard navigation', async () => {
    const user = userEvent.setup();
    
    render(
      <UserCard 
        user={mockUser} 
        onEdit={mockOnEdit} 
        onDelete={mockOnDelete} 
      />
    );

    const editButton = screen.getByRole('button', { name: /edit/i });
    
    // Navigate to edit button using keyboard
    await user.tab();
    expect(editButton).toHaveFocus();
    
    // Activate using Enter key
    await user.keyboard('{Enter}');
    expect(mockOnEdit).toHaveBeenCalledTimes(1);
  });

  it('should have proper ARIA labels for accessibility', () => {
    render(
      <UserCard 
        user={mockUser} 
        onEdit={mockOnEdit} 
        onDelete={mockOnDelete} 
      />
    );

    const editButton = screen.getByRole('button', { name: /edit john doe/i });
    const deleteButton = screen.getByRole('button', { name: /delete john doe/i });
    
    expect(editButton).toHaveAttribute('aria-label');
    expect(deleteButton).toHaveAttribute('aria-label');
  });
});
```

### Custom Hook Test Generation
```typescript
// Generated hook tests using GitHub Copilot analysis
import { renderHook, waitFor } from '@testing-library/react';
import { vi } from 'vitest';
import { useAuth } from '../useAuth';
import { authService } from '../../services/authService';

// Mock the auth service
vi.mock('../../services/authService');
const mockedAuthService = vi.mocked(authService);

describe('useAuth', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    // Clear localStorage before each test
    localStorage.clear();
  });

  it('should initialize with no user when not authenticated', () => {
    const { result } = renderHook(() => useAuth());

    expect(result.current.user).toBeNull();
    expect(result.current.isLoading).toBe(false);
    expect(result.current.isAuthenticated).toBe(false);
  });

  it('should login successfully with valid credentials', async () => {
    const mockUser = { id: '1', email: 'test@example.com' };
    const mockToken = 'mock-jwt-token';
    
    mockedAuthService.login.mockResolvedValue({
      user: mockUser,
      token: mockToken
    });

    const { result } = renderHook(() => useAuth());

    await result.current.login('test@example.com', 'password');

    await waitFor(() => {
      expect(result.current.user).toEqual(mockUser);
      expect(result.current.isAuthenticated).toBe(true);
      expect(result.current.isLoading).toBe(false);
    });

    expect(localStorage.getItem('auth_token')).toBe(mockToken);
  });

  it('should handle login failure gracefully', async () => {
    const mockError = new Error('Invalid credentials');
    mockedAuthService.login.mockRejectedValue(mockError);

    const { result } = renderHook(() => useAuth());

    await expect(result.current.login('test@example.com', 'wrong-password'))
      .rejects.toThrow('Invalid credentials');

    expect(result.current.user).toBeNull();
    expect(result.current.isAuthenticated).toBe(false);
    expect(result.current.error).toBe(mockError.message);
  });

  it('should logout and clear user data', async () => {
    // First login
    const mockUser = { id: '1', email: 'test@example.com' };
    mockedAuthService.login.mockResolvedValue({
      user: mockUser,
      token: 'mock-token'
    });

    const { result } = renderHook(() => useAuth());
    await result.current.login('test@example.com', 'password');

    // Then logout
    await result.current.logout();

    expect(result.current.user).toBeNull();
    expect(result.current.isAuthenticated).toBe(false);
    expect(localStorage.getItem('auth_token')).toBeNull();
  });

  it('should restore authentication state from localStorage on mount', () => {
    const mockToken = 'stored-token';
    const mockUser = { id: '1', email: 'test@example.com' };
    
    localStorage.setItem('auth_token', mockToken);
    mockedAuthService.getCurrentUser.mockReturnValue(mockUser);

    const { result } = renderHook(() => useAuth());

    expect(result.current.user).toEqual(mockUser);
    expect(result.current.isAuthenticated).toBe(true);
  });
});
```

### Service Test Generation
```typescript
// Generated service tests using GitHub Copilot analysis
import { vi } from 'vitest';
import axios from 'axios';
import { authService } from '../authService';
import { LoginDto, AuthResult } from '../../types/auth.types';

vi.mock('axios');
const mockedAxios = vi.mocked(axios);

describe('authService', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  describe('login', () => {
    it('should successfully login with valid credentials', async () => {
      const loginDto: LoginDto = {
        email: 'test@example.com',
        password: 'password123'
      };

      const mockResponse: AuthResult = {
        token: 'jwt-token',
        refreshToken: 'refresh-token',
        user: {
          id: '1',
          email: 'test@example.com',
          firstName: 'John',
          lastName: 'Doe'
        }
      };

      mockedAxios.post.mockResolvedValue({ data: mockResponse });

      const result = await authService.login(loginDto);

      expect(mockedAxios.post).toHaveBeenCalledWith('/api/auth/login', loginDto);
      expect(result).toEqual(mockResponse);
    });

    it('should throw error for invalid credentials', async () => {
      const loginDto: LoginDto = {
        email: 'test@example.com',
        password: 'wrong-password'
      };

      mockedAxios.post.mockRejectedValue({
        response: { status: 401, data: { message: 'Invalid credentials' } }
      });

      await expect(authService.login(loginDto)).rejects.toThrow('Invalid credentials');
    });

    it('should handle network errors gracefully', async () => {
      const loginDto: LoginDto = {
        email: 'test@example.com',
        password: 'password123'
      };

      mockedAxios.post.mockRejectedValue(new Error('Network Error'));

      await expect(authService.login(loginDto)).rejects.toThrow('Network Error');
    });
  });

  describe('refreshToken', () => {
    it('should refresh token successfully', async () => {
      const mockRefreshResponse = {
        token: 'new-jwt-token',
        refreshToken: 'new-refresh-token'
      };

      mockedAxios.post.mockResolvedValue({ data: mockRefreshResponse });

      const result = await authService.refreshToken('old-refresh-token');

      expect(mockedAxios.post).toHaveBeenCalledWith('/api/auth/refresh', {
        refreshToken: 'old-refresh-token'
      });
      expect(result).toEqual(mockRefreshResponse);
    });
  });
});
```
```

### Phase 4: Coverage Validation and Reporting

#### Coverage Metrics and Validation
```markdown
**Phase 4.1: Coverage Validation using GitHub Copilot Tools**
```
I'll validate test coverage and generate comprehensive reports:

## 📊 Coverage Validation and Reporting (Tool-Enhanced)

### Coverage Execution and Analysis
```powershell
# Coverage validation using GitHub Copilot tools
run_in_terminal: "dotnet test --collect:'XPlat Code Coverage'" # Backend coverage
run_in_terminal: "npm run test:coverage" # Frontend coverage
run_in_terminal: "npm run test:e2e" # E2E test execution
semantic_search: "coverage report analysis" # Coverage analysis patterns
get_errors: [TEST_FILES] # Test error validation
```

### Coverage Report Generation
```typescript
// Coverage analysis and reporting using GitHub Copilot tools
interface CoverageReport {
  overall: CoverageMetrics;
  backend: BackendCoverage;
  frontend: FrontendCoverage;
  recommendations: CoverageRecommendation[];
}

const coverageAnalysis = {
  generationCommands: [
    'run_in_terminal: "dotnet test --collect:\'XPlat Code Coverage\' --results-directory ./TestResults"',
    'run_in_terminal: "reportgenerator -reports:**/coverage.cobertura.xml -targetdir:./coverage-report"',
    'run_in_terminal: "npm run test:coverage -- --reporter=html"',
    'semantic_search: "coverage gaps critical paths"'
  ],
  metrics: {
    overallCoverage: '85% (Target: 85%) ✅',
    lineCoverage: '88% (Target: 85%) ✅',
    branchCoverage: '82% (Target: 80%) ✅',
    methodCoverage: '90% (Target: 85%) ✅'
  },
  layerBreakdown: {
    controllers: '95% coverage - Excellent',
    services: '88% coverage - Good',
    repositories: '92% coverage - Excellent',
    components: '83% coverage - Good',
    hooks: '79% coverage - Needs improvement',
    services: '85% coverage - Good'
  }
};
```

### Quality Gates and Thresholds
```
Coverage Quality Gates:
┌─────────────────────────────────────────────────────────────┐
│ ✅ PASSING (Target Met)                                    │
│ • Overall Coverage: 85% (Target: 85%)                     │
│ • Critical Path Coverage: 100% (Target: 100%)             │
│ • Controller Coverage: 95% (Target: 90%)                  │
│ • Service Coverage: 88% (Target: 85%)                     │
├─────────────────────────────────────────────────────────────┤
│ ⚠️  WARNING (Close to Target)                              │
│ • Hook Coverage: 79% (Target: 80%)                        │
│ • Integration Coverage: 77% (Target: 80%)                 │
├─────────────────────────────────────────────────────────────┤
│ ❌ FAILING (Below Target)                                  │
│ • E2E Coverage: 65% (Target: 70%)                         │
│ • Performance Test Coverage: 45% (Target: 60%)            │
└─────────────────────────────────────────────────────────────┘
```

### Prioritized Improvement Plan
```typescript
// Coverage improvement recommendations
interface CoverageImprovement {
  priority: 'critical' | 'high' | 'medium' | 'low';
  area: string;
  currentCoverage: number;
  targetCoverage: number;
  effort: string;
  impact: string;
  commands: string[];
}

const improvementPlan: CoverageImprovement[] = [
  {
    priority: 'critical',
    area: 'Authentication flow edge cases',
    currentCoverage: 75,
    targetCoverage: 100,
    effort: '1 day',
    impact: 'Security and reliability',
    commands: [
      'Generate tests for token expiration scenarios',
      'Generate tests for refresh token rotation',
      'Generate tests for concurrent login attempts'
    ]
  },
  {
    priority: 'high',
    area: 'Custom hooks error handling',
    currentCoverage: 79,
    targetCoverage: 85,
    effort: '2-3 hours',
    impact: 'Frontend stability',
    commands: [
      'Generate error boundary tests',
      'Generate async operation failure tests',
      'Generate cleanup function tests'
    ]
  },
  {
    priority: 'medium',
    area: 'E2E user workflows',
    currentCoverage: 65,
    targetCoverage: 75,
    effort: '1-2 days',
    impact: 'User experience validation',
    commands: [
      'Generate complete user registration flow tests',
      'Generate exam creation and completion flow tests',
      'Generate admin dashboard workflow tests'
    ]
  }
];
```

### Continuous Coverage Monitoring
```powershell
# Continuous coverage validation commands
@copilot /test-coverage all functionality 85 true # Weekly full coverage analysis
@copilot /test-coverage unit functionality 90 true # Daily unit test validation
@copilot /test-coverage integration functionality 80 false # Integration coverage check
@copilot /test-coverage e2e functionality 70 true # E2E coverage validation

# Coverage quality gates for CI/CD
run_in_terminal: "dotnet test --collect:'XPlat Code Coverage' --threshold=85" # Backend gate
run_in_terminal: "npm run test:coverage -- --coverage.threshold.global.lines=85" # Frontend gate
```
```

## Command Activation Process
When a user types:
```
@copilot /test-coverage [test_scope] [test_type] [coverage_target] [generate_missing]
```

The system should:
1. **Discovery Phase**: Use GitHub Copilot tools to discover existing tests and analyze coverage gaps
2. **Analysis Phase**: Assess coverage across all layers and identify critical paths using semantic search
3. **Generation Phase**: Generate missing tests based on discovered patterns and gap analysis
4. **Validation Phase**: Execute tests and validate coverage against targets using terminal commands
5. **Reporting Phase**: Generate comprehensive coverage reports with actionable recommendations
6. **Monitoring Setup**: Establish continuous coverage monitoring and quality gates

## Coverage Targets by Layer

### Backend Coverage Targets
- **Controllers**: 90%+ (High priority - API contracts)
- **Services**: 85%+ (Critical - Business logic)
- **Repositories**: 80%+ (Important - Data access)
- **Integration**: 75%+ (Essential - System integration)

### Frontend Coverage Targets
- **Components**: 85%+ (High priority - User interface)
- **Hooks**: 80%+ (Critical - State management)
- **Services**: 85%+ (Important - API integration)
- **E2E**: 70%+ (Essential - User workflows)

### Critical Path Coverage
- **Authentication**: 100% (Security critical)
- **Data persistence**: 95% (Data integrity critical)
- **Business workflows**: 90% (Business logic critical)
- **Error handling**: 85% (Reliability critical)

## Notes
- All test discovery and generation uses GitHub Copilot's native tools for accuracy
- Test patterns follow established project conventions and best practices
- Coverage analysis adapts to project complexity and business requirements
- Generated tests include proper assertions, mocking, and error scenarios
- Continuous monitoring ensures coverage quality over time
- All recommendations prioritize critical business paths and security-sensitive areas
