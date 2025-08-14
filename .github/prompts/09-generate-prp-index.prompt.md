---
mode: agent
description: "Generate comprehensive index of all available PRPs with categorization and metadata"
---

---
inputs:
  - name: filter
    description: Filter PRPs by module, layer, or category (optional)
    required: false
  - name: format
    description: Output format (list, dependency-order, by-module, quick-reference)
    required: false
    default: dependency-order
---

---
command: "/generate-prp-index"
---
# Generate PRP Index Command for GitHub Copilot

## Command Usage
```
@copilot /generate-prp-index [filter] [format]
```

## Purpose
This command provides a comprehensive, tool-enhanced index of all available Product Requirements Prompt (PRP) commands for the Ikhtibar examination management system. It analyzes the current project structure, discovers existing PRPs, and presents them in an organized, dependency-aware format for optimal implementation planning.

**Input Parameters**: 
- `filter` - Filter by module, layer, or category (e.g., "user-management", "foundation", "assessment")
- `format` - Output format: `list`, `dependency-order`, `by-module`, or `quick-reference`

## How /generate-prp-index Works

### Phase 1: PRP Discovery and Analysis
```markdown
I'll help you explore all available PRP commands for the Ikhtibar project. Let me analyze the current project structure and discover existing PRPs.

**Phase 1.1: Project Structure Analysis**
```
PRP Discovery Analysis:
- **Filter**: [FILTER_TYPE] (if specified)
- **Format**: [LIST/DEPENDENCY-ORDER/BY-MODULE/QUICK-REFERENCE]
- **Project Context**: ASP.NET Core + React.js examination management system
- **Architecture**: Folder-per-feature with Clean Architecture principles
```

**Phase 1.2: PRP Repository Analysis using GitHub Copilot Tools**
```
I'll discover existing PRPs and analyze project requirements:
- file_search: "**/*.github/copilot/PRPs/**/*.md" # Find existing PRPs
- semantic_search: "PRP implementation patterns" # Find PRP usage
- read_file: [REQUIREMENTS_FILES] # Load project requirements
- list_dir: ".github/copilot/PRPs/" # Explore PRP organization
- file_search: "**/*requirements*.md" # Find requirement documents
- grep_search: "generate-prp|PRP" # Find PRP references
```

**Phase 1.3: Module Dependency Analysis**
```
Module Architecture Analysis:
- [ ] **Foundation Layer**: Authentication, roles, audit logging, notifications
- [ ] **Core Infrastructure**: Tree management, media management, configuration
- [ ] **Content Management**: Question creation, review workflows, validation
- [ ] **Assessment Layer**: Exam creation, publishing, monitoring, student interface
- [ ] **Evaluation Layer**: Auto-grading, manual grading, results finalization
- [ ] **Analytics Layer**: Reporting, analytics dashboards, custom reports
- [ ] **Cross-Cutting**: Internationalization, security, performance optimization
```
```

### Phase 2: Comprehensive PRP Index Generation

```markdown
**Phase 2.1: Dependency-Ordered PRP Index (Tool-Enhanced)**
```
Based on project analysis, here's the comprehensive PRP command index:

## 📋 Ikhtibar PRP Commands Index (Tool-Discovered)

### Implementation Status Analysis (Tool-Generated)
```powershell
# PRP status analysis using GitHub Copilot tools
file_search: ".github/copilot/PRPs/**/*-prp.md" # Count existing PRPs
semantic_search: "implemented features" # Find completed implementations
get_changed_files: # Check recent PRP-related changes
```

### Foundation Layer (Essential Infrastructure) 🏗️
**Implementation Priority: CRITICAL - Must be completed first**

| Command | Module | Feature | Status | Dependencies |
|---------|--------|---------|---------|--------------|
| `@copilot /generate-prp user-management authentication` | User Management | Authentication & JWT | [STATUS] | Database, Security |
| `@copilot /generate-prp user-management role-management` | User Management | RBAC System | [STATUS] | Authentication |
| `@copilot /generate-prp user-management audit-logging` | User Management | Activity Tracking | [STATUS] | Authentication |
| `@copilot /generate-prp reporting notification-system` | Reporting | Multi-channel Notifications | [STATUS] | User Management |

