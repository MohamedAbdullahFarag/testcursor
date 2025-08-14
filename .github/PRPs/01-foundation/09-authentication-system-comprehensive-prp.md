# PRP: Authentication System Implementation

## 🎯 Objective
Implement a comprehensive authentication system for the Ikhtibar educational assessment platform supporting JWT-based authentication, OIDC SSO integration, refresh token rotation, and secure user session management with role-based authorization.

## 📋 Why
- **Business Value**: Secure access control for educational assessment platform
- **User Impact**: Seamless login experience with SSO integration and automatic session management
- **Integration**: Foundation for all protected features (exam management, grading, user management)
- **Security**: Implements industry-standard authentication patterns with token rotation

## 🎯 What (User-Visible Behavior)
1. **User Login Flow**: Email/password authentication with JWT token issuance
2. **SSO Integration**: OpenID Connect authentication with external identity providers
3. **Token Management**: Automatic refresh token rotation and session persistence
4. **Role-Based Access**: User roles and permissions enforcement across the application
5. **Secure Logout**: Token revocation and session cleanup

### Success Criteria
- [ ] Users can authenticate with email/password and receive JWT tokens
- [ ] SSO authentication flow works with OIDC providers
- [ ] Refresh tokens automatically rotate and maintain session state
- [ ] Role-based authorization protects endpoints appropriately
- [ ] Authentication state persists across browser sessions
- [ ] Logout invalidates all tokens and clears session data

## 📚 All Needed Context

### 🏗️ Architecture Pattern References
```yaml
existing_implementations:
  - file: backend/Ikhtibar.API/Controllers/AuthController.cs
    why: Complete authentication controller with login, refresh, SSO patterns
    
  - file: backend/Ikhtibar.Infrastructure/Services/TokenService.cs
    why: JWT generation and validation implementation patterns
    
  - file: backend/Ikhtibar.Infrastructure/Services/OidcService.cs
    why: OIDC integration and user info retrieval patterns
    
  - file: frontend/src/modules/auth/services/authService.ts
    why: Frontend authentication service with API integration
    
  - file: frontend/src/shared/store/authStore.ts
    why: Zustand-based authentication state management

guidelines:
  - file: .github/copilot/backend-guidelines.md
    why: SRP enforcement rules and service layer patterns
    
  - file: .github/copilot/api-guidelines.md
    why: API design standards and authentication patterns
    
  - file: .github/copilot/frontend-guidelines.md
    why: React component and hook patterns
```

### 🔧 Technical Implementation Context
```yaml
database_entities:
  - User: Core user entity with authentication properties
  - RefreshToken: Token storage for rotation security
  - UserRole: Role assignment for authorization
  - Role: Role definitions with permissions

dtos_and_models:
  - LoginDto: Email/password authentication request
  - AuthResultDto: Authentication response with tokens and user data
  - RefreshTokenDto: Token refresh request
  - SsoCallbackDto: OIDC callback parameters
  
configuration:
  - JwtSettings: Token generation configuration
  - OidcSettings: External identity provider configuration
  - AuthSettings: General authentication settings

dependencies:
  - Microsoft.AspNetCore.Authentication.JwtBearer
  - Microsoft.AspNetCore.Authentication.OpenIdConnect
  - System.IdentityModel.Tokens.Jwt
  - BCrypt.Net-Next (for password hashing)
```

## 📝 Implementation Blueprint

### 🏛️ Backend Implementation (Following SRP)

#### 1. Authentication Service Layer
```csharp
// PATTERN: Service layer following SRP - ONLY authentication business logic
public interface IAuthenticationService
{
    Task<AuthResult> AuthenticateAsync(LoginRequest request);
    Task<AuthResult> RefreshTokenAsync(string refreshToken);
    Task<AuthResult> ProcessSsoCallbackAsync(SsoCallbackData callbackData);
    Task<bool> RevokeTokenAsync(string refreshToken);
    Task<bool> ValidateTokenAsync(string accessToken);
}

// ✅ CORRECT: Focused authentication service
public class AuthenticationService : IAuthenticationService
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly IOidcService _oidcService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    
    // ONLY authentication-related operations
    // NO user management, NO data access, NO HTTP concerns
}
```

