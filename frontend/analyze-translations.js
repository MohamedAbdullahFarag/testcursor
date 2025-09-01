const fs = require('fs');
const path = require('path');

// Function to recursively find all files with specific extensions
function findFiles(dir, extensions) {
  const files = [];
  
  function traverse(currentDir) {
    try {
      const entries = fs.readdirSync(currentDir, { withFileTypes: true });
      
      for (const entry of entries) {
        const fullPath = path.join(currentDir, entry.name);
        
        if (entry.isDirectory() && !entry.name.startsWith('.') && entry.name !== 'node_modules') {
          traverse(fullPath);
        } else if (entry.isFile() && extensions.some(ext => entry.name.endsWith(ext))) {
          files.push(fullPath);
        }
      }
    } catch (error) {
      console.error(`Error reading directory ${currentDir}:`, error.message);
    }
  }
  
  traverse(dir);
  return files;
}

// Function to extract translation keys from files
function extractTranslationKeys(content) {
  const keys = new Set();
  
  // Match t('key') or t("key") patterns
  const tPatterns = [
    /t\(['"`]([^'"`]+)['"`]\)/g,
    /t\(['"`]([^'"`]+)['"`],\s*\{[^}]*\}/g
  ];
  
  tPatterns.forEach(pattern => {
    let match;
    while ((match = pattern.exec(content)) !== null) {
      keys.add(match[1]);
    }
  });
  
  return Array.from(keys);
}

// Function to extract available keys from locale files
function extractAvailableKeys(localeContent) {
  const keys = new Set();
  
  // Match key: value patterns in locale files
  const keyPatterns = [
    /(\w+):\s*['"`][^'"`]*['"`]/g,
    /['"`](\w+)['"`]:\s*['"`][^'"`]*['"`]/g
  ];
  
  keyPatterns.forEach(pattern => {
    let match;
    while ((match = pattern.exec(localeContent)) !== null) {
      keys.add(match[1]);
    }
  });
  
  return Array.from(keys);
}

// Main analysis function
function analyzeTranslations() {
  const srcDir = path.join(__dirname, 'src');
  const results = {
    modules: {},
    missingKeys: {},
    summary: {
      totalModules: 0,
      totalMissingKeys: 0,
      modulesMissingTranslations: []
    }
  };
  
  // Find all React component files
  const componentFiles = findFiles(srcDir, ['.tsx', '.ts']).filter(file => 
    !file.includes('__tests__') && 
    !file.includes('.test.') && 
    !file.includes('.spec.') &&
    !file.includes('node_modules')
  );
  
  // Find all locale files
  const localeFiles = findFiles(srcDir, ['.ts']).filter(file => 
    file.includes('/locales/') && 
    !file.includes('index.ts')
  );
  
  console.log(`Found ${componentFiles.length} component files`);
  console.log(`Found ${localeFiles.length} locale files`);
  
  // Analyze each module
  const modulesByPath = {};
  
  componentFiles.forEach(file => {
    try {
      const content = fs.readFileSync(file, 'utf8');
      const usedKeys = extractTranslationKeys(content);
      
      if (usedKeys.length > 0) {
        // Determine module path
        const relativePath = path.relative(srcDir, file);
        const pathParts = relativePath.split(path.sep);
        
        let moduleName = 'shared';
        if (pathParts[0] === 'modules' && pathParts.length > 1) {
          moduleName = pathParts[1];
        } else if (pathParts[0] === 'pages') {
          moduleName = 'pages';
        }
        
        if (!modulesByPath[moduleName]) {
          modulesByPath[moduleName] = {
            files: [],
            usedKeys: new Set(),
            availableKeys: new Set()
          };
        }
        
        modulesByPath[moduleName].files.push(file);
        usedKeys.forEach(key => modulesByPath[moduleName].usedKeys.add(key));
      }
    } catch (error) {
      console.error(`Error processing file ${file}:`, error.message);
    }
  });
  
  // Load available keys from locale files
  localeFiles.forEach(file => {
    try {
      const content = fs.readFileSync(file, 'utf8');
      const availableKeys = extractAvailableKeys(content);
      
      const relativePath = path.relative(srcDir, file);
      const pathParts = relativePath.split(path.sep);
      
      let moduleName = 'shared';
      if (pathParts[0] === 'modules' && pathParts.length > 1) {
        moduleName = pathParts[1];
      } else if (pathParts.includes('shared')) {
        moduleName = 'shared';
      }
      
      if (!modulesByPath[moduleName]) {
        modulesByPath[moduleName] = {
          files: [],
          usedKeys: new Set(),
          availableKeys: new Set()
        };
      }
      
      availableKeys.forEach(key => modulesByPath[moduleName].availableKeys.add(key));
    } catch (error) {
      console.error(`Error processing locale file ${file}:`, error.message);
    }
  });
  
  // Analyze missing keys for each module
  Object.keys(modulesByPath).forEach(moduleName => {
    const module = modulesByPath[moduleName];
    const usedKeys = Array.from(module.usedKeys);
    const availableKeys = Array.from(module.availableKeys);
    const missingKeys = usedKeys.filter(key => !availableKeys.includes(key));
    
    results.modules[moduleName] = {
      usedKeys: usedKeys.sort(),
      availableKeys: availableKeys.sort(),
      missingKeys: missingKeys.sort(),
      files: module.files
    };
    
    if (missingKeys.length > 0) {
      results.missingKeys[moduleName] = missingKeys;
      results.summary.modulesMissingTranslations.push(moduleName);
      results.summary.totalMissingKeys += missingKeys.length;
    }
    
    results.summary.totalModules++;
  });
  
  return results;
}

// Run analysis
console.log('Analyzing translation usage...\n');
const results = analyzeTranslations();

// Print summary
console.log('='.repeat(80));
console.log('TRANSLATION ANALYSIS SUMMARY');
console.log('='.repeat(80));
console.log(`Total modules analyzed: ${results.summary.totalModules}`);
console.log(`Modules with missing translations: ${results.summary.modulesMissingTranslations.length}`);
console.log(`Total missing translation keys: ${results.summary.totalMissingKeys}`);
console.log();

// Print detailed results for modules with missing keys
if (results.summary.modulesMissingTranslations.length > 0) {
  console.log('MODULES WITH MISSING TRANSLATION KEYS:');
  console.log('-'.repeat(50));
  
  results.summary.modulesMissingTranslations.forEach(moduleName => {
    const module = results.modules[moduleName];
    console.log(`\n📁 Module: ${moduleName}`);
    console.log(`   Used keys: ${module.usedKeys.length}`);
    console.log(`   Available keys: ${module.availableKeys.length}`);
    console.log(`   Missing keys (${module.missingKeys.length}):`);
    
    module.missingKeys.forEach(key => {
      console.log(`     ❌ ${key}`);
    });
  });
} else {
  console.log('✅ No missing translation keys found!');
}

// Print modules without missing keys
const modulesWithoutMissing = Object.keys(results.modules).filter(
  moduleName => !results.summary.modulesMissingTranslations.includes(moduleName)
);

if (modulesWithoutMissing.length > 0) {
  console.log('\n\nMODULES WITH COMPLETE TRANSLATIONS:');
  console.log('-'.repeat(50));
  
  modulesWithoutMissing.forEach(moduleName => {
    const module = results.modules[moduleName];
    console.log(`✅ ${moduleName}: ${module.usedKeys.length} keys (all available)`);
  });
}

console.log('\n' + '='.repeat(80));

// Save detailed results to file
const outputPath = path.join(__dirname, 'translation-analysis.json');
fs.writeFileSync(outputPath, JSON.stringify(results, null, 2));
console.log(`Detailed results saved to: ${outputPath}`);
