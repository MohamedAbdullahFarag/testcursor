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

test.describe('Audit Logs Type Safety and Regression Tests', () => {
  test.beforeEach(async ({ page }) => {
    await loginByStorage(page)
  })

  test('Prevents regression to legacy property names', async ({ page }) => {
    // Mock API response with legacy property names that should NOT work
    await page.route('**/api/audit-logs**', async (route) => {
      const legacyResponse = {
        items: [
          {
            // Legacy property names that caused the original bug
            id: 1, // Should be 'auditLogId'
            username: 'legacy-user', // Should be 'userIdentifier'  
            description: 'legacy description', // Should be 'details'
            timestamp: '2025-09-01T10:00:00Z',
            action: 'LEGACY_TEST',
            entityType: 'Test',
            severity: 1,
            category: 1,
            ipAddress: '127.0.0.1'
          }
        ],
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
        body: JSON.stringify(legacyResponse)
      })
    })

    await page.goto('/dashboard/audit-logs', { waitUntil: 'domcontentloaded' })
    await page.waitForLoadState('networkidle')

    // Should render table but with empty/default values for mapped fields
    await expect(page.locator('table')).toBeVisible()
    
    const firstRow = page.locator('tbody tr').first()
    await expect(firstRow).toBeVisible()
    
    // The cells should not display the legacy property values
    // because the mapping expects the correct property names
    const cells = firstRow.locator('td')
    
    // User cell (index 1) should not contain 'legacy-user' 
    // because it expects 'userIdentifier' not 'username'
    const userCell = await cells.nth(1).textContent()
    expect(userCell).not.toContain('legacy-user')
    
    // Should not cause JavaScript errors despite wrong property names
    const errors: string[] = []
    page.on('console', (msg) => {
      if (msg.type() === 'error') {
        errors.push(msg.text())
      }
    })

    await page.waitForTimeout(2000)

    const criticalErrors = errors.filter(error => 
      error.includes('Cannot read properties of undefined') ||
      error.includes('TypeError')
    )

    expect(criticalErrors).toHaveLength(0)
  })

  test('Verifies correct property mapping with proper backend format', async ({ page }) => {
    // Mock API response with CORRECT property names
    await page.route('**/api/audit-logs**', async (route) => {
      const correctResponse = {
        items: [
          {
            // Correct property names that should work
            auditLogId: 123,
            userIdentifier: 'correct-user',
            details: 'correct details',
            timestamp: '2025-09-01T10:00:00Z',
            action: 'CORRECT_TEST',
            entityType: 'Test',
            severity: 2,
            category: 3,
            ipAddress: '192.168.1.1'
          }
        ],
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
        body: JSON.stringify(correctResponse)
      })
    })

    await page.goto('/dashboard/audit-logs', { waitUntil: 'domcontentloaded' })
    await page.waitForLoadState('networkidle')

    await expect(page.locator('table')).toBeVisible()
    
    const firstRow = page.locator('tbody tr').first()
    await expect(firstRow).toBeVisible()
    
    const cells = firstRow.locator('td')
    
    // Verify correct mapping of each field
    // User cell should contain the correct user identifier
    const userCell = await cells.nth(1).textContent()
    expect(userCell).toContain('correct-user')
    
    // Action cell should contain the correct action
    const actionCell = await cells.nth(2).textContent()
    expect(actionCell).toContain('CORRECT_TEST')
    
    // Entity cell should contain the correct entity type
    const entityCell = await cells.nth(3).textContent()
    expect(entityCell).toContain('Test')
    
    // Severity cell should contain the correct severity
    const severityCell = await cells.nth(4).textContent()
    expect(severityCell).toContain('2')
    
    // Category cell should contain the correct category
    const categoryCell = await cells.nth(5).textContent()
    expect(categoryCell).toContain('3')
    
    // IP cell should contain the correct IP address
    const ipCell = await cells.nth(6).textContent()
    expect(ipCell).toContain('192.168.1.1')
  })

  test('Tests pagination conversion between 0-based and 1-based indexing', async ({ page }) => {
    let capturedPageNumber: number | null = null
    
    await page.route('**/api/audit-logs**', async (route) => {
      const url = new URL(route.request().url())
      const page = url.searchParams.get('page')
      capturedPageNumber = page ? parseInt(page) : null
      
      const response = {
        items: [{
          auditLogId: 1,
          userIdentifier: 'pagination-test',
          details: `page ${page}`,
          timestamp: '2025-09-01T10:00:00Z',
          action: 'PAGINATION_TEST',
          entityType: 'Test',
          severity: 1,
          category: 1,
          ipAddress: '127.0.0.1'
        }],
        totalCount: 100,
        pageNumber: parseInt(page || '1'), // Backend uses 1-based
        pageSize: 25,
        totalPages: 4,
        hasNextPage: parseInt(page || '1') < 4,
        hasPreviousPage: parseInt(page || '1') > 1
      }
      
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(response)
      })
    })

    await page.goto('/dashboard/audit-logs', { waitUntil: 'domcontentloaded' })
    await page.waitForLoadState('networkidle')

    // Initial load should request page 1 (1-based for backend)
    expect(capturedPageNumber).toBe(1)
    
    // If next page button exists and is enabled, test pagination
    const nextButton = page.locator('button').filter({ hasText: /next/i }).or(
      page.locator('button[aria-label*="next"]')
    )
    
    if (await nextButton.count() > 0 && await nextButton.isEnabled()) {
      await nextButton.click()
      await page.waitForLoadState('networkidle')
      
      // Should request page 2 (1-based for backend)
      expect(capturedPageNumber).toBe(2)
      
      // Verify pagination display shows correct frontend values
      const paginationText = await page.locator('text=/26-50 of 100/').count()
      expect(paginationText).toBeGreaterThan(0)
    }
  })

  test('Validates TypeScript interface contracts are maintained', async ({ page }) => {
    // This test validates that the data structure matches TypeScript interfaces
    let responseStructure: any = null
    
    await page.route('**/api/audit-logs**', async (route) => {
      const response = await route.fetch()
      responseStructure = await response.json()
      await route.fulfill({ response })
    })

    await page.goto('/dashboard/audit-logs', { waitUntil: 'domcontentloaded' })
    await page.waitForLoadState('networkidle')

    // Wait for API call to be captured
    await expect(() => {
      expect(responseStructure).not.toBeNull()
    }).toPass({ timeout: 10000 })

    // Validate backend PagedResult structure
    if (responseStructure.success !== undefined) {
      // Wrapped format
      expect(responseStructure).toHaveProperty('success')
      expect(responseStructure).toHaveProperty('data')
      
      const data = responseStructure.data
      expect(data).toHaveProperty('items')
      expect(data).toHaveProperty('totalCount')
      expect(data).toHaveProperty('pageNumber')
      expect(data).toHaveProperty('pageSize')
      
      // Validate AuditLog interface properties
      if (data.items && data.items.length > 0) {
        const auditLog = data.items[0]
        expect(auditLog).toHaveProperty('auditLogId')
        expect(auditLog).toHaveProperty('userIdentifier')
        expect(auditLog).toHaveProperty('details')
        expect(auditLog).toHaveProperty('timestamp')
        expect(auditLog).toHaveProperty('action')
        expect(auditLog).toHaveProperty('entityType')
        expect(auditLog).toHaveProperty('severity')
        expect(auditLog).toHaveProperty('category')
        expect(auditLog).toHaveProperty('ipAddress')
        
        // Ensure legacy properties are NOT present
        expect(auditLog).not.toHaveProperty('id')
        expect(auditLog).not.toHaveProperty('username')  
        expect(auditLog).not.toHaveProperty('description')
      }
    } else {
      // Direct format
      expect(responseStructure).toHaveProperty('items')
      expect(responseStructure).toHaveProperty('totalCount')
      expect(responseStructure).toHaveProperty('pageNumber')
      expect(responseStructure).toHaveProperty('pageSize')
      
      // Validate AuditLog interface properties
      if (responseStructure.items && responseStructure.items.length > 0) {
        const auditLog = responseStructure.items[0]
        expect(auditLog).toHaveProperty('auditLogId')
        expect(auditLog).toHaveProperty('userIdentifier')
        expect(auditLog).toHaveProperty('details')
        expect(auditLog).toHaveProperty('timestamp')
        expect(auditLog).toHaveProperty('action')
        expect(auditLog).toHaveProperty('entityType')
        expect(auditLog).toHaveProperty('severity')
        expect(auditLog).toHaveProperty('category')
        expect(auditLog).toHaveProperty('ipAddress')
        
        // Ensure legacy properties are NOT present
        expect(auditLog).not.toHaveProperty('id')
        expect(auditLog).not.toHaveProperty('username')
        expect(auditLog).not.toHaveProperty('description')
      }
    }
  })

  test('Ensures no console errors related to property access', async ({ page }) => {
    const propertyAccessErrors: string[] = []
    
    // Monitor for specific property access errors
    page.on('console', (msg) => {
      if (msg.type() === 'error') {
        const text = msg.text()
        if (
          text.includes("Cannot read properties of undefined (reading 'items')") ||
          text.includes("Cannot read properties of undefined (reading 'auditLogId')") ||
          text.includes("Cannot read properties of undefined (reading 'userIdentifier')") ||
          text.includes("Cannot read properties of undefined (reading 'details')") ||
          text.includes("reading 'id'") || // Legacy property
          text.includes("reading 'username'") || // Legacy property  
          text.includes("reading 'description'") // Legacy property
        ) {
          propertyAccessErrors.push(text)
        }
      }
    })

    await page.goto('/dashboard/audit-logs', { waitUntil: 'domcontentloaded' })
    await page.waitForLoadState('networkidle')

    // Interact with the page to trigger various code paths
    
    // Try pagination if available
    const nextButton = page.locator('button').filter({ hasText: /next/i })
    if (await nextButton.count() > 0 && await nextButton.isEnabled()) {
      await nextButton.click()
      await page.waitForLoadState('networkidle')
    }
    
    // Try refresh if available
    const refreshButton = page.locator('button').filter({ hasText: /refresh/i })
    if (await refreshButton.count() > 0) {
      await refreshButton.click()
      await page.waitForLoadState('networkidle')
    }
    
    // Try filters if available
    const filtersButton = page.locator('button').filter({ hasText: /filters/i })
    if (await filtersButton.count() > 0) {
      await filtersButton.click()
      await page.waitForTimeout(1000)
    }

    // Wait for any async operations to complete
    await page.waitForTimeout(3000)

    // Verify no property access errors occurred
    expect(propertyAccessErrors, `Property access errors detected: ${propertyAccessErrors.join(', ')}`).toHaveLength(0)
  })

  test('Tests service layer error handling and fallbacks', async ({ page }) => {
    // Test various error scenarios the service layer should handle gracefully
    
    const errorScenarios = [
      {
        name: 'Network timeout',
        handler: async (route: any) => {
          await new Promise(resolve => setTimeout(resolve, 30000)) // Simulate timeout
        }
      },
      {
        name: 'Invalid JSON response',
        handler: async (route: any) => {
          await route.fulfill({
            status: 200,
            contentType: 'application/json',
            body: 'invalid json content'
          })
        }
      },
      {
        name: 'Empty response body',
        handler: async (route: any) => {
          await route.fulfill({
            status: 200,
            contentType: 'application/json',
            body: ''
          })
        }
      }
    ]

    for (const scenario of errorScenarios) {
      await page.route('**/api/audit-logs**', scenario.handler)

      await page.goto('/dashboard/audit-logs', { waitUntil: 'domcontentloaded' })
      
      // Should not crash and should show some form of error handling
      await expect(page.locator('table')).toBeVisible()
      
      // Capture any errors
      const errors: string[] = []
      page.on('console', (msg) => {
        if (msg.type() === 'error') {
          errors.push(msg.text())
        }
      })

      await page.waitForTimeout(2000)

      // Should not have critical mapping errors
      const mappingErrors = errors.filter(error => 
        error.includes('Cannot read properties of undefined') ||
        error.includes('reading \'items\'')
      )

      expect(mappingErrors, `Scenario '${scenario.name}' should not cause mapping errors`).toHaveLength(0)
    }
  })
})