#### 2. Token Management Service
```csharp
// PATTERN: Token operations following SRP
public interface ITokenService
{
    Task<string> GenerateJwtAsync(User user);
    Task<string> GenerateRefreshTokenAsync();
    Task<ClaimsPrincipal> ValidateJwtAsync(string token);
    Task<bool> IsTokenValidAsync(string token);
}

// ✅ CORRECT: Focused token service
public class TokenService : ITokenService
{
    // ONLY token generation, validation, and management
    // NO user operations, NO database access, NO HTTP concerns
}
```

#### 3. Refresh Token Repository
```csharp
// PATTERN: Repository following SRP - ONLY RefreshToken data operations
public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);
    Task<RefreshToken> AddAsync(RefreshToken refreshToken);
    Task<bool> RevokeByUserIdAsync(int userId);
    Task<bool> RevokeByTokenHashAsync(string tokenHash);
    Task<bool> CleanupExpiredTokensAsync();
}

// ✅ CORRECT: Focused repository
public class RefreshTokenRepository : IRefreshTokenRepository
{
    // ONLY RefreshToken data operations
    // NO business logic, NO authentication logic, NO user operations
}
```

#### 4. Authentication Controller
```csharp
// PATTERN: Thin controller - ONLY HTTP concerns
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    
    [HttpPost("login")]
    public async Task<ActionResult<AuthResult>> Login(LoginRequest request)
    {
        // ✅ CORRECT: Thin controller - delegate to service immediately
        var result = await _authenticationService.AuthenticateAsync(request);
        return Ok(result);
    }
    
    // NO business logic, NO data access, NO validation logic
    // ONLY HTTP request/response handling and routing
}
```

### 🎨 Frontend Implementation (Following Component Patterns)

#### 1. Authentication Hook
```typescript
// PATTERN: Custom hook following SRP - ONLY auth state and operations
export const useAuth = (): AuthContextType => {
    const {
        user, accessToken, isAuthenticated, isLoading, error,
        login: loginStore, clearAuth, refreshAccessToken, setLoading, setError
    } = useAuthStore();

    const login = useCallback(async (credentials: LoginRequest): Promise<void> => {
        // ✅ CORRECT: Focused auth operations
        // NO UI logic, NO routing, NO form validation
    }, []);

    return {
        user, accessToken, isAuthenticated, isLoading, error,
        login, logout, refreshToken, clearError
    };
};
```

#### 2. Authentication Service
```typescript
// PATTERN: Service class following SRP - ONLY API communication
class AuthService {
    async login(credentials: LoginRequest): Promise<AuthServiceResponse<AuthResult>> {
        // ✅ CORRECT: API communication only
        // NO state management, NO UI logic, NO routing
    }
    
    async refreshToken(refreshTokenData?: RefreshTokenRequest): Promise<AuthServiceResponse<RefreshTokenResponse>> {
        // ✅ CORRECT: Token refresh API call
    }
}
```

#### 3. Authentication Store
```typescript
// PATTERN: State management following SRP - ONLY auth state
interface AuthState {
    user: User | null;
    tokens: AuthTokens | null;
    isAuthenticated: boolean;
    isLoading: boolean;
    error: string | null;
    
    // Actions
    login: (credentials: LoginParams) => Promise<void>;
    logout: () => void;
    refreshToken: () => Promise<void>;
}

// ✅ CORRECT: Focused state management
// NO business logic, NO API calls (delegated to service), NO UI concerns
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
// Backend: Authentication service tests
[Test]
public async Task AuthenticateAsync_Should_ReturnAuthResult_When_ValidCredentials()
{
    // Arrange: Mock user service, token service
    var mockUserService = new Mock<IUserService>();
    var mockTokenService = new Mock<ITokenService>();
    var validUser = new User { UserId = 1, Email = "test@example.com" };
    mockUserService.Setup(x => x.AuthenticateAsync("test@example.com", "password"))
               .ReturnsAsync(new UserDto { UserId = 1, Email = "test@example.com" });
    mockTokenService.Setup(x => x.GenerateJwtAsync(It.IsAny<User>()))
               .ReturnsAsync("jwt-token");

    var authService = new AuthenticationService(mockUserService.Object, mockTokenService.Object, /* other deps */);

    // Act
    var result = await authService.AuthenticateAsync(new LoginRequest 
    { 
        Email = "test@example.com", 
        Password = "password" 
    });

    // Assert
    Assert.IsNotNull(result);
    Assert.AreEqual("jwt-token", result.AccessToken);
    Assert.AreEqual("test@example.com", result.User.Email);
}

[Test]
public async Task AuthenticateAsync_Should_ReturnNull_When_InvalidCredentials()
{
    // Test invalid credentials scenario
}

[Test]
public async Task RefreshTokenAsync_Should_ReturnNewTokens_When_ValidRefreshToken()
{
    // Test token refresh flow
}
```

