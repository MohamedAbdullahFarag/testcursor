---
mode: agent
description: "Comprehensive code quality analysis, refactoring recommendations, and review system for multi-tech projects with SRP compliance validation"
---

---
inputs:
  - name: action
    description: Code quality action to perform (refactor, review, review-changes)
    required: true
  - name: scope
    description: Scope of analysis (current, staged, unstaged, all, file-path, folder-path)
    required: false
---
command: "/code-quality"
---
# Code Quality Command for GitHub Copilot

## Command Usage
```
@copilot /code-quality [action] [scope]
```

## Purpose
This command provides comprehensive code quality analysis and improvements for the Ikhtibar project, supporting C# backend and TypeScript/React frontend code. It follows project-specific patterns, SRP compliance, and architectural guidelines.

**Input Parameters**: 
- `action` - Code quality action: `refactor`, `review`, or `review-changes`
- `scope` - Analysis scope: `current`, `staged`, `unstaged`, `all`, specific file path, or folder path

## How /code-quality Works

### Phase 1: Action Selection and Context Analysis
```markdown
I'll help you with code quality improvements for the Ikhtibar project. Let me analyze your request and gather the necessary context.

**Phase 1.1: Parse Request Parameters**
```
Request Analysis:
- **Action**: [REFACTOR/REVIEW/REVIEW-CHANGES]
- **Scope**: [CURRENT/STAGED/UNSTAGED/ALL/FILE_PATH/FOLDER_PATH]
- **Project Type**: ASP.NET Core + React.js with TypeScript
- **Architecture**: Folder-per-feature with Clean Architecture principles
```

**Phase 1.2: Context Gathering using GitHub Copilot Tools**
```
I'll gather project context using GitHub Copilot's native tools:
- get_changed_files: [SCOPE_BASED_FILTER] # Get relevant changed files
- semantic_search: "code quality patterns" # Find existing quality patterns
- file_search: "**/*.cs" # Backend files (if applicable)
- file_search: "**/*.ts*" # Frontend files (if applicable)
- read_file: [RELEVANT_GUIDELINE_FILES] # Load project guidelines
- grep_search: "TODO|FIXME|HACK" # Find code quality issues
```

**Phase 1.3: Architecture and Pattern Analysis**
```
Project-Specific Analysis:
- [ ] **Tech Stack**: ASP.NET Core (.NET 8), React 18, TypeScript, Tailwind CSS
- [ ] **Architecture Patterns**: Folder-per-feature, Clean Architecture, CQRS
- [ ] **Code Patterns**: Repository, Service Layer, Controller patterns
- [ ] **SRP Compliance**: Single Responsibility Principle enforcement
- [ ] **Testing Patterns**: Unit tests, integration tests, type safety
- [ ] **Security Patterns**: Input validation, SQL injection prevention, XSS protection
- [ ] **Performance Patterns**: Caching, lazy loading, bundle optimization
- [ ] **i18n Patterns**: i18next, RTL support, Arabic/English localization
```
```

### Phase 2: Action-Specific Analysis

#### For Action: `refactor`
```markdown
**Phase 2.1: Refactoring Analysis using GitHub Copilot Tools**
```
I'll analyze the specified files/scope for refactoring opportunities:

## 🔧 Refactoring Analysis (Tool-Enhanced)

### SRP Compliance Analysis using GitHub Copilot Tools
```csharp
// Using semantic_search to find SRP violations
semantic_search: "mixed responsibilities|god class|large method"
file_search: [SCOPE_FILES] # Get all files in scope
get_errors: [SCOPE_FILES] # Check for compilation errors
```

### Code Quality Assessment with Tool Integration
- [ ] **Class Responsibilities**: [ANALYSIS_USING_SEMANTIC_SEARCH]
  - Tool: semantic_search for responsibility patterns
  - Status: [COMPLIANT/NEEDS_REFACTORING]
- [ ] **Method Complexity**: [ANALYSIS_USING_CODE_EXAMINATION]
  - Tool: grep_search for large methods
  - Status: [ACCEPTABLE/NEEDS_SPLITTING]
- [ ] **Dependency Management**: [ANALYSIS_USING_PATTERN_SEARCH]
  - Tool: semantic_search for dependency injection patterns
  - Status: [PROPERLY_INJECTED/NEEDS_REFACTORING]
- [ ] **Error Handling**: [ANALYSIS_USING_EXCEPTION_PATTERNS]
  - Tool: grep_search for try-catch patterns
  - Status: [COMPREHENSIVE/NEEDS_IMPROVEMENT]

