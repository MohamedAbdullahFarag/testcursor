---
mode: agent
description: "Intelligent conflict resolution with automated merge strategies and validation"
---

---
inputs:
  - name: conflict_type
    description: Type of conflicts to resolve (merge, rebase, cherry-pick, manual)
    required: false
    default: smart-detect
  - name: resolution_strategy
    description: Resolution strategy (auto, interactive, manual, conservative)
    required: false
    default: interactive
---

---
command: "/smart-resolve"
---
# Smart Git Conflict Resolution Command for GitHub Copilot

## Command Usage
```
@copilot /smart-resolve [conflict_type] [resolution_strategy]
```

## Purpose
This command provides intelligent Git conflict resolution for the Ikhtibar examination management system, analyzing code context, understanding project patterns, and providing smart merge suggestions with comprehensive validation.

**Input Parameters**: 
- `conflict_type` - Type of conflicts: `merge`, `rebase`, `cherry-pick`, `manual`, or `smart-detect`
- `resolution_strategy` - Resolution approach: `auto`, `interactive`, `manual`, or `conservative`

## How /smart-resolve Works

### Phase 1: Conflict Detection and Analysis
```markdown
I'll help you intelligently resolve Git conflicts in the Ikhtibar project. Let me analyze the conflicts and understand the context.

**Phase 1.1: Parse Conflict Request**
```
Smart Git Conflict Resolution:
- **Conflict Type**: [MERGE/REBASE/CHERRY-PICK/MANUAL/SMART-DETECT]
- **Resolution Strategy**: [AUTO/INTERACTIVE/MANUAL/CONSERVATIVE]
- **Project Context**: React 18 + .NET Core with Git workflow
- **Branch Strategy**: [DETECTED_BRANCH_STRATEGY]
```

**Phase 1.2: Conflict Discovery using GitHub Copilot Tools**
```
I'll detect and analyze current Git conflicts:
- get_changed_files: ["merge-conflicts"] # Get conflicted files
- run_in_terminal: "git status --porcelain" # Check Git status
- run_in_terminal: "git diff --name-only --diff-filter=U" # Find unmerged files
- semantic_search: "merge conflict patterns" # Find previous conflict resolutions
- file_search: "**/*.orig" # Find backup files
- grep_search: "<<<<<<< HEAD|=======|>>>>>>>" # Find conflict markers
```

**Phase 1.3: Conflict Context Analysis**
```
Git Conflict Analysis:
- [ ] **Conflicted Files**: [COUNT] files with conflicts
- [ ] **Conflict Types**: [TYPES_DETECTED]
- [ ] **Branch Information**: [SOURCE_BRANCH] → [TARGET_BRANCH]
- [ ] **Conflict Complexity**: [SIMPLE/MODERATE/COMPLEX]
- [ ] **File Categories**: [FRONTEND/BACKEND/CONFIG/DOCS]
- [ ] **Impact Assessment**: [LOW/MEDIUM/HIGH]
- [ ] **Auto-Resolution Feasibility**: [YES/NO/PARTIAL]
- [ ] **Human Review Required**: [YES/NO]
```
```

### Phase 2: Intelligent Conflict Analysis

#### For Conflict Type: `smart-detect`
```markdown
**Phase 2.1: Automatic Conflict Detection using GitHub Copilot Tools**
```
I'll automatically detect and categorize conflicts:

## 🔍 Smart Conflict Detection (Tool-Enhanced)

### Conflict Categorization
```powershell
# Comprehensive conflict analysis using GitHub Copilot tools
run_in_terminal: "git merge-tree $(git merge-base HEAD target-branch) HEAD target-branch" # Preview conflicts
grep_search: "<<<<<<< HEAD" # Find conflict start markers
grep_search: "=======" # Find conflict separators  
grep_search: ">>>>>>>" # Find conflict end markers
semantic_search: "conflict resolution patterns" # Find similar past resolutions
read_file: [CONFLICTED_FILES] # Read conflicted file contents
```

### Conflict Classification (Tool-Detected)
| File Type | Conflict Category | Complexity | Auto-Resolvable | Resolution Method |
|-----------|------------------|------------|-----------------|-------------------|
| `.tsx` files | Component merge | [LEVEL] | [YES/NO] | [METHOD] |
| `.cs` files | API logic merge | [LEVEL] | [YES/NO] | [METHOD] |
| `package.json` | Dependency conflict | [LEVEL] | [YES/NO] | [METHOD] |
| `.config` files | Configuration merge | [LEVEL] | [YES/NO] | [METHOD] |