```typescript
// Frontend: Auth hook tests
describe('useAuth', () => {
  it('should login successfully with valid credentials', async () => {
    // Arrange
    const mockAuthService = vi.mocked(authService);
    mockAuthService.login.mockResolvedValueOnce({
      success: true,
      data: { accessToken: 'token', refreshToken: 'refresh', user: mockUser }
    });

    const { result } = renderHook(() => useAuth());

    // Act
    await act(async () => {
      await result.current.login({ email: 'test@example.com', password: 'password' });
    });

    // Assert
    expect(result.current.isAuthenticated).toBe(true);
    expect(result.current.user).toEqual(mockUser);
  });

  it('should handle login failure gracefully', async () => {
    // Test error handling
  });

  it('should refresh token automatically when expired', async () => {
    // Test token refresh logic
  });
});
```

### Level 3: Integration Testing
```bash
# Start backend API
cd backend
dotnet run --project Ikhtibar.API

# Test authentication endpoints
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "admin@ikhtibar.com", "password": "Admin123!"}'
# Expected: 200 OK with JWT tokens

curl -X POST http://localhost:5000/api/auth/refresh \
  -H "Content-Type: application/json" \
  -d '{"refreshToken": "refresh-token-from-login"}'
# Expected: 200 OK with new JWT tokens

curl -X POST http://localhost:5000/api/auth/logout \
  -H "Authorization: Bearer jwt-token-from-login"
# Expected: 200 OK with success message
```

### Level 4: End-to-End Testing
```typescript
// E2E test for complete authentication flow
describe('Authentication Flow', () => {
  it('should complete full login to logout cycle', async () => {
    // 1. Navigate to login page
    await page.goto('/login');
    
    // 2. Fill credentials and submit
    await page.fill('[data-testid="email-input"]', 'admin@ikhtibar.com');
    await page.fill('[data-testid="password-input"]', 'Admin123!');
    await page.click('[data-testid="login-button"]');
    
    // 3. Verify redirect to dashboard
    await expect(page).toHaveURL('/dashboard');
    
    // 4. Verify user menu shows authenticated user
    await expect(page.locator('[data-testid="user-menu"]')).toBeVisible();
    
    // 5. Logout and verify redirect
    await page.click('[data-testid="logout-button"]');
    await expect(page).toHaveURL('/login');
  });
});
```

## 🚨 Anti-Patterns to Avoid (Critical SRP Violations)

### ❌ NEVER Generate These Patterns:

#### Mixed Responsibilities in Services
```csharp
// ❌ BAD: Authentication service with mixed concerns
public class AuthenticationService
{
    public async Task<AuthResult> AuthenticateAsync(LoginRequest request)
    {
        // ❌ Data access - belongs in repository
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        
        // ❌ HTTP concerns - belongs in controller
        if (user == null) return BadRequest("User not found");
        
        // ❌ UI logic - belongs in frontend
        var redirectUrl = DetermineRedirectUrl(user.Role);
        
        return Ok(new AuthResult { RedirectUrl = redirectUrl });
    }
}

// ✅ CORRECT: Focused authentication service
public class AuthenticationService
{
    public async Task<AuthResult> AuthenticateAsync(LoginRequest request)
    {
        // Business logic only - delegate data access to repositories
        var user = await _userService.AuthenticateAsync(request.Email, request.Password);
        if (user == null) return null; // Return data, not HTTP responses
        
        var tokens = await _tokenService.GenerateTokensAsync(user);
        return new AuthResult { User = user, Tokens = tokens };
    }
}
```

