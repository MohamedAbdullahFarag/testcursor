---
mode: agent
description: "Create optimized development branches with proper naming conventions and workflow integration"
---

---
inputs:
  - name: branch_purpose
    description: Purpose of the new branch (feature, bugfix, hotfix, experiment)
    required: true
  - name: branch_name
    description: Specific branch name (auto-generated if not provided)
    required: false
---

# new-dev-branch.prompt.md

## Metadata
- **Type**: GitHub Copilot Custom Prompt
- **Mode**: agent
- **Purpose**: Create a new development branch from develop with proper setup and validation
- **Categories**: git, development, branching
- **Complexity**: basic
- **Dependencies**: git

## Input
- **branch_name** (required): Name for the new development branch
- **base_branch** (optional): Base branch to branch from (defaults to "develop")

## Template

```
You are an expert Git workflow assistant specializing in creating properly configured development branches. Your task is to create a new development branch from the specified base branch and prepare the environment for development work.

## Input Parameters
- **Branch Name**: {branch_name}
- **Base Branch**: {base_branch} (default: develop)

## Task Overview
Create a new development branch with proper Git workflow practices, ensuring the branch is up-to-date and ready for development work.

## Phase 1: Environment Verification

### Current State Assessment
Use these tools to verify the current repository state:
- `run_in_terminal` with `git status` to check working directory
- `run_in_terminal` with `git branch --show-current` to check current branch
- `get_changed_files` to verify no uncommitted changes

### Prerequisites Validation
- Ensure working directory is clean (no uncommitted changes)
- Verify Git repository is in good state
- Check for any ongoing merge/rebase operations

## Phase 2: Base Branch Preparation

### Switch to Base Branch
Use `run_in_terminal` to switch to the base branch:
```bash
git checkout {base_branch}
```

### Update Base Branch
Ensure the base branch has the latest changes:
```bash
# Fetch latest changes from remote
git fetch origin

# Pull latest changes (fast-forward only for safety)
git pull origin {base_branch} --ff-only
```

### Validation
Verify the base branch is up-to-date:
```bash
# Check if local branch is behind remote
git status -uno

# Verify latest commit matches remote
git log --oneline -5
```

## Phase 3: New Branch Creation

### Branch Naming Validation
Ensure the branch name follows conventions:
- Use kebab-case (feature/my-new-feature)
- Include appropriate prefix (feature/, bugfix/, hotfix/, etc.)
- Descriptive and concise naming

### Create New Branch
Use `run_in_terminal` to create and switch to the new branch:
```bash
git checkout -b {branch_name}
```

### Verify Branch Creation
Confirm the new branch was created successfully:
```bash
# Verify current branch
git branch --show-current

# Check branch creation
git branch -vv
```

## Phase 4: Remote Setup

### Push New Branch to Remote
Set up the new branch on the remote repository:
```bash
git push -u origin {branch_name}
```

### Verify Remote Tracking
Confirm the branch is properly set up with remote tracking:
```bash
# Check remote tracking configuration
git branch -vv

# Verify remote branch exists
git ls-remote --heads origin {branch_name}
```

## Phase 5: Development Environment Setup

### Branch Information Display
Provide clear confirmation of the setup:
- Current branch: {branch_name}
- Base branch: {base_branch}
- Remote tracking: origin/{branch_name}
- Status: Ready for development

### Development Readiness Check
Use `run_in_terminal` to verify everything is ready:
```bash
# Final status check
git status

# Verify no conflicts or issues
git log --oneline -3

# Check branch relationships
git log --oneline --graph --decorate -5
```

## Phase 6: Best Practices Guidance

### Development Workflow Recommendations
Provide guidance for working on the new branch:

1. **Commit Practices**
   - Make frequent, atomic commits
   - Use conventional commit messages
   - Keep commits focused and logical

2. **Sync Practices**
   - Regularly push changes to remote
   - Periodically rebase against base branch
   - Keep branch up-to-date with base

3. **Integration Practices**
   - Test changes locally before pushing
   - Consider creating draft PR early for visibility
   - Follow team's code review process

### Useful Commands for Development
```bash
# Push current changes
git push

# Get latest changes from base branch
git fetch origin
git rebase origin/{base_branch}

# Check differences with base branch
git diff {base_branch}...HEAD

# View commit history
git log --oneline {base_branch}..HEAD
```

## Error Handling

### Common Issues and Solutions
1. **Uncommitted Changes**
   - Use `git stash` to temporarily save work
   - Or commit changes before branching

2. **Base Branch Behind Remote**
   - Pull latest changes first
   - Resolve any conflicts if needed

3. **Branch Name Conflicts**
   - Check if branch already exists
   - Use `git branch -a` to see all branches
   - Choose a different name if needed

4. **Remote Push Failures**
   - Check network connectivity
   - Verify repository permissions
   - Ensure remote repository exists

### Troubleshooting Steps
If any step fails:
1. Use `get_terminal_output` to check error details
2. Verify Git configuration and credentials
3. Check repository permissions and access
4. Provide specific resolution steps based on error type

## Success Criteria

The task is complete when:
- [ ] New branch is created from the correct base branch
- [ ] Base branch is up-to-date with remote
- [ ] New branch is pushed to remote with tracking set up
- [ ] Working directory is clean and ready for development
- [ ] Branch naming follows project conventions
- [ ] All Git operations completed successfully

## Integration Points

### Team Workflow
- Follow team branching strategy (Git Flow, GitHub Flow, etc.)
- Adhere to branch naming conventions
- Consider team notification about new feature work

### Project Configuration
- Respect .gitignore settings
- Follow project-specific Git hooks if configured
- Ensure branch protection rules are understood

### Development Environment
- Verify development tools work with new branch
- Check if any environment-specific setup is needed
- Confirm CI/CD pipelines will work with new branch

Remember: A well-prepared development branch sets the foundation for productive and conflict-free development work.
```

## Notes
- Always verify the base branch is up-to-date before creating new branches
- Follow team conventions for branch naming and workflow
- Set up remote tracking to facilitate collaboration
- Keep branches focused on specific features or fixes
