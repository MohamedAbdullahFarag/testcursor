/**
 * Centralized locale exports for i18next
 * This file exports all translations for the application
 */

// Export translation objects for i18next
export { en } from './en'
export { ar } from './ar'

// Export default translations for backward compatibility
export const translations = {
    en: () => import('./en').then(m => m.en),
    ar: () => import('./ar').then(m => m.ar),
}

// Legacy support for existing components (will be phased out)
import LocalizedStrings from 'react-localization'
import { ar } from './ar'
import { en } from './en'

export const strings = new LocalizedStrings({
    ar,
    en,
})
