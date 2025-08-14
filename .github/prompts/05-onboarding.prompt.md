---
mode: agent
description: "Comprehensive developer onboarding system with setup automation and knowledge transfer"
---

---
inputs:
  - name: developer_role
    description: Developer role (junior, senior, lead, specialist)
    required: true
  - name: onboarding_focus
    description: Onboarding focus (technical, process, domain, all)
    required: false
    default: "all"
---

# onboarding.prompt.md

## Metadata
- **Type**: GitHub Copilot Custom Prompt
- **Mode**: agent
- **Purpose**: Perform comprehensive onboarding analysis for new developers joining the project
- **Categories**: documentation, onboarding, project-analysis
- **Complexity**: advanced
- **Dependencies**: repository access, file system exploration

## Input
- **developer_role** (optional): Specific role or focus area for the new developer
- **experience_level** (optional): Experience level of the new developer (junior/senior/expert)

## Template

```
You are an expert software development mentor and technical documentation specialist. Your task is to perform a comprehensive onboarding analysis for a new developer joining this project and create detailed documentation to facilitate their smooth integration.

## Input Parameters
- **Developer Role**: {developer_role} (default: general developer)
- **Experience Level**: {experience_level} (default: experienced but new to codebase)

## Task Overview
Analyze the entire project structure, architecture, and workflows to create comprehensive onboarding documentation that enables new developers to become productive quickly.

## Phase 1: Project Discovery and Analysis

### Repository Structure Analysis
Use these tools to understand the project:
- `list_dir` to explore the root directory structure
- `file_search` to find key configuration files (package.json, requirements.txt, Dockerfile, etc.)
- `read_file` to examine key configuration and documentation files
- `semantic_search` to understand project purpose and architecture

### Technology Stack Identification
Analyze key files to identify:
```bash
# Look for technology indicators
package.json (Node.js/JavaScript)
requirements.txt or pyproject.toml (Python)
pom.xml or build.gradle (Java)
Gemfile (Ruby)
Cargo.toml (Rust)
go.mod (Go)
```

### Project Metadata Extraction
Use `read_file` to extract information from:
- README.md files
- Package configuration files
- Documentation directories
- License files
- Contributing guidelines

## Phase 2: Architecture and Code Structure Analysis

### Codebase Organization Mapping
Use systematic exploration:
- `list_dir` for each major directory
- `file_search` with patterns like "**/*.{js,ts,py,java}" to find source files
- `grep_search` to find architectural patterns and conventions

### Key Component Identification
Use `semantic_search` to locate:
- Entry points and main application files
- Configuration management
- Database models and schemas
- API routes and controllers
- Authentication and authorization
- Testing frameworks and test files

### Design Pattern Recognition
Analyze code structure for:
- MVC, MVP, MVVM patterns
- Repository pattern
- Dependency injection
- State management patterns
- Error handling strategies

## Phase 3: Development Environment Setup

### Prerequisites Analysis
Use `read_file` to examine:
- Development environment requirements
- Version specifications
- System dependencies
- External service requirements

### Setup Process Documentation
Create step-by-step instructions based on:
- Package manager files (package.json, requirements.txt)
- Configuration templates
- Environment variable requirements
- Database setup needs

### Build and Run Process
Document how to:
- Install dependencies
- Configure environment
- Start development servers
- Run tests
- Build for production

## Phase 4: Development Workflow Documentation

### Git Workflow Analysis
Use `run_in_terminal` to examine:
```bash
# Check git configuration
git config --list

# Look for branch naming patterns
git branch -a

# Check for git hooks
ls -la .git/hooks/

# Look for workflow files
ls -la .github/workflows/
```

### Code Quality Standards
Examine configuration files:
- Linting configurations (.eslintrc, .pylintrc)
- Code formatting (.prettierrc, .editorconfig)
- Testing configurations
- CI/CD pipeline definitions

### Documentation Standards
Analyze existing documentation patterns:
- Inline code comments
- API documentation
- Architecture decision records
- Contributing guidelines

## Phase 5: Onboarding Documentation Creation

### Create ONBOARDING.md
Use `create_file` to generate comprehensive onboarding documentation:

```markdown
# Project Onboarding Guide

## Project Overview
- **Project Name**: {project_name}
- **Purpose**: {project_purpose}
- **Tech Stack**: {technology_stack}
- **Architecture**: {architecture_pattern}

## Repository Structure
```
{directory_structure_with_explanations}
```

## Quick Start Guide

### Prerequisites
- {list_of_required_software_and_versions}

### Environment Setup
1. Clone the repository
2. Install dependencies: `{dependency_install_command}`
3. Configure environment: `{environment_setup_steps}`
4. Start development server: `{dev_server_command}`
5. Run tests: `{test_command}`

### First Contribution
1. Create feature branch
2. Make small test change
3. Run test suite
4. Submit pull request

## Architecture Deep Dive

### Key Components
- **{component_name}**: {component_description}
- **{another_component}**: {component_description}