#### Foundation Layer Requirements Files
```
Available requirements files for enhanced context:
- .github/copilot/requirements/auth-requirements.md
- .github/copilot/requirements/rbac-requirements.md
- .github/copilot/requirements/audit-requirements.md
- .github/copilot/requirements/notification-channels.md
```

### Core Infrastructure Layer 🔧
**Implementation Priority: HIGH - Core system capabilities**

| Command | Module | Feature | Status | Dependencies |
|---------|--------|---------|---------|--------------|
| `@copilot /generate-prp question-bank tree-management` | Question Bank | Hierarchical Organization | [STATUS] | User Management |
| `@copilot /generate-prp question-bank media-management` | Question Bank | File & Image Handling | [STATUS] | Tree Management |
| `@copilot /generate-prp configuration system-configuration` | Configuration | Application Settings | [STATUS] | User Management |
| `@copilot /generate-prp localization i18n-setup` | Localization | English/Arabic Support | [STATUS] | Core Infrastructure |

#### Core Infrastructure Requirements Files
```
- .github/copilot/requirements/tree-structure.md
- .github/copilot/requirements/media-specifications.md
- .github/copilot/requirements/configuration-schema.md
- .github/copilot/requirements/i18n-requirements.md
```

### Content Management Layer 📝
**Implementation Priority: HIGH - Content creation and management**

| Command | Module | Feature | Status | Dependencies |
|---------|--------|---------|---------|--------------|
| `@copilot /generate-prp question-bank question-management` | Question Bank | Question CRUD Operations | [STATUS] | Core Infrastructure |
| `@copilot /generate-prp question-bank question-review` | Question Bank | Review & Approval Workflow | [STATUS] | Question Management |
| `@copilot /generate-prp workflows question-creation-workflow` | Workflows | Question Lifecycle Management | [STATUS] | Question Review |
| `@copilot /generate-prp question-bank question-import-export` | Question Bank | Bulk Operations | [STATUS] | Question Management |

#### Content Management Requirements Files
```
- .github/copilot/requirements/question-types.md
- .github/copilot/requirements/review-workflow.md
- .github/copilot/requirements/import-export-formats.md
- .github/copilot/requirements/question-validation.md
```

### Assessment Layer 📊
**Implementation Priority: MEDIUM - Assessment delivery system**

| Command | Module | Feature | Status | Dependencies |
|---------|--------|---------|---------|--------------|
| `@copilot /generate-prp exam-management exam-creation` | Exam Management | Exam Builder Interface | [STATUS] | Content Management |
| `@copilot /generate-prp workflows exam-creation-workflow` | Workflows | Exam Development Process | [STATUS] | Exam Creation |
| `@copilot /generate-prp exam-management exam-publishing` | Exam Management | Exam Release System | [STATUS] | Exam Creation |
| `@copilot /generate-prp workflows publish-exam-workflow` | Workflows | Publishing Process | [STATUS] | Exam Publishing |
| `@copilot /generate-prp exam-management student-exam-interface` | Exam Management | Student Assessment UI | [STATUS] | Exam Publishing |
| `@copilot /generate-prp exam-management exam-monitoring` | Exam Management | Real-time Proctoring | [STATUS] | Student Interface |

#### Assessment Layer Requirements Files
```
- .github/copilot/requirements/exam-types.md
- .github/copilot/requirements/student-interface.md
- .github/copilot/requirements/proctoring-requirements.md
- .github/copilot/requirements/exam-security.md
```

### Evaluation Layer 🎯
**Implementation Priority: MEDIUM - Grading and evaluation**

| Command | Module | Feature | Status | Dependencies |
|---------|--------|---------|---------|--------------|
| `@copilot /generate-prp grading auto-grading` | Grading | Automated Assessment | [STATUS] | Assessment Layer |
| `@copilot /generate-prp grading manual-grading` | Grading | Manual Review Interface | [STATUS] | Auto-grading |
| `@copilot /generate-prp workflows grading-workflow` | Workflows | Grading Process Management | [STATUS] | Manual Grading |
| `@copilot /generate-prp grading results-finalization` | Grading | Grade Publishing | [STATUS] | Grading Workflow |
| `@copilot /generate-prp grading grade-appeals` | Grading | Appeals & Reassessment | [STATUS] | Results Finalization |

#### Evaluation Layer Requirements Files
```
- .github/copilot/requirements/grading-algorithms.md
- .github/copilot/requirements/manual-grading-ui.md
- .github/copilot/requirements/appeals-process.md
- .github/copilot/requirements/grade-security.md
```