#### Controller with Business Logic
```csharp
// ❌ BAD: Controller with authentication logic
[HttpPost("login")]
public async Task<ActionResult> Login(LoginRequest request)
{
    // ❌ Validation logic - belongs in validator or service
    if (string.IsNullOrEmpty(request.Email)) return BadRequest("Email required");
    
    // ❌ Business logic - belongs in service
    var user = await _userRepository.GetByEmailAsync(request.Email);
    if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
        return Unauthorized("Invalid credentials");
    
    // ❌ Token generation - belongs in token service
    var token = GenerateJwtToken(user);
    
    return Ok(new { token });
}

// ✅ CORRECT: Thin controller
[HttpPost("login")]
public async Task<ActionResult<AuthResult>> Login(LoginRequest request)
{
    var result = await _authenticationService.AuthenticateAsync(request);
    if (result == null) return Unauthorized();
    return Ok(result);
}
```

## 🔄 Integration Points (Configuration Required)

### Database Migrations
```sql
-- RefreshToken table for token rotation
CREATE TABLE RefreshTokens (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TokenHash NVARCHAR(255) NOT NULL,
    UserId INT NOT NULL,
    IssuedAt DATETIME2 NOT NULL,
    ExpiresAt DATETIME2 NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    INDEX IX_RefreshTokens_TokenHash (TokenHash),
    INDEX IX_RefreshTokens_UserId (UserId),
    INDEX IX_RefreshTokens_ExpiresAt (ExpiresAt)
);
```

### Configuration Updates
```json
// appsettings.json additions
{
  "JwtSettings": {
    "SecretKey": "your-secret-key-at-least-32-characters-long",
    "Issuer": "Ikhtibar.API",
    "Audience": "Ikhtibar.Client",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  },
  "OidcSettings": {
    "Authority": "https://your-oidc-provider.com",
    "ClientId": "ikhtibar-client",
    "ClientSecret": "your-client-secret",
    "Scopes": ["openid", "profile", "email"]
  }
}
```

### Frontend Environment Variables
```env
# .env.local additions
VITE_API_BASE_URL=http://localhost:5000
VITE_OIDC_CLIENT_ID=ikhtibar-client
VITE_OIDC_AUTHORITY=https://your-oidc-provider.com
```

### Dependency Injection Registration
```csharp
// Program.cs additions
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IOidcService, OidcService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

// Configure authentication middleware
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        // JWT configuration
    })
    .AddOpenIdConnect(options => {
        // OIDC configuration  
    });
```

## 🧪 Testing Strategy Checklist

- [ ] **Unit Tests**: Service layer logic, token generation, validation
- [ ] **Integration Tests**: API endpoints, database operations, OIDC flow
- [ ] **Component Tests**: React authentication components and hooks
- [ ] **E2E Tests**: Complete authentication workflows
- [ ] **Security Tests**: Token validation, injection attacks, session management
- [ ] **Performance Tests**: Token generation speed, concurrent authentications

## 🛡️ Security Validation Checklist

- [ ] JWT tokens use secure signing algorithms (HS256/RS256)
- [ ] Refresh tokens are properly hashed before storage
- [ ] Token expiration times follow security best practices
- [ ] OIDC integration uses PKCE for security
- [ ] Password comparison uses secure hashing (BCrypt)
- [ ] Authentication attempts are rate-limited
- [ ] Sensitive information is not logged
- [ ] CORS is properly configured for authentication endpoints

## 📊 Performance Considerations

- [ ] JWT token size is optimized (minimal claims)
- [ ] Refresh token cleanup background service implemented
- [ ] Authentication API endpoints have appropriate caching headers
- [ ] Database queries use proper indexing
- [ ] Frontend authentication state is efficiently managed
- [ ] Token validation is optimized for high throughput

## 🌐 Internationalization

- [ ] Authentication error messages support i18n
- [ ] Login form labels and validation messages are translatable
- [ ] User interface supports RTL languages (Arabic)
- [ ] Success/error notifications use localized strings

Remember: **Authentication is the foundation of security** - implement with comprehensive testing and validation loops to ensure robust, secure access control for the educational assessment platform.
