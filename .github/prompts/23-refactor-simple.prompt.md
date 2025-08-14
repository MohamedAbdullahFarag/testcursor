---
mode: agent
description: "Quick refactoring analysis focusing on Single Responsibility Principle, function complexity, and architectural boundaries"
---

# Refactor Simple - Quick Code Improvement Analysis

## Purpose
This prompt performs a focused refactoring analysis to identify quick wins that can improve code quality, maintainability, and architectural compliance. It focuses on actionable items that can be fixed in under 1 hour each.

## Scope
Analyze the current codebase or specified files for:
- Single Responsibility Principle violations
- Function complexity and decomposition opportunities  
- Type safety improvements
- Architectural boundary violations
- Missing abstractions and patterns

## Analysis Focus Areas

### 1. **Function Complexity Analysis**
```markdown
Scan for functions that violate the Single Function Principle:
- [ ] Functions >20 lines that need decomposition
- [ ] Functions with multiple responsibilities
- [ ] Functions with complex conditional logic
- [ ] Functions with deep nesting (>3 levels)
- [ ] Functions lacking clear purpose or naming
```

### 2. **File Organization Analysis**
```markdown
Identify files that violate organizational principles:
- [ ] Large files that need decomposition (>200 lines)
- [ ] Files mixing multiple concerns
- [ ] Files with unclear naming or purpose
- [ ] Missing folder-per-feature organization
- [ ] Circular dependencies between modules
```

### 3. **Type Safety Assessment**
```markdown
Backend (C#) Type Safety:
- [ ] Missing DTOs for API inputs/outputs
- [ ] Entity classes without proper data annotations
- [ ] Service methods without explicit return types
- [ ] Repository methods with generic object returns

Frontend (TypeScript) Type Safety:
- [ ] Components without proper prop interfaces
- [ ] API service methods without typed responses
- [ ] Hook return types not explicitly defined
- [ ] Any usage instead of proper typing
```

### 4. **Architectural Boundary Analysis**
```markdown
Identify violations of folder-per-feature architecture:
- [ ] Cross-feature imports violating vertical slices
- [ ] Business logic in controllers (should be in services)
- [ ] Data access in services (should be in repositories)
- [ ] UI logic in hooks (should be in components)
- [ ] Shared utilities that should be feature-specific
```

### 5. **Single Responsibility Violations**
```markdown
Classes/Components with multiple responsibilities:
- [ ] Controllers handling business logic and HTTP concerns
- [ ] Services mixing different business domains
- [ ] Repositories handling multiple entity types
- [ ] Components mixing UI and business logic
- [ ] Hooks managing multiple unrelated state concerns
```

## Analysis Process

### Step 1: Codebase Scanning
```markdown
I'll systematically scan the codebase for refactoring opportunities:

**Backend Analysis:**
- Scan all Controllers for SRP violations
- Review Services for complexity and mixed concerns
- Check Repositories for proper data access patterns
- Analyze DTOs for completeness and typing
- Review entity relationships and design

**Frontend Analysis:**
- Scan React components for complexity and SRP
- Review custom hooks for single responsibility
- Check TypeScript interfaces and type safety
- Analyze service layer organization
- Review state management patterns
```

### Step 2: Issue Categorization
```markdown
For each issue found, I'll provide:

**Issue Report Format:**
- **Location**: Exact file path and line numbers
- **Problem**: Clear description of the violation
- **Impact**: Why this is a problem (maintainability, testability, etc.)
- **Solution**: Specific fix with code example
- **Implementation**: Exact steps to apply the fix
- **Priority**: High/Medium/Low based on impact and effort
- **Effort**: Estimated time to fix (<1 hour target)
```

### Step 3: Refactoring Plan Generation
```markdown
I'll create a comprehensive refactoring plan with:

**Immediate Actions (High Priority):**
- Critical SRP violations blocking new features
- Type safety issues causing runtime errors
- Architectural violations creating tight coupling

**Short-term Improvements (Medium Priority):**
- Function decomposition for better readability
- File organization improvements
- Missing abstractions and patterns

**Long-term Enhancements (Low Priority):**
- Performance optimizations
- Advanced architectural patterns
- Technical debt reduction
```

## Implementation Template