### Analytics Layer 📈
**Implementation Priority: LOW - Reporting and insights**

| Command | Module | Feature | Status | Dependencies |
|---------|--------|---------|---------|--------------|
| `@copilot /generate-prp reporting analytics-dashboard` | Reporting | Performance Analytics | [STATUS] | Evaluation Layer |
| `@copilot /generate-prp reporting custom-reports` | Reporting | Report Builder | [STATUS] | Analytics Dashboard |
| `@copilot /generate-prp reporting data-export` | Reporting | Data Export Tools | [STATUS] | Custom Reports |
| `@copilot /generate-prp analytics predictive-analytics` | Analytics | ML-based Insights | [STATUS] | Data Export |

#### Analytics Layer Requirements Files
```
- .github/copilot/requirements/analytics-specs.md
- .github/copilot/requirements/report-templates.md
- .github/copilot/requirements/data-privacy.md
- .github/copilot/requirements/ml-requirements.md
```

### Cross-Cutting Concerns 🔄
**Implementation Priority: VARIES - Throughout development cycle**

| Command | Module | Feature | Status | Dependencies |
|---------|--------|---------|---------|--------------|
| `@copilot /generate-prp security security-hardening` | Security | Application Security | [STATUS] | All Layers |
| `@copilot /generate-prp performance optimization-suite` | Performance | Performance Optimization | [STATUS] | All Layers |
| `@copilot /generate-prp testing comprehensive-testing` | Testing | Full Test Coverage | [STATUS] | All Layers |
| `@copilot /generate-prp deployment azure-deployment` | Deployment | Cloud Infrastructure | [STATUS] | All Layers |

#### Cross-Cutting Requirements Files
```
- .github/copilot/requirements/security-requirements.md
- .github/copilot/requirements/performance-targets.md
- .github/copilot/requirements/testing-strategy.md
- .github/copilot/requirements/deployment-architecture.md
```
```

### Phase 3: Advanced Usage and Integration Guidance

```markdown
**Phase 3.1: Enhanced PRP Commands with Requirements Files (Tool-Informed)**
```
For maximum context and precision, use specific requirements files:

## 🎯 Advanced PRP Commands with Requirements Files

### User Management with Enhanced Context
```bash
# Authentication with security specifications
@copilot /generate-prp user-management authentication .github/copilot/requirements/auth-requirements.md

# Role management with RBAC specifications  
@copilot /generate-prp user-management role-management .github/copilot/requirements/rbac-requirements.md

# Audit logging with compliance requirements
@copilot /generate-prp user-management audit-logging .github/copilot/requirements/audit-requirements.md
```

### Question Bank with Domain Specifications
```bash
# Question management with type specifications
@copilot /generate-prp question-bank question-management .github/copilot/requirements/question-types.md

# Question review with workflow specifications
@copilot /generate-prp question-bank question-review .github/copilot/requirements/review-workflow.md

# Media management with storage specifications
@copilot /generate-prp question-bank media-management .github/copilot/requirements/media-specifications.md
```

### Exam Management with Assessment Specifications
```bash
# Exam creation with type specifications
@copilot /generate-prp exam-management exam-creation .github/copilot/requirements/exam-types.md

# Student interface with UX specifications
@copilot /generate-prp exam-management student-exam-interface .github/copilot/requirements/student-interface.md

# Exam monitoring with proctoring requirements
@copilot /generate-prp exam-management exam-monitoring .github/copilot/requirements/proctoring-requirements.md
```

### Grading with Evaluation Specifications
```bash
# Auto-grading with algorithm specifications
@copilot /generate-prp grading auto-grading .github/copilot/requirements/grading-algorithms.md

# Manual grading with UI specifications
@copilot /generate-prp grading manual-grading .github/copilot/requirements/manual-grading-ui.md

# Results finalization with security requirements
@copilot /generate-prp grading results-finalization .github/copilot/requirements/grade-security.md
```

### Reporting with Analytics Requirements
```bash
# Analytics dashboard with visualization specs
@copilot /generate-prp reporting analytics-dashboard .github/copilot/requirements/analytics-specs.md

# Notification system with channel specifications
@copilot /generate-prp reporting notification-system .github/copilot/requirements/notification-channels.md

