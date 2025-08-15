---
feature: "frontend-components"
module: "user-management"
order: "08"
description: |
  Build a comprehensive set of React components, hooks, services, models, views and translations
  to support user management CRUD operations (list, create, edit, delete) with search, paging,
  loading/error states, and i18n support (English + Arabic).
---

## Goal
Implement frontend components for the User Management feature in Ikhtibar.

## Why
- **Business value:** Enables admins to manage user accounts (CRUD) directly in the UI.
- **Integration:** Consumes the existing `Ikhtibar.API/api/users` endpoints.
- **Problem solved:** Provides a feature-rich user interface consistent with project patterns.

## What
- User listing with pagination and search/filter.
- User form to create or update (fullName, email, roles, isActive).
- Delete confirmation flow.
- Loading, error, empty states.
- React hook `useUserManagement` for data logic.
- Service `userService` for API calls.
- TypeScript interfaces for User, CreateUserRequest, UpdateUserRequest.
- English and Arabic translation files.

### Success Criteria
- [ ] `UserList.tsx` renders table with correct props.
- [ ] `UserForm.tsx` handles validation and submit.
- [ ] `useUserManagement` hook exposes `(users, total, loading, error, load, create, update, remove)`.
- [ ] `userService` implements async calls to GET/POST/PUT/DELETE `/api/users`.
- [ ] i18n keys present in `en.ts` and `ar.ts`.
- [ ] Code passes: `npm run type-check`, `npm run lint`, `npm run test`, `npm run build`.

## All Needed Context
```yaml
- file: .cursor/copilot/requirements/ikhtibar-features.txt
  why: Defines user-management feature requirements
- file: frontend/src/shared/models/user.types.ts
  why: Existing User interface patterns
- file: .cursor/copilot/examples/frontend/components/UserCard.tsx
  why: List component pattern
- file: .cursor/copilot/examples/frontend/hooks/useUsers.ts
  why: Hook pattern for fetch logic
- file: .cursor/copilot/frontend-guidelines.md
  why: Naming and folder-per-feature rules
``` 

## Implementation Blueprint

### 1. TypeScript Interfaces
Create `frontend/src/modules/user-management/models/user.types.ts`:
```typescript
export interface User {
  id: string;
  fullName: string;
  email: string;
  roles: string[];
  isActive: boolean;
  createdAt: string;
}
export interface CreateUserRequest { fullName: string; email: string; password: string; roles: string[]; }
export interface UpdateUserRequest { id: string; fullName?: string; email?: string; roles?: string[]; isActive?: boolean; }
```

> ❌ **Anti-Patterns:** Avoid `any`, mixing UI and business logic.

### 2. Service Layer
Create `frontend/src/modules/user-management/services/userService.ts`:
```typescript
const API = import.meta.env.VITE_API_BASE_URL;
export const userService = {
  list: async (page = 1, filter = '') => {
    const res = await fetch(`${API}/api/users?page=${page}&filter=${filter}`);
    if (!res.ok) throw new Error('Failed to load users');
    return res.json() as Promise<{ items: User[]; total: number }>;
  },
  create: (data: CreateUserRequest) =>
    fetch(`${API}/api/users`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(data) }),
  update: data =>
    fetch(`${API}/api/users/${data.id}`, { method: 'PUT', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(data) }),
  remove: id => fetch(`${API}/api/users/${id}`, { method: 'DELETE' }),
};
``` 

### 3. Custom Hook
Create `frontend/src/modules/user-management/hooks/useUserManagement.ts`:
```typescript
import { useState, useEffect, useCallback } from 'react';
export const useUserManagement = () => {
  const [users, setUsers] = useState<User[]>([]);
  const [total, setTotal] = useState(0);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async (page = 1, filter = '') => {
    setLoading(true); setError(null);
    try {
      const { items, total } = await userService.list(page, filter);
      setUsers(items); setTotal(total);
    } catch (e) { setError((e as Error).message); }
    finally { setLoading(false); }
  }, []);

  const create = useCallback(async data => { await userService.create(data); await load(); }, [load]);
  const update = useCallback(async data => { await userService.update(data); await load(); }, [load]);
  const remove = useCallback(async id => { await userService.remove(id); await load(); }, [load]);

  useEffect(() => { load(); }, [load]);
  return { users, total, loading, error, load, create, update, remove };
};
```

### 4. Components
- `UserList.tsx`: shows loading/error/empty, table of users, edit/delete callbacks.
- `UserForm.tsx`: form fields, validation, submit handler.

### 5. View
`UserManagementView.tsx` uses `useUserManagement`, toggles list and form.

### 6. Internationalization
Add `frontend/src/modules/user-management/locales/en.ts` and `ar.ts` with keys: `users`, `noUsers`, `loading`, `create`, `edit`, `delete`, etc.

## Integration Points
```yaml
DATABASE:
  - migration: "Add Users table"
CONFIG:
  - frontend: "VITE_API_BASE_URL in .env"
ROUTES:
  - frontend: "Add '/users' in DashboardRoutes"
NAVIGATION:
  - dashboard: "Add 'User Management' link"
INTERNATIONALIZATION:
  - translations: en/ar files
  - rtl: ensure layout flips
``` 

## Validation Loop
### Level 1
```powershell
npm run type-check
npm run lint
npm run test
npm run build
```
### Level 2
Create unit tests for hook and components under `__tests__`.
### Level 3
```bash
curl -X GET http://localhost:5000/api/users
```
