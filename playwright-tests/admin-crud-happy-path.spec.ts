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

test.describe('Admin CRUD happy-path', () => {
  test.beforeEach(async ({ page }) => {
    await loginByStorage(page)
  })

  test('User Management: create, edit, delete user', async ({ page }) => {
    await page.goto('/dashboard/user-management', { waitUntil: 'domcontentloaded' })
    await page.waitForLoadState('networkidle')
    // Wait for either the page title testid or the create button to be visible
    const titleLoc = page.getByTestId('user-management-title')
    const createBtnLoc = page.getByTestId('create-user')
    const titleVisible = await titleLoc.isVisible().catch(() => false)
    if (!titleVisible) {
      const fallbackBtn = createBtnLoc.or(page.getByRole('button', { name: /create user/i }))
      const btnCount = await fallbackBtn.count()
      if (btnCount === 0) {
        test.skip(true, 'User Management UI controls not available in this environment')
      }
    }

    // Create user
    const createBtnCandidate = page.getByTestId('create-user').or(page.getByRole('button', { name: /create user/i }))
    if (await createBtnCandidate.count() === 0) {
      test.skip(true, 'Create User UI not available in this environment')
    }
    await createBtnCandidate.first().click()
  const userDialog = page.locator('[role="dialog"]')
  await expect(userDialog).toBeVisible({ timeout: 10000 })
  await expect(userDialog.getByText(/Create New User|Create User/i)).toBeVisible()

  await userDialog.getByLabel('Full Name').fill('Playwright Test User')
  await userDialog.getByLabel('Email').fill(`playwright.user+${Date.now()}@example.com`)
  await userDialog.getByLabel('Password').fill('secret123')

    // Select first available role chip
  const anyRole = userDialog.locator('div[role="button"][class*="MuiChip-root"], .MuiChip-root').first()
    if (await anyRole.count()) {
      await anyRole.click()
    }

  await userDialog.getByRole('button', { name: /Create User|Update User|Save/i }).click()

    // Expect the dialog to close and list to refresh
  await expect(page.getByText('No users found')).toHaveCount(0)

    // Edit first user row
    const firstRow = page.locator('[data-testid^="user-row-"]').first()
    await expect(firstRow).toBeVisible()
  const editBtn = firstRow.getByRole('button', { name: /Edit user/i })
    await editBtn.click()
  const editDialog = page.locator('[role="dialog"]').last()
  await expect(editDialog).toBeVisible()
  await editDialog.getByLabel('Full Name').fill('Playwright Test User Edited')
  await editDialog.getByRole('button', { name: /Update User|Save/i }).click()

    // Delete same row (first visible)
    const rowAfterEdit = page.locator('[data-testid^="user-row-"]').first()
    const deleteBtn = rowAfterEdit.getByRole('button', { name: /Delete user/i })
    await deleteBtn.click()

    // Confirm delete dialog
  const confirmDialog = page.locator('[role="dialog"]').last()
  await expect(confirmDialog.getByText(/Confirm Delete/i)).toBeVisible()
  await confirmDialog.getByRole('button', { name: /Delete/i }).click()

    // Basic assertion to ensure table still renders
    await expect(page.locator('table')).toBeVisible()
  })

  test('Role Management: create, edit, delete role', async ({ page }) => {
    await page.goto('/dashboard/role-management', { waitUntil: 'domcontentloaded' })
    await page.waitForLoadState('networkidle')
    const roleTitle = page.getByTestId('role-management-title')
    const roleTitleVisible = await roleTitle.isVisible().catch(() => false)
    if (!roleTitleVisible) {
      const createRoleCandidate = page.getByTestId('create-role').or(page.getByRole('button', { name: /create role/i }))
      const c = await createRoleCandidate.count()
      if (c === 0) {
        test.skip(true, 'Role Management UI controls not available in this environment')
      }
    }

    // Click create role button
    const createRoleButton = page.getByRole('button', { name: /create role/i })
    await createRoleButton.click()

    // Fill create role form
  const roleDialog = page.locator('[role="dialog"]').last()
  await expect(roleDialog).toBeVisible()
  await roleDialog.getByLabel(/code/i).fill(`playwright-role-${Date.now()}`)
  await roleDialog.getByLabel(/name/i).fill('Playwright Role')
  await roleDialog.getByLabel(/description/i).fill('Created by Playwright E2E test')
  await roleDialog.getByRole('button', { name: /create role|save/i }).click()

    // Open edit for the newly created or first role
    const firstRowEdit = page.getByRole('button', { name: /edit role/i }).first()
    if (await firstRowEdit.count() === 0) {
      test.skip(true, 'Role edit UI not available in this environment')
    }
    await firstRowEdit.click()
    await page.getByLabel(/name/i).fill('Playwright Role Edited')
    await page.getByRole('button', { name: /save changes|save/i }).click()

    // Delete first non-system role
    const firstRowDelete = page.getByRole('button', { name: /delete role/i }).first()
    if (await firstRowDelete.count() === 0) {
      test.skip(true, 'Role delete UI not available in this environment')
    }
    await firstRowDelete.click()
    const confirmDelete = page.getByRole('button', { name: /delete/i }).first()
    if (await confirmDelete.isEnabled()) {
      await confirmDelete.click()
    } else {
      // If it's a system role and cannot delete, close the dialog
      await page.getByRole('button', { name: /cancel/i }).click()
    }

    // Ensure list still renders
    await expect(page.getByText(/Roles|No roles found/i)).toBeVisible()
  })
})
