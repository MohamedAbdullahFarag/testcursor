import { chromium, FullConfig } from '@playwright/test';
import { createPRPReporting } from './utils/reporting';
import * as fs from 'fs';
import * as path from 'path';

/**
 * Global setup for E2E tests
 * This runs once before all tests start
 */

async function globalSetup(config: FullConfig) {
  console.log('🚀 Starting global setup for E2E tests...');
  
  try {
    // Initialize PRP reporting
    const reporting = createPRPReporting('./tests/e2e/status');
    
    // Update PRP status to indicate test execution is starting
    await reporting.updatePRPStatus('Test Execution', 'in-progress', 
      'E2E test execution started at ' + new Date().toISOString());
    
    console.log('✅ PRP status updated - Test Execution phase started');
    
    // Create necessary directories
    
    const dirs = [
      './tests/e2e/status',
      './test-results',
      './screenshots',
      './test-results/screenshots'
    ];
    
    for (const dir of dirs) {
      if (!fs.existsSync(dir)) {
        fs.mkdirSync(dir, { recursive: true });
        console.log(`📁 Created directory: ${dir}`);
      }
    }
    
    // Launch browser to verify environment
    const browser = await chromium.launch();
    const context = await browser.newContext();
    const page = await context.newPage();
    
    // Test basic connectivity
    try {
      await page.goto('https://localhost:5173', { timeout: 10000 });
      console.log('✅ Application is accessible at https://localhost:5173');
    } catch (error) {
      console.warn('⚠️ Application not accessible at https://localhost:5173 - tests may fail');
      console.warn('Make sure to run: pnpm start');
    }
    
    await browser.close();
    
    console.log('✅ Global setup completed successfully');
    
  } catch (error) {
    console.error('❌ Global setup failed:', error);
    throw error;
  }
}

export default globalSetup;
