import '@testing-library/jest-dom'
import { vi } from 'vitest'

// Mock i18next with proper translations
const translations: Record<string, string> = {
  'auth.login': 'Login',
  'auth.logout': 'Logout',
  'auth.email': 'Email Address',
  'auth.password': 'Password',
  'auth.emailPlaceholder': 'Enter your email',
  'auth.passwordPlaceholder': 'Enter your password',
  'auth.loggingIn': 'Signing in...',
  'auth.loginTitle': 'Welcome to the Unified Portal',
  'auth.loginDescription': 'Enter your credentials to access your account',
  'auth.loginError': 'Login failed',
  'common.loading': 'Loading...',
  'common.error': 'Error',
  'common.retry': 'Retry',
  'common.close': 'Close',
  'errors.emailRequired': 'Email is required',
  'errors.passwordRequired': 'Password is required',
  'errors.invalidCredentials': 'Invalid credentials',
  'errors.loginFailed': 'Login failed',
  'errors.somethingWentWrong': 'Something went wrong',
  // Add hardcoded error messages from validation
  'Email is required': 'Email is required',
  'Password is required': 'Password is required',
}

const mockT = (key: string) => translations[key] || key
const mockI18n = {
  t: mockT,
  changeLanguage: () => Promise.resolve(),
  language: 'en',
}

vi.mock('react-i18next', () => ({
  useTranslation: () => ({
    t: mockT,
    i18n: mockI18n,
  }),
  I18nextProvider: ({ children }: { children: React.ReactNode }) => children,
  initReactI18next: {
    type: '3rdParty',
    init: () => {},
  },
}))

// Mock IntersectionObserver
global.IntersectionObserver = class IntersectionObserver {
  root: Element | null = null
  rootMargin: string = ''
  thresholds: ReadonlyArray<number> = []
  
  constructor() {}
  disconnect() {}
  observe() {}
  unobserve() {}
  takeRecords(): IntersectionObserverEntry[] {
    return []
  }
}

// Mock ResizeObserver
global.ResizeObserver = class ResizeObserver {
  constructor() {}
  disconnect() {}
  observe() {}
  unobserve() {}
}

// Mock matchMedia
Object.defineProperty(window, 'matchMedia', {
  writable: true,
  value: vi.fn().mockImplementation((query: string) => ({
    matches: false,
    media: query,
    onchange: null,
    addListener: vi.fn(), // deprecated
    removeListener: vi.fn(), // deprecated
    addEventListener: vi.fn(),
    removeEventListener: vi.fn(),
    dispatchEvent: vi.fn(),
  })),
})

// Mock fetch
global.fetch = vi.fn()
