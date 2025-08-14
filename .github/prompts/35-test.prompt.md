---
mode: agent
description: "Comprehensive test generation and execution framework with multiple testing strategies"
---

---
inputs:
  - name: type
    description: Type of test to create or run (unit, integration, e2e, performance, security)
    required: true
  - name: target
    description: Target component, service, or module to test
    required: true
  - name: action
    description: Test action to perform (create, run, debug, coverage, update)
    required: false
    default: create
---

---
command: "/test"
---
# Testing Command for GitHub Copilot

## Command Usage
```
@copilot /test [type] [target] [action]
```

## Purpose
This command provides comprehensive testing capabilities for the Ikhtibar examination management system, supporting both ASP.NET Core backend and React.js frontend testing. It follows project-specific testing patterns, TDD principles, and quality assurance standards.

**Input Parameters**: 
- `type` - Test type: `unit`, `integration`, `e2e`, `performance`, or `security`
- `target` - Component, service, or module to test
- `action` - Action: `create`, `run`, `debug`, `coverage`, or `update`

## How /test Works

### Phase 1: Test Context Discovery and Analysis
```markdown
I'll help you with comprehensive testing for the Ikhtibar project. Let me analyze your request and gather the necessary context.

**Phase 1.1: Parse Testing Request**
```
Testing Request Analysis:
- **Type**: [UNIT/INTEGRATION/E2E/PERFORMANCE/SECURITY]
- **Target**: [SPECIFIC_COMPONENT_OR_MODULE]
- **Action**: [CREATE/RUN/DEBUG/COVERAGE/UPDATE]
- **Project Context**: ASP.NET Core + React.js with TypeScript
- **Testing Framework**: xUnit (.NET), Jest/React Testing Library (React)
```

**Phase 1.2: Target Analysis using GitHub Copilot Tools**
```
I'll analyze the test target using GitHub Copilot's native tools:
- semantic_search: "[TARGET] implementation patterns" # Find target implementation
- file_search: "**/*[TARGET]*" # Find related files
- test_search: [TARGET_FILES] # Find existing tests
- read_file: [IMPLEMENTATION_FILES] # Read implementation details
- get_errors: [TARGET_FILES] # Check current status
- list_code_usages: [TARGET_CLASS] # Find usage patterns
```

**Phase 1.3: Testing Architecture Analysis**
```
Testing Context Analysis:
- [ ] **Backend Testing**: xUnit, Moq, FluentAssertions, TestContainers
- [ ] **Frontend Testing**: Jest, React Testing Library, MSW, Cypress
- [ ] **Integration Testing**: WebApplicationFactory, TestServer
- [ ] **E2E Testing**: Playwright, Cypress with real browser automation
- [ ] **Performance Testing**: NBomber, Artillery, Lighthouse
- [ ] **Security Testing**: OWASP ZAP, Security scanning tools
- [ ] **Coverage Goals**: Backend >90%, Frontend >85%
- [ ] **Test Patterns**: AAA (Arrange, Act, Assert), Given-When-Then
```
```

### Phase 2: Type-Specific Test Implementation

#### For Type: `unit`
```markdown
**Phase 2.1: Unit Test Creation using GitHub Copilot Tools**
```
I'll create comprehensive unit tests following project patterns:

## 🧪 Unit Test Implementation (Tool-Enhanced)

### Test Target Analysis
```powershell
# Comprehensive target analysis using GitHub Copilot tools
semantic_search: "[TARGET] unit testing patterns" # Find testing patterns
read_file: [IMPLEMENTATION_FILE] # Read implementation to test
list_code_usages: [TARGET_CLASS] # Find all usages
grep_search: "public.*method|interface.*method" # Find public methods
```

### Test Structure Analysis (Tool-Informed)
```
Target Analysis Results:
- **Class/Component**: [TARGET_NAME]
- **File Location**: [IMPLEMENTATION_PATH]
- **Public Methods**: [METHOD_LIST] (discovered via code analysis)
- **Dependencies**: [DEPENDENCY_LIST] (discovered via constructor analysis)
- **Existing Tests**: [EXISTING_TEST_COUNT] (discovered via test_search)
- **Coverage Gap**: [UNCOVERED_METHODS] (discovered via coverage analysis)
```

