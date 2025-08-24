/**
 * Application configuration store using Zustand
 * Manages app-wide settings, theme, language, and feature flags
 */

import { create } from 'zustand'
import { persist, createJSONStorage } from 'zustand/middleware'
import type { AppConfig } from '../models'

interface AppConfigState extends AppConfig {
    // Actions
    setTheme: (theme: Partial<AppConfig['theme']>) => void
    setLocale: (locale: Partial<AppConfig['locale']>) => void
    toggleDarkMode: () => void
    setLanguage: (language: 'en' | 'ar') => void
    setFeature: (feature: keyof AppConfig['features'], enabled: boolean) => void
    resetToDefaults: () => void
}

// Default configuration
const defaultConfig: AppConfig = {
    apiBaseUrl: import.meta.env.VITE_API_URL || 'https://localhost:7001/api',
    environment: 'development',
    features: {
        darkMode: true,
        notifications: true,
        analytics: false
    },
    theme: {
        primaryColor: '#3B82F6',
        secondaryColor: '#6B7280',
        mode: 'light'
    },
    locale: {
        language: 'en',
        direction: 'ltr',
        timeZone: 'UTC'
    }
}

export const useAppConfigStore = create<AppConfigState>()(
    persist(
        (set, get) => ({
            // Initial state from defaults
            ...defaultConfig,

            // Set theme action
            setTheme: (themeUpdate: Partial<AppConfig['theme']>) => {
                const currentTheme = get().theme
                set({
                    theme: { ...currentTheme, ...themeUpdate }
                })
            },

            // Set locale action
            setLocale: (localeUpdate: Partial<AppConfig['locale']>) => {
                const currentLocale = get().locale
                set({
                    locale: { ...currentLocale, ...localeUpdate }
                })
            },

            // Toggle dark mode
            toggleDarkMode: () => {
                const currentMode = get().theme.mode
                set({
                    theme: {
                        ...get().theme,
                        mode: currentMode === 'light' ? 'dark' : 'light'
                    }
                })
            },

            // Set language and direction
            setLanguage: (language: 'en' | 'ar') => {
                const direction = language === 'ar' ? 'rtl' : 'ltr'
                set({
                    locale: {
                        ...get().locale,
                        language,
                        direction
                    }
                })
            },

            // Set feature flag
            setFeature: (feature: keyof AppConfig['features'], enabled: boolean) => {
                const currentFeatures = get().features
                set({
                    features: {
                        ...currentFeatures,
                        [feature]: enabled
                    }
                })
            },

            // Reset to defaults
            resetToDefaults: () => {
                set(defaultConfig)
            }
        }),
        {
            name: 'ikhtibar-app-config',
            storage: createJSONStorage(() => localStorage),
            // Don't persist apiBaseUrl and environment - these should come from env vars
            partialize: (state) => ({
                features: state.features,
                theme: state.theme,
                locale: state.locale
            })
        }
    )
)

// Selectors for commonly used config parts
export const useTheme = () => useAppConfigStore((state) => state.theme)
export const useLocale = () => useAppConfigStore((state) => state.locale)
export const useFeatures = () => useAppConfigStore((state) => state.features)
export const useDarkMode = () => useAppConfigStore((state) => state.theme.mode === 'dark')
export const useLanguage = () => useAppConfigStore((state) => state.locale.language)
export const useDirection = () => useAppConfigStore((state) => state.locale.direction)

// Action selectors
export const useAppConfigActions = () => useAppConfigStore((state) => ({
    setTheme: state.setTheme,
    setLocale: state.setLocale,
    toggleDarkMode: state.toggleDarkMode,
    setLanguage: state.setLanguage,
    setFeature: state.setFeature,
    resetToDefaults: state.resetToDefaults
}))

// Computed selectors
export const useIsRTL = () => useAppConfigStore((state) => state.locale.direction === 'rtl')
export const useThemeClasses = () => {
    const theme = useTheme()
    const isDark = useDarkMode()
    
    return {
        primary: theme.primaryColor,
        secondary: theme.secondaryColor,
        mode: theme.mode,
        isDark,
        containerClass: `theme-${theme.mode}`,
        directionClass: useDirection()
    }
}
