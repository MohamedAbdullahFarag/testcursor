# PRP: API Foundation

## Module: Core
## Feature: API Foundation (03)

### 🎯 Objective
Establish the ASP.NET Core Web API base structure: configure middleware pipeline, routing, controllers scaffold, OpenAPI/Swagger, health checks, CORS, logging, and error handling.

### 📦 Dependencies
- Core Entities Setup PRP (`01-core-entities-setup-prp.md`)
- Base Repository Pattern PRP (`02-base-repository-pattern-prp.md`)
- Feature requirements in `ikhtibar-features.txt`

### 📝 Context
- Project: `backend/Ikhtibar.API`
- Use folder-per-feature for controllers under `Controllers/`
- Configure global settings in `Program.cs` and `appsettings.json`
- Follow `.github/copilot/examples/backend/controllers/UsersController.cs` and API guidelines

### 🔧 Implementation Tasks
1. **Configure Middleware & Services** (`Program.cs`)
   - Add JSON options, CORS policy (`AllowAllOrigins` for dev)
   - Register `IRepository<>`, `DbContext`, and logging
   - Configure `SwaggerGen` with OpenAPI information and XML comments

2. **Global Error Handling**
   - Implement custom middleware `ErrorHandlingMiddleware`
   - Map exceptions to appropriate `ProblemDetails`
   - ❌ DON'T: Use `app.UseDeveloperExceptionPage()` in production

3. **Health Checks**
   - Add `AddHealthChecks()` and map `/health` endpoint
   - Include a database readiness check using EF Core

4. **Base Controller Scaffold**
   - Create `ApiControllerBase.cs` in `Controllers/Base` with:
     ```csharp
     [ApiController]
     [Route("api/[controller]")]
     public abstract class ApiControllerBase : ControllerBase { }
     ```
   - ❌ DON'T: Include business logic here

5. **Sample Controller**
   - Add `HealthController : ApiControllerBase` with GET `/ping` returning 200 OK
   - Add template `SampleController` illustrating DI of `IService<T>`

6. **Swagger & Documentation**
   - Enable `UseSwagger()` and `UseSwaggerUI()`
   - Decorate controllers with `ProducesResponseType` and summary comments

7. **Unit Tests** (`Ikhtibar.Tests/Api`)
   - Test `ErrorHandlingMiddleware`, `HealthController`, and pipeline startup
   - Validate status codes and JSON schema

### 🔄 Integration Points
```yaml
CONFIG:
  - backend:
    - `Program.cs`: CORS and middleware additions
    - `appsettings.json`: Swagger sections, Logging levels

ROUTES:
  - Base path: `/api`
  - Health endpoint: `/api/health`
  - Swagger UI: `/swagger`

DATABASE:
  - EF migrations (no new models)

NAVIGATION:
  - N/A

INTERNATIONALIZATION:
  - Ensure default culture is set to `en-US` in middleware
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
// ❌ DON'T mix business logic in controllers
// ❌ DON'T hardcode CORS origins
// ❌ DON'T catch generic exceptions in controllers
```  

### 📋 Quality Gates
- [ ] Middleware pipeline starts correctly
- [ ] Swagger UI displays all controllers
- [ ] Health endpoint passes
- [ ] No formatting errors
- [ ] SRP compliance verified
