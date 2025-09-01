import { test, expect } from '@playwright/test'

async function loginByStorage(page: import('@playwright/test').Page) {
  await page.addInitScript(() => {
    const base64UrlEncode = (obj: any) => {
      const json = JSON.stringify(obj)
      const base64 = btoa(unescape(encodeURIComponent(json)))
      return base64.replace(/=/g, '').replace(/\+/g, '-').replace(/\//g, '_')
    }
    const createFakeJwt = (validMinutes = 60) => {
      const header = { alg: 'HS256', typ: 'JWT' }
      const now = Math.floor(Date.now() / 1000)
      const payload = { exp: now + validMinutes * 60, iat: now, sub: '10', email: 'admin@ikhtibar.com', roles: ['system-admin'] }
      return `${base64UrlEncode(header)}.${base64UrlEncode(payload)}.signature`
    }

    const accessToken = createFakeJwt(120)
    const refreshToken = createFakeJwt(7 * 24 * 60)
    const state = {
      user: {
        id: '10',
        fullName: 'Super Administrator',
        email: 'admin@ikhtibar.com',
        roles: ['system-admin'],
      },
      accessToken,
      refreshToken,
      isAuthenticated: true,
    }
    localStorage.setItem('auth-storage', JSON.stringify({ state, version: 0 }))
  })
  await page.goto('/dashboard', { waitUntil: 'domcontentloaded' })
}

test.describe('Audit Logs API Contract Tests', () => {
  test.beforeEach(async ({ page }) => {
    await loginByStorage(page)
  })

  test('API response handles both wrapped and direct PagedResult formats', async ({ page }) => {
    const responses: any[] = []

    // Mock different response formats to test compatibility
    await page.route('**/api/audit-logs**', async (route) => {
      const url = route.request().url()
      const originalResponse = await route.fetch()
      const originalData = await originalResponse.json()
      
      // Test direct PagedResult format (current backend behavior)
      const directFormat = {
        items: originalData.items || [],
        totalCount: originalData.totalCount || 0,
        pageNumber: originalData.pageNumber || 1,
        pageSize: originalData.pageSize || 25,
        totalPages: originalData.totalPages || 0,
        hasNextPage: originalData.hasNextPage || false,
        hasPreviousPage: originalData.hasPreviousPage || false
      }
      
      responses.push(directFormat)
      
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(directFormat)
      })
    })

    await page.goto('/dashboard/audit-logs', { waitUntil: 'domcontentloaded' })
    await page.waitForLoadState('networkidle')

    // Verify page loads without errors with direct format
    await expect(page.locator('table')).toBeVisible()
    await expect(page.locator('tbody tr').first()).toBeVisible()
    
    // Now test wrapped ApiResponse format
    await page.route('**/api/audit-logs**', async (route) => {
      const originalResponse = await route.fetch()
      const originalData = await originalResponse.json()
      
      // Test wrapped ApiResponse format
      const wrappedFormat = {
        success: true,
        message: 'Success',
        data: {
          items: originalData.items || [],
          totalCount: originalData.totalCount || 0,
          pageNumber: originalData.pageNumber || 1,
          pageSize: originalData.pageSize || 25,
          totalPages: originalData.totalPages || 0,
          hasNextPage: originalData.hasNextPage || false,
          hasPreviousPage: originalData.hasPreviousPage || false
        }
      }
      
      responses.push(wrappedFormat)
      
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(wrappedFormat)
      })
    })

    // Refresh to test wrapped format
    await page.reload({ waitUntil: 'domcontentloaded' })
    await page.waitForLoadState('networkidle')

    // Verify page still loads without errors with wrapped format
    await expect(page.locator('table')).toBeVisible()
    await expect(page.locator('tbody tr').first()).toBeVisible()
  })

  test('API error responses are handled gracefully', async ({ page }) => {
    // Test 500 server error
    await page.route('**/api/audit-logs**', async (route) => {
      await route.fulfill({
        status: 500,
        contentType: 'application/json',
        body: JSON.stringify({ message: 'Internal Server Error' })
      })
    })

    await page.goto('/dashboard/audit-logs', { waitUntil: 'domcontentloaded' })
    await page.waitForLoadState('networkidle')

    // Should show error message or empty state, not crash
    const hasErrorMessage = await page.locator('text=/error|failed|something went wrong/i').count() > 0
    const hasEmptyState = await page.locator('text=/no.*logs|no.*data/i').count() > 0
    const hasTable = await page.locator('table').count() > 0

    expect(hasErrorMessage || hasEmptyState || hasTable).toBe(true)

    // Test 401 unauthorized
    await page.route('**/api/audit-logs**', async (route) => {
      await route.fulfill({
        status: 401,
        contentType: 'application/json',
        body: JSON.stringify({ message: 'Unauthorized' })
      })
    })

    await page.reload({ waitUntil: 'domcontentloaded' })
    
    // Should handle unauthorized gracefully (might redirect to login or show error)
    // The important thing is no JavaScript crashes
    await page.waitForTimeout(2000)
    
    const criticalErrors: string[] = []
    page.on('console', (msg) => {
      if (msg.type() === 'error' && (
        msg.text().includes('Cannot read properties of undefined') ||
        msg.text().includes('TypeError') ||
        msg.text().includes('ReferenceError')
      )) {
        criticalErrors.push(msg.text())
      }
    })

    expect(criticalErrors).toHaveLength(0)
  })

  test('Malformed API responses are handled without crashes', async ({ page }) => {
    const testCases = [
      // Missing items property
      { totalCount: 100, pageNumber: 1, pageSize: 25 },
      // Null items
      { items: null, totalCount: 100, pageNumber: 1, pageSize: 25 },
      // Items is not an array
      { items: 'not an array', totalCount: 100, pageNumber: 1, pageSize: 25 },
      // Missing pagination properties
      { items: [] },
      // Invalid pagination values
      { items: [], totalCount: 'invalid', pageNumber: 'invalid', pageSize: 'invalid' },
      // Empty response
      {},
      // Null response
      null,
    ]

    for (let i = 0; i < testCases.length; i++) {
      const testCase = testCases[i]
      
      await page.route('**/api/audit-logs**', async (route) => {
        await route.fulfill({
          status: 200,
          contentType: 'application/json',
          body: JSON.stringify(testCase)
        })
      })

      await page.goto('/dashboard/audit-logs', { waitUntil: 'domcontentloaded' })
      await page.waitForLoadState('networkidle')

      // Capture any JavaScript errors
      const errors: string[] = []
      page.on('console', (msg) => {
        if (msg.type() === 'error') {
          errors.push(msg.text())
        }
      })

      // Wait for any async errors
      await page.waitForTimeout(2000)

      // Should not crash with critical errors
      const criticalErrors = errors.filter(error => 
        error.includes('Cannot read properties of undefined') ||
        error.includes('reading \'items\'') ||
        error.includes('TypeError') ||
        error.includes('ReferenceError')
      )

      expect(criticalErrors, `Test case ${i + 1} should not cause critical errors: ${JSON.stringify(testCase)}`).toHaveLength(0)

      // Page should still render basic structure
      await expect(page.locator('table')).toBeVisible()
    }
  })

  test('Audit log item property mapping is bulletproof', async ({ page }) => {
    // Test with items that have missing or incorrect properties
    const malformedItems = [
      // Missing auditLogId (old 'id' property)
      {
        userIdentifier: 'test-user',
        details: 'test action',
        timestamp: '2025-09-01T10:00:00Z',
        action: 'TEST',
        entityType: 'Test',
        severity: 1,
        category: 1,
        ipAddress: '127.0.0.1'
      },
      // Missing userIdentifier (old 'username' property)  
      {
        auditLogId: 1,
        details: 'test action',
        timestamp: '2025-09-01T10:00:00Z',
        action: 'TEST',
        entityType: 'Test',
        severity: 1,
        category: 1,
        ipAddress: '127.0.0.1'
      },
      // Missing details (old 'description' property)
      {
        auditLogId: 2,
        userIdentifier: 'test-user',
        timestamp: '2025-09-01T10:00:00Z',
        action: 'TEST',
        entityType: 'Test',
        severity: 1,
        category: 1,
        ipAddress: '127.0.0.1'
      },
      // Item with legacy property names (should not work)
      {
        id: 3, // Legacy property name
        username: 'legacy-user', // Legacy property name
        description: 'legacy description', // Legacy property name
        timestamp: '2025-09-01T10:00:00Z',
        action: 'TEST',
        entityType: 'Test',
        severity: 1,
        category: 1,
        ipAddress: '127.0.0.1'
      },
      // Completely malformed item
      {
        randomProperty: 'random value',
        anotherProperty: 123
      }
    ]

    await page.route('**/api/audit-logs**', async (route) => {
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          items: malformedItems,
          totalCount: malformedItems.length,
          pageNumber: 1,
          pageSize: 25,
          totalPages: 1,
          hasNextPage: false,
          hasPreviousPage: false
        })
      })
    })

    await page.goto('/dashboard/audit-logs', { waitUntil: 'domcontentloaded' })
    await page.waitForLoadState('networkidle')

    // Verify table renders without crashes
    await expect(page.locator('table')).toBeVisible()
    
    // Check that rows are rendered (even if with missing data)
    const tableRows = page.locator('tbody tr')
    const rowCount = await tableRows.count()
    expect(rowCount).toBe(malformedItems.length)

    // Verify no critical JavaScript errors
    const errors: string[] = []
    page.on('console', (msg) => {
      if (msg.type() === 'error') {
        errors.push(msg.text())
      }
    })

    await page.waitForTimeout(2000)

    const mappingErrors = errors.filter(error => 
      error.includes('Cannot read properties of undefined') ||
      error.includes('reading \'items\'') ||
      error.includes('auditLogId') ||
      error.includes('userIdentifier') ||
      error.includes('details')
    )

    expect(mappingErrors).toHaveLength(0)

    // Verify cells don't display 'undefined' or 'null'
    for (let i = 0; i < rowCount; i++) {
      const row = tableRows.nth(i)
      const cells = row.locator('td')
      const cellCount = await cells.count()
      
      for (let j = 0; j < cellCount; j++) {
        const cellText = await cells.nth(j).textContent()
        expect(cellText).not.toBe('undefined')
        expect(cellText).not.toBe('null')
        expect(cellText).not.toBeNull()
      }
    }
  })

  test('Pagination edge cases are handled correctly', async ({ page }) => {
    // Test with zero total count
    await page.route('**/api/audit-logs**', async (route) => {
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          items: [],
          totalCount: 0,
          pageNumber: 1,
          pageSize: 25,
          totalPages: 0,
          hasNextPage: false,
          hasPreviousPage: false
        })
      })
    })

    await page.goto('/dashboard/audit-logs', { waitUntil: 'domcontentloaded' })
    await page.waitForLoadState('networkidle')

    // Should show empty state or zero count
    const paginationText = await page.locator('text=/\\d+-\\d+ of \\d+/').textContent()
    expect(paginationText).toContain('0')

    // Test with single item
    await page.route('**/api/audit-logs**', async (route) => {
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          items: [{
            auditLogId: 1,
            userIdentifier: 'single-user',
            details: 'single action',
            timestamp: '2025-09-01T10:00:00Z',
            action: 'TEST',
            entityType: 'Test',
            severity: 1,
            category: 1,
            ipAddress: '127.0.0.1'
          }],
          totalCount: 1,
          pageNumber: 1,
          pageSize: 25,
          totalPages: 1,
          hasNextPage: false,
          hasPreviousPage: false
        })
      })
    })

    await page.reload({ waitUntil: 'domcontentloaded' })
    await page.waitForLoadState('networkidle')

    // Should show correct pagination for single item
    const singleItemPagination = await page.locator('text=/1-1 of 1/').count()
    expect(singleItemPagination).toBeGreaterThan(0)

    // Test with very large total count
    await page.route('**/api/audit-logs**', async (route) => {
      const largeDataset = Array.from({ length: 25 }, (_, i) => ({
        auditLogId: i + 1,
        userIdentifier: `user-${i + 1}`,
        details: `action ${i + 1}`,
        timestamp: '2025-09-01T10:00:00Z',
        action: 'TEST',
        entityType: 'Test',
        severity: 1,
        category: 1,
        ipAddress: '127.0.0.1'
      }))

      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify({
          items: largeDataset,
          totalCount: 999999,
          pageNumber: 1,
          pageSize: 25,
          totalPages: 39999,
          hasNextPage: true,
          hasPreviousPage: false
        })
      })
    })

    await page.reload({ waitUntil: 'domcontentloaded' })
    await page.waitForLoadState('networkidle')

    // Should handle large numbers without crashing
    const largePagination = await page.locator('text=/1-25 of 999999/').count()
    expect(largePagination).toBeGreaterThan(0)
  })

  test('Concurrent API requests are handled properly', async ({ page }) => {
    let requestCount = 0
    
    await page.route('**/api/audit-logs**', async (route) => {
      requestCount++
      
      // Simulate network delay
      await new Promise(resolve => setTimeout(resolve, 100))
      
      const testData = {
        items: [{
          auditLogId: requestCount,
          userIdentifier: `user-${requestCount}`,
          details: `concurrent request ${requestCount}`,
          timestamp: '2025-09-01T10:00:00Z',
          action: 'CONCURRENT_TEST',
          entityType: 'Test',
          severity: 1,
          category: 1,
          ipAddress: '127.0.0.1'
        }],
        totalCount: 1,
        pageNumber: 1,
        pageSize: 25,
        totalPages: 1,
        hasNextPage: false,
        hasPreviousPage: false
      }
      
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(testData)
      })
    })

    await page.goto('/dashboard/audit-logs', { waitUntil: 'domcontentloaded' })
    await page.waitForLoadState('networkidle')

    // Trigger multiple concurrent requests (refresh multiple times quickly)
    const refreshButton = page.locator('button').filter({ hasText: /refresh/i })
    if (await refreshButton.count() > 0) {
      // Click refresh multiple times rapidly
      await Promise.all([
        refreshButton.click(),
        refreshButton.click(),
        refreshButton.click()
      ])
      
      await page.waitForLoadState('networkidle')
      
      // Should not crash and should display data
      await expect(page.locator('table')).toBeVisible()
      await expect(page.locator('tbody tr').first()).toBeVisible()
    }

    // Verify no race condition errors
    const errors: string[] = []
    page.on('console', (msg) => {
      if (msg.type() === 'error') {
        errors.push(msg.text())
      }
    })

    await page.waitForTimeout(2000)

    const raceConditionErrors = errors.filter(error => 
      error.includes('Cannot read properties of undefined') ||
      error.includes('race condition') ||
      error.includes('concurrent')
    )

    expect(raceConditionErrors).toHaveLength(0)
  })
})