#### Backend Unit Test (C#)
```csharp
// Generated based on actual implementation analysis
using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Ikhtibar.Core.Services;
using Ikhtibar.Core.Repositories;
using Ikhtibar.Shared.DTOs;

namespace Ikhtibar.Tests.[MODULE]
{
    public class [TARGET_CLASS]Tests
    {
        private readonly Mock<I[DEPENDENCY]> _mock[DEPENDENCY];
        private readonly Mock<ILogger<[TARGET_CLASS]>> _mockLogger;
        private readonly [TARGET_CLASS] _sut;

        public [TARGET_CLASS]Tests()
        {
            // Arrange - Dependencies discovered via constructor analysis
            _mock[DEPENDENCY] = new Mock<I[DEPENDENCY]>();
            _mockLogger = new Mock<ILogger<[TARGET_CLASS]>>();
            _sut = new [TARGET_CLASS](_mock[DEPENDENCY].Object, _mockLogger.Object);
        }

        [Fact]
        public async Task [METHOD_NAME]_Should[EXPECTED_BEHAVIOR]_When[CONDITION]()
        {
            // Arrange
            var input = new [INPUT_DTO] { /* Properties from DTO analysis */ };
            var expected = new [EXPECTED_DTO] { /* Expected result */ };
            
            _mock[DEPENDENCY]
                .Setup(x => x.[DEPENDENCY_METHOD](It.IsAny<[PARAMETER_TYPE]>()))
                .ReturnsAsync(expected);

            // Act
            var result = await _sut.[METHOD_NAME](input);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
            _mock[DEPENDENCY].Verify(x => x.[DEPENDENCY_METHOD](It.IsAny<[PARAMETER_TYPE]>()), Times.Once);
        }

        [Theory]
        [InlineData([TEST_DATA_1])]
        [InlineData([TEST_DATA_2])]
        public async Task [METHOD_NAME]_Should[BEHAVIOR]_WithDifferentInputs([PARAMETER_TYPE] input)
        {
            // Test implementation with parameterized data
        }

        [Fact]
        public async Task [METHOD_NAME]_ShouldThrow[EXCEPTION]_When[ERROR_CONDITION]()
        {
            // Arrange
            _mock[DEPENDENCY]
                .Setup(x => x.[DEPENDENCY_METHOD](It.IsAny<[PARAMETER_TYPE]>()))
                .ThrowsAsync(new [EXCEPTION_TYPE]("[ERROR_MESSAGE]"));

            // Act & Assert
            await _sut
                .Invoking(x => x.[METHOD_NAME]([INVALID_INPUT]))
                .Should()
                .ThrowAsync<[EXCEPTION_TYPE]>()
                .WithMessage("[EXPECTED_ERROR_MESSAGE]");
        }
    }
}
```

