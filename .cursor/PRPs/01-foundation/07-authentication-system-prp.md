# PRP: Authentication System

## Module: Authentication
## Feature: Authentication System (04)

### 🎯 Objective
Design and implement a robust authentication system supporting JWT and OIDC-based SSO, including login, refresh tokens, and secure user context propagation.

### 📦 Dependencies
- Core Entities Setup PRP (`01-core-entities-setup-prp.md`)
- API Foundation PRP (`03-api-foundation-prp.md`)
- Feature requirements in `ikhtibar-features.txt` (User Roles & SSO)

### 📝 Context
- Projects:
  - `Ikhtibar.API` (Controllers)
  - `Ikhtibar.Core` (Services & Interfaces)
  - `Ikhtibar.Infrastructure` (Repositories & External integrations)
- Existing configuration models:
  - `JwtSettings`, `OidcSettings`, `AuthSettings` in `DTOs` and `Models`
- Example in `.cursor/copilot/examples/backend/services/UserService.cs` and `.cursor/copilot/examples/backend/controllers/UsersController.cs`

### 🔧 Implementation Tasks
1. **Configure Authentication Schemes** (`Program.cs`)
   - Add `services.AddAuthentication()`:
     - JWT Bearer: configure issuer, audience, key from `JwtSettings`
     - OpenID Connect: parameters from `OidcSettings`
   - Add `services.AddAuthorization()` with default policy requiring authenticated users.
   - ❌ DON'T: Hardcode secret keys or client credentials.

2. **Bind Settings Classes** (`Program.cs`)
   - `builder.Configuration.GetSection("JwtSettings").Bind<JwtSettings>()`
   - `builder.Configuration.GetSection("OidcSettings").Bind<OidcSettings>()`
   - Register `IOptions<JwtSettings>` and `IOptions<OidcSettings>`.

3. **Implement IOidcService** (`Ikhtibar.Core/Services/IOidcService.cs`)
   - Define methods:
     ```csharp
     Task<OidcTokenResponse> ExchangeCodeAsync(string code);
     Task<OidcUserInfo> GetUserInfoAsync(string accessToken);
     ```
   - ❌ DON'T: Mix business logic into service layer.

4. **Create OidcService** (`Ikhtibar.Infrastructure/Services/OidcService.cs`)
   - Inject `HttpClient` and `OidcSettings`.
   - Implement code exchange and user info retrieval with proper error handling.
   - Use extension methods for JSON deserialization.

5. **AuthController** (`Ikhtibar.API/Controllers/AuthController.cs`)
   - `[Route("api/[controller]")] [ApiController] public class AuthController : ControllerBase`
   - Actions:
     - `POST /login`: accept `LoginDto`, validate credentials via `UserService`, return JWT + refresh token.
     - `POST /refresh`: accept `RefreshTokenDto`, validate and issue new JWT.
     - `GET /sso/callback`: accept `SsoCallbackDto`, call `IOidcService.ExchangeCodeAsync`, map to local user, issue JWT.
   - Decorate actions with `[ProducesResponseType]` and Swagger summaries.

6. **Token Generation Utility** (`Ikhtibar.Core/Services/ITokenService.cs`)
   - Define `GenerateJwtAsync(User user)`, `GenerateRefreshToken()`.
   - Implement in `TokenService` with `JwtSettings`.

7. **Middleware for Refresh Token Rotation (Optional)**
   - Create `RefreshTokenMiddleware` to inspect expired tokens and auto-refresh.
   - ❌ DON'T: Catch generic exceptions without logging.

8. **Register HttpClient for OIDC** (`Program.cs`)
   - `services.AddHttpClient<IOidcService, OidcService>(client => { client.BaseAddress = new Uri(oidc.Authority); });`

9. **Unit Tests** (`Ikhtibar.Tests/Auth`)
   - Mock `IJwtSettings`, `IOidcService`, `UserService`.
   - Test login success, failure, refresh logic, SSO callback mapping.
   - Validate correct HTTP status codes and response payloads.

10. **Code Samples & Examples**
    - Reference `.cursor/copilot/examples/backend/services/AuthService.cs` and `AuthController.cs` for patterns.

### 🔄 Integration Points
```yaml
CONFIG:
  - backend:
    - `appsettings.json`: Add sections: JwtSettings, OidcSettings, AuthSettings
    - `Program.cs`: Authentication and Authorization middleware

ROUTES:
  - `/api/auth/login`
  - `/api/auth/refresh`
  - `/api/auth/sso/callback`

DATABASE:
  - `RefreshTokens` table migration

NAVIGATION:
  - N/A (API only)

INTERNATIONALIZATION:
  - N/A
```

### 🧪 Validation Loop
```powershell
# Backend validation
dotnet build backend/Ikhtibar.API/Ikhtibar.API.csproj
dotnet test Ikhtibar.Tests/Ikhtibar.Tests.csproj
dotnet format --verify-no-changes
``` 

### 🚨 Anti-Patterns to Avoid
```csharp
// ❌ DON'T write authentication logic in controllers beyond orchestration
// ❌ DON'T use sync HTTP calls in OidcService
// ❌ DON'T expose detailed error info in production
// ❌ DON'T swallow exceptions silently
``` 

### 📋 Quality Gates
- [ ] All authentication endpoints tested
- [ ] JWT tokens validated for signature, issuer, audience
- [ ] Refresh token persistence and rotation verified
- [ ] SRP compliance: Services only handle auth logic, controllers only orchestrate
- [ ] No formatting changes pending
