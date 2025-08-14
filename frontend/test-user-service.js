/**
 * Test script to validate user service transformation
 * Run this in browser console to test the transformation logic
 */

// Mock backend response (what we actually get from API)
const mockBackendResponse = {
  items: [
    {
      userId: 10,
      username: "admin",
      email: "admin@ikhtibar.com",
      firstName: "Super",
      lastName: "Administrator",
      phoneNumber: null,
      preferredLanguage: "en",
      isActive: true,
      emailVerified: true,
      phoneVerified: false,
      createdAt: "2025-07-26T13:39:22.7105454",
      updatedAt: "2025-07-26T13:39:22.7105454",
      roles: [
        {
          roleId: 1,
          name: "SystemAdmin",
          code: "system-admin"
        }
      ]
    }
  ],
  page: 1,
  pageSize: 10,
  totalCount: 10,  // Backend uses this
  totalPages: 1,
  hasPreviousPage: false,
  hasNextPage: false
};

// Transformation function (copy from userService.ts)
function transformUserFromBackend(backendUser) {
  return {
    id: backendUser.userId,  // Backend uses 'userId', frontend expects 'id'
    username: backendUser.username || '',
    email: backendUser.email || '',
    firstName: backendUser.firstName || '',
    lastName: backendUser.lastName || '',
    fullName: `${backendUser.firstName || ''} ${backendUser.lastName || ''}`.trim(),
    phoneNumber: backendUser.phoneNumber || undefined,
    preferredLanguage: backendUser.preferredLanguage || undefined,
    isActive: backendUser.isActive || false,
    emailVerified: backendUser.emailVerified || false,
    phoneVerified: backendUser.phoneVerified || false,
    roles: backendUser.roles?.map((role) => role.name || role.code || '') || [],
    permissions: [], // TODO: Add permissions mapping when available
    createdAt: backendUser.createdAt || new Date().toISOString(),
    modifiedAt: backendUser.updatedAt || undefined,
    lastLoginAt: backendUser.lastLoginAt || undefined,
  };
}

// Transform the response
const transformedResponse = {
  items: mockBackendResponse.items?.map((user) => transformUserFromBackend(user)) || [],
  total: mockBackendResponse.totalCount || 0,  // Backend uses 'totalCount', frontend expects 'total'
  page: mockBackendResponse.page || 1,
  pageSize: mockBackendResponse.pageSize || 10,
  totalPages: mockBackendResponse.totalPages || 0,
  hasNextPage: mockBackendResponse.hasNextPage || false,
  hasPreviousPage: mockBackendResponse.hasPreviousPage || false,
};

console.log('=== TRANSFORMATION TEST ===');
console.log('Backend Response:', mockBackendResponse);
console.log('Frontend Expected:', transformedResponse);
console.log('');
console.log('✅ Validation:');
console.log('- User ID mapping (userId → id):', transformedResponse.items[0].id === 10);
console.log('- Total count mapping (totalCount → total):', transformedResponse.total === 10);
console.log('- Full name generation:', transformedResponse.items[0].fullName === 'Super Administrator');
console.log('- Roles mapping:', transformedResponse.items[0].roles.includes('SystemAdmin'));
