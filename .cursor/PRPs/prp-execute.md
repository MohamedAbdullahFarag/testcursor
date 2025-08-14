# Ikhtibar PRP Execution Order

This document provides the correct order for executing Product Requirements Prompts (PRPs) to implement the Ikhtibar system. Execute PRPs in the specified numerical order to minimize integration issues and refactoring.

# Ikhtibar PRP Execution Order

This document provides the correct order for executing Product Requirements Prompts (PRPs) to implement the Ikhtibar system. Execute PRPs in the specified numerical order to minimize integration issues and refactoring.

## 1️⃣ Foundation Layer (01-14)
```bash
# Core Infrastructure and Authentication
cd 01-foundation
execute-prp 01-core-entities-setup-prp.md             # Core entities
execute-prp 02-base-repository-pattern-prp.md         # Repository pattern
execute-prp 03-api-foundation-prp.md                  # API foundation
execute-prp 04-frontend-foundation-prp.md             # Frontend foundation
execute-prp 05-database-initialization-prp.md         # Database setup
execute-prp 06-notification-system-comprehensive-prp.md # Notification system
execute-prp 07-authentication-system-prp.md           # Authentication
execute-prp 08-frontend-auth-prp.md                   # Frontend auth
execute-prp 09-authentication-system-comprehensive-prp.md # Auth comprehensive
execute-prp 10-backend-services-prp.md                # Backend services
execute-prp 11-frontend-components-prp.md             # Frontend components
execute-prp 12-backend-hierarchy-prp.md               # Backend hierarchy
execute-prp 13-audit-logging-comprehensive-prp.md     # Audit logging
execute-prp 14-role-management-comprehensive-prp.md   # Role management
```

## 2️⃣ Infrastructure Layer (15-16)
```bash
# Data Structure and Media Management
cd ../02-infrastructure
execute-prp 15-tree-management-comprehensive-prp.md   # Tree structure
execute-prp 16-media-management-comprehensive-prp.md  # Media handling
```

## 3️⃣ Content Management Layer (17-19)
```bash
# Question Bank Management
cd ../03-content
execute-prp 17-question-management-comprehensive-prp.md # Question management
execute-prp 18-question-review-prp.md                 # Question review
execute-prp 19-question-creation-workflow-prp.md      # Question workflow
```

## 4️⃣ Assessment Layer (20-26)
```bash
# Exam Creation and Management
cd ../04-assessment
execute-prp 20-publish-exam-workflow-prp.md           # Publish workflow
execute-prp 21-exam-creation-workflow-prp.md          # Creation workflow
execute-prp 22-exam-creation-prp.md                   # Exam creation
execute-prp 23-exam-publishing-prp.md                 # Exam publishing
execute-prp 25-student-exam-interface-prp.md          # Student interface
execute-prp 26-exam-monitoring-prp.md                 # Monitoring
```

## 5️⃣ Evaluation Layer (27-30)
```bash
# Grading and Results
cd ../05-evaluation
execute-prp 27-manual-grading-prp.md                  # Manual grading
execute-prp 28-auto-grading-prp.md                    # Auto grading
execute-prp 29-results-finalization-prp.md            # Results processing
execute-prp 30-grading-workflow-prp.md                # Grading workflow
```

## 6️⃣ Analytics Layer (31-32)
```bash
# Reporting and Analytics
cd ../06-analytics
execute-prp 31-analytics-dashboard-prp.md             # Analytics dashboard
execute-prp 32-custom-reports-prp.md                  # Custom reports
```

## 🔄 Execution Guidelines

1. **Sequential Execution**: Execute PRPs in numerical order (01-32)
2. **Validation**: Run all tests after each PRP execution
3. **Integration Testing**: Perform integration tests between layers
4. **Documentation**: Update documentation after each major layer completion
5. **Review Points**: Conduct code reviews at layer boundaries

## ⚠️ Important Notes

- Each PRP must pass all validation loops before moving to the next
- Integration tests must pass between layer transitions
- Document any deviations or dependencies discovered during implementation
- Update this execution order if new dependencies are identified
- All duplicate files have been removed and numbering is now sequential (01-32)

## 📋 Layer Completion Checklist

Before moving to the next layer, ensure:
- [ ] All PRPs in current layer executed successfully
- [ ] All unit tests passing
- [ ] Integration tests passing
- [ ] Documentation updated
- [ ] Code review completed
- [ ] No pending critical issues

## 🔍 Validation Commands

Run these commands after each PRP execution:

```bash
# Backend Validation
dotnet build
dotnet test
dotnet format --verify-no-changes

# Frontend Validation
pnpm type-check
pnpm lint
pnpm test

# Integration Testing
pnpm test:integration
```

## 📊 Estimated Timeline

- Foundation Layer (01-14): 3-4 weeks
- Infrastructure Layer (15-16): 1 week
- Content Management (17-19): 2 weeks
- Assessment Layer (20-26): 3-4 weeks
- Evaluation Layer (27-30): 2-3 weeks
- Analytics Layer (31-32): 1 week

Total Estimated Time: 12-16 weeks (32 PRPs)