### Refactoring Recommendations (Tool-Informed)
1. **Issue**: [SPECIFIC_SRP_VIOLATION]
   - **File**: [FILE_PATH]
   - **Detection Tool**: [semantic_search/grep_search]
   - **Problem**: [DETAILED_DESCRIPTION]
   - **Suggested Fix**: [REFACTORING_APPROACH]
   - **Implementation**: [STEP_BY_STEP_PLAN]

2. **Issue**: [PERFORMANCE_ISSUE]
   - **File**: [FILE_PATH]
   - **Detection Tool**: [pattern_analysis]
   - **Problem**: [DETAILED_DESCRIPTION]
   - **Suggested Fix**: [OPTIMIZATION_APPROACH]
   - **Implementation**: [STEP_BY_STEP_PLAN]

### Implementation Plan with Tool Support
```
I'll implement the refactoring using appropriate GitHub Copilot tools:
1. **Analysis Phase**: Use semantic_search and get_errors for comprehensive analysis
2. **Planning Phase**: Create detailed refactoring plan with SRP compliance
3. **Implementation Phase**: Use insert_edit_into_file/replace_string_in_file for changes
4. **Validation Phase**: Use run_in_terminal and get_errors for verification
5. **Testing Phase**: Use run_tests to ensure no regressions
```
```

#### For Action: `review`
```markdown
**Phase 2.2: Code Review Analysis using GitHub Copilot Tools**
```
I'll perform comprehensive code review based on the specified scope:

## 🔍 Code Review Analysis (Tool-Enhanced)

### Scope Determination with GitHub Copilot Tools
```powershell
# Determine review scope using appropriate tools
get_changed_files: [SCOPE_FILTER] # Get files based on scope
semantic_search: "code review patterns" # Find review guidelines
file_search: [SCOPE_PATTERN] # Get relevant files
read_file: [GUIDELINE_FILES] # Load review criteria
```

### Review Categories with Tool Integration

#### Architecture & Design Review
- [ ] **SRP Compliance**: [ASSESSMENT_WITH_SEMANTIC_SEARCH]
  - Tool: semantic_search for responsibility violations
  - Status: [COMPLIANT/VIOLATIONS_FOUND]
- [ ] **Pattern Consistency**: [ASSESSMENT_WITH_PATTERN_ANALYSIS]
  - Tool: comparison with existing patterns
  - Status: [CONSISTENT/INCONSISTENT]
- [ ] **Folder Structure**: [ASSESSMENT_WITH_FILE_ORGANIZATION]
  - Tool: file_search for structure analysis
  - Status: [CORRECT/NEEDS_ORGANIZATION]

#### Code Quality Review
- [ ] **Naming Conventions**: [ASSESSMENT_WITH_GREP_SEARCH]
  - Tool: grep_search for naming patterns
  - Status: [CONSISTENT/INCONSISTENT]
- [ ] **Error Handling**: [ASSESSMENT_WITH_EXCEPTION_ANALYSIS]
  - Tool: grep_search for try-catch patterns
  - Status: [COMPREHENSIVE/MISSING]
- [ ] **Async Patterns**: [ASSESSMENT_WITH_ASYNC_ANALYSIS]
  - Tool: semantic_search for async/await usage
  - Status: [PROPER/IMPROPER]

#### Security Review
- [ ] **Input Validation**: [ASSESSMENT_WITH_VALIDATION_SEARCH]
  - Tool: semantic_search for validation patterns
  - Status: [VALIDATED/MISSING_VALIDATION]
- [ ] **SQL Injection Prevention**: [ASSESSMENT_WITH_QUERY_ANALYSIS]
  - Tool: grep_search for SQL patterns
  - Status: [SAFE/POTENTIAL_ISSUES]
- [ ] **XSS Prevention**: [ASSESSMENT_WITH_XSS_ANALYSIS]
  - Tool: semantic_search for XSS patterns
  - Status: [PROTECTED/VULNERABLE]

#### Performance Review
- [ ] **Database Queries**: [ASSESSMENT_WITH_QUERY_OPTIMIZATION]
  - Tool: semantic_search for query patterns
  - Status: [OPTIMIZED/NEEDS_OPTIMIZATION]
- [ ] **Caching Strategy**: [ASSESSMENT_WITH_CACHE_ANALYSIS]
  - Tool: semantic_search for caching patterns
  - Status: [IMPLEMENTED/MISSING]
- [ ] **Bundle Size**: [ASSESSMENT_WITH_BUNDLE_ANALYSIS] (Frontend)
  - Tool: file analysis for import patterns
  - Status: [OPTIMIZED/BLOATED]

### Review Findings Summary (Tool-Generated)
```
Using comprehensive GitHub Copilot tool analysis:

