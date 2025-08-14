import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { vi, describe, it, expect, beforeEach } from 'vitest';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { BrowserRouter } from 'react-router-dom';
import { RoleList } from '../components/RoleList';
import { roleService } from '../services/roleService';
import type { RoleListResponse } from '../models/role.types';

// Mock the role service
vi.mock('../services/roleService', () => ({
  roleService: {
    getRoles: vi.fn(),
    deleteRole: vi.fn(),
  },
}));

// Mock react-router-dom navigate
const mockNavigate = vi.fn();
vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom');
  return {
    ...actual,
    useNavigate: () => mockNavigate,
  };
});

/**
 * Test suite for RoleList component.
 * Tests role display, interaction handling, and error scenarios.
 * Uses Material-UI theme provider and React Query for realistic environment.
 */
describe('RoleList Component', () => {
  let queryClient: QueryClient;
  const theme = createTheme();

  // Test data with proper format for getRoles response
  const mockRoleListResponse: RoleListResponse = {
    items: [
      {
        id: 1,
        code: 'admin',
        name: 'Administrator',
        description: 'System administrator with full access',
        isSystemRole: true,
        isActive: true,
        permissions: ['user.read', 'user.write', 'role.read', 'role.write'],
        createdAt: '2023-01-01T00:00:00Z',
        updatedAt: '2023-01-01T00:00:00Z',
      },
      {
        id: 2,
        code: 'editor',
        name: 'Content Editor',
        description: 'Can edit and manage content',
        isSystemRole: false,
        isActive: true,
        permissions: ['content.read', 'content.write'],
        createdAt: '2023-01-02T00:00:00Z',
        updatedAt: '2023-01-02T00:00:00Z',
      },
      {
        id: 3,
        code: 'viewer',
        name: 'Content Viewer',
        description: 'Read-only access to content',
        isSystemRole: false,
        isActive: true,
        permissions: ['content.read'],
        createdAt: '2023-01-03T00:00:00Z',
        updatedAt: '2023-01-03T00:00:00Z',
      },
    ],
    totalCount: 3,
    page: 1,
    pageSize: 10,
  };

  /**
   * Wrapper component providing necessary context providers
   */
  const TestWrapper: React.FC<{ children: React.ReactNode }> = ({ children }) => (
    <BrowserRouter>
      <QueryClientProvider client={queryClient}>
        <ThemeProvider theme={theme}>
          {children}
        </ThemeProvider>
      </QueryClientProvider>
    </BrowserRouter>
  );

  beforeEach(() => {
    queryClient = new QueryClient({
      defaultOptions: {
        queries: {
          retry: false,
        },
      },
    });
    mockNavigate.mockClear();
    vi.clearAllMocks();
  });

  /**
   * Test: RoleList should render list of roles correctly
   * Scenario: Component receives role data and displays it
   * Expected: All roles displayed with correct information
   */
  it('should render list of roles correctly', async () => {
    // Arrange
    vi.mocked(roleService.getRoles).mockResolvedValue(mockRoleListResponse);

    // Act
    render(
      <TestWrapper>
        <RoleList />
      </TestWrapper>
    );

    // Assert
    await waitFor(() => {
      expect(screen.getByText('Administrator')).toBeInTheDocument();
      expect(screen.getByText('Content Editor')).toBeInTheDocument();
      expect(screen.getByText('Content Viewer')).toBeInTheDocument();
    });

    expect(screen.getByText('System administrator with full access')).toBeInTheDocument();
    expect(screen.getByText('Can edit and manage content')).toBeInTheDocument();
  });

  /**
   * Test: Component should show loading state while fetching data
   * Scenario: API request is in progress
   * Expected: Loading indicator displayed
   */
  it('should show loading state while fetching data', () => {
    // Arrange
    vi.mocked(roleService.getRoles).mockImplementation(
      () => new Promise(() => {}) // Never resolves
    );

    // Act
    render(
      <TestWrapper>
        <RoleList />
      </TestWrapper>
    );

    // Assert
    expect(screen.getByTestId('loading-spinner')).toBeInTheDocument();
  });

  /**
   * Test: Component should handle error state gracefully
   * Scenario: API request fails with error
   * Expected: Error message displayed with retry option
   */
  it('should show error state when API fails', async () => {
    // Arrange
    const errorMessage = 'Failed to fetch roles';
    vi.mocked(roleService.getRoles).mockRejectedValue(new Error(errorMessage));

    // Act
    render(
      <TestWrapper>
        <RoleList />
      </TestWrapper>
    );

    // Assert
    await waitFor(() => {
      expect(screen.getByText(/error/i)).toBeInTheDocument();
    });
  });

  /**
   * Test: Component should show empty state when no roles exist
   * Scenario: API returns empty role list
   * Expected: Empty state message displayed
   */
  it('should show empty state when no roles exist', async () => {
    // Arrange
    const emptyResponse: RoleListResponse = {
      items: [],
      totalCount: 0,
      page: 1,
      pageSize: 10,
    };
    vi.mocked(roleService.getRoles).mockResolvedValue(emptyResponse);

    // Act
    render(
      <TestWrapper>
        <RoleList />
      </TestWrapper>
    );

    // Assert
    await waitFor(() => {
      expect(screen.getByText(/no roles found/i)).toBeInTheDocument();
    });
  });

  /**
   * Test: Edit button should navigate to edit page
   * Scenario: User clicks edit button for a specific role
   * Expected: Navigation triggered with correct role ID
   */
  it('should navigate to edit page when edit button is clicked', async () => {
    // Arrange
    vi.mocked(roleService.getRoles).mockResolvedValue(mockRoleListResponse);

    render(
      <TestWrapper>
        <RoleList />
      </TestWrapper>
    );

    // Wait for roles to load
    await waitFor(() => {
      expect(screen.getByText('Administrator')).toBeInTheDocument();
    });

    // Act
    const editButtons = screen.getAllByText(/edit/i);
    fireEvent.click(editButtons[0]);

    // Assert
    expect(mockNavigate).toHaveBeenCalledWith('/roles/1/edit');
  });

  /**
   * Test: Delete button should trigger confirmation and deletion
   * Scenario: User clicks delete button for a role
   * Expected: Confirmation dialog shown and deletion executed
   */
  it('should handle role deletion correctly', async () => {
    // Arrange
    vi.mocked(roleService.getRoles).mockResolvedValue(mockRoleListResponse);
    vi.mocked(roleService.deleteRole).mockResolvedValue(true);

    render(
      <TestWrapper>
        <RoleList />
      </TestWrapper>
    );

    // Wait for roles to load
    await waitFor(() => {
      expect(screen.getByText('Content Editor')).toBeInTheDocument();
    });

    // Act
    const deleteButtons = screen.getAllByText(/delete/i);
    fireEvent.click(deleteButtons[0]); // Delete Content Editor (non-system role)

    // Confirm deletion
    const confirmButton = await screen.findByText(/confirm/i);
    fireEvent.click(confirmButton);

    // Assert
    await waitFor(() => {
      expect(roleService.deleteRole).toHaveBeenCalledWith(2);
    });
  });

  /**
   * Test: System roles should not show delete button
   * Scenario: Role is marked as system role
   * Expected: Delete button not visible for system roles
   */
  it('should hide delete button for system roles', async () => {
    // Arrange
    vi.mocked(roleService.getRoles).mockResolvedValue(mockRoleListResponse);

    render(
      <TestWrapper>
        <RoleList />
      </TestWrapper>
    );

    // Wait for roles to load
    await waitFor(() => {
      expect(screen.getByText('Administrator')).toBeInTheDocument();
    });

    // Assert
    const adminRoleRow = screen.getByText('Administrator').closest('[data-testid="role-row"]');
    expect(adminRoleRow).not.toHaveTextContent('Delete');
  });

  /**
   * Test: Create new role button should navigate to create page
   * Scenario: User wants to create a new role
   * Expected: Navigation triggered to role creation page
   */
  it('should navigate to create page when create button is clicked', async () => {
    // Arrange
    vi.mocked(roleService.getRoles).mockResolvedValue(mockRoleListResponse);

    render(
      <TestWrapper>
        <RoleList />
      </TestWrapper>
    );

    // Act
    const createButton = screen.getByText(/create new role/i);
    fireEvent.click(createButton);

    // Assert
    expect(mockNavigate).toHaveBeenCalledWith('/roles/create');
  });

  /**
   * Test: Role permissions should be displayed correctly
   * Scenario: Roles have different permission sets
   * Expected: Permission count and details shown appropriately
   */
  it('should display role permissions correctly', async () => {
    // Arrange
    vi.mocked(roleService.getRoles).mockResolvedValue(mockRoleListResponse);

    render(
      <TestWrapper>
        <RoleList />
      </TestWrapper>
    );

    // Assert
    await waitFor(() => {
      expect(screen.getByText('4 permissions')).toBeInTheDocument(); // Administrator
      expect(screen.getByText('2 permissions')).toBeInTheDocument(); // Content Editor
      expect(screen.getByText('1 permission')).toBeInTheDocument(); // Content Viewer
    });
  });

  /**
   * Test: Search functionality should filter roles correctly
   * Scenario: User searches for specific roles
   * Expected: API called with search parameters and results filtered
   */
  it('should handle search functionality correctly', async () => {
    // Arrange
    const user = userEvent.setup();
    vi.mocked(roleService.getRoles)
      .mockResolvedValueOnce(mockRoleListResponse)
      .mockResolvedValueOnce({
        items: [mockRoleListResponse.items[0]], // Only Administrator
        totalCount: 1,
        page: 1,
        pageSize: 10,
      });

    render(
      <TestWrapper>
        <RoleList />
      </TestWrapper>
    );

    // Wait for initial load
    await waitFor(() => {
      expect(screen.getByText('Administrator')).toBeInTheDocument();
    });

    // Act
    const searchInput = screen.getByPlaceholderText(/search roles/i);
    await user.type(searchInput, 'admin');

    // Assert
    await waitFor(() => {
      expect(roleService.getRoles).toHaveBeenCalledTimes(2);
      expect(roleService.getRoles).toHaveBeenLastCalledWith(
        expect.objectContaining({
          filters: expect.objectContaining({
            searchTerm: 'admin',
          }),
        })
      );
    });
  });
});