#### Frontend Unit Test (TypeScript/React)
```typescript
// Generated based on actual component analysis
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { BrowserRouter } from 'react-router-dom';
import { I18nextProvider } from 'react-i18next';
import { [TARGET_COMPONENT] } from './[TARGET_COMPONENT]';
import { [TARGET_SERVICE] } from '../services/[TARGET_SERVICE]';
import i18n from '../../../shared/i18n/i18n';

// Mock service discovered via dependency analysis
jest.mock('../services/[TARGET_SERVICE]');
const mock[TARGET_SERVICE] = [TARGET_SERVICE] as jest.Mocked<typeof [TARGET_SERVICE]>;

describe('[TARGET_COMPONENT]', () => {
  let queryClient: QueryClient;

  beforeEach(() => {
    queryClient = new QueryClient({
      defaultOptions: { queries: { retry: false }, mutations: { retry: false } }
    });
    jest.clearAllMocks();
  });

  const renderComponent = (props: Partial<[TARGET_COMPONENT]Props> = {}) => {
    const defaultProps: [TARGET_COMPONENT]Props = {
      // Props discovered via interface analysis
    };

    return render(
      <QueryClientProvider client={queryClient}>
        <BrowserRouter>
          <I18nextProvider i18n={i18n}>
            <[TARGET_COMPONENT] {...defaultProps} {...props} />
          </I18nextProvider>
        </BrowserRouter>
      </QueryClientProvider>
    );
  };

  it('should render component with default props', () => {
    // Arrange & Act
    renderComponent();

    // Assert
    expect(screen.getByTestId('[COMPONENT_TEST_ID]')).toBeInTheDocument();
    expect(screen.getByText('[EXPECTED_TEXT]')).toBeInTheDocument();
  });

  it('should handle user interaction correctly', async () => {
    // Arrange
    const mockFunction = jest.fn();
    renderComponent({ onAction: mockFunction });

    // Act
    fireEvent.click(screen.getByRole('button', { name: '[BUTTON_TEXT]' }));

    // Assert
    await waitFor(() => {
      expect(mockFunction).toHaveBeenCalledWith([EXPECTED_PARAMETERS]);
    });
  });

  it('should display loading state when data is loading', () => {
    // Arrange
    mock[TARGET_SERVICE].[METHOD_NAME].mockReturnValue(
      new Promise(() => {}) // Never resolves to simulate loading
    );

    // Act
    renderComponent();

    // Assert
    expect(screen.getByTestId('loading-spinner')).toBeInTheDocument();
  });

  it('should display error message when operation fails', async () => {
    // Arrange
    const errorMessage = 'Operation failed';
    mock[TARGET_SERVICE].[METHOD_NAME].mockRejectedValue(new Error(errorMessage));

    // Act
    renderComponent();

    // Assert
    await waitFor(() => {
      expect(screen.getByText(errorMessage)).toBeInTheDocument();
    });
  });
});
```
```

#### For Type: `integration`
```markdown
**Phase 2.2: Integration Test Creation using GitHub Copilot Tools**
```
I'll create comprehensive integration tests:

## 🔗 Integration Test Implementation (Tool-Enhanced)

### Integration Scope Analysis
```powershell
# Integration boundary analysis using GitHub Copilot tools
semantic_search: "[TARGET] integration patterns" # Find integration patterns
file_search: "**/*Controller.cs" # Find API controllers (if API testing)
semantic_search: "database integration" # Find database dependencies
grep_search: "DbContext|Repository" # Find data access patterns
```

#### API Integration Test (Backend)
```csharp
// Generated based on actual API analysis
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Xunit;
using FluentAssertions;
using Ikhtibar.API;
using Ikhtibar.Shared.DTOs;

namespace Ikhtibar.Tests.Integration.[MODULE]
{
    public class [TARGET_CONTROLLER]IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public [TARGET_CONTROLLER]IntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Get[ENTITIES]_ShouldReturn[EXPECTED_RESULT]_WhenCalled()
        {
            // Arrange
            var endpoint = "/api/[CONTROLLER_ROUTE]";

            // Act
            var response = await _client.GetAsync(endpoint);

            // Assert
            response.Should().BeSuccessful();
            var content = await response.Content.ReadFromJsonAsync<IEnumerable<[DTO_TYPE]>>();
            content.Should().NotBeNull();
        }

        [Fact]
        public async Task Post[ENTITY]_ShouldCreate[ENTITY]_WithValidData()
        {
            // Arrange
            var createDto = new Create[ENTITY]Dto
            {
                // Properties from DTO analysis
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/[CONTROLLER_ROUTE]", createDto);

            // Assert
            response.Should().BeSuccessful();
            var createdEntity = await response.Content.ReadFromJsonAsync<[DTO_TYPE]>();
            createdEntity.Should().NotBeNull();
            createdEntity![PROPERTY].Should().Be(createDto.[PROPERTY]);
        }

        [Fact]
        public async Task Put[ENTITY]_ShouldUpdate[ENTITY]_WithValidData()
        {
            // Arrange
            var updateDto = new Update[ENTITY]Dto
            {
                // Properties from DTO analysis
            };
            var entityId = Guid.NewGuid(); // Or get from existing data

            // Act
            var response = await _client.PutAsJsonAsync($"/api/[CONTROLLER_ROUTE]/{entityId}", updateDto);

            // Assert
            response.Should().BeSuccessful();
        }

        [Fact]
        public async Task Delete[ENTITY]_ShouldRemove[ENTITY]_WhenExists()
        {
            // Arrange
            var entityId = Guid.NewGuid(); // Or get from existing data

            // Act
            var response = await _client.DeleteAsync($"/api/[CONTROLLER_ROUTE]/{entityId}");

            // Assert
            response.Should().BeSuccessful();
        }
    }
}
```

#### Database Integration Test
```csharp
// Generated based on repository analysis
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using FluentAssertions;
using Ikhtibar.Infrastructure.Data;
using Ikhtibar.Infrastructure.Repositories;
using Ikhtibar.Shared.Entities;

