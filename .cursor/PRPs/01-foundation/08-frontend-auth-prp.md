---
feature: "frontend-auth"
module: "authentication"
order: "05"
description: |
  Implement login/logout and token refresh flows on the frontend, including UI components, hooks,
  context provider, route guarding, secure token storage, and i18n support.
---

## Goal
Build a robust authentication module on the frontend with login form, auth context, secure token handling, and protected routes.

## Why
- **Business value:** Secure access control and seamless user experience.
- **Integration:** Connects to `Ikhtibar.API/api/auth/login`, `/refresh`, `/logout`.
- **Problem solved:** Centralizes auth logic, avoids mixing auth concerns in pages.

## What
- `<LoginForm>` component with validation and error display.
- `AuthProvider` context and `useAuth` hook exposing `(user, accessToken, login, logout)`.
- `authService` for POST `/login`, `/refresh`, `/logout`.
- `<PrivateRoute>` component for guarding protected routes.
- Secure token storage (in-memory or httpOnly cookies).
- Automatic token refresh on 401.
- i18n support (English + Arabic) for labels and messages.

### Success Criteria
- [ ] LoginForm validates inputs and calls `login`.
- [ ] `useAuth` sets and exposes auth state (user, token).
- [ ] Protected routes redirect unauthenticated users to `/login`.
- [ ] Token refresh logic triggers on 401 and retries request.
- [ ] Code passes: `npm run type-check`, `npm run lint`, `npm run test`, `npm run build`.

## All Needed Context
```yaml
- file: .cursor/copilot/requirements/ikhtibar-features.txt
  why: Defines authentication requirements
- file: frontend/src/modules/auth/models/auth.types.ts
  why: Data types for login and auth result
- file: .cursor/copilot/examples/frontend/components/LoginForm.tsx
  why: Login component pattern
- file: .cursor/copilot/examples/frontend/hooks/useUsers.ts
  why: Hook pattern for login flow
- file: .cursor/copilot/frontend-guidelines.md
  why: Naming and folder-per-feature rules
``` 

## Implementation Blueprint

### 1. Models & Types
Create `frontend/src/modules/auth/models/auth.types.ts`:
```typescript
export interface LoginRequest { email: string; password: string; }
export interface AuthResult { accessToken: string; refreshToken: string; user: { id: string; fullName: string; email: string; roles: string[]; }; }
```

### 2. Service Layer
Create `frontend/src/modules/auth/services/authService.ts`:
```typescript
const API = import.meta.env.VITE_API_BASE_URL;
export const authService = {
  login: (data: LoginRequest) =>
    fetch(`${API}/api/auth/login`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(data) })
      .then(res => { if (!res.ok) throw new Error('Invalid credentials'); return res.json() as Promise<AuthResult>; }),
  refresh: () =>
    fetch(`${API}/api/auth/refresh`, { method: 'POST', credentials: 'include' }).then(res => res.json() as Promise<AuthResult>),
  logout: () =>
    fetch(`${API}/api/auth/logout`, { method: 'POST', credentials: 'include' }).then(res => { if (!res.ok) throw new Error('Logout failed'); }),
};
```

### 3. Auth Context & Hook
Create `frontend/src/modules/auth/hooks/useAuth.ts`:
```typescript
import React, { createContext, useContext, useState, useCallback, useEffect } from 'react';
export const AuthContext = createContext<any>(null);
export const AuthProvider: React.FC = ({ children }) => {
  const [user, setUser] = useState(null);
  const [token, setToken] = useState<string | null>(null);

  const login = useCallback(async creds => {
    const result = await authService.login(creds);
    setUser(result.user);
    setToken(result.accessToken);
  }, []);

  const logout = useCallback(async () => { await authService.logout(); setUser(null); setToken(null); }, []);

  // Automatic refresh on mount or 401 handling can be added here

  return <AuthContext.Provider value={{ user, accessToken: token, login, logout }}>{children}</AuthContext.Provider>;
};
export const useAuth = () => { const ctx = useContext(AuthContext); if (!ctx) throw new Error('useAuth must be used within AuthProvider'); return ctx; };
```

### 4. LoginForm Component
Create `frontend/src/modules/auth/components/LoginForm.tsx`:
```tsx
import React, { useState } from 'react';
export const LoginForm: React.FC = memo(() => {
  const { login } = useAuth();
  const [data, setData] = useState<LoginRequest>({ email: '', password: '' });
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e) => { e.preventDefault(); try { await login(data); } catch (e) { setError(e.message); } };

  return (
    <form onSubmit={handleSubmit}>
      {/* inputs for email and password, submit button, error display */}
    </form>
  );
});
```

### 5. PrivateRoute
Create `frontend/src/shared/components/PrivateRoute.tsx`:
```tsx
import React from 'react';
import { Navigate } from 'react-router-dom';
export const PrivateRoute: React.FC<{ children: JSX.Element }> = ({ children }) => {
  const { accessToken } = useAuth();
  return accessToken ? children : <Navigate to="/login" />;
};
```

### 6. i18n Locales
Add `frontend/src/modules/auth/locales/en.ts` and `ar.ts` with: `email`, `password`, `login`, `logout`, `invalidCredentials`.

## Integration Points
```yaml
CONFIG:
  - frontend: "VITE_API_BASE_URL in .env"
ROUTES:
  - frontend: "Add '/login' in PortalRoutes and wrap protected routes"
NAVIGATION:
  - portal: "Show login/logout buttons"
SECURITY:
  - storage: "Use httpOnly cookie or in-memory"
INTERNATIONALIZATION:
  - translations: en/ar files for auth
``` 

## Validation Loop
### Level 1
```powershell
npm run type-check
npm run lint
npm run test
```
### Level 2
Add unit tests for `useAuth` and `LoginForm` under `__tests__`.
### Level 3
Manual browser test: login, navigate, refresh, logout redirects.
