# GitHub Copilot Instruction System - Ikhtibar Project

This directory contains a comprehensive set of GitHub Copilot instruction files designed for the Ikhtibar educational exam management system. These instructions automatically enhance AI responses based on file types and provide specialized guidance for different development scenarios.

## 📋 Quick Reference

### Main Instruction File
- **`../.github/copilot-instructions.md`**: Master instruction file with complete project context and foundation information

### File-Based Auto-Application
Instructions automatically apply to relevant files based on patterns. Manual reference is also available.

## 🎯 Core Development Instructions

### [General Features](./general-features.instructions.md)
**Auto-applies to**: `**/*.cs,**/*.js,**/*.ts,**/*.tsx,**/*.json`  
**Purpose**: Backend/Frontend feature development, API creation, component building  
**Foundation**: Builds on established PRP-01 through PRP-04 patterns

### [TypeScript Features](./typescript-features.instructions.md)  
**Auto-applies to**: `**/*.ts,**/*.tsx,**/tsconfig.json,**/package.json`  
**Purpose**: Type-safe frontend development with React 18 patterns  
**Foundation**: Uses established TypeScript interfaces and component patterns

### [General Rules](./general-rules.instructions.md)
**Auto-applies to**: `**/*`  
**Purpose**: Project-wide conventions and GitHub Copilot optimization  
**Foundation**: Enforces Single Responsibility Principle and validation loops

## 🏗️ Architecture & Design Instructions

### [API Guidelines](./api-guidelines.instructions.md)
**Auto-applies to**: `**/*.cs,**/Controllers/**,**/DTOs/**`  
**Purpose**: REST API design, HTTP status codes, validation patterns  
**Foundation**: Uses established BaseRepository and controller patterns

### [Backend Guidelines](./backend-guidelines.instructions.md)
**Auto-applies to**: `**/*.cs,**/Services/**,**/Repositories/**`  
**Purpose**: Clean architecture, repository patterns, service layer design  
**Foundation**: Builds on Dapper ORM and BaseRepository<T> implementation

### [Frontend Guidelines](./frontend-guidelines.instructions.md)
**Auto-applies to**: `**/*.tsx,**/components/**,**/modules/**`  
**Purpose**: Component architecture, state management, UI patterns  
**Foundation**: Uses Zustand stores and established React patterns

### [React Guidelines](./react-guidelines.instructions.md)
**Auto-applies to**: `**/*.tsx,**/*.jsx,**/hooks/**`  
**Purpose**: Advanced React development patterns and performance optimization  
**Foundation**: Uses React Query and i18n established setup

## 📋 Planning & Implementation Instructions

### [Planning & PRD](./planning-prd.instructions.md)
**Auto-applies to**: `**/*.md,**/README.md,**/*.txt,**/docs/**`  
**Purpose**: Feature planning, PRD creation, Mermaid diagrams  
**Foundation**: References established architecture and patterns

### [Feature Specifications](./feature-specifications.instructions.md)
**Auto-applies to**: `**/*.md,**/specifications/**,**/docs/**`  
**Purpose**: Breaking down complex features into implementation tasks  
**Foundation**: Uses established validation loops and file patterns

### [Task Management](./task-management.instructions.md)
**Auto-applies to**: `**/*.md,**/tasks/**,**/project-management/**`  
**Purpose**: Task breakdown with validation commands and concise execution  
**Foundation**: Provides executable validation commands for all changes

### [Implementation Guide](./implementation-guide.instructions.md)
**Auto-applies to**: `**/docs/**,**/planning/**`  
**Purpose**: Systematic feature implementation with dependency ordering  
**Foundation**: Uses PRP execution methodology for structured development

---

### 📋 [Planning & PRD](./planning-prd.instructions.md)
**Applies to**: `**/*.md,**/README.md,**/*.txt,**/docs/**`  
**Purpose**: Planning and PRD generation with visual diagrams and comprehensive analysis  
**Use for**: Feature planning, technical specifications, requirement gathering

**Key Features**:
- Mermaid diagram generation
- Comprehensive PRD templates
- Risk analysis and validation
- Implementation planning

---

### 📊 [Feature Specifications](./feature-specifications.instructions.md)
**Applies to**: `**/*.md,**/specifications/**,**/docs/**`  
**Purpose**: Detailed technical specifications with low-level implementation tasks  
**Use for**: Breaking down complex features into actionable tasks

**Key Features**:
- High-level to low-level decomposition
- Implementation-ready task lists
- Context-aware development guidance
- Comprehensive validation criteria

---

### ✅ [Task Management](./task-management.instructions.md)
**Applies to**: `**/*.md,**/tasks/**,**/project-management/**`  
**Purpose**: Task breakdown and management with validation loops  
**Use for**: Project planning, task creation, development workflow management

**Key Features**:
- Executable task templates
- Validation commands for each task
- Debug patterns and troubleshooting
- Dependency management

---

### 🚀 [API Guidelines](./api-guidelines.instructions.md)
**Applies to**: `**/*.cs,**/Controllers/**,**/DTOs/**`  
**Purpose**: REST API design guidelines with validation patterns and HTTP status codes  
**Use for**: API development, controller implementation, DTO design

**Key Features**:
- RESTful API standards
- HTTP status code guidelines
- Request/response format patterns
- Authentication and authorization patterns

---

### 🏗️ [Backend Guidelines](./backend-guidelines.instructions.md)
**Applies to**: `**/*.cs,**/Services/**,**/Repositories/**`  
**Purpose**: Backend architecture guidelines with clean architecture patterns  
**Use for**: Service layer design, repository patterns, dependency injection

**Key Features**:
- Clean Architecture implementation
- Repository and service patterns
- Dependency injection configuration
- Single Responsibility Principle enforcement

---

