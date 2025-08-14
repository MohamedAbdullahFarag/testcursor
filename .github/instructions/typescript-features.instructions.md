---
description: "TypeScript feature implementation with validation loops and context-rich development"
applyTo: "**/*.ts,**/*.tsx,**/tsconfig.json,**/package.json"
---

# TypeScript Feature Implementation Instructions

You are implementing features in the Ikhtibar educational exam management system using TypeScript with React.js 18 frontend and ASP.NET Core 8.0 backend.

## Core Implementation Principles

### Context is King
- Include ALL necessary documentation, examples, and caveats
- Reference existing patterns from the codebase before creating new ones
- Use descriptive variable names and function signatures
- Include examples and patterns directly in code comments

### Validation Loops
- Provide executable tests/lints that can be run and fixed
- Progressive enhancement through iterative refinement
- Each feature includes validation commands
- Self-documenting code with clear intent

### Information Dense
- Use keywords and patterns from the existing codebase
- Follow established naming conventions
- Maintain consistency with project architecture
- Include anti-pattern warnings in comments

## Project Context

### Frontend Stack
- **Framework**: React.js 18 with TypeScript (strict mode)
- **Build Tool**: Vite
- **Styling**: Tailwind CSS
- **State Management**: React Query + Zustand
- **Internationalization**: English/Arabic (RTL/LTR support)
- **Architecture**: Folder-per-feature structure

### Backend Stack
- **Framework**: ASP.NET Core 8.0 Web API
- **ORM**: Dapper (micro-ORM)
- **Database**: SQL Server
- **Authentication**: JWT Bearer tokens
- **Architecture**: Clean Architecture with folder-per-feature

## Implementation Blueprint

### Data Models First
Always start with TypeScript interfaces and types:

```typescript
// Define clear, comprehensive interfaces
export interface FeatureType {
  id: string;
  name: string;
  createdAt: Date;
  updatedAt: Date;
}

// Create request/response types
export interface CreateFeatureRequest {
  name: string;
  description?: string;
}

export interface FeatureResponse {
  data: FeatureType;
  success: boolean;
  message?: string;
}
```

### Component Structure
Follow the established React component patterns:

```typescript
import React, { memo } from 'react';
import { FeatureProps } from '../types/featureTypes';

interface FeatureComponentProps {
  data: FeatureProps;
  onAction?: (id: string) => void;
  className?: string;
}

const FeatureComponent: React.FC<FeatureComponentProps> = memo(({ 
  data, 
  onAction,
  className = '' 
}) => {
  // Implementation with proper error handling
  // Include loading and error states
  // Support both Arabic RTL and English LTR
});

FeatureComponent.displayName = 'FeatureComponent';
export default FeatureComponent;
```

### Custom Hooks Pattern
Create reusable custom hooks:

```typescript
import { useState, useCallback } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';

interface UseFeatureReturn {
  features: FeatureType[];
  isLoading: boolean;
  error: string | null;
  createFeature: (data: CreateFeatureRequest) => Promise<void>;
  updateFeature: (id: string, data: UpdateFeatureRequest) => Promise<void>;
  deleteFeature: (id: string) => Promise<void>;
}

export const useFeature = (): UseFeatureReturn => {
  const queryClient = useQueryClient();
  
  // Implement with proper error handling and optimistic updates
  // Include validation and loading states
  // Use proper TypeScript typing throughout
};
```

## Required Patterns

### Folder Structure
Follow folder-per-feature architecture:

```
src/modules/[feature]/
├── components/     # UI components
├── hooks/          # Custom React hooks  
├── services/       # API integration
├── types/          # TypeScript interfaces
├── utils/          # Helper functions
└── views/          # Page components
```

### Service Layer Pattern
```typescript
import { apiService } from '../../shared/services/apiService';

class FeatureService {
  private readonly baseUrl = '/api/features';

  async getAll(): Promise<FeatureType[]> {
    return apiService.get<FeatureType[]>(this.baseUrl);
  }

  async create(data: CreateFeatureRequest): Promise<FeatureType> {
    return apiService.post<FeatureType>(this.baseUrl, data);
  }

  // Include proper error handling
  // Use consistent response types
  // Implement request/response transformation
}

export const featureService = new FeatureService();
```

## Validation Requirements

### Level 1: Syntax & Style
```bash
# Always run these commands after implementation
npm run type-check     # TypeScript validation
npm run lint          # ESLint checks  
npm run format        # Prettier formatting
```

### Level 2: Testing
```bash
# Create and run tests
npm run test          # Run all tests
npm run test:coverage # Test coverage report
npm run test:watch    # Watch mode for development
```

### Level 3: Integration
```bash
# End-to-end validation
npm run build         # Production build test
npm run preview       # Preview build locally
```

## Anti-Patterns to Avoid

```typescript
// ❌ DON'T: Create components without proper typing
function MyComponent(props: any) {  // Avoid 'any' type

// ❌ DON'T: Forget to handle loading and error states
const { data } = useQuery(['features']); // Missing loading/error

// ❌ DON'T: Use useEffect without cleanup
useEffect(() => {
  const subscription = subscribe();
  // Missing cleanup function
}, []);

// ❌ DON'T: Skip memoization for expensive operations
const expensiveValue = computeExpensiveValue(data); // Should use useMemo

// ❌ DON'T: Ignore accessibility requirements
<button onClick={handleClick}>Click</button> // Missing aria-label

// ❌ DON'T: Hardcode strings (use i18n)
<h1>User Management</h1> // Should use translation key
```

## Integration Points

### API Integration
- Use established API service patterns
- Implement proper error handling
- Include request/response transformation
- Follow consistent response formats

### State Management
- Use React Query for server state
- Use Zustand for client state
- Implement optimistic updates
- Handle loading and error states

### Internationalization
- Support both English and Arabic
- Use translation keys consistently
- Handle RTL/LTR layout switching
- Include proper text direction support

## Success Criteria Checklist

- [ ] TypeScript strict mode compliance (no 'any' types)
- [ ] All components are properly typed with interfaces
- [ ] Custom hooks follow established patterns
- [ ] API services use consistent error handling
- [ ] Components support both RTL and LTR layouts
- [ ] Loading and error states are handled
- [ ] Tests are written and passing
- [ ] Code follows folder-per-feature structure
- [ ] Validation commands execute successfully
- [ ] Integration with existing features is seamless

Always reference existing patterns in the codebase before creating new implementations. When in doubt, follow the established conventions and ask for clarification on project-specific requirements.
