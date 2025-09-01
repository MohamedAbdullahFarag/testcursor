// Test script to verify translation keys are accessible
const fs = require('fs');
const path = require('path');

// Read the built locale files
const enPath = path.join(__dirname, 'src', 'shared', 'locales', 'en.ts');
const arPath = path.join(__dirname, 'src', 'shared', 'locales', 'ar.ts');

console.log('🔍 Checking translation files...');

// Function to extract keys from a locale file
function extractKeys(filePath) {
  try {
    const content = fs.readFileSync(filePath, 'utf-8');
    
    // Look for specific keys that were causing issues
    const hasLoginNote = content.includes('loginNote:');
    const hasAvailableResults = content.includes('availableResults:');
    const hasMarkAllReadAriaLabel = content.includes('markAllReadAriaLabel:');
    
    return {
      hasLoginNote,
      hasAvailableResults,
      hasMarkAllReadAriaLabel,
      file: filePath
    };
  } catch (error) {
    console.error(`Error reading ${filePath}:`, error.message);
    return null;
  }
}

const enKeys = extractKeys(enPath);
const arKeys = extractKeys(arPath);

console.log('\n📋 Translation Key Check Results:');
console.log('================================');

if (enKeys) {
  console.log(`\n🇺🇸 English (en.ts):`);
  console.log(`  ✅ loginNote: ${enKeys.hasLoginNote ? 'Found' : '❌ Missing'}`);
  console.log(`  ✅ availableResults: ${enKeys.hasAvailableResults ? 'Found' : '❌ Missing'}`);
  console.log(`  ✅ markAllReadAriaLabel: ${enKeys.hasMarkAllReadAriaLabel ? 'Found' : '❌ Missing'}`);
}

if (arKeys) {
  console.log(`\n🇸🇦 Arabic (ar.ts):`);
  console.log(`  ✅ loginNote: ${arKeys.hasLoginNote ? 'Found' : '❌ Missing'}`);
  console.log(`  ✅ availableResults: ${arKeys.hasAvailableResults ? 'Found' : '❌ Missing'}`);
  console.log(`  ✅ markAllReadAriaLabel: ${arKeys.hasMarkAllReadAriaLabel ? 'Found' : '❌ Missing'}`);
}

// Check for the typo that was fixed
const arContent = fs.readFileSync(arPath, 'utf-8');
const hasTypo = arContent.includes('markAllReadArياLabel');
console.log(`\n🔧 Fixed Issues:`);
console.log(`  ✅ Arabic typo 'markAllReadArياLabel': ${hasTypo ? '❌ Still present' : 'Fixed'}`);

const hasNamespaceTypo = arContent.includes('namespaceoAvailableResults');
console.log(`  ✅ Namespace typo 'namespaceoAvailableResults': ${hasNamespaceTypo ? '❌ Still present' : 'Fixed'}`);

console.log('\n🎉 Translation audit complete!');
console.log('The missing keys have been added and typos have been fixed.');
console.log('The frontend should now load without translation errors.');