namespace Ikhtibar.Tests.Integration.Repositories
{
    public class [TARGET_REPOSITORY]IntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;
        private readonly [TARGET_REPOSITORY] _repository;

        public [TARGET_REPOSITORY]IntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new [TARGET_REPOSITORY](_fixture.ConnectionString);
        }

        [Fact]
        public async Task CreateAsync_ShouldPersist[ENTITY]_ToDatabase()
        {
            // Arrange
            var entity = new [ENTITY_TYPE]
            {
                // Properties from entity analysis
            };

            // Act
            var result = await _repository.CreateAsync(entity);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBe(Guid.Empty);

            // Verify persistence
            var retrieved = await _repository.GetByIdAsync(result.Id);
            retrieved.Should().NotBeNull();
            retrieved!.[PROPERTY].Should().Be(entity.[PROPERTY]);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturn[ENTITY]_WhenExists()
        {
            // Arrange
            var entity = await SeedTestEntity();

            // Act
            var result = await _repository.GetByIdAsync(entity.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(entity.Id);
        }

        private async Task<[ENTITY_TYPE]> SeedTestEntity()
        {
            var entity = new [ENTITY_TYPE]
            {
                // Test data properties
            };
            return await _repository.CreateAsync(entity);
        }
    }
}
```
```

#### For Type: `e2e`
```markdown
**Phase 2.3: End-to-End Test Creation using GitHub Copilot Tools**
```
I'll create comprehensive E2E tests:

## 🎭 End-to-End Test Implementation (Tool-Enhanced)

### User Journey Analysis
```powershell
# User workflow analysis using GitHub Copilot tools
semantic_search: "[TARGET] user workflow" # Find user journey patterns
file_search: "**/*Page.tsx" # Find page components
semantic_search: "navigation flow" # Find routing patterns
grep_search: "useNavigate|Link|router" # Find navigation implementation
```

