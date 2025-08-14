---
description: "Planning and PRD generation with visual diagrams and comprehensive analysis"
applyTo: "**/*.md,**/README.md,**/*.txt,**/docs/**"
---

# Planning and PRD Generation Instructions

You are creating comprehensive Product Requirements Documents (PRDs) and technical specifications for the Ikhtibar educational exam management system.

## Planning Philosophy

### Research First
- Gather comprehensive context before planning
- Analyze existing solutions and patterns
- Understand technical constraints and requirements
- Consider user needs and business objectives

### Visual Thinking
- Use Mermaid diagrams to clarify concepts
- Create flowcharts for user workflows
- Design system architecture diagrams
- Illustrate data flow and component relationships

### Validation Built-in
- Include challenges and edge cases
- Document assumptions and risks
- Define measurable success criteria
- Plan implementation validation steps

### Implementation Ready
- Output feeds directly into development PRPs
- Include technical specifications
- Define clear acceptance criteria
- Provide actionable next steps

## Planning Process

### Phase 1: Idea Expansion & Research

#### Context Gathering
```yaml
research_areas:
  market_analysis:
    - "Examine existing educational platforms"
    - "Analyze exam management solutions"
    - "Study user experience patterns"
  
  technical_analysis:
    - "Review current system architecture"
    - "Identify integration points"
    - "Assess technical constraints"
  
  user_research:
    - "Define user personas and workflows"
    - "Identify pain points and requirements"
    - "Map user journey and touchpoints"
```

#### Initial Exploration Template
```markdown
## Research Findings

### Similar Solutions Analysis
- **Platform 1**: [Key features and approach]
- **Platform 2**: [Strengths and limitations]  
- **Platform 3**: [Innovation opportunities]

### Technical Constraints
- **Backend**: ASP.NET Core 8.0, Dapper, SQL Server
- **Frontend**: React.js 18, TypeScript, Tailwind CSS
- **Architecture**: Clean Architecture, folder-per-feature
- **Integration**: JWT authentication, i18n support

### User Requirements
- **Primary Users**: [Educators, Students, Administrators]
- **Key Workflows**: [Exam creation, taking, grading, analytics]
- **Success Metrics**: [Performance, usability, security]
```

### Phase 2: PRD Structure Generation

#### 1. Executive Summary Template
```markdown
## Problem Statement
[Clear articulation of the problem being solved]
- **Current Challenge**: [Specific pain point]
- **Impact**: [Who is affected and how]
- **Opportunity**: [Business value of solving this]

## Solution Overview
[High-level description of proposed solution]
- **Approach**: [Core strategy]
- **Key Features**: [Primary capabilities]
- **Differentiation**: [Unique value proposition]

## Success Metrics
- **User Metrics**: [Adoption, engagement, satisfaction]
- **Technical Metrics**: [Performance, reliability, scalability]
- **Business Metrics**: [ROI, efficiency gains, cost reduction]
```

#### 2. User Stories & Scenarios Template
```markdown
## Primary User Flow
\```mermaid
graph LR
    A[User Login] --> B{Role Check}
    B -->|Teacher| C[Create Exam]
    B -->|Student| D[Take Exam]
    B -->|Admin| E[System Management]
    C --> F[Configure Questions]
    D --> G[Submit Answers]
    E --> H[Monitor System]
\```

## User Stories

### For Teachers
1. **As a teacher**, I want to create comprehensive exams so that I can assess student knowledge effectively
   - **Acceptance Criteria**:
     - Can add multiple question types (MCQ, essay, true/false)
     - Can set time limits and attempt restrictions
     - Can organize questions by topics and difficulty
     - Can preview exam before publishing

2. **As a teacher**, I want to review exam results so that I can provide meaningful feedback
   - **Acceptance Criteria**:
     - Can view individual and aggregate performance
     - Can add comments and grades
     - Can export results for record keeping

### For Students  
1. **As a student**, I want to take exams in a user-friendly interface so that I can focus on answering questions
   - **Acceptance Criteria**:
     - Clear question presentation with proper formatting
     - Progress indicator and time remaining
     - Ability to mark questions for review
     - Auto-save functionality

### For Administrators
1. **As an administrator**, I want to monitor system usage so that I can ensure optimal performance
   - **Acceptance Criteria**:
     - Real-time system metrics dashboard
     - User activity monitoring
     - Performance analytics and reporting
```