### Intelligent Conflict Analysis
```typescript
// Conflict analysis results from tool discovery
interface ConflictAnalysis {
  file: string;
  conflictType: 'content' | 'whitespace' | 'structure' | 'logic';
  complexity: 'simple' | 'moderate' | 'complex';
  autoResolvable: boolean;
  riskLevel: 'low' | 'medium' | 'high';
  recommendedAction: string;
  contextualFactors: string[];
}

// Example conflict detected via analysis
const conflictExample: ConflictAnalysis = {
  file: 'src/modules/exam/components/ExamForm.tsx',
  conflictType: 'content',
  complexity: 'moderate',
  autoResolvable: false,
  riskLevel: 'medium',
  recommendedAction: 'manual_review_with_guidance',
  contextualFactors: [
    'TypeScript interface changes',
    'Component prop modifications',
    'Event handler updates'
  ]
};
```

### Conflict Resolution Recommendations (Tool-Generated)
#### High Confidence Auto-Resolution
```bash
# Files that can be automatically resolved (Tool-verified)
git checkout --ours package-lock.json    # Keep current lock file
git checkout --theirs .gitignore         # Accept incoming ignore rules
git add package-lock.json .gitignore
```

#### Guided Manual Resolution
```typescript
// src/modules/exam/components/ExamForm.tsx
// CONFLICT DETECTED: Component prop interface changes

// <<<<<<< HEAD (Current)
interface ExamFormProps {
  exam: Exam;
  onSubmit: (data: ExamData) => void;
  onCancel: () => void;
}
// =======
interface ExamFormProps {
  exam: Exam;
  onSubmit: (data: ExamFormData) => Promise<void>;
  onCancel: () => void;
  isLoading?: boolean;
}
// >>>>>>> feature/enhanced-forms

// SMART RESOLUTION RECOMMENDATION:
// Accept the enhanced version with async onSubmit and loading state
// This aligns with the project's move toward better UX patterns

interface ExamFormProps {
  exam: Exam;
  onSubmit: (data: ExamFormData) => Promise<void>; // Enhanced: async handling
  onCancel: () => void;
  isLoading?: boolean; // Enhanced: loading state support
}
```
```

#### For Resolution Strategy: `interactive`
```markdown
**Phase 2.2: Interactive Conflict Resolution using GitHub Copilot Tools**
```
I'll guide you through interactive conflict resolution:

## ⚡ Interactive Resolution Process (Tool-Guided)

### Step-by-Step Conflict Resolution
```powershell
# Interactive resolution using GitHub Copilot tools
run_in_terminal: "git mergetool --tool=vscode" # Launch merge tool
semantic_search: "best merge practices" # Get resolution guidelines
grep_search: "TODO|FIXME|BUG" # Check for existing issues
get_errors: [CONFLICTED_FILES] # Check for syntax errors
```

### Conflict Resolution Workflow
#### 1. Pre-Resolution Analysis
```
Conflict Resolution Plan:
┌─────────────────────────────────────────────────────────────┐
│ File: src/modules/auth/AuthController.cs                   │
│ Conflict: Method signature changes                         │
│ Complexity: Moderate                                       │
│ Risk: Medium                                               │
├─────────────────────────────────────────────────────────────┤
│ Resolution Options:                                         │
│ 1. Accept Current (HEAD): Keep existing async pattern      │
│ 2. Accept Incoming: Use new error handling approach        │
│ 3. Manual Merge: Combine both improvements                │
│ 4. Custom Solution: Create enhanced version               │
├─────────────────────────────────────────────────────────────┤
│ Recommendation: Option 3 - Manual merge for best outcome  │
└─────────────────────────────────────────────────────────────┘
```

#### 2. Guided Resolution Process
```csharp
// backend/Ikhtibar.API/Controllers/AuthController.cs
// INTERACTIVE RESOLUTION GUIDANCE

// Option Analysis:
// Current (HEAD): Uses async/await pattern
// Incoming: Enhanced error handling with custom exceptions