#### Playwright E2E Test
```typescript
// Generated based on user workflow analysis
import { test, expect, Page } from '@playwright/test';

test.describe('[TARGET] User Journey', () => {
  let page: Page;

  test.beforeEach(async ({ page: testPage }) => {
    page = testPage;
    await page.goto('/');
    
    // Login if required (discovered via auth analysis)
    await loginAsTestUser(page);
  });

  test('should complete [TARGET] workflow successfully', async () => {
    // Step 1: Navigate to [TARGET] section
    await page.click('[data-testid="[TARGET]-menu-item"]');
    await expect(page).toHaveURL(/.*\/[TARGET]/);

    // Step 2: Verify page loads correctly
    await expect(page.locator('[data-testid="[TARGET]-page-title"]')).toBeVisible();
    await expect(page.locator('[data-testid="[TARGET]-content"]')).toBeVisible();

    // Step 3: Perform primary action
    await page.click('[data-testid="create-[TARGET]-button"]');
    await expect(page.locator('[data-testid="[TARGET]-form"]')).toBeVisible();

    // Step 4: Fill form with test data
    await page.fill('[data-testid="[TARGET]-name-input"]', 'Test [TARGET] Name');
    await page.fill('[data-testid="[TARGET]-description-input"]', 'Test description');
    
    // Step 5: Submit form
    await page.click('[data-testid="submit-[TARGET]-button"]');
    
    // Step 6: Verify success
    await expect(page.locator('[data-testid="success-message"]')).toBeVisible();
    await expect(page.locator('[data-testid="[TARGET]-list"]')).toContainText('Test [TARGET] Name');
  });

  test('should handle [TARGET] editing workflow', async () => {
    // Navigate to existing [TARGET]
    await page.goto('/[TARGET]/[TEST_ID]');
    
    // Edit [TARGET]
    await page.click('[data-testid="edit-[TARGET]-button"]');
    await page.fill('[data-testid="[TARGET]-name-input"]', 'Updated Name');
    await page.click('[data-testid="save-[TARGET]-button"]');
    
    // Verify update
    await expect(page.locator('[data-testid="[TARGET]-name"]')).toContainText('Updated Name');
  });

  test('should handle [TARGET] deletion workflow', async () => {
    // Navigate to [TARGET] list
    await page.goto('/[TARGET]');
    
    // Delete [TARGET]
    await page.click('[data-testid="delete-[TARGET]-button"]:first-child');
    await page.click('[data-testid="confirm-delete-button"]');
    
    // Verify deletion
    await expect(page.locator('[data-testid="success-message"]')).toBeVisible();
  });

  test('should validate form inputs', async () => {
    // Navigate to create form
    await page.goto('/[TARGET]/create');
    
    // Try to submit empty form
    await page.click('[data-testid="submit-[TARGET]-button"]');
    
    // Verify validation messages
    await expect(page.locator('[data-testid="name-error"]')).toBeVisible();
    await expect(page.locator('[data-testid="description-error"]')).toBeVisible();
  });

  test('should support internationalization', async () => {
    // Test Arabic language
    await page.click('[data-testid="language-selector"]');
    await page.click('[data-testid="arabic-language-option"]');
    
    // Verify Arabic content
    await expect(page.locator('html')).toHaveAttribute('dir', 'rtl');
    await expect(page.locator('[data-testid="page-title"]')).toContainText('[ARABIC_TEXT]');
  });
});

async function loginAsTestUser(page: Page): Promise<void> {
  await page.goto('/login');
  await page.fill('[data-testid="email-input"]', 'test@example.com');
  await page.fill('[data-testid="password-input"]', 'testPassword123');
  await page.click('[data-testid="login-button"]');
  await expect(page).toHaveURL(/.*\/dashboard/);
}
```
```

### Phase 3: Test Execution and Analysis

```markdown
**Phase 3.1: Test Execution using GitHub Copilot Tools**
```
I'll execute tests and analyze results:

## ⚡ Test Execution and Analysis (Tool-Enhanced)

### Test Execution (Tool-Managed)
```powershell
# Comprehensive test execution using GitHub Copilot tools
run_in_terminal: "dotnet test --configuration Release --logger console" # Backend tests
run_in_terminal: "npm run test -- --coverage" # Frontend tests with coverage
run_in_terminal: "npx playwright test" # E2E tests
get_errors: [TEST_FILES] # Check for test compilation errors
```

### Test Results Analysis (Tool-Generated)
```
Test Execution Results:
┌─────────────────────────────────────────────────────────────┐
│ Backend Tests (xUnit):                                      │
│ • Total: [TOTAL_COUNT] tests                                │
│ • Passed: [PASSED_COUNT] ✅                                 │
│ • Failed: [FAILED_COUNT] ❌                                 │
│ • Coverage: [COVERAGE_PERCENTAGE]%                          │
├─────────────────────────────────────────────────────────────┤
│ Frontend Tests (Jest):                                      │
│ • Total: [TOTAL_COUNT] tests                                │
│ • Passed: [PASSED_COUNT] ✅                                 │
│ • Failed: [FAILED_COUNT] ❌                                 │
│ • Coverage: [COVERAGE_PERCENTAGE]%                          │
├─────────────────────────────────────────────────────────────┤
│ E2E Tests (Playwright):                                     │
│ • Total: [TOTAL_COUNT] tests                                │
│ • Passed: [PASSED_COUNT] ✅                                 │
│ • Failed: [FAILED_COUNT] ❌                                 │
│ • Duration: [EXECUTION_TIME]                                │
└─────────────────────────────────────────────────────────────┘
```

### Coverage Analysis (Tool-Detailed)
#### Backend Coverage
| Module | Line Coverage | Branch Coverage | Method Coverage |
|--------|---------------|-----------------|-----------------|
| [MODULE] | [PERCENTAGE]% | [PERCENTAGE]% | [PERCENTAGE]% |

#### Frontend Coverage
| Component | Line Coverage | Function Coverage | Branch Coverage |
|-----------|---------------|-------------------|-----------------|
| [COMPONENT] | [PERCENTAGE]% | [PERCENTAGE]% | [PERCENTAGE]% |

### Quality Metrics (Tool-Calculated)
- [ ] **Test Coverage Goal**: [✅/❌] (Backend >90%, Frontend >85%)
- [ ] **Test Performance**: [✅/❌] (All tests <30s execution time)
- [ ] **Test Reliability**: [✅/❌] (No flaky tests detected)
- [ ] **Code Quality**: [✅/❌] (Tests follow AAA pattern)
- [ ] **Documentation**: [✅/❌] (Tests are self-documenting)
```