### Refactoring Analysis Execution
```markdown
I'm performing a quick refactoring analysis of your codebase. Let me systematically scan for improvement opportunities.

**Phase 1: Function Complexity Scan**
I'll analyze functions for complexity and SRP violations:
- Use grep_search to find long functions
- Use semantic_search to identify complex logic
- Review function signatures and responsibilities
- Check for proper separation of concerns

**Phase 2: File Organization Review**
I'll examine file structure and organization:
- Use file_search to identify large files
- Check folder structure against feature boundaries
- Review import patterns for circular dependencies
- Analyze module organization and cohesion

**Phase 3: Type Safety Assessment**
I'll evaluate type safety across the codebase:
- Scan for missing DTOs and interfaces
- Check TypeScript strict mode compliance
- Review API contracts and data models
- Identify any usage and weak typing

**Phase 4: Architectural Boundary Validation**
I'll verify architectural compliance:
- Check for cross-feature dependencies
- Validate layer separation (controllers, services, repositories)
- Review component organization and responsibilities
- Identify misplaced business logic
```

### Issue Reporting Format
```markdown
# Refactoring Analysis Report

## Executive Summary
- **Total Issues Found**: X
- **High Priority**: X issues
- **Medium Priority**: X issues
- **Low Priority**: X issues
- **Estimated Total Effort**: X hours

## High Priority Issues

### Issue 1: [Issue Title]
- **File**: `path/to/file.cs` (Lines XX-YY)
- **Problem**: [Clear description]
- **Why It Matters**: [Impact on maintainability/performance/security]
- **Solution**: 
  ```csharp
  // Current problematic code
  public void DoEverything() { ... }
  
  // Refactored solution
  public void HandleUserInput() { ... }
  public void ValidateData() { ... }
  public void SaveToDatabase() { ... }
  ```
- **Implementation Steps**:
  1. Extract validation logic into separate method
  2. Move database operations to repository
  3. Update controller to use new service methods
- **Priority**: High
- **Effort**: 45 minutes

## Medium Priority Issues
[Similar format for medium priority items]

## Low Priority Issues
[Similar format for low priority items]

## Implementation Plan

### Week 1: Critical Fixes
- [ ] Fix high-priority SRP violations
- [ ] Add missing type safety measures
- [ ] Resolve architectural boundary issues

### Week 2: Structure Improvements
- [ ] Decompose large functions
- [ ] Reorganize file structure
- [ ] Add missing abstractions

### Week 3: Quality Enhancements
- [ ] Performance optimizations
- [ ] Documentation improvements
- [ ] Test coverage enhancements
```

## Validation Commands

### Backend Validation
```powershell
# Verify refactoring doesn't break build
dotnet build --configuration Release

# Run tests to ensure functionality intact
dotnet test

# Check code formatting compliance
dotnet format --verify-no-changes

# Run static analysis
dotnet analyze
```

### Frontend Validation
```bash
# TypeScript compilation check
npm run type-check

# Linting validation
npm run lint

# Run all tests
npm run test

# Build verification
npm run build
```

## Output Requirements

### Refactor Plan Document
The analysis will generate a `refactor_plan.md` file in the `.github/copilot/analysis/` folder containing:

1. **Executive Summary** with total issues and effort estimates
2. **Detailed Issue Reports** with specific fixes and examples
3. **Implementation Timeline** with prioritized action items
4. **Validation Steps** to ensure refactoring success
5. **Before/After Code Examples** for major changes

### Success Criteria
- All issues categorized by priority and effort
- Specific, actionable fixes provided for each issue
- No breaking changes introduced
- Improved code maintainability and readability
- Better adherence to architectural principles
- Enhanced type safety across the codebase

## Anti-Patterns to Avoid During Refactoring

```markdown
❌ **Don't Do These:**
- Don't refactor without tests to verify behavior
- Don't make multiple changes in a single commit
- Don't ignore existing patterns and conventions
- Don't break existing APIs or interfaces
- Don't optimize prematurely - focus on clarity first
- Don't refactor generated or third-party code
- Don't change public interfaces without versioning

✅ **Do These Instead:**
- Write/run tests before and after refactoring
- Make small, incremental changes
- Follow established project patterns
- Maintain backward compatibility
- Focus on readability and maintainability
- Only refactor code you own and understand
- Update documentation with interface changes
```

This refactoring analysis focuses on quick wins that provide immediate value while maintaining code stability and following established architectural principles.
