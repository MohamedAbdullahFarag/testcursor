import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import LanguageDetector from 'i18next-browser-languagedetector';

// Import translation files directly for better bundling
import { en as enTranslations } from './locales/en';
import { ar as arTranslations } from './locales/ar';

/**
 * i18next configuration for internationalization
 * Supports English (en) and Arabic (ar) languages
 * Includes automatic language detection and fallback mechanisms
 */
i18n
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
    
    // Debug mode (disabled to prevent excessive logging)
    debug: false,

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
    ns: ['common', 'userManagement', 'users', 'content-managment', 'categoryPages', 'roleManagement', 'media-management', 'notifications', 'auth', 'media'],

    // Resources (translations)
    resources: {
      en: {
        common: enTranslations.common,
        userManagement: enTranslations.userManagement,
        users: enTranslations.users,
        'content-managment': enTranslations['content-managment'],
        categoryPages: enTranslations.categoryPages,
        roleManagement: enTranslations.roleManagement,
        'media-management': enTranslations['media-management'],
        notifications: enTranslations.notifications,
        auth: enTranslations.auth,
        media: enTranslations.media,
      },
      ar: {
        common: arTranslations.common,
        userManagement: arTranslations.userManagement,
        users: arTranslations.users,
        'content-managment': arTranslations['content-managment'],
        categoryPages: arTranslations.categoryPages,
        roleManagement: arTranslations.roleManagement,
        'media-management': arTranslations['media-management'],
        notifications: arTranslations.notifications,
        auth: arTranslations.auth,
        media: arTranslations.media,
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
  });

export default i18n;