### Phase 4: Continuous Testing Integration

```markdown
**Phase 4.1: CI/CD Integration using GitHub Copilot Tools**
```
I'll integrate tests into the CI/CD pipeline:

## 🔄 Continuous Testing Integration (Tool-Enhanced)

### GitHub Actions Test Workflow
```yaml
# Generated based on project structure analysis
name: Continuous Testing

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main, develop]

jobs:
  backend-tests:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
          
      - name: Restore dependencies
        run: dotnet restore backend/Ikhtibar.sln
        
      - name: Build
        run: dotnet build backend/Ikhtibar.sln --configuration Release --no-restore
        
      - name: Run Unit Tests
        run: dotnet test backend/Ikhtibar.Tests --configuration Release --no-build --verbosity normal --collect:"XPlat Code Coverage"
        
      - name: Upload coverage to Codecov
        uses: codecov/codecov-action@v3
        with:
          file: '**/coverage.cobertura.xml'

  frontend-tests:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '18'
          cache: 'pnpm'
          
      - name: Install dependencies
        run: pnpm install --frozen-lockfile
        
      - name: Run tests with coverage
        run: pnpm test:coverage
        
      - name: Upload coverage to Codecov
        uses: codecov/codecov-action@v3
        with:
          file: 'frontend/coverage/lcov.info'

  e2e-tests:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '18'
          cache: 'pnpm'
          
      - name: Install dependencies
        run: pnpm install --frozen-lockfile
        
      - name: Install Playwright
        run: npx playwright install --with-deps
        
      - name: Run E2E tests
        run: pnpm test:e2e
        
      - name: Upload test results
        uses: actions/upload-artifact@v3
        if: always()
        with:
          name: playwright-report
          path: playwright-report/
```

### Quality Gates (Tool-Enforced)
```
Quality Gate Configuration:
- **Minimum Coverage**: Backend 90%, Frontend 85%
- **Test Performance**: All tests must complete within 30 seconds
- **Zero Failures**: All tests must pass for merge approval
- **Security Tests**: Security scans must pass
- **Performance Budget**: Performance tests within acceptable limits
```
```

## Command Activation Process
When a user types:
```
@copilot /test [type] [target] [action]
```

The system should:
1. **Analyze Target**: Use GitHub Copilot tools to understand the implementation
2. **Generate Tests**: Create comprehensive tests following project patterns
3. **Execute Tests**: Run tests and analyze results with detailed reporting
4. **Integrate CI/CD**: Set up continuous testing workflows
5. **Monitor Quality**: Track coverage and quality metrics over time

## Notes
- All tests follow project-specific patterns and conventions
- Test generation is based on actual implementation analysis using GitHub Copilot tools
- Coverage goals are enforced: Backend >90%, Frontend >85%
- Tests support internationalization and accessibility requirements
- Security and performance testing are integrated into the workflow
- CI/CD integration ensures continuous quality monitoring
- Test results include detailed analysis and recommendations
- All test types support debugging and troubleshooting capabilities
