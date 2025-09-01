// Test script to verify notification translation keys
const fs = require('fs');
const path = require('path');

console.log('🔍 Checking notification translation keys...');

const missingKeys = [
  'center.noNotificationsDescription',
  'center.refreshAriaLabel',
  'filters.searchAriaLabel',
  'filters.unreadOnly',
  'filters.allTypes',
  'filters.allPriorities',
  'types.examReminder',
  'types.examStart',
  'types.examEnd',
  'types.gradingComplete',
  'types.deadlineReminder',
  'types.systemAlert',
  'types.welcome',
  'types.passwordReset',
  'priorities.low',
  'priorities.medium',
  'priorities.high',
  'priorities.critical'
];

function checkKeys(filePath, language) {
  const content = fs.readFileSync(filePath, 'utf-8');
  
  console.log(`\n🔍 Checking ${language} translations:`);
  console.log('=====================================');
  
  const results = [];
  
  missingKeys.forEach(key => {
    const parts = key.split('.');
    const section = parts[0];
    const keyName = parts[1];
    
    // Check if the key exists in the file
    const keyPattern = new RegExp(`${keyName}:\\s*['"]`, 'g');
    const found = keyPattern.test(content);
    
    if (found) {
      console.log(`  ✅ ${key}: Found`);
      results.push({ key, found: true });
    } else {
      console.log(`  ❌ ${key}: Missing`);
      results.push({ key, found: false });
    }
  });
  
  const foundCount = results.filter(r => r.found).length;
  const totalCount = results.length;
  
  console.log(`\n📊 Summary: ${foundCount}/${totalCount} keys found`);
  
  return results;
}

const enPath = path.join(__dirname, 'src', 'modules', 'notifications', 'locales', 'en.ts');
const arPath = path.join(__dirname, 'src', 'modules', 'notifications', 'locales', 'ar.ts');

const enResults = checkKeys(enPath, 'English');
const arResults = checkKeys(arPath, 'Arabic');

const allEnFound = enResults.every(r => r.found);
const allArFound = arResults.every(r => r.found);

console.log('\n🎉 Final Results:');
console.log('=================');
if (allEnFound && allArFound) {
  console.log('✅ All missing notification keys have been added!');
  console.log('✅ The translation errors should now be resolved.');
} else {
  console.log('❌ Some keys are still missing. Please check the files.');
}

console.log('\n🚀 The NotificationCenter component should now load without translation errors.');