#### Critical Issues (Must Fix)
1. **Issue**: [CRITICAL_SECURITY_ISSUE]
   - **File**: [FILE_PATH]
   - **Detection Tool**: [TOOL_USED]
   - **Risk Level**: HIGH
   - **Fix Required**: [IMMEDIATE_ACTION_NEEDED]

#### Major Issues (Should Fix)
1. **Issue**: [SRP_VIOLATION]
   - **File**: [FILE_PATH]
   - **Detection Tool**: [semantic_search]
   - **Impact**: [MAINTAINABILITY_IMPACT]
   - **Suggested Fix**: [REFACTORING_APPROACH]

#### Minor Issues (Nice to Fix)
1. **Issue**: [NAMING_INCONSISTENCY]
   - **File**: [FILE_PATH]
   - **Detection Tool**: [grep_search]
   - **Impact**: [READABILITY_IMPACT]
   - **Suggested Fix**: [RENAMING_APPROACH]

#### Positive Observations
- [GOOD_PATTERNS_FOUND_WITH_TOOLS]
- [PROPER_IMPLEMENTATIONS_IDENTIFIED]
```
```

#### For Action: `review-changes`
```markdown
**Phase 2.3: Change Review Analysis using GitHub Copilot Tools**
```
I'll review both staged and unstaged changes comprehensively:

## 🔄 Change Review Analysis (Tool-Enhanced)

### Change Detection with GitHub Copilot Tools
```powershell
# Comprehensive change analysis using GitHub Copilot tools
get_changed_files: ["staged", "unstaged"] # Get all changed files
semantic_search: "recent changes impact" # Understand change context
get_errors: [CHANGED_FILES] # Check for errors in changes
grep_search: "TODO|FIXME" (includePattern: [CHANGED_FILES]) # Find incomplete work
```

### Change Categories Analysis
#### Staged Changes Review
- [ ] **Files Changed**: [STAGED_FILE_COUNT] files
  - Tool: get_changed_files with "staged" filter
  - Analysis: [FILE_LIST_WITH_CHANGE_TYPES]
- [ ] **Change Impact**: [IMPACT_ASSESSMENT]
  - Tool: semantic_search for related components
  - Risk Level: [LOW/MEDIUM/HIGH]
- [ ] **Test Coverage**: [TEST_COVERAGE_STATUS]
  - Tool: test_search for related tests
  - Status: [COVERED/NEEDS_TESTS]

#### Unstaged Changes Review
- [ ] **Files Changed**: [UNSTAGED_FILE_COUNT] files
  - Tool: get_changed_files with "unstaged" filter
  - Analysis: [FILE_LIST_WITH_CHANGE_TYPES]
- [ ] **Incomplete Work**: [INCOMPLETE_WORK_ASSESSMENT]
  - Tool: grep_search for TODO/FIXME markers
  - Status: [READY/NEEDS_COMPLETION]
- [ ] **Compilation Status**: [COMPILATION_STATUS]
  - Tool: get_errors for error checking
  - Status: [COMPILES/HAS_ERRORS]

### Integration Impact Analysis (Tool-Enhanced)
```
I'll analyze the impact of changes on the overall system:
- semantic_search: "integration points" # Find system integration points
- file_search: [RELATED_PATTERN] # Find potentially affected files
- run_in_terminal: "dotnet build" # Verify compilation
- run_tests: [AFFECTED_TEST_FILES] # Run related tests
```

### Recommendations for Changes (Tool-Informed)
#### Staging Recommendations
1. **Recommendation**: [STAGING_ADVICE]
   - **Reasoning**: [TOOL_BASED_ANALYSIS]
   - **Files to Stage**: [FILE_LIST]
   - **Files to Leave Unstaged**: [FILE_LIST_WITH_REASONS]

#### Pre-Commit Checklist
- [ ] **All Errors Fixed**: [STATUS] (verified with get_errors)
- [ ] **Tests Passing**: [STATUS] (verified with run_tests)
- [ ] **SRP Compliance**: [STATUS] (verified with pattern analysis)
- [ ] **Security Validated**: [STATUS] (verified with security patterns)
- [ ] **Performance Impact**: [STATUS] (verified with performance analysis)
```
```

### Phase 3: Quality Validation and Recommendations

```markdown
**Phase 3.1: Comprehensive Quality Assessment using GitHub Copilot Tools**
```
I'll perform final quality validation using comprehensive tool integration:

## ✅ Quality Validation Results (Tool-Enhanced)