### 🎨 [Frontend Guidelines](./frontend-guidelines.instructions.md)
**Applies to**: `**/*.tsx,**/components/**,**/modules/**`  
**Purpose**: Frontend architecture guidelines with component patterns and state management  
**Use for**: Component architecture, UI patterns, state management

**Key Features**:
- Folder-per-feature structure
- Component templates and patterns
- Error handling and loading states
- Performance optimization guidelines

---

### ⚛️ [React Guidelines](./react-guidelines.instructions.md)
**Applies to**: `**/*.tsx,**/*.jsx,**/hooks/**`  
**Purpose**: React 19 development patterns with hooks and performance optimization  
**Use for**: React component development, custom hooks, modern React patterns

**Key Features**:
- React 19 compiler optimizations
- TypeScript integration patterns
- Modern React patterns (Actions, use() API)
- Performance best practices

---

### 📖 [General Rules](./general-rules.instructions.md)
**Applies to**: `**/*`  
**Purpose**: Project-wide conventions and GitHub Copilot optimization guidelines  
**Use for**: All development activities, AI-powered development workflows

**Key Features**:
- GitHub Copilot optimization principles
- Context engineering techniques
- Validation-first development
- Cross-cutting concerns

---

### 🚀 [Implementation Guide](./implementation-guide.instructions.md)
**Applies to**: `**/docs/**,**/planning/**`  
**Purpose**: Dependency-ordered feature implementation with PRP execution guide  
**Use for**: Feature implementation planning, execution order, integration strategy

**Key Features**:
- Dependency-ordered implementation
- PRP execution commands
- Layer-by-layer development approach
- Integration and testing strategies

## How These Instructions Work

### Automatic Application
GitHub Copilot automatically applies these instructions based on the `applyTo` glob patterns in each file's front matter. When you work on files matching these patterns, the relevant instructions are automatically included in your chat context.

### Manual Application
You can also manually reference these instructions in your Copilot chat:
- Use the Configure Chat button → Instructions → Select instruction file
- Reference by name: "Follow the TypeScript Features instructions..."
- Upload as context when starting conversations

### File Structure and Naming
- **Location**: `.github/instructions/` (workspace-specific)
- **Naming**: `[purpose].instructions.md`
- **Format**: Markdown with YAML front matter
- **Scope**: Automatically applied based on file patterns

## Integration with Project

### Project Context
All instruction files are tailored specifically for the **Ikhtibar Educational Exam Management System**:

**Backend Stack**:
- ASP.NET Core 8.0 Web API
- Dapper ORM with SQL Server
- Clean Architecture with folder-per-feature
- JWT authentication with role-based access

**Frontend Stack**:
- React.js 18 with TypeScript
- Vite build tool with Tailwind CSS
- React Query + Zustand state management
- i18next internationalization (English/Arabic)

### Architectural Patterns
The instructions enforce these key patterns:
- **Folder-per-feature** organization
- **Single Responsibility Principle** adherence
- **Repository pattern** for data access
- **Service layer** for business logic
- **Validation-first** development approach

## Usage Examples

### For Backend Development
When working on `.cs` files, the General Features instructions automatically apply:
```csharp
// Instructions will guide you to:
// - Follow Clean Architecture patterns
// - Implement proper error handling
// - Use repository and service patterns
// - Include comprehensive validation
```

### For Frontend Development  
When working on `.tsx` files, TypeScript Features instructions apply:
```typescript
// Instructions will guide you to:
// - Use strict TypeScript typing
// - Follow React 18 patterns
// - Implement proper state management
// - Support internationalization
```

### For Documentation
When working on `.md` files, Planning instructions apply:
```markdown
<!-- Instructions will guide you to: -->
<!-- - Create comprehensive specifications -->
<!-- - Include Mermaid diagrams -->
<!-- - Define clear acceptance criteria -->
<!-- - Plan implementation phases -->
```

## Best Practices

### 1. Trust the Patterns
These instructions codify proven patterns from the Ikhtibar project. Follow the guidance for consistent, high-quality code.

### 2. Use Validation Commands
Each instruction includes validation commands. Always run these to ensure code quality:
```bash
# Backend validation
dotnet build --configuration Release
dotnet test

# Frontend validation  
npm run type-check
npm run lint
npm run test
```

### 3. Follow the Anti-Patterns
Pay attention to the "❌ DON'T" examples in each instruction file. These prevent common mistakes.

### 4. Reference Existing Code
Instructions emphasize finding and following existing patterns. Always check for similar implementations first.

### 5. Maintain Consistency
Use the same patterns across features for maintainable, predictable code architecture.

## Updating Instructions

### When to Update
- Project architecture changes
- New patterns or standards adopted
- Common issues identified
- Technology stack updates

### How to Update
1. Modify the relevant `.instructions.md` file
2. Test with sample scenarios
3. Update examples and patterns
4. Communicate changes to team

### Version Control
- All instruction files are version controlled
- Track changes to understand pattern evolution
- Review changes during code reviews
- Document reasoning for major updates

## Troubleshooting

### Instructions Not Applied
- Check the `applyTo` glob pattern matches your file
- Verify VS Code setting: `github.copilot.chat.codeGeneration.useInstructionFiles: true`
- Manually attach instructions if needed

### Conflicting Instructions
- Instructions are combined without priority order
- Avoid conflicting guidance between files
- Use specific `applyTo` patterns to prevent conflicts

### Instructions Too Generic
- Make instructions more specific to file types
- Include project-specific examples and patterns
- Reference actual files and implementations from the codebase

---

These custom instructions ensure that GitHub Copilot provides consistent, high-quality responses that align with the Ikhtibar project's architecture, patterns, and standards. They transform generic AI responses into project-specific, actionable guidance that accelerates development while maintaining code quality.