#### 3. System Architecture Template
```markdown
## High-Level Architecture
\```mermaid
graph TB
    subgraph "Frontend (React)"
        A[User Interface]
        B[State Management]
        C[API Client]
    end
    
    subgraph "Backend (ASP.NET Core)"
        D[Controllers]
        E[Services]
        F[Repositories]
    end
    
    subgraph "Data Layer"
        G[SQL Server]
        H[File Storage]
    end
    
    A --> B
    B --> C
    C --> D
    D --> E
    E --> F
    F --> G
    F --> H
\```

## Component Breakdown

### Frontend Components
- **Exam Builder**: Question creation and configuration interface
- **Exam Player**: Student exam-taking interface
- **Results Dashboard**: Performance analytics and reporting
- **User Management**: Authentication and profile management

### Backend Services
- **Exam Service**: Exam CRUD operations and business logic
- **Question Service**: Question bank management
- **User Service**: Authentication and authorization
- **Analytics Service**: Performance tracking and reporting

### Data Models
- **Exam Model**: Exam metadata, settings, and relationships
- **Question Model**: Question content, types, and scoring
- **User Model**: User profiles, roles, and permissions
- **Result Model**: Exam attempts, answers, and scores
```

#### 4. Technical Specifications Template
```markdown
## API Design
\```mermaid
sequenceDiagram
    participant U as User
    participant F as Frontend
    participant A as API
    participant D as Database
    
    U->>F: Create Exam
    F->>A: POST /api/exams
    A->>D: Insert Exam
    D-->>A: Exam Created
    A-->>F: Exam Response
    F-->>U: Success Confirmation
\```

## API Endpoints

### Exam Management
- **POST /api/exams**
  - **Purpose**: Create new exam
  - **Request**: `{title, description, settings, questions}`
  - **Response**: `{id, title, createdAt, status}`
  - **Errors**: `400 Bad Request`, `401 Unauthorized`, `422 Validation Error`

- **GET /api/exams/{id}**
  - **Purpose**: Retrieve exam details
  - **Response**: `{exam, questions, settings}`
  - **Errors**: `404 Not Found`, `403 Forbidden`

- **PUT /api/exams/{id}**
  - **Purpose**: Update exam
  - **Request**: `{title, description, settings}`
  - **Response**: `{updated_exam}`

### Question Management
- **POST /api/questions**
  - **Purpose**: Create question
  - **Request**: `{content, type, options, correctAnswer}`
  - **Response**: `{question_id, created_at}`

## Data Flow
\```mermaid
flowchart TD
    A[Exam Creation] --> B{Validation}
    B -->|Valid| C[Save to Database]
    B -->|Invalid| D[Return Errors]
    C --> E[Generate Exam ID]
    E --> F[Update Question Bank]
    F --> G[Return Success Response]
    D --> H[Display Error Messages]
\```
```

