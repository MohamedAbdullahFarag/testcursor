import { test, expect } from '@playwright/test';

const routes = [
  '/',
  '/login',
  '/faq',
  '/support',
  '/eparticipation',
  '/dashboard',
  '/dashboard/media',
  '/dashboard/user-management',
  '/dashboard/notifications',
];

for (const route of routes) {
  test(`route ${route} should load`, async ({ page }) => {
    const res = await page.goto(route);
    // Page may be a client-side SPA; ensure we at least got a response and body content
    expect(res).not.toBeNull();
    const content = await page.content();
    expect(content.length).toBeGreaterThan(100);
  });
}