### Code Quality Metrics (Tool-Measured)
```powershell
# Comprehensive quality validation using GitHub Copilot tools
run_in_terminal: "dotnet build --configuration Release" # Backend compilation
run_in_terminal: "npm run type-check" # Frontend type checking
run_in_terminal: "npm run lint" # Frontend linting
get_errors: [ALL_ANALYZED_FILES] # Comprehensive error checking
run_tests: [RELEVANT_TEST_FILES] # Test execution
```

### Quality Scoring (Tool-Validated)
- [ ] **Compilation Status**: [PASS/FAIL] (verified with run_in_terminal)
- [ ] **Type Safety**: [SCORE/10] (verified with TypeScript compiler)
- [ ] **Code Style**: [SCORE/10] (verified with linting tools)
- [ ] **SRP Compliance**: [SCORE/10] (verified with semantic_search)
- [ ] **Test Coverage**: [PERCENTAGE]% (verified with run_tests)
- [ ] **Security Compliance**: [SCORE/10] (verified with security analysis)
- [ ] **Performance Impact**: [SCORE/10] (verified with performance analysis)

**Overall Quality Score: [SCORE]/10**
*(Minimum 8/10 required for production readiness)*

### Final Recommendations (Tool-Enhanced)
#### Immediate Actions Required
1. **Action**: [CRITICAL_FIX_NEEDED]
   - **Detected by**: [TOOL_NAME]
   - **Priority**: HIGH
   - **Implementation**: [SPECIFIC_STEPS]

#### Suggested Improvements
1. **Improvement**: [ENHANCEMENT_OPPORTUNITY]
   - **Detected by**: [TOOL_NAME]
   - **Priority**: MEDIUM
   - **Implementation**: [SPECIFIC_STEPS]

#### Validation Commands for User
```powershell
# User can run these commands to verify quality improvements
dotnet build --configuration Release
dotnet test --logger console
npm run type-check
npm run lint
npm run test
```
```

### Phase 4: Implementation Support (If Refactoring Selected)

```markdown
**Phase 4.1: Guided Implementation using GitHub Copilot Tools**
```
If the action is "refactor", I'll provide guided implementation support:

## 🛠️ Implementation Support (Tool-Assisted)

### Step-by-Step Implementation
1. **Pre-Implementation Validation**
   - Tool: get_errors to ensure clean starting state
   - Tool: run_tests to establish baseline
   - Status: [READY/NEEDS_FIXES]

2. **Refactoring Implementation**
   - Tool: insert_edit_into_file for new implementations
   - Tool: replace_string_in_file for modifications
   - Tool: create_file for new components (if needed)
   - Progress: [STEP_BY_STEP_TRACKING]

3. **Post-Implementation Validation**
   - Tool: get_errors to verify no compilation errors
   - Tool: run_tests to ensure no regressions
   - Tool: run_in_terminal for integration testing
   - Status: [VALIDATED/NEEDS_FIXES]

### Quality Assurance Checklist
- [ ] **SRP Compliance Maintained**: [STATUS] (verified with pattern analysis)
- [ ] **All Tests Passing**: [STATUS] (verified with run_tests)
- [ ] **No Compilation Errors**: [STATUS] (verified with get_errors)
- [ ] **Performance Not Degraded**: [STATUS] (verified with benchmarks)
- [ ] **Security Not Compromised**: [STATUS] (verified with security analysis)

### Success Criteria (Tool-Validated)
✅ **Code Quality Improved**: [METRICS_COMPARISON]
✅ **Architecture Compliance**: [PATTERN_VALIDATION]
✅ **Functionality Preserved**: [TEST_VALIDATION]
✅ **Performance Maintained**: [PERFORMANCE_VALIDATION]
```

## Command Activation Process
When a user types:
```
@copilot /code-quality [action] [scope]
```

The system should:
1. **Parse Parameters**: Extract action and scope parameters
2. **Gather Context**: Use GitHub Copilot tools for comprehensive analysis
3. **Execute Analysis**: Run action-specific analysis with tool integration
4. **Provide Recommendations**: Generate actionable improvement suggestions
5. **Support Implementation**: Provide guided implementation support (if refactoring)
6. **Validate Results**: Use tools to verify improvements and quality

## Notes
- The command adapts to Ikhtibar project-specific patterns and architectural guidelines
- All analysis uses GitHub Copilot's native tools for maximum accuracy
- SRP compliance is enforced throughout all recommendations
- Quality scoring follows project standards (minimum 8/10 for production)
- Security and performance considerations are integral to all assessments
- Internationalization (en/ar) and RTL support are considered in frontend analysis
- All recommendations include validation commands for user verification
