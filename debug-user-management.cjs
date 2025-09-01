const { chromium } = require('playwright');

(async () => {
  const browser = await chromium.launch({ headless: false });
  const context = await browser.newContext({ 
    ignoreHTTPSErrors: true 
  });
  const page = await context.newPage();

  // Listen to console logs
  page.on('console', msg => {
    console.log(`CONSOLE ${msg.type()}: ${msg.text()}`);
  });

  // Listen to errors
  page.on('pageerror', error => {
    console.log(`PAGE ERROR: ${error.message}`);
  });

  // Listen to network requests
  page.on('request', request => {
    console.log(`REQUEST: ${request.method()} ${request.url()}`);
  });

  page.on('response', response => {
    console.log(`RESPONSE: ${response.status()} ${response.url()}`);
  });

  try {
    // Set up authentication like in the tests
    await page.goto('https://localhost:5173/');
    await page.evaluate(() => {
      const token = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwiZW1haWwiOiJhZG1pbkBpa2h0aWJhci5jb20iLCJyb2xlIjoic3lzdGVtLWFkbWluIiwiaWF0IjoxNjMwMDAwMDAwLCJleHAiOjk5OTk5OTk5OTl9.placeholder';
      const state = {
        token,
        user: { id: 1, email: 'admin@ikhtibar.com', role: 'system-admin' },
        isAuthenticated: true
      };
      localStorage.setItem('auth-storage', JSON.stringify({ state, version: 0 }));
    });

    // Navigate to user management
    await page.goto('https://localhost:5173/dashboard/user-management');
    await page.waitForTimeout(3000);
    
    console.log('Page title:', await page.title());
    console.log('Current URL:', page.url());
    
    // Check if the page loaded properly
    const content = await page.textContent('body');
    console.log('Page contains "User Management":', content.includes('User Management'));
    
    // Wait a bit more to see all network requests
    await page.waitForTimeout(5000);

  } catch (error) {
    console.error('Error:', error);
  } finally {
    await browser.close();
  }
})();
