# Code Examples for GitHub Copilot

This folder contains code examples that demonstrate the patterns and structures to follow in our Ikhtibar project.

## 📁 Folder Structure
```
examples/
├── backend/
│   ├── feature-complete/           # Complete feature implementation
│   ├── controllers/                # API controller examples
│   ├── services/                   # Business logic service examples
│   ├── repositories/               # Data access examples
│   ├── models/                     # DTO and entity examples
│   ├── validators/                 # Validation examples
│   └── tests/                      # Unit test examples
├── frontend/
│   ├── feature-complete/           # Complete feature implementation
│   ├── components/                 # React component examples
│   ├── hooks/                      # Custom hook examples
│   ├── services/                   # API service examples
│   ├── types/                      # TypeScript type examples
│   └── tests/                      # Component test examples
└── shared/
    ├── configurations/             # Configuration examples
    ├── middleware/                 # Middleware examples
    └── utilities/                  # Utility function examples
```

## 🎯 How to Use These Examples

### For Backend Development:
- **Controllers**: Use `controllers/` examples for API endpoint patterns
- **Services**: Follow `services/` examples for business logic structure
- **Repositories**: Use `repositories/` examples for data access patterns
- **Models**: Follow `models/` examples for DTO and entity structures
- **Complete Features**: Reference `feature-complete/` for full implementations

### For Frontend Development:
- **Components**: Use `components/` examples for React component patterns
- **Hooks**: Follow `hooks/` examples for custom hook implementations
- **Services**: Use `services/` examples for API communication patterns
- **Types**: Follow `types/` examples for TypeScript type definitions
- **Complete Features**: Reference `feature-complete/` for full implementations

## 📝 Example Usage Instructions

When GitHub Copilot generates code, it should:
1. **Follow the folder-per-feature structure** shown in examples
2. **Use the same naming conventions** as demonstrated
3. **Implement similar error handling patterns**
4. **Follow the same TypeScript/C# typing patterns**
5. **Use consistent styling and formatting**

## 🔄 Keep Examples Updated

- Add new examples when implementing new patterns
- Update existing examples when patterns evolve
- Remove outdated examples that no longer match current standards
- Document any breaking changes or pattern updates

## 🎭 Pattern Categories

### Backend Patterns:
- **CRUD Operations**: Standard create, read, update, delete patterns
- **Authentication**: JWT token handling and authorization
- **Validation**: Input validation with FluentValidation
- **Error Handling**: Global exception handling middleware
- **Logging**: Structured logging patterns
- **Caching**: Memory and distributed caching implementations

### Frontend Patterns:
- **Component Structure**: Functional components with proper TypeScript
- **State Management**: Context API and custom hooks
- **API Integration**: Service layer with proper error handling
- **Form Handling**: Form validation and submission patterns
- **Loading States**: Loading, error, and success state handling
- **Internationalization**: i18next integration patterns

Remember: These examples are the **source of truth** for how code should be structured in our project.
