# Prompt File Organization Guide

This document explains the organization of prompt files in the Ikhtibar project. The files are named with 2-digit prefixes to indicate their logical order in the development workflow.

## Organization Structure

### Project Setup & Architecture (01-09)
- **01-quick-start-prps.prompt.md**: Initial project setup and bootstrapping
- **02-codebase-analysis.prompt.md**: Analysis of existing codebase and architecture
- **03-architecture-review.prompt.md**: Review and enhancement of architecture
- **04-ikhtibar-implementation-prps.prompt.md**: Comprehensive implementation planning
- **05-onboarding.prompt.md**: Developer onboarding and environment setup
- **06-task-list-init.prompt.md**: Initialize project task lists
- **07-prime-core.prompt.md**: Core functionality preparation
- **08-generate-prp.prompt.md**: Generate Product Requirement Prompts
- **09-generate-prp-index.prompt.md**: Generate PRP indexes for navigation

### PRP Creation & Planning (10-19)
- **10-prp-planning-create.prompt.md**: Create high-level PRP plans
- **11-create-feature-prp.prompt.md**: Create feature-specific PRPs
- **12-prp-spec-create.prompt.md**: Create detailed PRP specifications
- **13-prp-task-create.prompt.md**: Break down PRPs into actionable tasks
- **14-api-contract-define.prompt.md**: Define API contracts and interfaces
- **15-requirements-mapping.prompt.md**: Map requirements to implementation tasks
- **16-requirements-traceability.prompt.md**: Trace requirements back to implementation and tests
- **17-prp-status-check.prompt.md**: Check PRP implementation status and update status tracking file

### Implementation & Execution (20-29)
- **20-prp-spec-execute.prompt.md**: Execute PRP specifications
- **21-prp-task-execute.prompt.md**: Execute individual PRP tasks
- **22-user-story-rapid.prompt.md**: Implement user stories rapidly
- **23-refactor-simple.prompt.md**: Perform simple refactoring tasks
- **24-execute-prp.prompt.md**: Execute PRPs with validation
- **25-prp-base-create.prompt.md**: Create base PRPs for project foundation
- **26-prp-base-execute.prompt.md**: Execute base PRPs
- **27-ts-create-base-prp.prompt.md**: Create TypeScript-specific base PRPs
- **28-ts-execute-base-prp.prompt.md**: Execute TypeScript-specific base PRPs
- **29-prp-analyze-run.prompt.md**: Analyze and run PRPs

### Validation, Review & Quality Control (30-39)
- **30-prp-validate.prompt.md**: Validate PRPs for completeness and correctness
- **31-code-quality.prompt.md**: Check code quality against standards
- **32-code-quality-unified.prompt.md**: Unified code quality checks
- **33-review-general.prompt.md**: General code review
- **34-create-pr.prompt.md**: Create pull requests
- **35-test.prompt.md**: Basic testing procedures
- **36-test-coverage.prompt.md**: Test coverage analysis
- **37-security-analysis.prompt.md**: Security analysis of code
- **38-performance.prompt.md**: Performance testing and optimization
- **39-document.prompt.md**: Documentation generation

### Issue Resolution & Source Control (40-49)
- **40-conflict-resolver-general.prompt.md**: Resolve general conflicts
- **41-conflict-resolver-specific.prompt.md**: Resolve specific conflicts
- **42-git.prompt.md**: Git operations and source control
- **43-smart-commit.prompt.md**: Smart commit message generation
- **44-smart-resolve.prompt.md**: Smart conflict resolution
- **45-smart-resolver.prompt.md**: Enhanced conflict resolver
- **46-new-dev-branch.prompt.md**: Create new development branches
- **47-debug-rca.prompt.md**: Debug and root cause analysis
- **48-ts-review.prompt.md**: TypeScript-specific review
- **49-review-staged-unstaged.prompt.md**: Review staged and unstaged changes

### TypeScript Specific Tools (50-59)
- **50-ts-review-general.prompt.md**: General TypeScript review
- **51-ts-review-staged-unstaged.prompt.md**: TypeScript staged/unstaged review

## Usage Guide for New Developers

1. **Project Setup**: Start with prompts 01-09 to understand the project architecture and set up your environment
2. **Planning Phase**: Use prompts 10-19 to plan your implementation work
3. **Implementation Phase**: Use prompts 20-29 to execute your planned tasks
4. **Quality Control**: Use prompts 30-39 to validate your work before submission
5. **Issue Resolution & Source Control**: Use prompts 40-49 for handling conflicts and source control operations
6. **TypeScript Development**: Use prompts 50-59 for TypeScript-specific tasks

Each prompt file is designed to be used in sequence, with later prompts building upon the outputs of earlier ones. This organization helps new developers understand the logical flow of development activities and ensures consistent, high-quality outputs across the team.

## Benefits of This Organization

- **Clear Dependencies**: Makes it obvious which prompts should be run before others
- **Logical Grouping**: Related prompts are grouped by development phase
- **Onboarding Support**: New developers can follow the numbered sequence
- **Workflow Optimization**: Encourages a methodical approach to development
- **Quality Assurance**: Built-in checks and validations at appropriate phases

For any questions about this organization or how to use specific prompts, please refer to the detailed documentation within each prompt file or contact the project maintainers.
