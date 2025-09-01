const { chromium } = require('playwright');

async function testTranslations() {
  const browser = await chromium.launch({ headless: false });
  const page = await browser.newPage();
  
  // Listen for console errors
  const consoleErrors = [];
  page.on('console', (msg) => {
    if (msg.type() === 'error' || msg.text().includes('missingKey')) {
      consoleErrors.push(msg.text());
    }
  });
  
  try {
    // Navigate to the app
    await page.goto('https://localhost:5174/');
    
    // Wait for initial load
    await page.waitForTimeout(3000);
    
    // Try to navigate to content management page if it exists
    try {
      await page.click('text=Content Management');
      await page.waitForTimeout(2000);
    } catch (e) {
      console.log('Content Management link not found, checking URL navigation');
      await page.goto('https://localhost:5174/content-management');
      await page.waitForTimeout(2000);
    }
    
    // Check for translation errors
    if (consoleErrors.length === 0) {
      console.log('✅ No translation errors found!');
    } else {
      console.log('❌ Found translation errors:');
      consoleErrors.forEach(error => console.log(`  - ${error}`));
    }
    
  } catch (error) {
    console.error('Test failed:', error.message);
  }
  
  await browser.close();
}

testTranslations();
