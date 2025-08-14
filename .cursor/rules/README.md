# Cursor Rules for Ikhtibar Project

This directory contains Cursor rules that provide AI assistance for the Ikhtibar educational exam management system.

## Rule Types

### Always Apply Rules
- **`architecture.mdc`** - Core project architecture and patterns (applies to all files)

### Auto Attached Rules
- **`backend-guidelines.mdc`** - Backend .NET development guidelines
- **`frontend-guidelines.mdc`** - Frontend React/TypeScript guidelines
- **`authentication-security.mdc`** - Authentication and security patterns
- **`i18n-accessibility.mdc`** - Internationalization and accessibility
- **`testing-qa.mdc`** - Testing and quality assurance practices

### Manual Rules
- **`development-workflow.mdc`** - Development commands and workflows

## How to Use

### Automatic Application
Rules with `alwaysApply: true` are automatically included in all AI interactions.

### File-Based Application
Rules with specific `globs` patterns automatically attach when working with matching files:
- Backend rules apply when working with `.cs` files
- Frontend rules apply when working with `.ts`/`.tsx` files
- Authentication rules apply when working with auth-related files

### Manual Invocation
Use `@ruleName` to explicitly include a rule:
```
@development-workflow
```

## Rule Descriptions

1. **Architecture** - Core project structure and patterns
2. **Backend Guidelines** - .NET development best practices
3. **Frontend Guidelines** - React/TypeScript development patterns
4. **Authentication & Security** - Security implementation guidelines
5. **I18n & Accessibility** - Multi-language and accessibility support
6. **Testing & QA** - Testing strategies and quality gates
7. **Development Workflow** - Build, test, and run commands

## Migration from .cursorrules

These rules replace the old `.cursorrules` file with a more organized, scoped approach that provides better context and automatic application based on file types and patterns.

## Customization

To modify or extend these rules:
1. Edit the `.mdc` files in this directory
2. Adjust the `globs` patterns to change when rules apply
3. Set `alwaysApply: true` for project-wide rules
4. Use `@filename` references to include specific files as context
