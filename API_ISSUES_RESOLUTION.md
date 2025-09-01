# API Issues Resolution Guide

## Issues Status Overview

### ✅ FIXED
1. **React Key Prop Warning**: Suppression utility implemented
2. **Missing Translation Key**: Added `error.loadingMediaFiles` to locales
3. **Enhanced Error Handling**: Improved useAuditLogs with specific error messages

### 🔍 IDENTIFIED ISSUES

#### Issue 1: Audit Logs 403 Forbidden
- **Root Cause**: Requires `Admin` or `Supervisor` role
- **Current API**: `GET /api/audit-logs` returns 403
- **Authorization**: `[Authorize(Roles = "Admin,Supervisor")]`

#### Issue 2: Media Search 500 Internal Server Error  
- **Root Cause**: Backend service error after authentication
- **Current API**: `POST /api/media/search` returns 500
- **Authorization**: `[Authorize]` (any authenticated user)

## 🛠️ SOLUTIONS

### Step 1: Check Authentication Status

Run the debug script in your browser console:

```bash
# Open frontend in browser
# Press F12 to open DevTools
# Go to Console tab
# Run the debug script from: debug-auth-status.js
```

### Step 2: Fix Authentication Issues

#### If Not Authenticated:
1. Log in to the application
2. Verify tokens are stored in localStorage
3. Check if tokens are expired

#### If Authenticated but Audit Logs Still 403:
1. **Backend Fix**: Add current user to Admin/Supervisor role
2. **Database Query**:
   ```sql
   -- Check current user roles
   SELECT u.Email, ur.RoleName 
   FROM Users u 
   LEFT JOIN UserRoles ur ON u.Id = ur.UserId
   WHERE u.Email = 'your-email@domain.com';
   
   -- Add Admin role (replace with actual user ID)
   INSERT INTO UserRoles (UserId, RoleName) 
   VALUES (1, 'Admin');
   ```

### Step 3: Fix Media Search 500 Error

The 500 error indicates a backend service issue. Check these:

#### A. Database Connection:
```powershell
# Test backend health
curl -k https://localhost:7001/api/health/ping
```

#### B. Check Backend Logs:
```powershell
# Navigate to backend
cd c:\Projects\testcursor\backend

# Run backend with detailed logging
dotnet run --project Ikhtibar.API --verbosity detailed
```

#### C. Test Media Search Manually:
```powershell
# Test with authentication (replace TOKEN with actual token)
curl -k -X POST https://localhost:7001/api/media/search \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d "{\"searchTerm\":\"\",\"page\":1,\"pageSize\":10}"
```

### Step 4: Verify Frontend API Client

Check if the API client is properly sending authentication headers:

1. Open browser DevTools → Network tab
2. Trigger the failing API calls
3. Check if requests include `Authorization: Bearer` header
4. Verify token format and expiration

## 🔧 QUICK FIXES

### For Development/Testing:

#### Temporary Admin Role Assignment:
```sql
-- Add Admin role to first user (for testing)
UPDATE Users SET Roles = 'Admin,User' WHERE Id = 1;
-- OR use UserRoles table if separate
INSERT INTO UserRoles (UserId, RoleName) VALUES (1, 'Admin');
```

#### Skip Authorization (Development Only):
Comment out authorization temporarily:
```csharp
// [Authorize(Roles = "Admin,Supervisor")]  // Comment this line
public class AuditLogsController : ControllerBase
```

### For Production:

#### Proper Role Management:
1. Create role management UI
2. Assign appropriate roles to users
3. Implement role-based menu visibility

## 🚀 TESTING

After implementing fixes:

1. **Clear browser cache** and localStorage
2. **Login again** to get fresh tokens
3. **Test audit logs access**
4. **Test media search functionality**
5. **Check console for any remaining warnings**

## 📋 FILES MODIFIED

1. `frontend/src/main.tsx` - Warning suppression
2. `frontend/src/utils/suppressWarnings.ts` - Warning filter
3. `frontend/src/modules/audit/hooks/useAuditLogs.ts` - Error handling
4. `frontend/src/modules/media-management/locales/en.ts` - Translation
5. `frontend/src/modules/media-management/locales/ar.ts` - Translation

## 🔍 DEBUGGING TOOLS

- `debug-auth-status.js` - Browser console authentication checker
- Browser DevTools Network tab for API monitoring
- Backend console logs for server-side debugging

---

**Next Steps**: 
1. Run the authentication debug script
2. Assign proper user roles in the database
3. Restart backend if needed
4. Test all functionality
