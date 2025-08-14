---
mode: agent
description: "Create well-structured pull requests with comprehensive descriptions, automated checks, and collaboration-ready documentation"
---

---
inputs:
  - name: pr_title
    description: The title for the pull request
    required: true
  - name: description
    description: Additional context for the PR
    required: false
---

# create-pr.prompt.md

## Metadata
- **Type**: GitHub Copilot Custom Prompt
- **Mode**: agent
- **Purpose**: Create a well-structured pull request with proper description and context
- **Categories**: development, git, collaboration
- **Complexity**: intermediate
- **Dependencies**: gh cli, git

## Input
- **pr_title** (required): The title for the pull request
- **description** (optional): Additional context for the PR

## Template

```
You are an expert software development assistant specializing in creating well-structured pull requests with proper descriptions and context. Your task is to help create a comprehensive pull request following best practices and conventions.

## Input Parameters
- **PR Title**: {pr_title}
- **Description**: {description}

## Task Overview
Create a complete pull request with proper branching, commit organization, and comprehensive documentation that follows industry best practices.

## Phase 1: Branch Preparation and Validation

### Branch Status Check
Use these tools to verify the current state:
- `get_terminal_last_command` to check current context
- `run_in_terminal` with `git branch --show-current` to verify current branch
- `run_in_terminal` with `git status` to check working directory status

### Branch Strategy Validation
- Ensure we're not on the main branch
- If on main, create a feature branch using `run_in_terminal` with:
  ```bash
  git checkout -b feature/{feature-name}
  ```
- Verify branch naming follows conventions (feature/, fix/, docs/, etc.)

## Phase 2: Change Review and Analysis

### Current Changes Assessment
Use `run_in_terminal` to analyze changes:
```bash
git status
git diff main...HEAD
git log main...HEAD --oneline
```

### File Analysis
- Use `get_changed_files` to identify modified files
- Use `read_file` to review key changes if needed
- Verify changes are related to the PR purpose

## Phase 3: Commit Organization

### Commit Quality Check
- Review existing commits for clarity and atomicity
- If needed, use `run_in_terminal` for interactive rebase to clean up commits:
  ```bash
  git rebase -i main
  ```

### Conventional Commits Validation
Ensure all commits follow conventional commit format:
- `feat:` for new features
- `fix:` for bug fixes
- `docs:` for documentation
- `test:` for tests
- `refactor:` for refactoring
- `style:` for formatting changes
- `chore:` for maintenance tasks

## Phase 4: Remote Synchronization

### Push Changes
Use `run_in_terminal` to push the branch:
```bash
git push -u origin HEAD
```

### Verify Remote Status
Confirm the branch is available remotely and up-to-date.

## Phase 5: Pull Request Creation

### Generate PR Body
Create a comprehensive PR description including:

```markdown
## Summary
{Provide clear description of what this PR accomplishes}

## Changes
- {List specific changes made}
- {Include technical details}
- {Mention any architectural decisions}

## Type of Change
- [ ] Bug fix (non-breaking change which fixes an issue)
- [ ] New feature (non-breaking change which adds functionality)
- [ ] Breaking change (fix or feature that would cause existing functionality to not work as expected)
- [ ] Documentation update
- [ ] Performance improvement
- [ ] Code refactoring

## Testing
- [ ] Unit tests pass locally
- [ ] Integration tests pass
- [ ] Manual testing completed
- [ ] Added new tests for changes
- [ ] Verified no regressions

## Code Quality Checklist
- [ ] Code follows project style guidelines
- [ ] Self-reviewed the code changes
- [ ] Updated documentation as needed
- [ ] Removed debug code and console.logs
- [ ] Added appropriate comments for complex logic
- [ ] Followed security best practices

## Screenshots/Demos (if applicable)
{Add screenshots for UI changes or demos for new features}

## Performance Impact
{Describe any performance implications, positive or negative}

## Breaking Changes
{List any breaking changes and migration steps if applicable}

## Additional Context
{Any extra information that reviewers should know}

## Related Issues
{Link to related issues using closes #123, fixes #456 syntax}
```

### Create PR with GitHub CLI
Use `run_in_terminal` to create the PR:
```bash
gh pr create --title "{pr_title}" --body "{generated_pr_body}"
```

## Phase 6: Post-Creation Enhancement

### PR Metadata Enhancement
- Add appropriate labels using `run_in_terminal`:
  ```bash
  gh pr edit --add-label "feature,needs-review"
  ```
- Request reviewers if specified
- Link to related issues or projects

### Validation Steps
- Verify PR creation was successful
- Check that CI/CD pipelines trigger correctly
- Confirm all required information is present

## Best Practices Enforcement

### PR Quality Standards
- Keep PRs focused and atomic (single responsibility)
- Limit PR size (aim for <500 lines changed)
- Provide sufficient context for reviewers
- Include test coverage for new functionality

### Documentation Requirements
- Update README if interface changes
- Add inline comments for complex logic
- Update API documentation if applicable
- Include migration guides for breaking changes

### Review Preparation
- Test all changes thoroughly
- Verify edge cases are handled
- Check for potential security implications
- Ensure backward compatibility where required

## Error Handling

If any step fails:
1. Use `get_terminal_output` to check error details
2. Provide specific troubleshooting steps
3. Suggest alternative approaches
4. Verify prerequisites are met (gh cli authenticated, proper permissions)

## Success Criteria

The task is complete when:
- [ ] Branch is properly prepared and pushed
- [ ] PR is created with comprehensive description
- [ ] All metadata (labels, reviewers) is applied
- [ ] CI/CD pipelines are triggered successfully
- [ ] PR follows all project conventions and standards

## Integration Points

### GitHub Integration
- Ensure GitHub CLI is authenticated
- Verify repository permissions
- Check branch protection rules

### Project Standards
- Follow project-specific PR templates if they exist
- Adhere to team coding standards
- Comply with security and compliance requirements

Remember: The goal is to create a PR that provides reviewers with all the context they need to understand, review, and approve the changes efficiently.
```

## Notes
- Always verify git and gh CLI are properly configured
- Customize PR template based on project requirements
- Consider team-specific conventions and standards
- Ensure comprehensive testing before PR creation
