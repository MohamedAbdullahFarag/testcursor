import { test, expect } from '@playwright/test';

test.describe('Audit Logs Filters', () => {
  test.beforeEach(async ({ page }) => {
    // Navigate to audit logs page
    await page.goto('/admin/audit-logs');
    await page.waitForLoadState('networkidle');
  });

  test('should show filter controls and filter options should be properly labeled', async ({ page }) => {
    // Click show filters button
    await page.click('button:has-text("Show Filters")');
    
    // Wait for filters to appear
    await page.waitForSelector('[data-testid="severity-filter"], [role="combobox"]:has([aria-labelledby*="severity" i])', { timeout: 5000 });
    
    // Check severity filter dropdown
    const severitySelect = page.locator('[data-testid="severity-filter"], [role="combobox"]:has([aria-labelledby*="severity" i])').first();
    await severitySelect.click();
    
    // Verify severity options are labeled correctly (not numbers)
    const severityOptions = page.locator('li[role="option"]');
    const severityTexts = await severityOptions.allTextContents();
    
    expect(severityTexts).toContain('Critical');
    expect(severityTexts).toContain('High');
    expect(severityTexts).toContain('Medium');
    expect(severityTexts).toContain('Low');
    expect(severityTexts).toContain('Any Severity');
    
    // Verify no raw numbers in dropdown
    const hasNumbers = severityTexts.some(text => /^\d+$/.test(text.trim()));
    expect(hasNumbers).toBeFalsy();
    
    // Close severity dropdown
    await page.keyboard.press('Escape');
    
    // Check category filter dropdown
    const categorySelect = page.locator('[data-testid="category-filter"], [role="combobox"]:has([aria-labelledby*="category" i])').first();
    await categorySelect.click();
    
    // Verify category options are labeled correctly (not numbers)
    const categoryTexts = await page.locator('li[role="option"]').allTextContents();
    
    expect(categoryTexts).toContain('Authentication');
    expect(categoryTexts).toContain('Authorization');
    expect(categoryTexts).toContain('User Management');
    expect(categoryTexts).toContain('Data Access');
    expect(categoryTexts).toContain('System');
    expect(categoryTexts).toContain('Security');
    expect(categoryTexts).toContain('Any Category');
    
    // Verify no raw numbers in category dropdown
    const hasNumbersCategory = categoryTexts.some(text => /^\d+$/.test(text.trim()));
    expect(hasNumbersCategory).toBeFalsy();
  });

  test('should filter by severity correctly', async ({ page }) => {
    // Show filters
    await page.click('button:has-text("Show Filters")');
    
    // Wait for table to load with data
    await page.waitForSelector('table tbody tr', { timeout: 10000 });
    
    // Get initial row count
    const initialRows = await page.locator('table tbody tr').count();
    expect(initialRows).toBeGreaterThan(0);
    
    // Select High severity filter
    const severitySelect = page.locator('[data-testid="severity-filter"], [role="combobox"]:has([aria-labelledby*="severity" i])').first();
    await severitySelect.click();
    await page.click('li[role="option"]:has-text("High")');
    
    // Apply filters
    await page.click('button:has-text("Apply Filters")');
    await page.waitForLoadState('networkidle');
    
    // Wait for filtered results
    await page.waitForTimeout(1000);
    
    // Verify all visible rows show "High" severity
    const visibleRows = page.locator('table tbody tr');
    const rowCount = await visibleRows.count();
    
    if (rowCount > 0) {
      for (let i = 0; i < rowCount; i++) {
        const severityCell = visibleRows.nth(i).locator('td').nth(4); // Severity is 5th column (0-indexed: 4)
        const severityText = await severityCell.textContent();
        expect(severityText?.trim()).toBe('High');
      }
    }
  });

  test('should filter by category correctly', async ({ page }) => {
    // Show filters
    await page.click('button:has-text("Show Filters")');
    
    // Wait for table to load with data
    await page.waitForSelector('table tbody tr', { timeout: 10000 });
    
    // Get initial row count
    const initialRows = await page.locator('table tbody tr').count();
    expect(initialRows).toBeGreaterThan(0);
    
    // Select Authentication category filter
    const categorySelect = page.locator('[data-testid="category-filter"], [role="combobox"]:has([aria-labelledby*="category" i])').first();
    await categorySelect.click();
    await page.click('li[role="option"]:has-text("Authentication")');
    
    // Apply filters
    await page.click('button:has-text("Apply Filters")');
    await page.waitForLoadState('networkidle');
    
    // Wait for filtered results
    await page.waitForTimeout(1000);
    
    // Verify all visible rows show "Authentication" category
    const visibleRows = page.locator('table tbody tr');
    const rowCount = await visibleRows.count();
    
    if (rowCount > 0) {
      for (let i = 0; i < rowCount; i++) {
        const categoryCell = visibleRows.nth(i).locator('td').nth(5); // Category is 6th column (0-indexed: 5)
        const categoryText = await categoryCell.textContent();
        expect(categoryText?.trim()).toBe('Authentication');
      }
    }
  });

  test('should clear filters correctly', async ({ page }) => {
    // Show filters
    await page.click('button:has-text("Show Filters")');
    
    // Wait for table to load
    await page.waitForSelector('table tbody tr', { timeout: 10000 });
    const initialRows = await page.locator('table tbody tr').count();
    
    // Apply severity filter
    const severitySelect = page.locator('[data-testid="severity-filter"], [role="combobox"]:has([aria-labelledby*="severity" i])').first();
    await severitySelect.click();
    await page.click('li[role="option"]:has-text("High")');
    
    // Apply filters
    await page.click('button:has-text("Apply Filters")');
    await page.waitForLoadState('networkidle');
    await page.waitForTimeout(1000);
    
    const filteredRows = await page.locator('table tbody tr').count();
    
    // Clear filters
    await page.click('button:has-text("Clear Filters")');
    await page.waitForLoadState('networkidle');
    await page.waitForTimeout(1000);
    
    const clearedRows = await page.locator('table tbody tr').count();
    
    // Should have more or equal rows after clearing (unless all data was already "High" severity)
    expect(clearedRows).toBeGreaterThanOrEqual(filteredRows);
  });
});
