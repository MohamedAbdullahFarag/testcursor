# PRP: Base Repository Pattern

## Module: Core
## Feature: Base Repository Pattern (02)

### 🎯 Objective
Define and implement a generic repository pattern to centralize data access logic for all core entities, following SRP and folder-per-feature conventions.

### 📦 Dependencies
- Core Entities Setup PRP (`01-core-entities-setup-prp.md`) must be completed.
- Database schema available in `.cursor/copilot/requirements/schema.sql`.

### 📝 Context
- Implement `IRepository<T>` interface in the `Ikhtibar.Core` project under `Repositories/Interfaces`.
- Create `BaseRepository<T>` in `Ikhtibar.Infrastructure/Repositories` that implements async CRUD operations using Dapper.
- Ensure decimal precision, soft-delete patterns (where applicable), and parameterized queries are handled.
- Use connection factory pattern for database connections and proper disposal.

### 🔧 Implementation Tasks
1. **Define IRepository Interface** (`Ikhtibar.Core/Repositories/Interfaces/IRepository.cs`)
   - Add methods:
     ```csharp
     Task<T?> GetByIdAsync(int id);
     Task<IEnumerable<T>> GetAllAsync(string? where = null, object? parameters = null);
     Task<T> AddAsync(T entity);
     Task<T> UpdateAsync(T entity);
     Task<bool> DeleteAsync(int id);
     Task<bool> ExistsAsync(int id);
     Task<IEnumerable<T>> QueryAsync(string sql, object? parameters = null);
     ```
   - ❌ DON'T: Include business logic or validation here.

2. **Implement BaseRepository Class** (`Ikhtibar.Infrastructure/Repositories/BaseRepository.cs`)
   - Inject `IDbConnectionFactory` via constructor.
   - Implement all methods asynchronously with Dapper.
   - Apply soft-delete filters automatically (IsDeleted = 0).
   - Use parameterized queries for all database operations.
   - ❌ DON'T: Catch generic `Exception` without rethrowing or wrapping.

3. **Create Connection Factory** (`Ikhtibar.Infrastructure/Data/IDbConnectionFactory.cs` & `DbConnectionFactory.cs`)
   - Implement factory pattern for database connections.
   - Register in DI container with proper lifetime management.
   - Support SQL Server connections using Microsoft.Data.SqlClient.

4. **Register in DI Container** (`Ikhtibar.API/Program.cs`)
   - In `Program.cs`, add:
     ```csharp
     services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
     services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
     ```
   - ❌ DON'T: Register concrete repositories here—use generic injection.

5. **Write Unit Tests** (`Ikhtibar.Tests/Repositories/BaseRepositoryTests.cs`)
   - Use in-memory SQLite or test containers.
   - Test CRUD operations and filter behavior. Follow AAA pattern.

6. **Code Samples & Examples**
   - Reference `.cursor/copilot/examples/backend/repositories/BaseRepository.cs` and `IRepository.cs` for patterns.

### 🔄 Integration Points
```yaml
DATABASE:
  - migration: "Database schema created manually using schema.sql"
  - indexes: "Create indexes as needed for performance"

CONFIG:
  - backend:
      - `Program.cs` DI registration for Dapper
      - Connection string configuration in `appsettings.json`
      - IDbConnectionFactory registration

ROUTES:
  - None (repositories are consumed by services)

NAVIGATION:
  - N/A

INTERNATIONALIZATION:
  - N/A
``` 

### 🧪 Validation Loop
```powershell
# Backend validation
dotnet build Ikhtibar.API/Ikhtibar.API.csproj
dotnet test Ikhtibar.Tests/Ikhtibar.Tests.csproj
dotnet format --verify-no-changes
``` 

### 🚨 Anti-Patterns to Avoid
```csharp
// ❌ DON'T mix business logic in repositories
// ❌ DON'T use string concatenation for SQL queries (SQL injection risk)
// ❌ DON'T swallow exceptions without logging or rethrowing
// ❌ DON'T hardcode entity-specific queries in base repository
// ❌ DON'T forget to dispose database connections
// ❌ DON'T use synchronous Dapper calls (e.g., Query() instead of QueryAsync())
``` 

### 📋 Quality Gates
- [ ] All methods async and tested
- [ ] Coverage > 80% for repository code
- [ ] No formatting changes pending
- [ ] SRP compliance verified (Repository only handles data access)
