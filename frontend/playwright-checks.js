const { chromium } = require('playwright');

(async () => {
  const browser = await chromium.launch({ headless: true });
  const context = await browser.newContext({ ignoreHTTPSErrors: true });
  const page = await context.newPage();

  const base = 'https://localhost:5173';
  const routes = ['/', '/login', '/faq', '/support', '/eparticipation', '/dashboard', '/dashboard/media', '/dashboard/user-management'];

  for (const r of routes) {
    const url = base + r;
    try {
      const resp = await page.goto(url, { waitUntil: 'domcontentloaded', timeout: 15000 });
      const title = await page.title();
      console.log(`${url} -> status=${resp ? resp.status() : 'no-response'} title=${title}`);
    } catch (e) {
      console.log(`${url} -> ERROR: ${e.message}`);
    }
  }

  await browser.close();
})();
