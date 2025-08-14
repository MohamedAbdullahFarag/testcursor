import { vi, describe, it, expect, beforeEach } from 'vitest';
import { roleService } from '../services/roleService';

// Mock fetch globally
const mockFetch = vi.fn();
global.fetch = mockFetch;

/**
 * Test suite for Role Service API integration.
 * Tests API calls, response handling, and error scenarios.
 */
describe('RoleService', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    // Reset environment variable
    import.meta.env.VITE_API_BASE_URL = 'http://localhost:5000';
  });

  /**
   * Test: getRoles should make correct API call
   * Scenario: Valid pagination parameters provided
   * Expected: Correct URL and parameters sent to API
   */
  it('should make correct API call for getRoles', async () => {
    // Arrange
    const mockResponse = {
      data: [
        {
          roleId: 1,
          code: 'admin',
          name: 'Administrator',
          description: 'System administrator',
          isSystemRole: true,
        },
      ],
      pagination: {
        page: 1,
        pageSize: 10,
        totalPages: 1,
        totalItems: 1,
      },
    };

    mockFetch.mockResolvedValueOnce({
      ok: true,
      json: () => Promise.resolve(mockResponse),
    });

    const params = {
      page: 1,
      pageSize: 10,
      filters: {
        searchTerm: 'admin',
        includeSystemRoles: true,
        isActive: true,
      },
    };

    // Act
    const result = await roleService.getRoles(params);

    // Assert
    expect(mockFetch).toHaveBeenCalledWith(
      expect.stringContaining('/api/roles'),
      expect.objectContaining({
        method: 'GET',
      })
    );

    expect(result).toEqual(mockResponse);
  });

  /**
   * Test: createRole should make correct API call
   * Scenario: Valid role data provided
   * Expected: POST request with correct data
   */
  it('should make correct API call for createRole', async () => {
    // Arrange
    const newRole = {
      code: 'editor',
      name: 'Content Editor',
      description: 'Can edit content',
      permissionIds: [1, 2, 3],
    };

    const mockResponse = {
      roleId: 2,
      ...newRole,
      isSystemRole: false,
      isActive: true,
    };

    mockFetch.mockResolvedValueOnce({
      ok: true,
      json: () => Promise.resolve(mockResponse),
    });

    // Act
    const result = await roleService.createRole(newRole);

    // Assert
    expect(mockFetch).toHaveBeenCalledWith(
      expect.stringContaining('/api/roles'),
      expect.objectContaining({
        method: 'POST',
        headers: expect.objectContaining({
          'Content-Type': 'application/json',
        }),
        body: JSON.stringify(newRole),
      })
    );

    expect(result).toEqual(mockResponse);
  });

  /**
   * Test: deleteRole should make correct API call
   * Scenario: Valid role ID provided
   * Expected: DELETE request to correct endpoint
   */
  it('should make correct API call for deleteRole', async () => {
    // Arrange
    const roleId = 2;

    mockFetch.mockResolvedValueOnce({
      ok: true,
      json: () => Promise.resolve(true),
    });

    // Act
    const result = await roleService.deleteRole(roleId);

    // Assert
    expect(mockFetch).toHaveBeenCalledWith(
      expect.stringContaining(`/api/roles/${roleId}`),
      expect.objectContaining({
        method: 'DELETE',
      })
    );

    expect(result).toBe(true);
  });

  /**
   * Test: API should handle HTTP errors gracefully
   * Scenario: Server returns 404 error
   * Expected: Error thrown with appropriate message
   */
  it('should handle HTTP errors gracefully', async () => {
    // Arrange
    mockFetch.mockResolvedValueOnce({
      ok: false,
      status: 404,
      statusText: 'Not Found',
      json: () => Promise.resolve({ message: 'Role not found' }),
    });

    // Act & Assert
    await expect(roleService.getRole(999)).rejects.toThrow();
  });

  /**
   * Test: API should handle network errors
   * Scenario: Network request fails
   * Expected: Error thrown with network error message
   */
  it('should handle network errors', async () => {
    // Arrange
    mockFetch.mockRejectedValueOnce(new Error('Network error'));

    const params = {
      page: 1,
      pageSize: 10,
    };

    // Act & Assert
    await expect(roleService.getRoles(params)).rejects.toThrow('Network error');
  });

  /**
   * Test: updateRole should make correct API call
   * Scenario: Valid role ID and update data provided
   * Expected: PUT request with correct data
   */
  it('should make correct API call for updateRole', async () => {
    // Arrange
    const roleId = 1;
    const updateData = {
      name: 'Updated Role Name',
      description: 'Updated description',
      permissionIds: [1, 2],
    };

    const mockResponse = {
      roleId,
      code: 'admin',
      ...updateData,
      isSystemRole: true,
      isActive: true,
    };

    mockFetch.mockResolvedValueOnce({
      ok: true,
      json: () => Promise.resolve(mockResponse),
    });

    // Act
    const result = await roleService.updateRole(roleId, updateData);

    // Assert
    expect(mockFetch).toHaveBeenCalledWith(
      expect.stringContaining(`/api/roles/${roleId}`),
      expect.objectContaining({
        method: 'PUT',
        headers: expect.objectContaining({
          'Content-Type': 'application/json',
        }),
        body: JSON.stringify(updateData),
      })
    );

    expect(result).toEqual(mockResponse);
  });

  /**
   * Test: assignRoleToUser should make correct API call
   * Scenario: Valid user ID and role ID provided
   * Expected: POST request to user-roles endpoint
   */
  it('should make correct API call for assignRoleToUser', async () => {
    // Arrange
    const assignData = {
      userId: 1,
      roleId: 2,
    };

    mockFetch.mockResolvedValueOnce({
      ok: true,
      json: () => Promise.resolve(true),
    });

    // Act
    const result = await roleService.assignRoleToUser(assignData);

    // Assert
    expect(mockFetch).toHaveBeenCalledWith(
      expect.stringContaining('/api/user-roles'),
      expect.objectContaining({
        method: 'POST',
        headers: expect.objectContaining({
          'Content-Type': 'application/json',
        }),
        body: JSON.stringify(assignData),
      })
    );

    expect(result).toBe(true);
  });
});