# Custom reports with template specifications
@copilot /generate-prp reporting custom-reports .github/copilot/requirements/report-templates.md
```

### Quick Reference Commands (Tool-Generated)
```bash
# Most commonly used PRPs (based on project analysis)
@copilot /generate-prp user-management authentication
@copilot /generate-prp question-bank question-management  
@copilot /generate-prp exam-management exam-creation
@copilot /generate-prp grading auto-grading
@copilot /generate-prp reporting analytics-dashboard
```
```

### Phase 4: Implementation Planning and Status Tracking

```markdown
**Phase 4.1: Implementation Strategy (Tool-Enhanced)**
```
Based on dependency analysis and project requirements:

## 📅 Recommended Implementation Timeline

### Sprint 1: Foundation (Weeks 1-2)
```bash
# Critical path - must be completed first
@copilot /generate-prp user-management authentication
@copilot /generate-prp user-management role-management
@copilot /generate-prp user-management audit-logging
```

### Sprint 2: Core Infrastructure (Weeks 3-4)
```bash
# Enable content management capabilities
@copilot /generate-prp question-bank tree-management
@copilot /generate-prp question-bank media-management
@copilot /generate-prp configuration system-configuration
```

### Sprint 3: Content Management (Weeks 5-6)
```bash
# Question creation and management
@copilot /generate-prp question-bank question-management
@copilot /generate-prp question-bank question-review
@copilot /generate-prp workflows question-creation-workflow
```

### Sprint 4: Assessment System (Weeks 7-8)
```bash
# Exam creation and delivery
@copilot /generate-prp exam-management exam-creation
@copilot /generate-prp exam-management exam-publishing
@copilot /generate-prp exam-management student-exam-interface
```

### Sprint 5: Evaluation System (Weeks 9-10)
```bash
# Grading and results
@copilot /generate-prp grading auto-grading
@copilot /generate-prp grading manual-grading
@copilot /generate-prp grading results-finalization
```

### Sprint 6: Analytics & Optimization (Weeks 11-12)
```bash
# Reporting and insights
@copilot /generate-prp reporting analytics-dashboard
@copilot /generate-prp reporting notification-system
@copilot /generate-prp performance optimization-suite
```

### PRP Status Tracking (Tool-Monitored)
```
Using GitHub Copilot tools to track implementation status:
- file_search: Track generated PRP files
- semantic_search: Find implemented features
- get_changed_files: Monitor recent implementation progress
- run_in_terminal: Execute validation commands
```

### Validation Commands for Each Layer
```powershell
# Foundation Layer Validation
dotnet test --filter "Category=Authentication"
dotnet test --filter "Category=Authorization"

# Core Infrastructure Validation  
dotnet test --filter "Category=QuestionBank"
dotnet test --filter "Category=MediaManagement"

# Content Management Validation
dotnet test --filter "Category=QuestionManagement"
npm run test -- --testNamePattern="Question"

# Assessment Layer Validation
dotnet test --filter "Category=ExamManagement"
npm run test -- --testNamePattern="Exam"

# Evaluation Layer Validation
dotnet test --filter "Category=Grading"
npm run test -- --testNamePattern="Grading"

# Analytics Layer Validation
dotnet test --filter "Category=Reporting"
npm run test -- --testNamePattern="Analytics"
```
```

## Command Activation Process
When a user types:
```
@copilot /generate-prp-index [filter] [format]
```

The system should:
1. **Analyze Project**: Use GitHub Copilot tools to discover existing PRPs and project structure
2. **Apply Filters**: Filter results based on specified module, layer, or category
3. **Generate Index**: Create comprehensive index in requested format
4. **Show Dependencies**: Display implementation dependencies and recommended order
5. **Provide Guidance**: Include usage examples and requirements file recommendations

## Notes
- All PRP commands follow the standardized syntax: `@copilot /generate-prp [module-name] [feature-name] [optional-requirements-file]`
- PRPs are organized by implementation dependency order for optimal development flow
- Each PRP includes comprehensive context, SRP compliance, and validation loops
- Requirements files provide enhanced context for specific domain knowledge
- Implementation timeline follows critical path analysis for maximum efficiency
- Status tracking uses GitHub Copilot tools for real-time progress monitoring
- All generated PRPs are saved to: `.github/copilot/PRPs/{module-order-2digits}-{module-name}/{feature-order-2digits}-{feature-name}-prp.md`