#### 5. Implementation Strategy Template
```markdown
## Development Phases
\```mermaid
graph LR
    A[Foundation<br/>2 weeks] --> B[Core Features<br/>4 weeks]
    B --> C[Advanced Features<br/>3 weeks]
    C --> D[Integration<br/>2 weeks]
    D --> E[Testing & Polish<br/>2 weeks]
    
    A1[User Auth<br/>DB Setup] -.-> A
    B1[Exam Creation<br/>Question Bank] -.-> B
    C1[Analytics<br/>Reporting] -.-> C
    D1[Performance<br/>Security] -.-> D
    E1[E2E Testing<br/>Deployment] -.-> E
\```

## Implementation Priority

### Phase 1: Foundation (2 weeks)
- **User Authentication**: JWT-based auth system
- **Database Schema**: Core tables and relationships
- **Basic UI**: Layout and navigation structure
- **API Framework**: Controllers and middleware setup

### Phase 2: Core Features (4 weeks)  
- **Exam Creation**: Full exam builder interface
- **Question Bank**: Question management system
- **Exam Taking**: Student exam interface
- **Basic Grading**: Automated scoring system

### Phase 3: Advanced Features (3 weeks)
- **Analytics Dashboard**: Performance reporting
- **Advanced Question Types**: Essay questions, media support
- **Collaboration**: Teacher collaboration tools
- **Notifications**: Email and in-app notifications

### Phase 4: Integration & Optimization (2 weeks)
- **Performance Optimization**: Caching and query optimization
- **Security Hardening**: Penetration testing and fixes
- **Integration Testing**: End-to-end workflow validation
- **Documentation**: API docs and user guides

### Phase 5: Testing & Polish (2 weeks)
- **User Acceptance Testing**: Stakeholder validation
- **Bug Fixes**: Issue resolution and stability
- **Performance Testing**: Load testing and optimization
- **Deployment**: Production deployment and monitoring
```

### Phase 3: Challenge & Validation

#### Devil's Advocate Analysis Template
```yaml
challenges:
  technical_risks:
    - risk: "Database performance with large question banks"
      mitigation: "Implement pagination and caching strategies"
      impact: "High"
    
    - risk: "Real-time exam synchronization"
      mitigation: "Use SignalR for real-time updates"
      impact: "Medium"
  
  user_experience_risks:
    - risk: "Complex exam creation interface"
      mitigation: "Progressive disclosure and guided workflows"
      impact: "High"
    
    - risk: "Mobile responsiveness for exam taking"
      mitigation: "Mobile-first design approach"
      impact: "Medium"
  
  business_risks:
    - risk: "User adoption and training requirements"
      mitigation: "Comprehensive onboarding and documentation"
      impact: "High"
```

#### Success Criteria Template
```markdown
## Definition of Done

### Technical Criteria
- [ ] All API endpoints are implemented and tested
- [ ] Frontend components are responsive and accessible
- [ ] Database performance meets requirements (<200ms queries)
- [ ] Security vulnerabilities are addressed
- [ ] Code coverage exceeds 80%

### User Experience Criteria
- [ ] Users can complete core workflows without assistance
- [ ] Interface supports both Arabic and English
- [ ] Mobile experience is fully functional
- [ ] Loading times are under 3 seconds

### Business Criteria
- [ ] System handles expected user load
- [ ] Data integrity is maintained
- [ ] Backup and recovery procedures are tested
- [ ] Compliance requirements are met

## Measurable Outcomes
- **Performance**: Page load times < 3s, API response < 500ms
- **Usability**: Task completion rate > 90%, user satisfaction > 4.5/5
- **Reliability**: Uptime > 99.5%, data loss incidents = 0
- **Adoption**: Active user growth > 20% monthly
```

## Output Format Guidelines

### Document Structure
1. **Executive Summary** (1 page)
2. **Problem Definition** (1-2 pages)
3. **Solution Design** (2-3 pages with diagrams)
4. **Technical Specifications** (3-4 pages)
5. **Implementation Plan** (2-3 pages)
6. **Risk Analysis** (1-2 pages)
7. **Success Metrics** (1 page)

### Visual Requirements
- Include at least 3 Mermaid diagrams per PRD
- Use consistent diagram styling and colors
- Provide detailed captions for all visuals
- Include both high-level and detailed views

### Validation Checklist
- [ ] All stakeholder needs are addressed
- [ ] Technical feasibility is confirmed
- [ ] Implementation timeline is realistic
- [ ] Success metrics are measurable
- [ ] Risks are identified with mitigations
- [ ] Documentation is comprehensive and clear

Always ensure that PRDs are actionable, comprehensive, and ready for immediate implementation planning. Include specific technical details relevant to the Ikhtibar project stack and architectural patterns.
