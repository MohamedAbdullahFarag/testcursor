import { FullConfig } from '@playwright/test';
import { createPRPReporting } from './utils/reporting';
import * as fs from 'fs';
import * as path from 'path';

/**
 * Global teardown for E2E tests
 * This runs once after all tests complete
 */

async function globalTeardown(config: FullConfig) {
  console.log('🏁 Starting global teardown for E2E tests...');
  
  try {
    // Initialize PRP reporting
    const reporting = createPRPReporting('./tests/e2e/status');
    
    // Get final test results
    const overallResults = await reporting.getOverallTestResults();
    const allPhases = await reporting.getAllPhasesStatus();
    
    // Update PRP status based on test results
    if (overallResults.failedTests === 0 && overallResults.totalTests > 0) {
      await reporting.updatePRPStatus('Test Execution', 'completed', 
        `All ${overallResults.totalTests} tests passed successfully at ${new Date().toISOString()}`);
      
      // Move to next phase
      await reporting.updatePRPStatus('Test Evaluation', 'in-progress',
        'Starting test evaluation phase at ' + new Date().toISOString());
        
      console.log('✅ Test Execution phase completed successfully');
      console.log('🔄 Test Evaluation phase started');
      
    } else if (overallResults.failedTests > 0) {
      await reporting.updatePRPStatus('Test Execution', 'completed', 
        `${overallResults.passedTests}/${overallResults.totalTests} tests passed. ${overallResults.failedTests} tests failed at ${new Date().toISOString()}`);
      
      console.log('⚠️ Test Execution phase completed with failures');
      
    } else {
      await reporting.updatePRPStatus('Test Execution', 'completed', 
        'No tests were executed at ' + new Date().toISOString());
      
      console.log('ℹ️ Test Execution phase completed - no tests executed');
    }
    
    // Generate and save final test report
    await reporting.saveTestReport();
    
    // Display summary
    console.log('\n📊 Test Execution Summary:');
    console.log(`   Total Tests: ${overallResults.totalTests}`);
    console.log(`   Passed: ${overallResults.passedTests}`);
    console.log(`   Failed: ${overallResults.failedTests}`);
    console.log(`   Skipped: ${overallResults.skippedTests}`);
    console.log(`   Blocked: ${overallResults.blockedTests}`);
    console.log(`   Coverage: ${overallResults.coverage}%`);
    
    console.log('\n📋 Phase Status:');
    for (const [phaseName, phaseStatus] of Object.entries(allPhases)) {
      const statusIcon = phaseStatus.status === 'completed' ? '✅' : 
                        phaseStatus.status === 'in-progress' ? '🔄' : 
                        phaseStatus.status === 'blocked' ? '🚫' : '⏳';
      
      console.log(`   ${statusIcon} ${phaseName}: ${phaseStatus.status}`);
    }
    
    // Export final status to JSON
    const finalStatus = await reporting.exportStatusToJson();
    
    const statusPath = './tests/e2e/status/final-prp-status.json';
    fs.writeFileSync(statusPath, finalStatus);
    console.log(`\n💾 Final PRP status saved to: ${statusPath}`);
    
    console.log('\n✅ Global teardown completed successfully');
    
  } catch (error) {
    console.error('❌ Global teardown failed:', error);
    throw error;
  }
}

export default globalTeardown;
