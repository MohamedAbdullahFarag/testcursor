import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import LanguageDetector from 'i18next-browser-languagedetector';
import Backend from 'i18next-http-backend';

// Import translation files directly for better bundling
import { en as enTranslations } from './locales/en';
import { ar as arTranslations } from './locales/ar';

/**
 * i18next configuration for internationalization
 * Supports English (en) and Arabic (ar) languages
 * Includes automatic language detection and fallback mechanisms
 */
i18n
  // Load translation using http backend
  .use(Backend)
  // Detect user language
  .use(LanguageDetector)
  // Pass the i18n instance to react-i18next
  .use(initReactI18next)
  // Initialize i18next
  .init({
    // Fallback language
    fallbackLng: 'en',
    
    // Supported languages
    supportedLngs: ['en', 'ar'],
    
    // Debug mode (only in development)
    debug: import.meta.env.DEV,

    // Language detection configuration
    detection: {
      // Detection order - check localStorage first, then browser language
      order: ['localStorage', 'navigator', 'htmlTag'],
      
      // Cache user language in localStorage
      caches: ['localStorage'],
      
      // Key to store language in localStorage
      lookupLocalStorage: 'i18nextLng',
    },

    // Interpolation configuration
    interpolation: {
      escapeValue: false, // React already does escaping
    },

    // Namespace configuration
    defaultNS: 'common',
    ns: ['common', 'userManagement', 'users'],

    // Resources (translations)
    resources: {
      en: {
        common: enTranslations,
        userManagement: enTranslations.userManagement,
        users: enTranslations.users,
      },
      ar: {
        common: arTranslations,
        userManagement: arTranslations.userManagement,
        users: arTranslations.users,
      },
    },

    // React-specific configuration
    react: {
      // Use React Suspense for loading translations
      useSuspense: false,
      
      // Bind i18n instance to React component tree
      bindI18n: 'languageChanged',
      
      // Bind store to React component tree
      bindI18nStore: '',
      
      // Use HTML tags in translations (be careful with user input)
      transSupportBasicHtmlNodes: true,
      transKeepBasicHtmlNodesFor: ['br', 'strong', 'i', 'em'],
    },

    // Backend configuration (if using HTTP backend)
    backend: {
      // Path where resources get loaded from
      loadPath: '/locales/{{lng}}/{{ns}}.json',
      
      // Allow cross domain requests
      crossDomain: false,
      
      // Allow credentials on cross domain requests
      withCredentials: false,
    },
  });

export default i18n;
