---
mode: agent
description: "Comprehensive Git operations management with commit automation, branch strategies, and conflict resolution"
---

---
inputs:
  - name: action
    description: Git action to perform (commit, branch, merge, status, history, cleanup)
    required: true
  - name: scope
    description: Scope of the action (current, staged, all, branch-name)
    required: false
  - name: message
    description: Commit message or branch description
    required: false
---

---
command: "/git"
---
# Git Operations Command for GitHub Copilot

## Command Usage
```
@copilot /git [action] [scope] [message]
```

## Purpose
This command provides intelligent Git operations for the Ikhtibar project, following conventional commit standards, branch naming conventions, and project-specific Git workflows. It integrates with GitHub Copilot tools for comprehensive repository analysis.

**Input Parameters**: 
- `action` - Git action: `commit`, `branch`, `merge`, `status`, `history`, or `cleanup`
- `scope` - Operation scope: `current`, `staged`, `all`, or specific branch name
- `message` - Commit message or descriptive text for the operation

## How /git Works

### Phase 1: Repository State Analysis
```markdown
I'll help you with Git operations for the Ikhtibar project. Let me analyze the current repository state and understand your request.

**Phase 1.1: Parse Git Operation Request**
```
Git Operation Analysis:
- **Action**: [COMMIT/BRANCH/MERGE/STATUS/HISTORY/CLEANUP]
- **Scope**: [CURRENT/STAGED/ALL/BRANCH_NAME]
- **Message**: [PROVIDED_MESSAGE_OR_AUTO_GENERATED]
- **Project Context**: ASP.NET Core + React.js with TypeScript
- **Workflow**: Feature branches with conventional commits
```

**Phase 1.2: Repository Analysis using GitHub Copilot Tools**
```
I'll analyze the repository state using GitHub Copilot's native tools:
- get_changed_files: ["staged", "unstaged"] # Get all changed files
- get_terminal_selection: # Check current terminal context
- run_in_terminal: "git status --porcelain" # Get detailed git status
- run_in_terminal: "git branch --show-current" # Get current branch
- run_in_terminal: "git log --oneline -5" # Get recent commits
- semantic_search: "recent changes context" # Understand change purpose
```

**Phase 1.3: Project State Assessment**
```
Repository Context Analysis:
- [ ] **Current Branch**: [BRANCH_NAME] (tool-discovered)
- [ ] **Staged Changes**: [COUNT] files (tool-counted)
- [ ] **Unstaged Changes**: [COUNT] files (tool-counted)
- [ ] **Untracked Files**: [COUNT] files (tool-counted)
- [ ] **Behind/Ahead**: [STATUS] (tool-analyzed)
- [ ] **Merge Conflicts**: [STATUS] (tool-detected)
- [ ] **Build Status**: [STATUS] (tool-verified)
- [ ] **Test Status**: [STATUS] (tool-verified)
```
```

### Phase 2: Action-Specific Git Operations

#### For Action: `commit`
```markdown
**Phase 2.1: Intelligent Commit Processing using GitHub Copilot Tools**
```
I'll create an intelligent commit with conventional commit standards:

## 📝 Smart Commit Generation (Tool-Enhanced)

### Change Analysis with GitHub Copilot Tools
```powershell
# Comprehensive change analysis for commit generation
get_changed_files: ["staged", "unstaged"] # Analyze all changes
semantic_search: "commit message patterns" # Find project commit conventions
grep_search: "TODO|FIXME|WIP" # Check for incomplete work
get_errors: [CHANGED_FILES] # Verify no compilation errors
run_tests: # Ensure tests pass before commit
```

### Conventional Commit Generation (Tool-Informed)
```
Based on change analysis, I'll generate a conventional commit:

Change Type Analysis:
- **Type**: [feat/fix/docs/style/refactor/test/chore] (determined from file analysis)
- **Scope**: [component/module/service] (determined from affected areas)
- **Breaking Change**: [yes/no] (determined from API changes)
- **Issue References**: [#123] (determined from branch/commit patterns)
```

#### Generated Commit Message
```
[type]([scope]): [description]

[optional body with details from change analysis]

[optional footer with breaking changes/issue references]
```

