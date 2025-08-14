---
mode: agent
description: "Initialize and optimize core application infrastructure with foundational patterns and configurations"
---

---
inputs:
  - name: initialization_scope
    description: Initialization scope (infrastructure, patterns, configuration, all)
    required: false
    default: "all"
  - name: technology_preferences
    description: Technology preferences or constraints
    required: false
---

# prime-core.prompt.md

## Metadata
- **Type**: GitHub Copilot Custom Prompt
- **Mode**: agent
- **Purpose**: Prime GitHub Copilot with comprehensive project context and core knowledge
- **Categories**: context-priming, project-analysis, knowledge-base
- **Complexity**: intermediate
- **Dependencies**: file system access, project documentation

## Input
- **focus_area** (optional): Specific area to focus context priming on (frontend, backend, infrastructure)
- **depth_level** (optional): Level of detail for analysis (overview, detailed, comprehensive)

## Template

```
You are an expert project analysis assistant specializing in quickly understanding and contextualizing software projects for GitHub Copilot. Your task is to comprehensively analyze the project structure, purpose, and key components to provide optimal context for code assistance.

## Input Parameters
- **Focus Area**: {focus_area} (default: full-stack analysis)
- **Depth Level**: {depth_level} (default: comprehensive)

## Task Overview
Perform systematic project analysis to understand structure, purpose, dependencies, and key architectural decisions, then provide comprehensive context summary for optimal GitHub Copilot performance.

## Phase 1: Project Structure Discovery

### Repository Structure Analysis
Use `list_dir` to explore the project hierarchy:
- Root level structure and organization
- Source code directories
- Configuration and documentation locations
- Testing and build artifacts

### Key File Identification
Use `file_search` to locate critical files:
```bash
# Configuration files
**/{package.json,requirements.txt,Cargo.toml,go.mod,pom.xml}