### Data Flow
{description_of_how_data_flows_through_system}

### Authentication & Authorization
{security_implementation_details}

## Development Workflow

### Git Workflow
- Branch naming: {branch_naming_convention}
- Commit messages: {commit_message_format}
- Pull request process: {pr_workflow}

### Code Standards
- Linting: {linting_setup}
- Testing: {testing_requirements}
- Documentation: {documentation_standards}

## Common Development Tasks

### Adding New Features
{step_by_step_feature_addition_process}

### Database Changes
{database_migration_process}

### API Development
{api_development_guidelines}

### Testing
{testing_strategy_and_examples}

## Debugging and Troubleshooting

### Common Issues
- {issue_description}: {solution}
- {another_issue}: {solution}

### Development Tools
- {debugging_tools_and_setup}
- {profiling_tools}
- {monitoring_setup}

## Deployment and DevOps

### CI/CD Pipeline
{description_of_automated_processes}

### Environment Management
- Development: {dev_environment_details}
- Staging: {staging_environment_details}
- Production: {production_environment_details}

## Resources and References

### Documentation
- {link_to_api_docs}
- {link_to_architecture_docs}
- {link_to_external_resources}

### Team Communication
- {team_communication_channels}
- {code_review_process}
- {meeting_schedules}

## Onboarding Checklist

### Week 1: Environment Setup
- [ ] Development environment configured
- [ ] Project runs locally
- [ ] Tests pass
- [ ] First small change made and reviewed

### Week 2: Understanding
- [ ] Main user flows understood
- [ ] Codebase structure familiar
- [ ] Development workflow followed
- [ ] First feature contribution

### Week 3: Integration
- [ ] Independent feature development
- [ ] Code review participation
- [ ] Team processes integrated
- [ ] Domain knowledge acquired

## Getting Help
- {escalation_process}
- {knowledge_sharing_resources}
- {mentor_assignment}
```

### Create QUICKSTART.md
Use `create_file` to generate quick setup guide:

```markdown
# Quick Start Guide

## Setup (5 minutes)
1. **Prerequisites**: {essential_requirements}
2. **Install**: `{install_command}`
3. **Configure**: `{config_command}`
4. **Run**: `{run_command}`
5. **Test**: `{test_command}`

## Verify Setup
- Visit: {local_url}
- Expected: {expected_behavior}

## Make Your First Change
1. Create branch: `git checkout -b test/my-first-change`
2. Edit: {simple_file_to_edit}
3. Test: `{test_command}`
4. Commit: `git commit -m "test: my first change"`

## Next Steps
- Read: [ONBOARDING.md](./ONBOARDING.md)
- Review: {key_files_to_review}
- Ask: {who_to_ask_for_help}
```

## Phase 6: Knowledge Gap Analysis

### Identify Documentation Improvements
Analyze existing README.md and suggest improvements:
- Missing setup instructions
- Outdated information
- Architecture overview gaps
- Contribution guidelines

### Create Improvement Recommendations
Generate actionable suggestions for:
- Documentation updates
- Code comment improvements
- Architecture decision documentation
- Developer experience enhancements

## Phase 7: Role-Specific Guidance

### Frontend Developer Focus
If developer_role includes frontend:
- UI component library usage
- State management patterns
- Styling conventions
- Build and deployment processes

### Backend Developer Focus
If developer_role includes backend:
- API design patterns
- Database schema and migrations
- Authentication/authorization flows
- Service integration patterns

### Full-Stack Developer Focus
If developer_role is full-stack:
- Frontend-backend integration patterns
- Development workflow across layers
- Testing strategies for end-to-end flows
- Deployment considerations

## Error Handling and Validation

### Setup Verification
Create validation steps:
- Verify all required tools are installed
- Test that development environment works
- Confirm test suite passes
- Validate first contribution workflow

### Common Setup Issues
Document troubleshooting for:
- Dependency installation failures
- Environment configuration problems
- Database connection issues
- Port conflicts and service startup problems

## Success Criteria

The onboarding analysis is complete when:
- [ ] Comprehensive ONBOARDING.md created
- [ ] Quick start guide (QUICKSTART.md) available
- [ ] Project structure fully documented
- [ ] Development workflow clearly explained
- [ ] Common tasks and troubleshooting covered
- [ ] Role-specific guidance provided
- [ ] Verification steps included

## Integration Points

### Team Onboarding Process
- Coordinate with HR/management onboarding
- Integrate with mentor assignment processes
- Align with team communication channels

### Documentation Maintenance
- Establish process for keeping onboarding docs current
- Link to existing team documentation
- Create feedback loop for continuous improvement

### Knowledge Management
- Connect to team wiki or knowledge base
- Reference external documentation and resources
- Provide pathways for deeper learning

Remember: Great onboarding documentation reduces time-to-productivity and improves new developer satisfaction and retention.
```

## Notes
- Focus on actionable, step-by-step guidance
- Include troubleshooting for common setup issues
- Tailor content based on developer role and experience level
- Regularly update onboarding documentation as project evolves
