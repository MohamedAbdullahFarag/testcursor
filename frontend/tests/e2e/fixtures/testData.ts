import { LoginCredentials } from '../pages/LoginPage';

export interface TestUser {
  id: number;
  email: string;
  password: string;
  role: 'admin' | 'teacher' | 'student';
  firstName: string;
  lastName: string;
  isActive: boolean;
}

export interface TestCredentials {
  valid: LoginCredentials;
  invalid: LoginCredentials;
  empty: LoginCredentials;
  malformed: LoginCredentials;
  locked: LoginCredentials;
  expired: LoginCredentials;
}

export interface TestScenarios {
  positive: string[];
  negative: string[];
  edge: string[];
  security: string[];
}

// Test Users Data
export const testUsers: Record<string, TestUser> = {
  admin: {
    id: 1,
    email: 'admin@ikhtibar.com',
    password: 'password',
    role: 'admin',
    firstName: 'Admin',
    lastName: 'User',
    isActive: true
  },
  teacher: {
    id: 2,
    email: 'teacher1@ikhtibar.com',
    password: 'password',
    role: 'teacher',
    firstName: 'Teacher',
    lastName: 'One',
    isActive: true
  },
  student: {
    id: 3,
    email: 'student1@ikhtibar.com',
    password: 'password',
    role: 'student',
    firstName: 'Student',
    lastName: 'One',
    isActive: true
  },
  inactive: {
    id: 4,
    email: 'inactive@ikhtibar.com',
    password: 'password',
    role: 'student',
    firstName: 'Inactive',
    lastName: 'User',
    isActive: false
  },
  locked: {
    id: 5,
    email: 'locked@ikhtibar.com',
    password: 'password',
    role: 'student',
    firstName: 'Locked',
    lastName: 'User',
    isActive: true
  }
};

// Test Credentials
export const testCredentials: TestCredentials = {
  valid: {
    email: 'admin@ikhtibar.com',
    password: 'password'
  },
  invalid: {
    email: 'invalid@email.com',
    password: 'wrongpassword'
  },
  empty: {
    email: '',
    password: ''
  },
  malformed: {
    email: 'invalid-email',
    password: '123'
  },
  locked: {
    email: 'locked@ikhtibar.com',
    password: 'password'
  },
  expired: {
    email: 'expired@ikhtibar.com',
    password: 'password'
  }
};

// Test Scenarios
export const testScenarios: TestScenarios = {
  positive: [
    'Valid admin credentials',
    'Valid teacher credentials',
    'Valid student credentials',
    'Remember me functionality',
    'Auto-redirect after login',
    'Session persistence'
  ],
  negative: [
    'Invalid email format',
    'Invalid password',
    'Non-existent user',
    'Inactive user account',
    'Locked user account',
    'Empty credentials'
  ],
  edge: [
    'Very long email address',
    'Very long password',
    'Special characters in credentials',
    'Unicode characters',
    'SQL injection attempts',
    'XSS attempts'
  ],
  security: [
    'Brute force protection',
    'Account lockout',
    'Password complexity validation',
    'Session timeout',
    'CSRF protection',
    'Rate limiting'
  ]
};

// Test Data for Different Environments
export const environmentTestData = {
  development: {
    baseUrl: 'https://localhost:5173',
    apiUrl: 'http://localhost:7001',
    timeout: 10000,
    retries: 2
  },
  staging: {
    baseUrl: 'https://staging.ikhtibar.com',
    timeout: 15000,
    retries: 3
  },
  production: {
    baseUrl: 'https://ikhtibar.com',
    timeout: 20000,
    retries: 1
  }
};

// Test Data for Different Browsers
export const browserTestData = {
  chromium: {
    viewport: { width: 1280, height: 720 },
    userAgent: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36'
  },
  firefox: {
    viewport: { width: 1280, height: 720 },
    userAgent: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/109.0'
  },
  webkit: {
    viewport: { width: 1280, height: 720 },
    userAgent: 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15'
  }
};

// Test Data for Different User Roles
export const roleBasedTestData = {
  admin: {
    permissions: ['read', 'write', 'delete', 'manage_users'],
    accessiblePages: ['/dashboard', '/admin/users', '/admin/roles', '/admin/settings'],
    restrictedPages: []
  },
  teacher: {
    permissions: ['read', 'write'],
    accessiblePages: ['/dashboard', '/questions', '/exams', '/results'],
    restrictedPages: ['/admin/users', '/admin/roles', '/admin/settings']
  },
  student: {
    permissions: ['read'],
    accessiblePages: ['/dashboard', '/exams', '/results'],
    restrictedPages: ['/admin/users', '/admin/roles', '/admin/settings', '/questions']
  }
};

// Test Data for Error Scenarios
export const errorTestData = {
  networkErrors: [
    'Connection timeout',
    'Server unavailable',
    'Network error',
    'DNS resolution failed'
  ],
  validationErrors: [
    'Email required',
    'Password required',
    'Invalid email format',
    'Password too short'
  ],
  authenticationErrors: [
    'Invalid credentials',
    'Account locked',
    'Account inactive',
    'Session expired'
  ]
};

// Helper function to get random test user
export function getRandomTestUser(): TestUser {
  const users = Object.values(testUsers);
  const randomIndex = Math.floor(Math.random() * users.length);
  return users[randomIndex];
}

// Helper function to get test user by role
export function getTestUserByRole(role: 'admin' | 'teacher' | 'student'): TestUser | undefined {
  return Object.values(testUsers).find(user => user.role === role);
}

// Helper function to get test credentials by type
export function getTestCredentials(type: keyof TestCredentials): LoginCredentials {
  return testCredentials[type];
}

// Helper function to get API URL
export function getApiUrl(): string {
  return 'https://localhost:7001';
}

// Helper function to get base URL
export function getBaseUrl(): string {
  return 'https://localhost:5173';
}