// RECOMMENDED MERGE:
[HttpPost("login")]
public async Task<ActionResult<AuthResultDto>> Login(LoginDto loginDto)
{
    try 
    {
        // Keep async pattern from current (HEAD)
        var result = await _authService.LoginAsync(loginDto);
        
        // Integrate enhanced error handling from incoming
        if (!result.IsSuccess)
        {
            return BadRequest(new { 
                message = result.ErrorMessage,
                code = result.ErrorCode // Enhanced from incoming branch
            });
        }
        
        return Ok(result.Data);
    }
    catch (AuthenticationException ex) // Enhanced exception from incoming
    {
        _logger.LogWarning(ex, "Authentication failed for user {Email}", loginDto.Email);
        return Unauthorized(new { message = "Invalid credentials" });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Login error for user {Email}", loginDto.Email);
        return StatusCode(500, new { message = "Internal server error" });
    }
}
```

#### 3. Validation and Testing
```powershell
# Post-resolution validation using GitHub Copilot tools
run_in_terminal: "npm run build" # Build validation for frontend
run_in_terminal: "dotnet build" # Build validation for backend
run_in_terminal: "npm run test" # Run tests
run_in_terminal: "npm run lint" # Check code quality
get_errors: [RESOLVED_FILES] # Check for new errors
```
```

#### For Resolution Strategy: `conservative`
```markdown
**Phase 2.3: Conservative Resolution Strategy using GitHub Copilot Tools**
```
I'll apply conservative resolution to minimize risk:

## 🛡️ Conservative Resolution (Risk-Minimized)

### Conservative Resolution Principles
1. **Preserve Existing Functionality**: Keep working code intact
2. **Minimal Changes**: Make smallest possible modifications
3. **Extensive Testing**: Validate all changes thoroughly
4. **Rollback Capability**: Ensure easy reversion if needed

### Conservative Resolution Process
```powershell
# Conservative resolution using GitHub Copilot tools
run_in_terminal: "git stash" # Backup current changes
run_in_terminal: "git checkout --ours [CRITICAL_FILES]" # Keep current for critical files
run_in_terminal: "git checkout --theirs [SAFE_FILES]" # Accept incoming for safe files
semantic_search: "breaking changes detection" # Find potential breaking changes
run_tests: [AFFECTED_MODULES] # Test impacted functionality
```

### Risk Assessment Matrix (Tool-Generated)
| File | Change Type | Risk Level | Action | Justification |
|------|-------------|------------|--------|---------------|
| `AuthController.cs` | Method signature | HIGH | Keep current | Preserve API stability |
| `ExamForm.tsx` | Props interface | MEDIUM | Manual merge | Combine improvements safely |
| `package.json` | Dependencies | LOW | Accept incoming | Standard dependency updates |
| `tsconfig.json` | Compiler options | MEDIUM | Keep current | Preserve build configuration |

