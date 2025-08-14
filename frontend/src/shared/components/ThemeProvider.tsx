/**
 * Theme Provider using React Context API
 * Provides theme configuration and dark/light mode support
 */

import React, { createContext, useContext, useEffect, ReactNode } from 'react'
import { useTheme, useAppConfigActions, useDarkMode, useDirection } from '../store/appConfigStore'

interface ThemeContextValue {
    theme: {
        primaryColor: string
        secondaryColor: string
        mode: 'light' | 'dark'
    }
    isDark: boolean
    direction: 'ltr' | 'rtl'
    toggleDarkMode: () => void
    setTheme: (theme: Partial<{ primaryColor: string; secondaryColor: string; mode: 'light' | 'dark' }>) => void
}

const ThemeContext = createContext<ThemeContextValue | undefined>(undefined)

interface ThemeProviderProps {
    children: ReactNode
}

export const ThemeProvider: React.FC<ThemeProviderProps> = ({ children }) => {
    const theme = useTheme()
    const isDark = useDarkMode()
    const direction = useDirection()
    const { toggleDarkMode, setTheme } = useAppConfigActions()

    // Apply theme classes to document root
    useEffect(() => {
        const root = document.documentElement
        
        // Set theme mode class
        root.classList.remove('light', 'dark')
        root.classList.add(theme.mode)
        
        // Set direction
        document.dir = direction
        
        // Set CSS custom properties for colors
        root.style.setProperty('--color-primary', theme.primaryColor)
        root.style.setProperty('--color-secondary', theme.secondaryColor)
        
        // Set theme attribute for better CSS targeting
        root.setAttribute('data-theme', theme.mode)
        root.setAttribute('data-direction', direction)
        
    }, [theme, direction])

    const value: ThemeContextValue = {
        theme,
        isDark,
        direction,
        toggleDarkMode,
        setTheme
    }

    return (
        <ThemeContext.Provider value={value}>
            {children}
        </ThemeContext.Provider>
    )
}

/**
 * Hook to use theme context
 */
export const useThemeContext = (): ThemeContextValue => {
    const context = useContext(ThemeContext)
    if (context === undefined) {
        throw new Error('useThemeContext must be used within a ThemeProvider')
    }
    return context
}

export default ThemeProvider
