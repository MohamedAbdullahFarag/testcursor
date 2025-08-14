# PRP: User Management Backend Services

## Module: User Management
## Feature: Backend Services (06)

### 🎯 Objective
Implement complete backend support for user management: CRUD operations for users, roles, and user–role assignments respecting SRP, using repository and service layers.

### 📦 Dependencies
- Core Entities Setup PRP (`01-core-entities-setup-prp.md`)
- Base Repository Pattern PRP (`02-base-repository-pattern-prp.md`)
- API Foundation PRP (`03-api-foundation-prp.md`)
- Authentication System PRP (`04-authentication-system-prp.md`)
- Seed data definitions in `data.sql`

### 📝 Context
- Projects involved:
  - `Ikhtibar.Core` (Entities, Interfaces, DTOs)
  - `Ikhtibar.Infrastructure` (Repositories)
  - `Ikhtibar.API` (Controllers, DTOs)
- Entities: `User`, `Role`, `UserRole` in `Ikhtibar.Core/Entities`
- DTOs for user creation, update, and response already partially defined.
- Example patterns in `.github/copilot/examples/backend/services/UserService.cs` and `UsersController.cs`.

### 🔧 Implementation Tasks
1. **Define Repository Interfaces** (`Ikhtibar.Core/Repositories/Interfaces`)
   - `IUserRepository : IRepository<User>` with extra:
     ```csharp
     Task<User?> GetByEmailAsync(string email);
     Task<bool> UserExistsAsync(Guid id);
     ```
   - `IRoleRepository : IRepository<Role>` with extra:
     ```csharp
     Task<Role?> GetByCodeAsync(string code);
     ```
   - `IUserRoleRepository` for assignments:
     ```csharp
     Task AssignRoleAsync(Guid userId, Guid roleId);
     Task RemoveRoleAsync(Guid userId, Guid roleId);
     Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId);
     ```
   - ❌ DON'T mix other entity logic here.

2. **Implement Repositories** (`Ikhtibar.Infrastructure/Repositories`)
   - Create `UserRepository`, `RoleRepository`, `UserRoleRepository` inheriting `BaseRepository<T>` and implementing above interfaces.
   - Use Dapper for async database operations with proper SQL queries.
   - Ensure no business logic or cross-entity joins.

3. **Define Service Interfaces & DTOs** (`Ikhtibar.Core/Services` & `API/DTOs`)
   - `IUserService`: methods: `CreateUserAsync(CreateUserDto)`, `GetUserAsync(Guid)`, `UpdateUserAsync(Guid, UpdateUserDto)`, `DeleteUserAsync(Guid)`, `GetAllUsersAsync()`.
   - `IRoleService`: similar for roles. Also methods to seed default roles.
   - `IUserRoleService`: assign/remove roles.
   - DTOs: `CreateUserDto`, `UpdateUserDto`, `UserDto`, `RoleDto`, `AssignRoleDto` under `Ikhtibar.API/DTOs`.

4. **Implement Services** (`Ikhtibar.Core/Services/Implementations`)
   - `UserService` injects `IUserRepository`, `IMapper`, `ILogger`.
   - Validate inputs, map DTO to Entity, call repository, map back.
   - `RoleService` and `UserRoleService` follow SRP.
   - ❌ DON'T perform HTTP or persistence here beyond repos.

5. **Configure DI** (`Program.cs`)
   - Register:
     ```csharp
     services.AddScoped<IUserRepository, UserRepository>();
     services.AddScoped<IRoleRepository, RoleRepository>();
     services.AddScoped<IUserRoleRepository, UserRoleRepository>();
     services.AddScoped<IUserService, UserService>();
     services.AddScoped<IRoleService, RoleService>();
     services.AddScoped<IUserRoleService, UserRoleService>();
     ```

6. **Controllers** (`Ikhtibar.API/Controllers/UserManagement`)
   - `UsersController` inherits `ApiControllerBase`:
     - `GET /api/users` → `GetAllUsersAsync`
     - `GET /api/users/{id}` → `GetUserAsync`
     - `POST /api/users` → `CreateUserAsync`
     - `PUT /api/users/{id}` → `UpdateUserAsync`
     - `DELETE /api/users/{id}` → `DeleteUserAsync`
   - `RolesController` and `UserRolesController` similarly.
   - Use `ActionResult<T>`, proper status codes and Swagger comments.

7. **Database Schema**
   - Create SQL scripts for: `Users`, `Roles`, `UserRoles` tables, with FKs and indexes on `Email` and `Code`.

8. **Unit Tests** (`Ikhtibar.Tests/UserManagement`)
   - Mock repositories for service tests (e.g., `UserServiceTests`).
   - Integration tests for controllers using `WebApplicationFactory`.
   - Validate status codes, payloads, error states.

9. **Seed Roles & Permissions**
   - Ensure `data.sql` runs on startup or via migration script to populate roles and permissions.

10. **Examples & References**
    - Check `.github/copilot/examples/backend/controllers/UsersController.cs` and `.github/copilot/examples/backend/services/UserService.cs` for style.

### 🔄 Integration Points
```yaml
DATABASE:
  - migration: "Add Users, Roles, UserRoles tables"
  - indexes: "Email(unq), Code(unq)"

CONFIG:
  - backend: Program.cs DI registration

ROUTES:
  - `/api/users`, `/api/roles`, `/api/user-roles`

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
// ❌ DON'T mix role logic into UserRepository
// ❌ DON'T embed SQL or raw queries in repositories
// ❌ DON'T catch generic exceptions without logging
// ❌ DON'T return EF entities directly from controllers
``` 

### 📋 Quality Gates
- [ ] CRUD operations implemented and tested
- [ ] >80% coverage for service/repository
- [ ] SRP compliance for all classes (one responsibility)
- [ ] No linting or formatting errors
- [ ] Data integrity enforced (unique email/role codes)
