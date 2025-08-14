# PRP-01: Core Entities Setup

## 🎯 Context Analysis
### What We're Building
All foundational database entities necessary across the Ikhtibar platform, including users, roles, permissions,
tree nodes, question types, difficulty levels, statuses, media types, and junction tables.

### Why This Matters
- Establishes domain model shared by every feature
- Enables repository and service scaffolding
- Ensures consistent data relationships and constraints

### Dependencies
None (Foundation layer)

## 📊 Data Models
Use the provided schema.sql to generate:
```sql
-- Lookup tables: QuestionTypes, DifficultyLevels, QuestionStatuses, MediaTypes, TreeNodeTypes
-- Core tables: Users, Roles, Permissions, RolePermissions, UserRoles, TreeNodes, Media
-- Content tables: Questions, Answers, QuestionMedia, AnswerMedia
-- Auth tables: RefreshTokens, LoginAttempts
-- CurriculumAlignments, Exams, ExamQuestions
``` 

## 🛠️ Implementation Tasks

### Task 1: Dapper Entities & Data Access
- Create C# classes under `Ikhtibar.Shared.Entities` matching each table
- Implement base entity class with common properties (Id, CreatedAt, UpdatedAt, IsDeleted)
- Use data annotations for validation and mapping hints

### Task 2: DTO Definitions
- Add DTO classes in `Ikhtibar.Shared.DTOs` for Users, Roles, Permissions, Questions, etc.
- Include validation attributes and summary XML

### Task 3: Database Schema
- Create database schema using provided `schema.sql`
- Set up connection factory for Dapper data access

## 🔗 Integration Points
```yaml
database:
  - create schema: Run schema.sql on target database
  - connection: Configure connection string in appsettings.json
config:
  - appsettings.json: ConnectionStrings:IkhtibarDatabase
  - Program.cs: Register Dapper connection factory
```

## 🧪 Validation Loops
```bash
# Backend validation
dotnet build
dotnet test
```

## 🚨 Anti-Patterns to Avoid
- ❌ Don't store business logic in entities
- ❌ Don't ignore soft-delete patterns

## 🎯 Quality Gates
- [ ] Entities compile
- [ ] Database schema created
- [ ] Connection factory working
- [ ] No build warnings
