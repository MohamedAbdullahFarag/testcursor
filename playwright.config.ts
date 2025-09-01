import { defineConfig, devices } from '@playwright/test';

// Minimal typing to satisfy TS when using process.env in this isolated config
// eslint-disable-next-line @typescript-eslint/no-explicit-any
declare const process: any;

export default defineConfig({
  testDir: './playwright-tests',
  timeout: 30_000,
  expect: { timeout: 5000 },
  fullyParallel: true,
  retries: 0,
  reporter: [
    ['list'],
    ['html', { outputFolder: 'playwright-report', open: 'never' }],
    ['json', { outputFile: 'playwright-report/report.json' }],
    ['junit', { outputFile: 'playwright-report/junit.xml' }],
  ],
  use: {
    headless: true,
    ignoreHTTPSErrors: true,
    viewport: { width: 1280, height: 800 },
    actionTimeout: 10000,
    baseURL: process.env.E2E_BASE_URL || 'https://localhost:5173',
  },
  projects: [
    { name: 'chromium', use: { ...devices['Desktop Chrome'] } },
  ],
});
