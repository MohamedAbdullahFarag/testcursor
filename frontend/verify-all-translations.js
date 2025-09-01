#!/usr/bin/env node

/**
 * Comprehensive Translation Verification Script
 * Verifies that all translation keys used in components exist in locale files
 */

import fs from 'fs';
import path from 'path';
import { glob } from 'glob';
import { fileURLToPath } from 'url';

const __dirname = path.dirname(fileURLToPath(import.meta.url));

console.log('🔍 Starting comprehensive translation verification...\n');

// Define modules and their namespace mappings
const moduleConfigs = [
  {
    name: 'notifications',
    namespace: 'notifications',
    localeFiles: {
      en: 'src/modules/notifications/locales/en.ts',
      ar: 'src/modules/notifications/locales/ar.ts'
    },
    componentPattern: 'src/modules/notifications/**/*.tsx'
  },
  {
    name: 'role-management',
    namespace: 'roleManagement', 
    localeFiles: {
      en: 'src/modules/role-management/locales/en.ts',
      ar: 'src/modules/role-management/locales/ar.ts'
    },
    componentPattern: 'src/modules/role-management/**/*.tsx'
  },
  {
    name: 'media-management',
    namespace: 'media-management',
    localeFiles: {
      en: 'src/modules/media-management/locales/en.ts', 
      ar: 'src/modules/media-management/locales/ar.ts'
    },
    componentPattern: 'src/modules/media-management/**/*.tsx'
  },
  {
    name: 'content-management',
    namespace: 'content-managment',
    localeFiles: {
      en: 'src/modules/content-managment/locales/en.ts',
      ar: 'src/modules/content-managment/locales/ar.ts'
    },
    componentPattern: 'src/modules/content-managment/**/*.tsx'
  },
  {
    name: 'auth',
    namespace: 'auth',
    localeFiles: {
      en: 'src/modules/auth/locales/en.ts',
      ar: 'src/modules/auth/locales/ar.ts'
    },
    componentPattern: 'src/modules/auth/**/*.tsx'
  },
  {
    name: 'media',
    namespace: 'media',
    localeFiles: {
      en: 'src/modules/question-bank/media/locales/en.ts',
      ar: 'src/modules/question-bank/media/locales/ar.ts'
    },
    componentPattern: 'src/modules/question-bank/media/**/*.tsx'
  },
  {
    name: 'categoryPages',
    namespace: 'categoryPages',
    localeFiles: {
      en: 'src/shared/locales/categoryPages.ts',
      ar: 'src/shared/locales/categoryPagesAr.ts'
    },
    componentPattern: 'src/pages/*CategoryPage.tsx'
  }
];

// Helper function to extract translation keys from a file
function extractTranslationKeys(filePath, targetNamespace) {
  try {
    const content = fs.readFileSync(filePath, 'utf8');
    
    // Check if this file uses the target namespace
    const namespaceRegex = new RegExp(`useTranslation\\(['"]${targetNamespace}['"]\\)`);
    if (!namespaceRegex.test(content)) {
      return [];
    }

    // Extract t('key') patterns
    const tKeyRegex = /t\(['"]([^'"]+)['"]\)/g;
    const keys = [];
    let match;
    
    while ((match = tKeyRegex.exec(content)) !== null) {
      keys.push(match[1]);
    }
    
    return keys;
  } catch (error) {
    console.warn(`⚠️  Could not read file: ${filePath}`);
    return [];
  }
}

// Helper function to get all keys from locale object
function getAllKeysFromObject(obj, prefix = '') {
  const keys = [];
  
  for (const key in obj) {
    if (typeof obj[key] === 'object' && obj[key] !== null && !Array.isArray(obj[key])) {
      keys.push(...getAllKeysFromObject(obj[key], prefix ? `${prefix}.${key}` : key));
    } else {
      keys.push(prefix ? `${prefix}.${key}` : key);
    }
  }
  
  return keys;
}

// Helper function to load locale file
function loadLocaleFile(filePath) {
  try {
    const fullPath = path.resolve(filePath);
    if (!fs.existsSync(fullPath)) {
      console.warn(`⚠️  Locale file not found: ${filePath}`);
      return {};
    }
    
    const content = fs.readFileSync(fullPath, 'utf8');
    
    // Simple parsing to extract the exported object
    const exportMatch = content.match(/export\s+(?:const|default)\s+\w*\s*=\s*({[\s\S]*?});?\s*$/m);
    if (exportMatch) {
      try {
        // Use Function constructor to safely evaluate the object
        const objectStr = exportMatch[1];
        const evalFunc = new Function('return ' + objectStr);
        return evalFunc();
      } catch (e) {
        console.warn(`⚠️  Could not parse locale file: ${filePath}`, e.message);
        return {};
      }
    }
    
    return {};
  } catch (error) {
    console.warn(`⚠️  Could not load locale file: ${filePath}`, error.message);
    return {};
  }
}

let totalModules = 0;
let passedModules = 0;
let totalMissingKeys = 0;

// Verify each module
for (const config of moduleConfigs) {
  console.log(`📦 Checking ${config.name} module (${config.namespace})...`);
  totalModules++;
  
  // Find component files
  const componentFiles = glob.sync(config.componentPattern);
  
  if (componentFiles.length === 0) {
    console.log(`   ⚠️  No component files found for pattern: ${config.componentPattern}`);
    continue;
  }
  
  // Extract all used translation keys
  const usedKeys = new Set();
  componentFiles.forEach(file => {
    const keys = extractTranslationKeys(file, config.namespace);
    keys.forEach(key => usedKeys.add(key));
  });
  
  if (usedKeys.size === 0) {
    console.log(`   ✅ No translation keys used`);
    passedModules++;
    continue;
  }
  
  console.log(`   📋 Found ${usedKeys.size} translation keys used`);
  
  // Check English locale
  const enLocale = loadLocaleFile(config.localeFiles.en);
  const enKeys = new Set(getAllKeysFromObject(enLocale));
  
  // Check Arabic locale  
  const arLocale = loadLocaleFile(config.localeFiles.ar);
  const arKeys = new Set(getAllKeysFromObject(arLocale));
  
  // Find missing keys
  const missingEnKeys = [...usedKeys].filter(key => !enKeys.has(key));
  const missingArKeys = [...usedKeys].filter(key => !arKeys.has(key));
  
  if (missingEnKeys.length === 0 && missingArKeys.length === 0) {
    console.log(`   ✅ All translation keys are present in both locales`);
    passedModules++;
  } else {
    console.log(`   ❌ Found missing translation keys:`);
    
    if (missingEnKeys.length > 0) {
      console.log(`      English missing: ${missingEnKeys.join(', ')}`);
      totalMissingKeys += missingEnKeys.length;
    }
    
    if (missingArKeys.length > 0) {
      console.log(`      Arabic missing: ${missingArKeys.join(', ')}`);
      totalMissingKeys += missingArKeys.length;
    }
  }
  
  console.log('');
}

// Summary
console.log('📊 Translation Verification Summary:');
console.log(`   Total modules checked: ${totalModules}`);
console.log(`   Modules passed: ${passedModules}`);
console.log(`   Modules with issues: ${totalModules - passedModules}`);
console.log(`   Total missing keys: ${totalMissingKeys}`);

if (totalMissingKeys === 0) {
  console.log('\n🎉 All translations are complete! No missing keys found.');
  process.exit(0);
} else {
  console.log('\n⚠️  Some translation keys are missing. Please add them to the respective locale files.');
  process.exit(1);
}
