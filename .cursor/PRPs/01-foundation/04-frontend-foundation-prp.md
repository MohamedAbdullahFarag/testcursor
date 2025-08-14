# PRP: Frontend Foundation

## Module: Frontend
## Feature: Frontend Foundation (05)

### 🎯 Objective
Establish the React.js frontend base: project structure, global state management, theme, routing, API integration utilities, internationalization (i18next), and shared UI components.

### 📦 Dependencies
- Feature requirements in `ikhtibar-features.txt` (Frontend Foundation section)
- Vite configuration in `vite.config.ts`

### 📝 Context
- Project: `frontend/src`
- Folder-per-feature under `modules/` and shared components under `shared/`
- Existing examples:
  - `.github/copilot/examples/frontend/components/UserCard.tsx`
  - `.github/copilot/examples/frontend/hooks/useUsers.ts`
- Use Tailwind CSS as configured in `tailwind.config.js`

### 🔧 Implementation Tasks
1. **Define TypeScript Interfaces** (`frontend/src/shared/models/index.ts`)
   - `ApiResponse<T>`, `PaginatedResponse<T>`
   - ❌ DON'T: Use `any` type

2. **API Client Setup** (`frontend/src/shared/services/apiClient.ts`)
   - Create `axios` instance with base URL from environment
   - Add interceptors for auth token injection and error handling

3. **Global Store** (`frontend/src/shared/store`)
   - Set up Zustand or Redux Toolkit store for auth and app config
   - Provide hooks: `useAuthStore`, `useAppConfigStore`
   - ❌ DON'T: Put presentation logic in store

4. **Routing** (`frontend/src/App.tsx`)
   - Implement `react-router-dom` configuration
   - Define `PublicRoute`, `PrivateRoute` components for auth-protected pages
   - Load routes from `frontend/src/routes/index.tsx`

5. **Theme & Layout** (`frontend/src/layouts`)
   - Create `ThemeProvider` using Context API
   - Set up dark/light mode toggling
   - Scaffold `DashboardLayout` and `PortalLayout` with common header/footer

6. **Internationalization** (`frontend/src/shared/i18n.ts`)
   - Configure `i18next` with `en` and `ar` namespaces
   - Create `useTranslation` hook wrappers
   - ❌ DON'T: Hardcode strings in components

7. **Shared UI Components** (`frontend/src/shared/components`)
   - Create `Loader`, `Modal`, `Toast`, `ErrorBoundary`
   - Implement accessible ARIA attributes and keyboard support

8. **Custom Hooks** (`frontend/src/shared/hooks`)
   - `useFetch<T>` for API calls with loading/error state
   - `useMediaQuery` for responsive design

9. **Unit Tests** (`frontend/src/shared/__tests__`)
   - Test `apiClient`, store hooks, `useFetch`, and UI components
   - Use React Testing Library and Jest

10. **Code Samples & Examples**
    - Reference `.github/copilot/examples/frontend/components/UserCard.tsx` and `useUsers.ts`

### 🔄 Integration Points
```yaml
CONFIG:
  - frontend:
    - `.env` entries: VITE_API_BASE_URL
    - `tailwind.config.js`: theme extensions

ROUTES:
  - Defined in `src/routes/index.tsx`
  - Public vs Private routes via layout

STYLES:
  - global CSS: `assets/css/shared.css`
  - Tailwind directives in `index.css`

INTERNATIONALIZATION:
  - `src/modules/**/locales/en.ts` and `ar.ts`
```

### 🧪 Validation Loop
```powershell
# Frontend validation
npm install
npm run type-check
npm run lint
npm run test
npm run build
```  

### 🚨 Anti-Patterns to Avoid
```typescript
// ❌ DON'T use any in TypeScript types
// ❌ DON'T bypass lint rules (// eslint-disable)
// ❌ DON'T forget effect cleanup
// ❌ DON'T mix CSS-in-JS with Tailwind
```  

### 📋 Quality Gates
- [ ] All custom hooks tested
- [ ] No TypeScript errors
- [ ] No lint warnings
- [ ] Components accessible and responsive
- [ ] SRP compliance in hooks and components
