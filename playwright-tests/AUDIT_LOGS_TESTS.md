# Audit Logs Test Suite

This test suite provides comprehensive coverage for the audit logs functionality to prevent regression of data mapping issues that could cause JavaScript errors like "Cannot read properties of undefined (reading 'items')".

## Test Files Overview

### 1. `audit-logs-data-integrity.spec.ts`
**Purpose**: Core functionality and data integrity validation

**Test Cases**:
- ✅ **Page loads without JavaScript errors** - Monitors console for critical errors
- ✅ **Table renders with correct column headers** - Validates all 7 required columns
- ✅ **Data populates correctly** - Ensures API responses are handled properly
- ✅ **Field mappings are correct** - Validates each cell displays expected data types
- ✅ **Pagination controls work** - Tests pagination navigation and state
- ✅ **API response structure validation** - Validates both wrapped and direct response formats
- ✅ **Filter functionality** - Tests filter controls without errors
- ✅ **Export functionality** - Validates CSV/Excel export capabilities
- ✅ **Frontend-backend mapping consistency** - Ensures data flows correctly from API to UI
- ✅ **Refresh maintains data integrity** - Tests refresh functionality

### 2. `audit-logs-api-contract.spec.ts`
**Purpose**: API contract validation and edge case handling

**Test Cases**:
- ✅ **Both API response formats supported** - Tests wrapped ApiResponse and direct PagedResult formats
- ✅ **Error responses handled gracefully** - Tests 500/401 errors without crashes
- ✅ **Malformed responses don't crash app** - Tests missing properties, null values, invalid types
- ✅ **Property mapping is bulletproof** - Tests legacy property names and malformed items
- ✅ **Pagination edge cases** - Tests zero count, single item, large datasets
- ✅ **Concurrent requests handled properly** - Tests race conditions and multiple requests

### 3. `audit-logs-regression.spec.ts`
**Purpose**: Regression prevention and type safety validation

**Test Cases**:
- ✅ **Prevents regression to legacy property names** - Ensures `id`/`username`/`description` don't work
- ✅ **Verifies correct property mapping** - Confirms `auditLogId`/`userIdentifier`/`details` work correctly
- ✅ **Pagination conversion testing** - Validates 0-based frontend ↔ 1-based backend conversion
- ✅ **TypeScript interface contracts** - Validates data structure matches interfaces
- ✅ **No property access errors** - Monitors for specific property access console errors
- ✅ **Service layer error handling** - Tests timeout, invalid JSON, empty responses

### 4. Enhanced `admin-happy-path.spec.ts`
**Purpose**: Integration with existing admin navigation tests

**Enhancements**:
- ✅ **Specific audit logs validation** - Added detailed checks for audit logs page
- ✅ **Column header verification** - Ensures all required headers are present
- ✅ **JavaScript error monitoring** - Watches for critical console errors
- ✅ **Pagination display validation** - Confirms pagination info renders correctly

## Key Issues Prevented

### 1. **Property Mapping Issues**
- **Original Problem**: Frontend expected `id`, `username`, `description` but backend sends `auditLogId`, `userIdentifier`, `details`
- **Prevention**: Tests verify correct property names work and legacy names fail gracefully

### 2. **API Response Structure Mismatches**
- **Original Problem**: Frontend expected `ApiResponse<PagedResult<T>>` but backend returns `PagedResult<T>` directly
- **Prevention**: Tests validate both response formats are handled correctly

### 3. **Undefined Property Access**
- **Original Problem**: `Cannot read properties of undefined (reading 'items')` JavaScript errors
- **Prevention**: Comprehensive error monitoring and malformed response testing

### 4. **Pagination Index Mismatches**
- **Original Problem**: Frontend uses 0-based pagination but backend expects 1-based
- **Prevention**: Tests validate conversion logic works correctly

## Running the Tests

### Run All Audit Logs Tests
```bash
pnpm exec playwright test -c playwright.config.ts -g "Audit Logs"
```

### Run Individual Test Suites
```bash
# Data integrity tests
pnpm exec playwright test -c playwright.config.ts audit-logs-data-integrity.spec.ts

# API contract tests  
pnpm exec playwright test -c playwright.config.ts audit-logs-api-contract.spec.ts

# Regression tests
pnpm exec playwright test -c playwright.config.ts audit-logs-regression.spec.ts
```

### Using VS Code Tasks
Available tasks in VS Code Command Palette:
- `Playwright: run audit logs tests` - All audit logs test files
- `Playwright: run audit logs data integrity tests` - Core functionality
- `Playwright: run audit logs regression tests` - Regression prevention
- `Playwright: run all audit logs validation tests` - All tests matching "Audit Logs"

## Test Environment Requirements

### Prerequisites
- Frontend running on `https://localhost:5173`
- Backend running on `https://localhost:7001`
- Admin authentication working (`admin@ikhtibar.com`)

### Test Data
Tests use:
- Mocked API responses for controlled testing
- Real API responses for integration testing
- Authentication via localStorage seeding (bypasses login flow)

## Expected Results

### ✅ **Success Criteria**
- All tests pass without failures
- No console errors containing "Cannot read properties of undefined"
- Table renders with correct data structure
- Pagination works correctly
- All API response formats supported

### ❌ **Failure Indicators**
- JavaScript errors in console logs
- Table fails to render
- Data mapping shows "undefined" or "null" values
- Pagination breaks or shows incorrect values
- API responses cause crashes

## Maintenance Notes

### Adding New Tests
When adding new audit logs functionality:

1. **Add data integrity test** - Verify core functionality works
2. **Add API contract test** - Test edge cases and error conditions  
3. **Add regression test** - Prevent specific issues from recurring
4. **Update admin happy-path** - Ensure integration works

### Monitoring for Regressions
Key error patterns to watch for:
- `Cannot read properties of undefined (reading 'items')`
- `Cannot read properties of undefined (reading 'auditLogId')`
- `Cannot read properties of undefined (reading 'userIdentifier')`
- `Cannot read properties of undefined (reading 'details')`

### Backend API Changes
If backend API structure changes:
1. Update response validation tests
2. Verify property mapping tests still pass
3. Update TypeScript interface validation
4. Test both old and new response formats during transition

## Contributing

When modifying audit logs functionality:
1. Run all audit logs tests before committing
2. Add new test cases for new functionality
3. Update this README if test coverage changes
4. Ensure all error scenarios are covered

## Debugging Failed Tests

### Common Issues
1. **Authentication failures** - Check localStorage seeding
2. **API endpoint changes** - Verify routes are correct
3. **Response structure changes** - Update expected schemas
4. **Timing issues** - Adjust wait strategies

### Debug Commands
```bash
# Run with debug output
DEBUG=pw:api pnpm exec playwright test audit-logs-data-integrity.spec.ts

# Run with headed browser  
pnpm exec playwright test audit-logs-data-integrity.spec.ts --headed

# Run specific test
pnpm exec playwright test audit-logs-data-integrity.spec.ts -g "loads without JavaScript errors"
```
