# GitHub Integration Guide for Cursor

This guide explains how to best utilize your existing `.github/` content (instructions, PRPs, and prompts) within Cursor's enhanced AI capabilities.

## **Strategy 1: Convert to Cursor Custom Modes (Recommended)**

The most effective approach is to convert your specialized workflows into Cursor custom modes, which we've already implemented:

### **Available Custom Modes**
- **PRP Specialist**: Handles your 6-phase PRP methodology
- **Instruction Specialist**: Manages your detailed instructions and guidelines
- **Prompt Specialist**: Optimizes AI interactions and prompt engineering
- **Feature Architect**: System design and technical planning
- **Implementation Agent**: Production-ready code implementation
- **Test Engineer**: Comprehensive testing strategy
- **Task Manager**: Task decomposition and project planning
- **Code Reviewer**: Code quality assessment and security review
- **Documentation Specialist**: Technical documentation and user guides
- **DevOps Engineer**: Deployment and infrastructure setup

## **Strategy 2: Direct Reference Integration**

For immediate use without conversion, you can reference your existing `.github/` content directly:

### **Using Instructions**
```markdown
# Reference existing instructions
@.github/instructions/backend-guidelines.instructions.md
@.github/instructions/frontend-guidelines.instructions.md
@.github/instructions/general-rules.instructions.md
```

### **Using PRPs**
```markdown
# Reference existing PRPs
@.github/PRPs/ikhtibar-implementation-strategy.md
@.github/PRPs/prp-execute.md
```

### **Using Prompts**
```markdown
# Reference existing prompts
@.github/prompts/01-quick-start-prps.prompt.md
@.github/prompts/02-codebase-analysis.prompt.md
@.github/prompts/35-test.prompt.md
```

## **Strategy 3: Hybrid Approach**

Combine custom modes with direct references for maximum effectiveness:

### **Phase 1: Strategic Planning**
1. Use **PRP Specialist** custom mode
2. Reference `@.github/PRPs/ikhtibar-implementation-strategy.md`
3. Apply 6-phase methodology

### **Phase 2: Requirements & Guidelines**
1. Use **Instruction Specialist** custom mode
2. Reference relevant instruction files:
   - `@.github/instructions/general-rules.instructions.md`
   - `@.github/instructions/backend-guidelines.instructions.md`
   - `@.github/instructions/frontend-guidelines.instructions.md`

### **Phase 3: AI Interaction Optimization**
1. Use **Prompt Specialist** custom mode
2. Reference existing prompts:
   - `@.github/prompts/02-codebase-analysis.prompt.md`
   - `@.github/prompts/35-test.prompt.md`
   - `@.github/prompts/37-security-analysis.prompt.md`

## **Content Mapping Guide**

### **Instructions → Custom Modes**
| GitHub Instructions | Cursor Custom Mode | Purpose |
|-------------------|-------------------|---------|
| `general-rules.instructions.md` | Instruction Specialist | Core development principles |
| `backend-guidelines.instructions.md` | Backend Guidelines (Rules) | .NET development standards |
| `frontend-guidelines.instructions.md` | Frontend Guidelines (Rules) | React/TypeScript standards |
| `api-guidelines.instructions.md` | Instruction Specialist | API design and documentation |
| `feature-specifications.instructions.md` | Feature Architect | Feature planning and specs |
| `task-management.instructions.md` | Task Manager | Project planning and breakdown |

### **PRPs → Custom Modes**
| GitHub PRPs | Cursor Custom Mode | Purpose |
|-------------|-------------------|---------|
| `ikhtibar-implementation-strategy.md` | PRP Specialist | Overall project strategy |
| `prp-execute.md` | PRP Specialist | PRP execution guidelines |
| Phase directories | PRP Specialist | Phase-specific planning |

### **Prompts → Custom Modes**
| GitHub Prompts | Cursor Custom Mode | Purpose |
|----------------|-------------------|---------|
| `01-quick-start-prps.prompt.md` | PRP Specialist | Rapid project initiation |
| `02-codebase-analysis.prompt.md` | Prompt Specialist | Code analysis prompts |
| `35-test.prompt.md` | Test Engineer | Testing strategy prompts |
| `37-security-analysis.prompt.md` | Code Reviewer | Security review prompts |

## **Implementation Workflow**

### **1. Project Initiation**
```bash
# Use PRP Specialist custom mode
# Reference: @.github/PRPs/ikhtibar-implementation-strategy.md
# Apply: 6-phase methodology
```

### **2. Requirements Analysis**
```bash
# Use Instruction Specialist custom mode
# Reference: @.github/instructions/general-rules.instructions.md
# Apply: Development standards and principles
```

### **3. Architecture Design**
```bash
# Use Feature Architect custom mode
# Reference: @.github/instructions/feature-specifications.instructions.md
# Apply: Feature planning and system design
```

### **4. Development Implementation**
```bash
# Use Implementation Agent custom mode
# Reference: @.github/instructions/backend-guidelines.instructions.md
# Apply: Clean Architecture and coding standards
```

### **5. Quality Assurance**
```bash
# Use Test Engineer custom mode
# Reference: @.github/prompts/35-test.prompt.md
# Apply: Comprehensive testing strategy
```

### **6. Code Review**
```bash
# Use Code Reviewer custom mode
# Reference: @.github/prompts/37-security-analysis.prompt.md
# Apply: Security and quality review
```

## **Best Practices**

### **Immediate Benefits**
1. **Custom Modes**: Specialized expertise for each development phase
2. **Direct References**: Leverage existing, proven content
3. **Enhanced AI**: Better integration with Cursor's capabilities
4. **Consistent Patterns**: Maintain established project standards

### **Long-term Strategy**
1. **Gradual Migration**: Convert more content to custom modes over time
2. **Content Evolution**: Update and improve existing content
3. **Pattern Recognition**: Identify successful patterns for automation
4. **Team Adoption**: Train team on new custom modes

### **Content Maintenance**
1. **Version Control**: Keep `.github/` content in sync with custom modes
2. **Regular Updates**: Update both systems with new patterns
3. **Feedback Loop**: Use AI interactions to improve content
4. **Documentation**: Maintain clear mapping between systems

## **Migration Checklist**

- [ ] Import all custom modes into Cursor
- [ ] Test custom modes with existing content
- [ ] Identify content gaps and opportunities
- [ ] Plan gradual migration strategy
- [ ] Train team on new workflow
- [ ] Monitor effectiveness and iterate

## **Success Metrics**

- **Efficiency**: Faster development cycles with specialized modes
- **Quality**: Better code quality through focused expertise
- **Consistency**: Maintained project standards and patterns
- **Innovation**: Enhanced AI capabilities for complex tasks
- **Team Productivity**: Improved developer experience and output

Remember: The goal is to enhance your existing workflow, not replace it. Use custom modes to amplify your proven methodologies while maintaining the valuable content you've already created.
