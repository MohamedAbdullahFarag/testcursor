# Cursor Custom Modes for Ikhtibar Project

This directory contains specialized Cursor custom modes designed for different phases of the Software Development Life Cycle (SDLC) for the Ikhtibar educational exam management system.

## Available Custom Modes

### 🏗️ [Feature Architect](./feature-architect.json)
**Focus**: System Design & Technical Planning  
**Use When**: Starting new features, designing system architecture, creating technical specifications  
**Expertise**: High-level architecture, API design, data flow diagrams, implementation planning  

**Example Prompts:**
- "Design a notification system for exam alerts"
- "Architect a question bank with categorization and tagging"  
- "Plan the user management and role-based access system"

---

### 💻 [Implementation Agent](./implementation-agent.json)
**Focus**: Writing Production-Ready Code  
**Use When**: Converting specifications into working code, following established patterns  
**Expertise**: Full-stack development, Clean Architecture, SOLID principles, testing  

**Example Prompts:**
- "Implement user management CRUD operations"
- "Create a question bank component with search and filtering"
- "Build the exam taking interface with timer and auto-save"

---

### 🧪 [Test Engineer](./test-engineer.json)
**Focus**: Comprehensive Testing Strategy  
**Use When**: Creating test suites, improving code quality, ensuring reliability  
**Expertise**: Unit testing, integration testing, E2E testing, performance testing  

**Example Prompts:**
- "Create comprehensive tests for the user authentication system"
- "Test the question bank CRUD operations with edge cases"
- "Write E2E tests for the exam taking workflow"

---

### 📋 [Task Manager](./task-manager.json)
**Focus**: Breaking Down Complex Work  
**Use When**: Planning implementation, creating actionable tasks with validation  
**Expertise**: Task decomposition, dependency management, validation planning  

**Example Prompts:**
- "Break down user authentication system implementation into tasks"
- "Create tasks for building a question bank with categorization"
- "Generate task list for exam monitoring dashboard"

---

### 👁️ [Code Reviewer](./code-reviewer.json)
**Focus**: Code Quality Assessment  
**Use When**: Reviewing code for quality, security, performance, and best practices  
**Expertise**: Security review, performance analysis, architecture validation  

**Example Prompts:**
- "Review the user authentication implementation for security issues"
- "Analyze the question bank API for performance problems"
- "Check the exam monitoring dashboard for code quality"

---

### 📚 [Documentation Specialist](./documentation-specialist.json)
**Focus**: Creating Comprehensive Documentation  
**Use When**: Writing technical docs, user guides, API documentation  
**Expertise**: Technical writing, API specs, user guides, architectural documentation  

**Example Prompts:**
- "Create API documentation for the user management endpoints"
- "Write a user guide for exam creation and management"
- "Document the deployment process for the application"

---

### 🚀 [DevOps Engineer](./devops-engineer.json)
**Focus**: Deployment & Infrastructure  
**Use When**: Setting up CI/CD, containerization, cloud infrastructure  
**Expertise**: Docker, Azure, GitHub Actions, monitoring, security  

**Example Prompts:**
- "Set up CI/CD pipeline for the Ikhtibar application"
- "Create Docker containers for backend and frontend"
- "Design Azure infrastructure for production deployment"

---

### 📋 [PRP Specialist](./prp-specialist.json)
**Focus**: Project Requirements & Planning  
**Use When**: Creating comprehensive PRPs, managing 6-phase planning, strategic project planning  
**Expertise**: Requirements analysis, phase management, risk assessment, strategic planning  

**Example Prompts:**
- "Create a comprehensive PRP for the exam monitoring system"
- "Plan the 6-phase implementation for user management"
- "Analyze requirements and create strategic implementation plan"

---

### 📚 [Instruction Specialist](./instruction-specialist.json)
**Focus**: Specialized Instructions & Guidelines  
**Use When**: Creating detailed instructions, guidelines, and development standards  
**Expertise**: Development standards, best practices, feature specifications, API guidelines  

**Example Prompts:**
- "Create detailed instructions for implementing Clean Architecture"
- "Write guidelines for React component development"
- "Create feature specification templates"

---

### 🎯 [Prompt Specialist](./prompt-specialist.json)
**Focus**: AI Prompt Engineering & Interactions  
**Use When**: Creating effective AI prompts, analyzing codebases, generating specialized AI interactions  
**Expertise**: Prompt engineering, codebase analysis, PRP generation, AI interaction optimization  

**Example Prompts:**
- "Create an effective prompt for codebase analysis"
- "Design prompts for PRP generation"
- "Optimize AI interaction prompts for testing"