### Conservative Resolution Example
```typescript
// Conservative approach for React component conflicts
// Priority: Maintain existing functionality, gradual enhancement

// Instead of major interface changes:
interface ExamFormProps {
  exam: Exam;
  onSubmit: (data: ExamData) => void; // Keep existing signature
  onCancel: () => void;
  // Add new props as optional to avoid breaking changes
  isLoading?: boolean; // Safe addition
  validationErrors?: Record<string, string>; // Safe addition
}

// Gradual enhancement approach:
const ExamForm: React.FC<ExamFormProps> = ({ 
  exam, 
  onSubmit, 
  onCancel,
  isLoading = false, // Default values for safety
  validationErrors = {}
}) => {
  // Implementation preserves existing behavior
  // New features are additive, not disruptive
};
```
```

### Phase 3: Resolution Execution and Validation

```markdown
**Phase 3.1: Execute Resolution Plan using GitHub Copilot Tools**
```
I'll execute the resolution plan with comprehensive validation:

## 🔧 Resolution Execution (Tool-Validated)

### Resolution Execution Steps
```powershell
# Execute resolution plan using GitHub Copilot tools
run_in_terminal: "git add [RESOLVED_FILES]" # Stage resolved files
run_in_terminal: "git commit -m 'resolve: merge conflicts with smart resolution'" # Commit resolution
run_in_terminal: "npm run build" # Validate frontend build
run_in_terminal: "dotnet build" # Validate backend build  
run_in_terminal: "npm run test -- --coverage" # Run comprehensive tests
get_errors: [ALL_PROJECT_FILES] # Check for new errors
```

### Post-Resolution Validation
```
Resolution Validation Checklist:
┌─────────────────────────────────────────────────────────────┐
│ ✅ Build Status:                                           │
│   • Frontend Build: [SUCCESS/FAILED]                      │
│   • Backend Build: [SUCCESS/FAILED]                       │
│   • TypeScript Check: [SUCCESS/FAILED]                    │
│   • Linting: [SUCCESS/FAILED]                            │
├─────────────────────────────────────────────────────────────┤
│ ✅ Test Results:                                           │
│   • Unit Tests: [PASSED/FAILED] ([COUNT] tests)          │
│   • Integration Tests: [PASSED/FAILED] ([COUNT] tests)   │
│   • E2E Tests: [PASSED/FAILED] ([COUNT] tests)           │
│   • Coverage: [PERCENTAGE]%                               │
├─────────────────────────────────────────────────────────────┤
│ ✅ Quality Checks:                                         │
│   • Code Quality: [SCORE]/10                              │
│   • Type Safety: [SCORE]/10                               │
│   • Performance: [SCORE]/10                               │
│   • Security: [SCORE]/10                                  │
└─────────────────────────────────────────────────────────────┘
```

### Resolution Summary Report
```typescript
interface ResolutionReport {
  totalConflicts: number; // Tool-counted
  resolvedConflicts: number; // Tool-verified
  autoResolved: number; // Tool-automated
  manualResolved: number; // Human-guided
  filesModified: string[]; // Tool-tracked
  testsAffected: string[]; // Tool-identified
  buildStatus: 'success' | 'failed'; // Tool-validated
  riskAssessment: 'low' | 'medium' | 'high'; // Tool-calculated
  recommendedFollowUp: string[]; // Tool-generated
}

// Example resolution report
const resolutionSummary: ResolutionReport = {
  totalConflicts: 8,
  resolvedConflicts: 8,
  autoResolved: 3,
  manualResolved: 5,
  filesModified: [
    'src/modules/auth/AuthController.cs',
    'src/modules/exam/components/ExamForm.tsx',
    'package.json',
    'tsconfig.json'
  ],
  testsAffected: ['auth.test.ts', 'exam-form.test.tsx'],
  buildStatus: 'success',
  riskAssessment: 'low',
  recommendedFollowUp: [
    'Monitor authentication flow in staging',
    'Update documentation for new ExamForm props',
    'Schedule code review for manual resolutions'
  ]
};
```

### Prevention Recommendations (Tool-Generated)
```powershell
# Future conflict prevention
git config merge.tool vscode # Set up merge tool
git config pull.rebase true # Use rebase for cleaner history
git config rerere.enabled true # Remember conflict resolutions

# Establish merge strategies for file types
echo "*.json merge=union" >> .gitattributes
echo "package-lock.json merge=ours" >> .gitattributes
echo "*.cs merge=diff3" >> .gitattributes
```

### Follow-up Actions
1. **Immediate**: Monitor application in development environment
2. **Short-term**: Schedule team review of resolution decisions
3. **Long-term**: Implement conflict prevention strategies
4. **Documentation**: Update merge guidelines based on lessons learned
```

## Command Activation Process
When a user types:
```
@copilot /smart-resolve [conflict_type] [resolution_strategy]
```

The system should:
1. **Detect Conflicts**: Use GitHub Copilot tools to find and analyze conflicts
2. **Analyze Context**: Understand codebase patterns and project conventions  
3. **Generate Resolution Plan**: Create intelligent resolution strategy
4. **Execute Resolution**: Apply resolutions with continuous validation
5. **Validate Results**: Comprehensive testing and quality checks
6. **Provide Report**: Generate detailed resolution summary

## Notes
- All conflict resolution uses GitHub Copilot's native tools for maximum accuracy
- Resolution strategies adapt to Ikhtibar project-specific patterns and conventions
- Both frontend (React/TypeScript) and backend (.NET Core) conflicts are handled
- Comprehensive validation ensures resolution quality and system stability
- Conservative approach prioritizes system stability over feature completeness
- All resolutions include rollback capabilities and extensive testing
- Internationalization considerations are preserved during conflict resolution
- Prevention strategies are recommended to minimize future conflicts