### Pre-Commit Validation (Tool-Executed)
```powershell
# Comprehensive validation before commit
run_in_terminal: "dotnet build --configuration Release" # Backend validation
run_in_terminal: "npm run type-check" # Frontend type validation
run_in_terminal: "npm run lint" # Code style validation
run_in_terminal: "npm run test" # Unit test validation
get_errors: [ALL_STAGED_FILES] # Final error check
```

### Validation Results
- [ ] **Build Success**: [✅/❌] (verified with compilation)
- [ ] **Type Safety**: [✅/❌] (verified with TypeScript)
- [ ] **Code Style**: [✅/❌] (verified with linting)
- [ ] **Tests Passing**: [✅/❌] (verified with test execution)
- [ ] **No Errors**: [✅/❌] (verified with error checking)

### Commit Execution (Tool-Managed)
```powershell
# If all validations pass, execute the commit
run_in_terminal: "git add [STAGED_FILES]" # Stage validated files
run_in_terminal: "git commit -m \"[GENERATED_MESSAGE]\"" # Execute commit
run_in_terminal: "git log -1 --oneline" # Confirm commit
```
```

#### For Action: `branch`
```markdown
**Phase 2.2: Intelligent Branch Management using GitHub Copilot Tools**
```
I'll manage branches following project conventions:

## 🌿 Smart Branch Operations (Tool-Enhanced)

### Branch Context Analysis
```powershell
# Comprehensive branch analysis using GitHub Copilot tools
run_in_terminal: "git branch -a" # List all branches
run_in_terminal: "git status --porcelain" # Check working directory
semantic_search: "branch naming conventions" # Find project patterns
grep_search: "feature|bugfix|hotfix" # Identify branch types
```

### Branch Naming Convention (Project-Specific)
```
Ikhtibar Project Branch Naming:
- **Feature**: feature/[module]/[description] (e.g., feature/user-management/role-assignment)
- **Bugfix**: bugfix/[issue-number]/[description] (e.g., bugfix/123/login-validation)
- **Hotfix**: hotfix/[version]/[description] (e.g., hotfix/1.2.1/security-patch)
- **Release**: release/[version] (e.g., release/1.3.0)
- **Documentation**: docs/[section]/[description] (e.g., docs/api/authentication)
```

### Branch Operations (Tool-Executed)

#### Creating New Branch
```powershell
# Intelligent branch creation
run_in_terminal: "git checkout -b [GENERATED_BRANCH_NAME]" # Create and switch
run_in_terminal: "git push -u origin [GENERATED_BRANCH_NAME]" # Set upstream
```

#### Branch Analysis
```
Branch Analysis Results:
- **Suggested Name**: [INTELLIGENT_BRANCH_NAME] (based on scope/message analysis)
- **Base Branch**: [main/develop] (determined from project workflow)
- **Purpose**: [FEATURE/BUGFIX/HOTFIX/DOCS] (determined from context)
- **Related Issues**: [#123, #456] (determined from message analysis)
```

### Working Directory Validation
- [ ] **Clean Working Directory**: [✅/❌] (verified with git status)
- [ ] **No Uncommitted Changes**: [✅/❌] (verified with change detection)
- [ ] **Up to Date**: [✅/❌] (verified with remote comparison)
```

#### For Action: `status`
```markdown
**Phase 2.3: Comprehensive Repository Status using GitHub Copilot Tools**
```
I'll provide detailed repository status analysis:

## 📊 Repository Status Analysis (Tool-Enhanced)

### Comprehensive Status Check
```powershell
# Multi-dimensional status analysis using GitHub Copilot tools
get_changed_files: ["staged", "unstaged", "merge-conflicts"] # Get all changes
run_in_terminal: "git status --porcelain -b" # Detailed git status
run_in_terminal: "git log --oneline -5" # Recent commit history
run_in_terminal: "git remote -v" # Remote configuration
run_in_terminal: "git branch -vv" # Branch tracking information
```

### Working Directory Status (Tool-Analyzed)
```
Current Repository State:
┌─────────────────────────────────────────────────────────────┐
│ Branch: [CURRENT_BRANCH]                                    │
│ Upstream: [TRACKING_BRANCH] ([AHEAD/BEHIND_STATUS])        │
│ Last Commit: [LAST_COMMIT_HASH] [COMMIT_MESSAGE]          │
├─────────────────────────────────────────────────────────────┤
│ Changes Summary:                                            │
│ • Staged: [COUNT] files                                     │
│ • Modified: [COUNT] files                                   │
│ • Untracked: [COUNT] files                                  │
│ • Deleted: [COUNT] files                                    │
│ • Conflicted: [COUNT] files                                 │
└─────────────────────────────────────────────────────────────┘
```

### File-by-File Analysis (Tool-Detailed)
#### Staged Changes
| Status | File | Change Type | Module |
|--------|------|-------------|---------|
| [A/M/D] | [FILE_PATH] | [CHANGE_DESCRIPTION] | [MODULE] |

#### Modified Files
| Status | File | Change Type | Module |
|--------|------|-------------|---------|
| [M/??] | [FILE_PATH] | [CHANGE_DESCRIPTION] | [MODULE] |

### Project Health Check (Tool-Validated)
```powershell
# Comprehensive project health validation
get_errors: [ALL_FILES] # Check for compilation errors
run_in_terminal: "dotnet build --configuration Release --verbosity quiet" # Backend health
run_in_terminal: "npm run type-check" # Frontend type health
run_in_terminal: "npm run lint --max-warnings 0" # Code quality health
```

#### Health Status
- [ ] **Backend Compiles**: [✅/❌] (verified with dotnet build)
- [ ] **Frontend Types**: [✅/❌] (verified with type-check)
- [ ] **Code Quality**: [✅/❌] (verified with linting)
- [ ] **No Conflicts**: [✅/❌] (verified with merge analysis)
- [ ] **Tests Passing**: [✅/❌] (verified with test execution)

### Next Action Recommendations (Tool-Informed)
```
Based on analysis, recommended next actions:
1. **Priority**: [HIGH/MEDIUM/LOW] - [RECOMMENDED_ACTION]
   - **Reason**: [ANALYSIS_BASED_REASONING]
   - **Command**: [SUGGESTED_GIT_COMMAND]

2. **Priority**: [HIGH/MEDIUM/LOW] - [RECOMMENDED_ACTION]
   - **Reason**: [ANALYSIS_BASED_REASONING]
   - **Command**: [SUGGESTED_GIT_COMMAND]
```
```

#### For Action: `history`
```markdown
**Phase 2.4: Repository History Analysis using GitHub Copilot Tools**
```
I'll provide comprehensive commit history analysis:

## 📚 Repository History Analysis (Tool-Enhanced)

### Commit History Retrieval
```powershell
# Comprehensive history analysis using GitHub Copilot tools
run_in_terminal: "git log --oneline --graph -20" # Visual commit history
run_in_terminal: "git log --stat -5" # Recent commits with file changes
run_in_terminal: "git shortlog -sn --since='1 month ago'" # Contributor analysis
semantic_search: "commit patterns recent" # Analyze commit quality
```

### Recent Commits Analysis (Tool-Generated)
```
Recent Commit History:
┌─────────────────────────────────────────────────────────────┐
│ Commit Graph (Last 20 commits):                            │
├─────────────────────────────────────────────────────────────┤
│ [VISUAL_COMMIT_GRAPH_FROM_GIT_LOG]                         │
└─────────────────────────────────────────────────────────────┘
```

### Commit Quality Analysis (Tool-Assessed)
| Commit | Type | Scope | Quality Score | Files Changed |
|--------|------|-------|---------------|---------------|
| [HASH] | [TYPE] | [SCOPE] | [SCORE]/10 | [COUNT] |

### Development Patterns (Tool-Identified)
```
Analysis Results:
- **Commit Frequency**: [DAILY_AVERAGE] commits/day
- **Conventional Commits**: [PERCENTAGE]% follow standards
- **Average Files/Commit**: [AVERAGE_COUNT] files
- **Most Active Module**: [MODULE_NAME] ([COMMIT_COUNT] commits)
- **Code Quality Trend**: [IMPROVING/STABLE/DECLINING]
```

### Contributor Analysis (Tool-Generated)
| Developer | Commits | Lines Added | Lines Removed | Primary Areas |
|-----------|---------|-------------|---------------|---------------|
| [NAME] | [COUNT] | [ADDED] | [REMOVED] | [MODULES] |

### Branch Timeline (Tool-Visualized)
```
Branch Development Timeline:
[BRANCH_TIMELINE_FROM_ANALYSIS]
```
```

### Phase 3: Validation and Execution

```markdown
**Phase 3.1: Pre-Operation Validation using GitHub Copilot Tools**
```
I'll validate the operation before execution:

## ✅ Operation Validation (Tool-Enhanced)

### Pre-Operation Checks (Tool-Executed)
```powershell
# Comprehensive validation using GitHub Copilot tools
get_errors: [RELEVANT_FILES] # Check for compilation errors
run_in_terminal: "git fsck --full" # Repository integrity check
run_in_terminal: "git status --porcelain" # Working directory validation
run_tests: [AFFECTED_TESTS] # Test validation if applicable
```

### Validation Results
- [ ] **Repository Integrity**: [✅/❌] (verified with git fsck)
- [ ] **Working Directory Clean**: [✅/❌] (verified with git status)
- [ ] **No Compilation Errors**: [✅/❌] (verified with build)
- [ ] **Tests Passing**: [✅/❌] (verified with test execution)
- [ ] **Conventional Standards**: [✅/❌] (verified with message analysis)

### Safety Checks (Critical Validations)
```
Critical Safety Validations:
- **Main Branch Protection**: [PROTECTED/UNPROTECTED]
- **Force Push Protection**: [ENABLED/DISABLED]
- **Merge Conflicts**: [NONE/PRESENT]
- **Backup Status**: [CURRENT/OUTDATED]
```

### Operation Execution Plan
```
Execution Plan:
1. **Pre-checks**: [COMPLETED/PENDING]
2. **Operation**: [READY/BLOCKED]
3. **Post-validation**: [PLANNED]
4. **Rollback Plan**: [AVAILABLE]
```
```

### Phase 4: Post-Operation Verification

```markdown
**Phase 4.1: Post-Operation Verification using GitHub Copilot Tools**
```
After executing the Git operation, I'll verify success:

## 🔍 Post-Operation Verification (Tool-Enhanced)

### Operation Success Validation
```powershell
# Comprehensive post-operation validation
run_in_terminal: "git status" # Verify operation completion
run_in_terminal: "git log -1 --oneline" # Verify latest commit (if applicable)
run_in_terminal: "git branch --show-current" # Verify branch state
get_errors: [AFFECTED_FILES] # Verify no new errors introduced
```

### Success Criteria Verification
- [ ] **Operation Completed**: [✅/❌] (verified with git status)
- [ ] **No Errors Introduced**: [✅/❌] (verified with error checking)
- [ ] **Repository Consistent**: [✅/❌] (verified with integrity check)
- [ ] **Workflow Continued**: [✅/❌] (verified with next action readiness)

### Next Steps Recommendation (Tool-Informed)
```
Recommended Next Actions:
1. **Immediate**: [NEXT_ACTION_RECOMMENDATION]
   - **Command**: [SUGGESTED_COMMAND]
   - **Reason**: [WORKFLOW_BASED_REASONING]

2. **Follow-up**: [FOLLOW_UP_ACTION]
   - **Timeline**: [SUGGESTED_TIMELINE]
   - **Dependencies**: [ACTION_DEPENDENCIES]
```
```

## Command Activation Process
When a user types:
```
@copilot /git [action] [scope] [message]
```

The system should:
1. **Analyze Repository**: Use GitHub Copilot tools to understand current state
2. **Validate Operation**: Ensure the requested operation is safe and appropriate
3. **Execute with Intelligence**: Perform the operation with project-specific conventions
4. **Verify Success**: Confirm the operation completed successfully
5. **Recommend Next Steps**: Suggest logical follow-up actions

## Notes
- All Git operations follow Ikhtibar project conventions and workflows
- Conventional commit standards are enforced for commit operations
- Branch naming follows project-specific patterns
- Safety validations prevent destructive operations
- All operations are verified using GitHub Copilot tools
- Repository health is monitored and reported
- Intelligent recommendations are provided based on project context
- Integration with project build/test systems ensures code quality
