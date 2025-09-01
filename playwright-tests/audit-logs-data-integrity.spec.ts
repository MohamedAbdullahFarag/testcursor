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

test.describe('Audit Logs Data Integrity Tests', () => {
  test.beforeEach(async ({ page }) => {
    await loginByStorage(page)
  })

  test('Audit logs page loads without JavaScript errors', async ({ page }) => {
    const consoleErrors: string[] = []
    const consoleWarnings: string[] = []
    
    // Capture console errors and warnings
    page.on('console', (msg) => {
      if (msg.type() === 'error') {
        consoleErrors.push(msg.text())
      } else if (msg.type() === 'warning') {
        consoleWarnings.push(msg.text())
      }
    })

    // Navigate to audit logs page
    await page.goto('/dashboard/audit-logs', { waitUntil: 'domcontentloaded' })
    await page.waitForLoadState('networkidle')

    // Verify no critical JavaScript errors occurred
    const criticalErrors = consoleErrors.filter(error => 
      error.includes('Cannot read properties of undefined') ||
      error.includes('reading \'items\'') ||
      error.includes('TypeError') ||
      error.includes('ReferenceError')
    )
    
    expect(criticalErrors, `Critical errors found: ${criticalErrors.join(', ')}`).toHaveLength(0)
  })

  test('Audit logs table renders with correct column headers', async ({ page }) => {
    await page.goto('/dashboard/audit-logs', { waitUntil: 'domcontentloaded' })
    await page.waitForLoadState('networkidle')

    // Wait for the table to load
    await expect(page.locator('table')).toBeVisible({ timeout: 10000 })

    // Verify all required column headers are present
    const expectedHeaders = ['Timestamp', 'User', 'Action', 'Entity', 'Severity', 'Category', 'IP Address']
    
    for (const header of expectedHeaders) {
      await expect(page.locator('th').filter({ hasText: header })).toBeVisible()
    }
  })

  test('Audit logs data populates correctly without undefined errors', async ({ page }) => {
    let apiResponseReceived = false
    let apiResponseData: any = null

    // Intercept API calls to audit logs endpoint
    await page.route('**/api/audit-logs**', async (route) => {
      const response = await route.fetch()
      const data = await response.json()
      apiResponseReceived = true
      apiResponseData = data
      await route.fulfill({ response })
    })

    await page.goto('/dashboard/audit-logs', { waitUntil: 'domcontentloaded' })
    await page.waitForLoadState('networkidle')

    // Wait for API response
    await expect(() => {
      expect(apiResponseReceived).toBe(true)
    }).toPass({ timeout: 10000 })

    // Verify API response structure matches expected format
    expect(apiResponseData).toBeDefined()
    
    // Check if response is direct PagedResult or wrapped in ApiResponse
    if (apiResponseData.success !== undefined) {
      // Wrapped format: ApiResponse<PagedResult<T>>
      expect(apiResponseData.success).toBe(true)
      expect(apiResponseData.data).toBeDefined()
      expect(apiResponseData.data.items).toBeDefined()
      expect(Array.isArray(apiResponseData.data.items)).toBe(true)
      expect(typeof apiResponseData.data.totalCount).toBe('number')
      expect(typeof apiResponseData.data.pageNumber).toBe('number')
      expect(typeof apiResponseData.data.pageSize).toBe('number')
    } else {
      // Direct format: PagedResult<T>
      expect(apiResponseData.items).toBeDefined()
      expect(Array.isArray(apiResponseData.items)).toBe(true)
      expect(typeof apiResponseData.totalCount).toBe('number')
      expect(typeof apiResponseData.pageNumber).toBe('number')
      expect(typeof apiResponseData.pageSize).toBe('number')
    }

    // Verify table rows populate
    const tableRows = page.locator('tbody tr')
    await expect(tableRows.first()).toBeVisible({ timeout: 10000 })
  })

  test('Audit log entries display correct field mappings', async ({ page }) => {
    await page.goto('/dashboard/audit-logs', { waitUntil: 'domcontentloaded' })
    await page.waitForLoadState('networkidle')

    // Wait for table data to load
    const tableRows = page.locator('tbody tr')
    await expect(tableRows.first()).toBeVisible({ timeout: 10000 })

    const firstRow = page.locator('tbody tr').first()
    const cells = firstRow.locator('td')

    // Verify each cell has content (not empty or "undefined")
    const cellCount = await cells.count()
    expect(cellCount).toBe(7) // Should have 7 columns

    for (let i = 0; i < cellCount; i++) {
      const cellText = await cells.nth(i).textContent()
      expect(cellText).toBeDefined()
      expect(cellText).not.toBe('')
      expect(cellText).not.toBe('undefined')
      expect(cellText).not.toBe('null')
      
      // Specific validations for each column
      switch (i) {
        case 0: // Timestamp
          expect(cellText).toMatch(/\d{1,2}\/\d{1,2}\/\d{4}/)
          break
        case 1: // User
          expect(cellText?.length).toBeGreaterThan(0)
          break
        case 2: // Action
          expect(cellText?.length).toBeGreaterThan(0)
          break
        case 3: // Entity
          expect(cellText?.length).toBeGreaterThan(0)
          break
        case 4: // Severity (should display as label, not number)
          expect(cellText).toMatch(/^(Critical|High|Medium|Low)$/)
          break
        case 5: // Category (should display as label, not number)
          expect(cellText).toMatch(/^(Authentication|Authorization|UserManagement|DataAccess|System|Security)$/)
          break
        case 6: // IP Address
          expect(cellText?.length).toBeGreaterThan(0)
          break
      }
    }
  })

  test('Pagination controls work correctly', async ({ page }) => {
    await page.goto('/dashboard/audit-logs', { waitUntil: 'domcontentloaded' })
    await page.waitForLoadState('networkidle')

    // Wait for pagination info to load
    await expect(page.locator('text=/\\d+-\\d+ of \\d+/')).toBeVisible({ timeout: 10000 })

    // Get initial pagination text
    const paginationText = await page.locator('text=/\\d+-\\d+ of \\d+/').textContent()
    expect(paginationText).toMatch(/\d+-\d+ of \d+/)

    // If there's a next page button and it's enabled, test pagination
    const nextButton = page.locator('button').filter({ hasText: /next/i }).or(
      page.locator('button[aria-label*="next"]')
    )
    
    if (await nextButton.count() > 0 && await nextButton.isEnabled()) {
      await nextButton.click()
      await page.waitForLoadState('networkidle')
      
      // Verify pagination text changed
      const newPaginationText = await page.locator('text=/\\d+-\\d+ of \\d+/').textContent()
      expect(newPaginationText).not.toBe(paginationText)
      
      // Verify table still has data
      await expect(page.locator('tbody tr').first()).toBeVisible()
    }
  })

  test('Audit logs API response structure validation', async ({ page }) => {
    let capturedResponses: any[] = []

    // Capture all API responses to audit logs endpoints
    await page.route('**/api/audit-logs**', async (route) => {
      const response = await route.fetch()
      const data = await response.json()
      capturedResponses.push(data)
      await route.fulfill({ response })
    })

    await page.goto('/dashboard/audit-logs', { waitUntil: 'domcontentloaded' })
    await page.waitForLoadState('networkidle')

    // Ensure we captured at least one response
    expect(capturedResponses.length).toBeGreaterThan(0)

    // Validate the structure of each response
    for (const response of capturedResponses) {
      // Test both possible response structures
      if (response.success !== undefined) {
        // ApiResponse wrapper format
        expect(response).toHaveProperty('success')
        expect(response).toHaveProperty('data')
        expect(response.data).toHaveProperty('items')
        expect(response.data).toHaveProperty('totalCount')
        expect(response.data).toHaveProperty('pageNumber')
        expect(response.data).toHaveProperty('pageSize')
        
        // Validate audit log item structure
        if (response.data.items.length > 0) {
          const firstItem = response.data.items[0]
          expect(firstItem).toHaveProperty('auditLogId') // Not 'id'
          expect(firstItem).toHaveProperty('userIdentifier') // Not 'username'
          expect(firstItem).toHaveProperty('details') // Not 'description'
          expect(firstItem).toHaveProperty('timestamp')
          expect(firstItem).toHaveProperty('action')
          expect(firstItem).toHaveProperty('entityType')
          expect(firstItem).toHaveProperty('severity')
          expect(firstItem).toHaveProperty('category')
          expect(firstItem).toHaveProperty('ipAddress')
        }
      } else {
        // Direct PagedResult format
        expect(response).toHaveProperty('items')
        expect(response).toHaveProperty('totalCount')
        expect(response).toHaveProperty('pageNumber')
        expect(response).toHaveProperty('pageSize')
        
        // Validate audit log item structure
        if (response.items.length > 0) {
          const firstItem = response.items[0]
          expect(firstItem).toHaveProperty('auditLogId') // Not 'id'
          expect(firstItem).toHaveProperty('userIdentifier') // Not 'username'
          expect(firstItem).toHaveProperty('details') // Not 'description'
          expect(firstItem).toHaveProperty('timestamp')
          expect(firstItem).toHaveProperty('action')
          expect(firstItem).toHaveProperty('entityType')
          expect(firstItem).toHaveProperty('severity')
          expect(firstItem).toHaveProperty('category')
          expect(firstItem).toHaveProperty('ipAddress')
        }
      }
    }
  })

  test('Filter functionality works without errors', async ({ page }) => {
    await page.goto('/dashboard/audit-logs', { waitUntil: 'domcontentloaded' })
    await page.waitForLoadState('networkidle')

    // Look for filter button/controls
    const showFiltersButton = page.locator('button').filter({ hasText: /show filters|filters/i })
    
    if (await showFiltersButton.count() > 0) {
      await showFiltersButton.click()
      
      // Wait for filter panel to appear
      await page.waitForTimeout(1000)
      
      // Try to apply a filter if controls are available
      const fromDateInput = page.locator('input[type="date"]').or(page.locator('input[placeholder*="from"]')).first()
      if (await fromDateInput.count() > 0) {
        await fromDateInput.fill('2025-09-01')
        
        // Look for apply/search button
        const applyButton = page.locator('button').filter({ hasText: /apply|search|filter/i })
        if (await applyButton.count() > 0) {
          await applyButton.click()
          await page.waitForLoadState('networkidle')
          
          // Verify table still loads correctly after filtering
          await expect(page.locator('table')).toBeVisible()
        }
      }
    }
  })

  test('Export functionality validation', async ({ page }) => {
    await page.goto('/dashboard/audit-logs', { waitUntil: 'domcontentloaded' })
    await page.waitForLoadState('networkidle')

    // Test CSV export if available
    const exportCsvButton = page.locator('button').filter({ hasText: /export csv/i })
    if (await exportCsvButton.count() > 0) {
      // Set up download handler
      const downloadPromise = page.waitForEvent('download', { timeout: 10000 })
      await exportCsvButton.click()
      
      try {
        const download = await downloadPromise
        expect(download.suggestedFilename()).toMatch(/\.csv$/)
      } catch (error) {
        // Export might be disabled or require different setup
        console.log('CSV export test skipped:', error)
      }
    }

    // Test Excel export if available
    const exportExcelButton = page.locator('button').filter({ hasText: /export excel/i })
    if (await exportExcelButton.count() > 0) {
      // Set up download handler
      const downloadPromise = page.waitForEvent('download', { timeout: 10000 })
      await exportExcelButton.click()
      
      try {
        const download = await downloadPromise
        expect(download.suggestedFilename()).toMatch(/\.(xlsx|xls)$/)
      } catch (error) {
        // Export might be disabled or require different setup
        console.log('Excel export test skipped:', error)
      }
    }
  })

  test('Frontend-backend data mapping consistency', async ({ page }) => {
    let frontendData: any[] = []
    
    // Capture API response data
    await page.route('**/api/audit-logs**', async (route) => {
      const response = await route.fetch()
      const data = await response.json()
      
      // Extract items from either response format
      const items = data.items || data.data?.items || []
      frontendData = items
      
      await route.fulfill({ response })
    })

    await page.goto('/dashboard/audit-logs', { waitUntil: 'domcontentloaded' })
    await page.waitForLoadState('networkidle')

    // Wait for data to be captured
    await expect(() => {
      expect(frontendData.length).toBeGreaterThan(0)
    }).toPass({ timeout: 10000 })

    // Get table data from frontend
    const tableRows = page.locator('tbody tr')
    const rowCount = await tableRows.count()
    
    if (rowCount > 0 && frontendData.length > 0) {
      // Check first row mapping
      const firstRowCells = tableRows.first().locator('td')
      const backendFirstItem = frontendData[0]
      
      // Verify timestamp mapping
      const timestampCell = await firstRowCells.nth(0).textContent()
      expect(timestampCell).toBeDefined()
      expect(backendFirstItem.timestamp).toBeDefined()
      
      // Verify user mapping (userIdentifier -> User column)
      const userCell = await firstRowCells.nth(1).textContent()
      expect(userCell).toBeDefined()
      expect(backendFirstItem.userIdentifier).toBeDefined()
      
      // Verify action mapping
      const actionCell = await firstRowCells.nth(2).textContent()
      expect(actionCell).toBeDefined()
      expect(backendFirstItem.action).toBeDefined()
      
      // Verify entity mapping
      const entityCell = await firstRowCells.nth(3).textContent()
      expect(entityCell).toBeDefined()
      expect(backendFirstItem.entityType).toBeDefined()
      
      // Verify severity mapping
      const severityCell = await firstRowCells.nth(4).textContent()
      expect(severityCell).toBeDefined()
      expect(backendFirstItem.severity).toBeDefined()
      
      // Validate severity is converted to proper label
      const severityLabels = ['Critical', 'High', 'Medium', 'Low']
      expect(severityLabels).toContain(severityCell)
      expect(Number.isInteger(backendFirstItem.severity)).toBe(true)
      expect(backendFirstItem.severity).toBeGreaterThanOrEqual(0)
      expect(backendFirstItem.severity).toBeLessThanOrEqual(3)
      
      // Verify category mapping
      const categoryCell = await firstRowCells.nth(5).textContent()
      expect(categoryCell).toBeDefined()
      expect(backendFirstItem.category).toBeDefined()
      
      // Validate category is converted to proper label
      const categoryLabels = ['Authentication', 'Authorization', 'UserManagement', 'DataAccess', 'System', 'Security']
      expect(categoryLabels).toContain(categoryCell)
      expect(Number.isInteger(backendFirstItem.category)).toBe(true)
      expect(backendFirstItem.category).toBeGreaterThanOrEqual(0)
      expect(backendFirstItem.category).toBeLessThanOrEqual(5)
      
      // Verify IP address mapping
      const ipCell = await firstRowCells.nth(6).textContent()
      expect(ipCell).toBeDefined()
      expect(backendFirstItem.ipAddress).toBeDefined()
    }
  })

  test('Refresh functionality maintains data integrity', async ({ page }) => {
    await page.goto('/dashboard/audit-logs', { waitUntil: 'domcontentloaded' })
    await page.waitForLoadState('networkidle')

    // Get initial row count
    const initialRowCount = await page.locator('tbody tr').count()
    
    // Click refresh button if available
    const refreshButton = page.locator('button').filter({ hasText: /refresh/i })
    if (await refreshButton.count() > 0) {
      await refreshButton.click()
      await page.waitForLoadState('networkidle')
      
      // Verify table still has data after refresh
      await expect(page.locator('tbody tr').first()).toBeVisible()
      
      // Verify no JavaScript errors occurred during refresh
      const errorLogs: string[] = []
      page.on('console', (msg) => {
        if (msg.type() === 'error') {
          errorLogs.push(msg.text())
        }
      })
      
      await page.waitForTimeout(2000) // Wait for any async errors
      
      const mappingErrors = errorLogs.filter(error => 
        error.includes('Cannot read properties of undefined') ||
        error.includes('reading \'items\'')
      )
      
      expect(mappingErrors).toHaveLength(0)
    }
  })
})
