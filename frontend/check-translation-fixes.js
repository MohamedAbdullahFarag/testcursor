#!/usr/bin/env node

// Simple translation checker
import fs from 'fs';

console.log('🔍 Checking key translation fixes...\n');

// Check media-management loading states
try {
  const enContent = fs.readFileSync('src/modules/media-management/locales/en.ts', 'utf8');
  const arContent = fs.readFileSync('src/modules/media-management/locales/ar.ts', 'utf8');
  
  const hasLoadingStates = enContent.includes('loadingStates') && arContent.includes('loadingStates');
  const hasMediaFiles = enContent.includes('mediaFiles') && arContent.includes('mediaFiles');
  
  if (hasLoadingStates && hasMediaFiles) {
    console.log('✅ Media-management loading states: Fixed');
  } else {
    console.log('❌ Media-management loading states: Missing');
  }
} catch (e) {
  console.log('⚠️ Could not check media-management files');
}

// Check if MediaLibrary uses correct key
try {
  const mediaLibContent = fs.readFileSync('src/modules/media-management/components/MediaLibrary.tsx', 'utf8');
  
  if (mediaLibContent.includes('loadingStates.mediaFiles')) {
    console.log('✅ MediaLibrary component: Using correct translation key');
  } else if (mediaLibContent.includes('loading.mediaFiles')) {
    console.log('❌ MediaLibrary component: Still using old translation key');
  } else {
    console.log('⚠️ MediaLibrary component: Loading key not found');
  }
} catch (e) {
  console.log('⚠️ Could not check MediaLibrary component');
}

console.log('\n✨ Translation fixes verification complete!');
