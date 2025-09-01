import { test, expect } from '@playwright/test'

// Admin routes to validate. Where possible we assert a stable heading; otherwise we assert URL and content presence.
const adminRoutes: Array<{
  path: string
  expectHeading?: RegExp
  validate?: (page: import('@playwright/test').Page) => Promise<void>
}> = [
  { path: '/dashboard', expectHeading: /welcome to the unified portal administration/i },
  { path: '/dashboard/content-managment', expectHeading: /content managment/i },
  {
    path: '/dashboard/support',
    // Support page uses a PageHeader component; validate via known texts/buttons
    validate: async (page) => {
  const header = page.getByTestId('support-page-title')
  await expect(header).toBeVisible()
  await expect(page.locator('text=Open Support Ticket')).toBeVisible()
    }
  },
  {
    path: '/dashboard/user-management',
    // Validate via the Create User button and table headers
    validate: async (page) => {
  await expect(page.getByTestId('user-management-title')).toBeVisible()
  await expect(page.getByTestId('create-user')).toBeVisible()
      await expect(page.locator('text=Name')).toBeVisible()
      await expect(page.locator('text=Email')).toBeVisible()
    }
  },
  { path: '/dashboard/role-management' },
  { path: '/dashboard/audit-logs', expectHeading: /audit logs/i, 
    validate: async (page) => {
      // Specific validation for audit logs functionality
      await expect(page.getByRole('heading', { name: /audit logs/i })).toBeVisible()
      await expect(page.locator('table')).toBeVisible()
      
      // Verify all required column headers are present
      const expectedHeaders = ['Timestamp', 'User', 'Action', 'Entity', 'Severity', 'Category', 'IP Address']
      for (const header of expectedHeaders) {
        await expect(page.locator('th').filter({ hasText: header })).toBeVisible()
      }
      
      // Verify no critical JavaScript errors
      const errors: string[] = []
      page.on('console', (msg) => {
        if (msg.type() === 'error' && (
          msg.text().includes('Cannot read properties of undefined') ||
          msg.text().includes("reading 'items'")
        )) {
          errors.push(msg.text())
        }
      })
      
      await page.waitForTimeout(2000)
      expect(errors).toHaveLength(0)
      
      // Verify pagination info is displayed
      await expect(page.locator('text=/\\d+-\\d+ of \\d+/')).toBeVisible({ timeout: 10000 })
    }
  },
  { path: '/dashboard/media' },
  { path: '/dashboard/question-bank' },
  { path: '/dashboard/notifications' },
  // Additional sections (placeholders still resolve to dashboard view)
  { path: '/dashboard/system/settings' },
  { path: '/dashboard/system/api-docs' },
  { path: '/dashboard/system/health' },
  { path: '/dashboard/analytics/dashboard' },
  { path: '/dashboard/analytics/users' },
  { path: '/dashboard/analytics/content' },
  { path: '/dashboard/customer-experience/surveys' },
  { path: '/dashboard/customer-experience/feedback' },
  { path: '/dashboard/e-participation/portal' },
  { path: '/dashboard/e-participation/initiatives' },
  { path: '/dashboard/help/faq' },
  { path: '/dashboard/help/terms' },
  { path: '/dashboard/help/privacy' },
]

async function loginByStorage(page: import('@playwright/test').Page) {
  // Seed the persisted zustand store expected by the app entirely in the browser context
  await page.addInitScript(() => {
    const base64UrlEncode = (obj: any) => {
      const json = JSON.stringify(obj)
      // btoa is available in the browser
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

  // Now navigate to dashboard
  await page.goto('/dashboard', { waitUntil: 'domcontentloaded' })
  await expect(page).toHaveURL(/\/dashboard/)
}

test.describe('Admin happy-path navigation', () => {
  test.beforeEach(async ({ page }) => {
  // Ensure we are authenticated before each test via storage seeding
  await loginByStorage(page)
  })

  for (const route of adminRoutes) {
    test(`navigates to ${route.path} and renders content`, async ({ page }) => {
      // Navigate directly to route (baseURL configured in playwright.config.ts)
      const resp = await page.goto(route.path, { waitUntil: 'domcontentloaded' })
      expect(resp, `Route ${route.path} should respond`).not.toBeNull()

      // Stay on the intended route (not redirected to login/portal)
      await expect(page).toHaveURL(new RegExp(route.path.replace(/[.*+?^${}()|[\]\\]/g, '\\$&')))

      // If we know a stable heading, assert it; otherwise assert meaningful body content
      if (route.expectHeading) {
        const heading = page.getByRole('heading', { name: route.expectHeading })
        const hasAnyHeading = await heading.count()
        if (hasAnyHeading > 0) {
          await expect(heading.first()).toBeVisible()
        } else {
          // Fallback: verify text exists somewhere on the page
          const textLoc = page.locator(`text=${route.expectHeading.source.replace(/\\/g, '')}`)
          await expect(async () => {
            const c = await textLoc.count()
            expect(c).toBeGreaterThan(0)
          }).toPass()
        }
      } else {
        const html = await page.content()
        expect(html.length).toBeGreaterThan(200)
        // Also ensure we didn’t land on a generic not-found or error page
        await expect(page.locator('text=Page not found')).toHaveCount(0)
        await expect(page.locator('text=Sorry, something went wrong')).toHaveCount(0)
      }
    })
  }
})