---

### 🚀 [PRP Executor](./prp-executor.json)
**Focus**: Automated PRP Execution & Sequential Implementation  
**Use When**: Executing PRPs in sequence, managing project phases, automated validation  
**Expertise**: Sequential execution, dependency management, validation automation, progress tracking  

**Example Prompts:**
- "Execute PRP 01-core-entities-setup-prp.md from the foundation phase"
- "Run all PRPs in sequence automatically"
- "Show me the current PRP execution status"
- "Execute the entire foundation phase with validation"

## How to Use These Custom Modes

### Method 1: Cursor Settings
1. Open Cursor Settings (`Ctrl+,`)
2. Navigate to `Chat` → `Custom Modes`
3. Click "Import" and select the desired `.json` file
4. The custom mode will appear in your mode picker

### Method 2: Quick Mode Switching
1. Use the mode picker dropdown in the Agent panel
2. Press `Ctrl+.` for quick mode switching
3. Select your desired custom mode

### Method 3: Keyboard Shortcuts
Set up keyboard shortcuts in settings for quick access to frequently used modes.

## Mode-Specific Tool Access

Each custom mode has specific tools enabled based on their role:

- **Feature Architect**: `codebase`, `read_file`, `search_replace`
- **Implementation Agent**: `codebase`, `read_file`, `edit_file`, `search_replace`, `run_terminal_cmd`
- **Test Engineer**: `codebase`, `read_file`, `edit_file`, `search_replace`, `run_terminal_cmd`
- **Code Reviewer**: `codebase`, `read_file`, `search_replace`, `run_terminal_cmd`
- **Task Manager**: `codebase`, `read_file`, `edit_file`, `search_replace`
- **Documentation Specialist**: `codebase`, `read_file`, `edit_file`, `search_replace`
- **DevOps Engineer**: `codebase`, `read_file`, `edit_file`, `search_replace`, `run_terminal_cmd`
- **PRP Specialist**: `codebase`, `read_file`, `edit_file`, `search_replace`
- **Instruction Specialist**: `codebase`, `read_file`, `edit_file`, `search_replace`
- **Prompt Specialist**: `codebase`, `read_file`, `edit_file`, `search_replace`
- **PRP Executor**: `codebase`, `read_file`, `edit_file`, `search_replace`, `run_terminal_cmd`

## Integration with Project Rules

These custom modes work seamlessly with the project rules in `.cursor/rules/`:

- **Automatic Context**: Rules automatically apply based on file types
- **Enhanced Guidance**: Custom modes provide specialized expertise
- **Consistent Patterns**: All modes follow established project patterns
- **Quality Assurance**: Built-in validation and testing approaches

## Customization

To modify or extend these custom modes:

1. Edit the `.json` files in this directory
2. Adjust the `tools` array to enable/disable specific capabilities
3. Modify the `instructions` to customize behavior
4. Update the `description` for better identification
5. Import the modified mode through Cursor settings

## Best Practices

### When to Use Each Mode
- **Planning Phase**: Feature Architect, Task Manager, PRP Specialist
- **Requirements Phase**: Instruction Specialist, Prompt Specialist
- **Execution Phase**: PRP Executor, Implementation Agent, Test Engineer
- **Review Phase**: Code Reviewer
- **Documentation Phase**: Documentation Specialist
- **Deployment Phase**: DevOps Engineer

### Mode Switching Strategy
- Start with **PRP Specialist** for strategic planning
- Use **Feature Architect** for system design
- Switch to **Instruction Specialist** for detailed guidelines
- Use **Prompt Specialist** for AI interaction optimization
- Use **PRP Executor** for automated implementation
- Switch to **Implementation Agent** for coding
- Use **Test Engineer** for quality assurance
- Apply **Code Reviewer** for final validation
- Use **Documentation Specialist** for user guides
- Deploy with **DevOps Engineer**

## Migration from GitHub Copilot

These custom modes replace the GitHub Copilot chat modes with:

- **Better Integration**: Seamless integration with Cursor's AI capabilities
- **Tool Access**: Direct access to Cursor's powerful tools
- **Project Context**: Automatic application of project-specific rules
- **Enhanced Workflow**: Streamlined development process

## Support and Feedback

For questions or improvements to these custom modes:

1. Review the mode instructions and adjust as needed
2. Share feedback with the development team
3. Contribute improvements through pull requests
4. Document new patterns and best practices

Remember: These custom modes are designed to enhance your development workflow. Use them strategically based on your current task and phase of development.