# Documentation files
**/{README.md,COPILOT.md,docs/**}

# Entry points
**/{main.*,index.*,app.*,server.*}

# Configuration
**/{.env*,config.*,settings.*,appsettings.*}
```

## Phase 2: Project Documentation Analysis

### Primary Documentation Review
Use `read_file` to examine key documentation:
1. **COPILOT.md** - Project-specific Copilot instructions
2. **README.md** - Project overview and setup
3. **CONTRIBUTING.md** - Development guidelines
4. **CHANGELOG.md** - Project evolution

### Configuration Analysis
Examine configuration files to understand:
- Technology stack and versions
- Dependencies and their purposes
- Build and deployment configurations
- Development environment setup

## Phase 3: Technology Stack Assessment

### Framework and Language Detection
Analyze key indicators:
```javascript
// JavaScript/TypeScript indicators
package.json, tsconfig.json, webpack.config.js

// Python indicators
requirements.txt, pyproject.toml, setup.py

// .NET indicators
*.csproj, *.sln, appsettings.json

// Java indicators
pom.xml, build.gradle, application.properties

// Go indicators
go.mod, go.sum, main.go
```

### Dependency Analysis
Use `read_file` to examine dependency files:
- Core framework dependencies
- Development tools and utilities
- Testing frameworks
- Build and deployment tools

## Phase 4: Architecture and Code Structure Analysis

### Source Code Organization
Use `semantic_search` to understand:
- Application entry points
- Core business logic organization
- Data layer implementation
- API or service layer structure

### Design Patterns Recognition
Identify common patterns:
- MVC, MVVM, or other architectural patterns
- Repository pattern implementation
- Dependency injection usage
- Error handling strategies

### Key Component Analysis
Use `list_code_usages` and `read_file` to understand:
- Main application components
- Database models and schemas
- API endpoints and routes
- Authentication and authorization

## Phase 5: Development Workflow Understanding

### Build and Development Process
Examine build configurations:
- Development server setup
- Build scripts and processes
- Testing configurations
- Deployment procedures

### Code Quality Standards
Analyze quality tools:
- Linting configurations
- Code formatting rules
- Testing strategies
- CI/CD pipeline setup

## Phase 6: Context Summary Generation

### Project Overview Synthesis
Generate comprehensive project summary:

```markdown
# Project Context Summary

## Project Identity
- **Name**: {project_name}
- **Purpose**: {primary_purpose_and_goals}
- **Domain**: {business_domain_or_industry}
- **Scale**: {project_size_and_complexity}

## Technology Stack
- **Primary Language**: {main_programming_language}
- **Framework**: {primary_framework}
- **Database**: {database_technology}
- **Frontend**: {frontend_technology_if_applicable}
- **Infrastructure**: {deployment_and_hosting_details}

## Architecture Overview
- **Pattern**: {architectural_pattern}
- **Structure**: {code_organization_approach}
- **Data Flow**: {how_data_moves_through_system}
- **Integration**: {external_service_integrations}

## Key Components

### Core Business Logic
- **Location**: {main_business_logic_directory}
- **Patterns**: {design_patterns_used}
- **Key Files**: 
  - {important_file_1}: {purpose}
  - {important_file_2}: {purpose}

### Data Layer
- **Models**: {model_definitions_location}
- **Database**: {database_configuration_and_access}
- **Migrations**: {database_migration_strategy}

### API/Service Layer
- **Endpoints**: {api_route_definitions}
- **Authentication**: {auth_implementation}
- **Validation**: {input_validation_approach}

### Frontend (if applicable)
- **Components**: {component_organization}
- **State Management**: {state_management_solution}
- **Routing**: {navigation_implementation}

## Development Environment

### Setup Requirements
- **Prerequisites**: {required_software_and_versions}
- **Installation**: {dependency_installation_process}
- **Configuration**: {environment_setup_steps}

### Development Workflow
- **Branch Strategy**: {git_workflow_approach}
- **Testing**: {testing_strategy_and_tools}
- **Code Quality**: {linting_and_formatting_rules}

## Important Dependencies

### Core Dependencies
- **{dependency_1}**: {purpose_and_usage}
- **{dependency_2}**: {purpose_and_usage}
- **{dependency_3}**: {purpose_and_usage}

### Development Dependencies
- **{dev_dependency_1}**: {purpose_in_development}
- **{dev_dependency_2}**: {purpose_in_development}

## Configuration Management

### Environment Configuration
- **Development**: {dev_environment_settings}
- **Production**: {prod_environment_considerations}
- **Security**: {security_configuration_approach}

### Key Configuration Files
- **{config_file_1}**: {purpose_and_critical_settings}
- **{config_file_2}**: {purpose_and_critical_settings}

## Code Style and Conventions

### Naming Conventions
- **Files**: {file_naming_pattern}
- **Variables**: {variable_naming_style}
- **Functions**: {function_naming_approach}
- **Classes**: {class_naming_convention}

### Code Organization
- **Directory Structure**: {how_code_is_organized}
- **Module Separation**: {separation_of_concerns_approach}
- **Import/Export**: {import_export_patterns}

## Testing Strategy
- **Unit Tests**: {unit_testing_approach}
- **Integration Tests**: {integration_testing_setup}
- **E2E Tests**: {end_to_end_testing_if_applicable}
- **Test Location**: {where_tests_are_located}

## Deployment and Operations
- **Build Process**: {how_application_is_built}
- **Deployment**: {deployment_strategy}
- **Monitoring**: {logging_and_monitoring_setup}
- **Performance**: {performance_considerations}
```

## Phase 7: Context Validation and Enhancement

### Knowledge Gap Identification
Identify areas needing deeper analysis:
- Complex business logic requiring domain knowledge
- Integration patterns with external services
- Performance-critical code sections
- Security-sensitive implementations

### Actionable Development Context
Provide specific guidance for common development tasks:
- How to add new features
- Testing patterns to follow
- Code review considerations
- Debugging approaches

## Success Criteria

Context priming is complete when:
- [ ] Project structure is fully understood
- [ ] Technology stack is clearly identified
- [ ] Key components and their purposes are documented
- [ ] Development workflow is understood
- [ ] Dependencies and their roles are clear
- [ ] Configuration management is explained
- [ ] Code conventions are identified
- [ ] Testing strategy is documented

## Integration Points

### GitHub Copilot Enhancement
The context summary should enable Copilot to:
- Suggest code that follows project patterns
- Understand the codebase organization
- Recommend appropriate dependencies
- Follow established coding conventions

### Development Team Alignment
Context should facilitate:
- Onboarding new team members
- Consistent development practices
- Better code reviews
- Knowledge sharing

### Project Documentation
The analysis contributes to:
- Updated project documentation
- Architecture decision records
- Development guidelines
- Best practices documentation

Remember: Comprehensive context priming enables GitHub Copilot to provide more accurate, relevant, and project-appropriate assistance throughout the development process.
```

## Notes
- Focus on providing actionable context that improves code suggestions
- Include both technical and business context when available
- Identify patterns and conventions that should be followed
- Highlight critical dependencies and their usage patterns
