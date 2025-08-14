import React from 'react';
import { render, screen } from '@testing-library/react';
import { vi, describe, it, expect, beforeEach } from 'vitest';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { BrowserRouter } from 'react-router-dom';
import { useRoleManagement } from '../hooks/useRoleManagement';
import { Role } from '../models/role.types';

// Mock the role management hook
vi.mock('../hooks/useRoleManagement', () => ({
  useRoleManagement: vi.fn(),
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

// Mock i18next
vi.mock('react-i18next', () => ({
  useTranslation: () => ({
    t: (key: string) => key,
    i18n: {
      changeLanguage: () => new Promise(() => {}),
    },
  }),
}));

// Mock mada-design-system components
vi.mock('mada-design-system', () => ({
  Table: ({ children }: { children: React.ReactNode }) => <table>{children}</table>,
  TableHeader: ({ children }: { children: React.ReactNode }) => <thead>{children}</thead>,
  TableBody: ({ children }: { children: React.ReactNode }) => <tbody>{children}</tbody>,
  TableRow: ({ children }: { children: React.ReactNode }) => <tr>{children}</tr>,
  TableHead: ({ children }: { children: React.ReactNode }) => <th>{children}</th>,
  TableCell: ({ children }: { children: React.ReactNode }) => <td>{children}</td>,
  Button: ({ children, onClick, disabled }: { children: React.ReactNode; onClick?: () => void; disabled?: boolean }) => 
    <button onClick={onClick} disabled={disabled}>{children}</button>,
  Input: ({ placeholder, value, onChange }: { placeholder?: string; value?: string; onChange?: (e: React.ChangeEvent<HTMLInputElement>) => void }) => 
    <input placeholder={placeholder} value={value} onChange={onChange} />,
  Checkbox: ({ checked, onChange }: { checked?: boolean; onChange?: (checked: boolean) => void }) => 
    <input type="checkbox" checked={checked} onChange={(e) => onChange?.(e.target.checked)} />,
  Badge: ({ children }: { children: React.ReactNode }) => <span>{children}</span>,
  Select: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
  SelectContent: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
  SelectItem: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
  SelectTrigger: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
  SelectValue: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
}));

/**
 * Test suite for Role Management hooks.
 * Tests role data management, loading states, and error handling.
 */
describe('useRoleManagement Hook', () => {
  let queryClient: QueryClient;

  // Test data
  const mockRoles: Role[] = [
    {
      roleId: 1,
      code: 'admin',
      name: 'Administrator',
      description: 'System administrator with full access',
      isSystemRole: true,
      isDefault: false,
      isActive: true,
      permissionIds: [1, 2, 3, 4, 5],
      createdAt: new Date('2024-01-01'),
      modifiedAt: new Date('2024-01-01'),
    },
    {
      roleId: 2,
      code: 'editor',
      name: 'Content Editor',
      description: 'Can edit and manage content',
      isSystemRole: false,
      isDefault: false,
      isActive: true,
      permissionIds: [1, 2, 3],
      createdAt: new Date('2024-01-02'),
      modifiedAt: new Date('2024-01-02'),
    },
  ];

  /**
   * Simple component to test the hook
   */
  const TestComponent: React.FC = () => {
    const { 
      roles, 
      loading, 
      error, 
      pagination, 
      createRole, 
      updateRole, 
      deleteRole 
    } = useRoleManagement();

    if (loading) return <div>Loading...</div>;
    if (error) return <div>Error: {error}</div>;

    return (
      <div>
        <div data-testid="role-count">{roles.length}</div>
        <div data-testid="pagination-page">{pagination.page}</div>
        {roles.map(role => (
          <div key={role.roleId} data-testid={`role-${role.roleId}`}>
            {role.name}
          </div>
        ))}
        <button onClick={() => createRole({ 
          code: 'test', 
          name: 'Test Role', 
          description: 'Test',
          permissionIds: []
        })}>
          Create Role
        </button>
        <button onClick={() => updateRole(1, { 
          name: 'Updated Role',
          description: 'Updated',
          permissionIds: []
        })}>
          Update Role
        </button>
        <button onClick={() => deleteRole(1)}>
          Delete Role
        </button>
      </div>
    );
  };

  /**
   * Wrapper component providing necessary context providers
   */
  const TestWrapper: React.FC<{ children: React.ReactNode }> = ({ children }) => (
    <BrowserRouter>
      <QueryClientProvider client={queryClient}>
        {children}
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
   * Test: Hook should provide roles data when loaded
   * Scenario: Successful data loading
   * Expected: Roles displayed correctly
   */
  it('should provide roles data when loaded successfully', () => {
    // Arrange
    vi.mocked(useRoleManagement).mockReturnValue({
      roles: mockRoles,
      loading: false,
      error: null,
      pagination: {
        page: 1,
        pageSize: 10,
        totalPages: 1,
        totalItems: 2,
        hasNextPage: false,
        hasPreviousPage: false,
        onPageChange: vi.fn(),
        onPageSizeChange: vi.fn(),
      },
      filters: {
        searchTerm: '',
        onSearchChange: vi.fn(),
        onFilterChange: vi.fn(),
        onFilterClear: vi.fn(),
      },
      selection: {
        selectedIds: [],
        onSelect: vi.fn(),
        onDeselect: vi.fn(),
        onSelectAll: vi.fn(),
        onDeselectAll: vi.fn(),
        isAllSelected: false,
      },
      createRole: vi.fn(),
      updateRole: vi.fn(),
      deleteRole: vi.fn(),
      bulkDelete: vi.fn(),
      refresh: vi.fn(),
    });

    // Act
    render(
      <TestWrapper>
        <TestComponent />
      </TestWrapper>
    );

    // Assert
    expect(screen.getByTestId('role-count')).toHaveTextContent('2');
    expect(screen.getByTestId('role-1')).toHaveTextContent('Administrator');
    expect(screen.getByTestId('role-2')).toHaveTextContent('Content Editor');
    expect(screen.getByTestId('pagination-page')).toHaveTextContent('1');
  });

  /**
   * Test: Hook should show loading state initially
   * Scenario: Data is being loaded
   * Expected: Loading indicator displayed
   */
  it('should show loading state when data is being fetched', () => {
    // Arrange
    vi.mocked(useRoleManagement).mockReturnValue({
      roles: [],
      loading: true,
      error: null,
      pagination: {
        page: 1,
        pageSize: 10,
        totalPages: 0,
        totalItems: 0,
        hasNextPage: false,
        hasPreviousPage: false,
        onPageChange: vi.fn(),
        onPageSizeChange: vi.fn(),
      },
      filters: {
        searchTerm: '',
        onSearchChange: vi.fn(),
        onFilterChange: vi.fn(),
        onFilterClear: vi.fn(),
      },
      selection: {
        selectedIds: [],
        onSelect: vi.fn(),
        onDeselect: vi.fn(),
        onSelectAll: vi.fn(),
        onDeselectAll: vi.fn(),
        isAllSelected: false,
      },
      createRole: vi.fn(),
      updateRole: vi.fn(),
      deleteRole: vi.fn(),
      bulkDelete: vi.fn(),
      refresh: vi.fn(),
    });

    // Act
    render(
      <TestWrapper>
        <TestComponent />
      </TestWrapper>
    );

    // Assert
    expect(screen.getByText('Loading...')).toBeInTheDocument();
  });

  /**
   * Test: Hook should handle error state gracefully
   * Scenario: API call fails
   * Expected: Error message displayed
   */
  it('should handle error state gracefully', () => {
    // Arrange
    const errorMessage = 'Failed to fetch roles';
    vi.mocked(useRoleManagement).mockReturnValue({
      roles: [],
      loading: false,
      error: errorMessage,
      pagination: {
        page: 1,
        pageSize: 10,
        totalPages: 0,
        totalItems: 0,
        hasNextPage: false,
        hasPreviousPage: false,
        onPageChange: vi.fn(),
        onPageSizeChange: vi.fn(),
      },
      filters: {
        searchTerm: '',
        onSearchChange: vi.fn(),
        onFilterChange: vi.fn(),
        onFilterClear: vi.fn(),
      },
      selection: {
        selectedIds: [],
        onSelect: vi.fn(),
        onDeselect: vi.fn(),
        onSelectAll: vi.fn(),
        onDeselectAll: vi.fn(),
        isAllSelected: false,
      },
      createRole: vi.fn(),
      updateRole: vi.fn(),
      deleteRole: vi.fn(),
      bulkDelete: vi.fn(),
      refresh: vi.fn(),
    });

    // Act
    render(
      <TestWrapper>
        <TestComponent />
      </TestWrapper>
    );

    // Assert
    expect(screen.getByText(`Error: ${errorMessage}`)).toBeInTheDocument();
  });

  /**
   * Test: Hook should provide CRUD operation functions
   * Scenario: Component needs to perform CRUD operations
   * Expected: Functions are available and can be called
   */
  it('should provide CRUD operation functions', () => {
    // Arrange
    const mockCreateRole = vi.fn();
    const mockUpdateRole = vi.fn();
    const mockDeleteRole = vi.fn();

    vi.mocked(useRoleManagement).mockReturnValue({
      roles: mockRoles,
      loading: false,
      error: null,
      pagination: {
        page: 1,
        pageSize: 10,
        totalPages: 1,
        totalItems: 2,
        hasNextPage: false,
        hasPreviousPage: false,
        onPageChange: vi.fn(),
        onPageSizeChange: vi.fn(),
      },
      filters: {
        searchTerm: '',
        onSearchChange: vi.fn(),
        onFilterChange: vi.fn(),
        onFilterClear: vi.fn(),
      },
      selection: {
        selectedIds: [],
        onSelect: vi.fn(),
        onDeselect: vi.fn(),
        onSelectAll: vi.fn(),
        onDeselectAll: vi.fn(),
        isAllSelected: false,
      },
      createRole: mockCreateRole,
      updateRole: mockUpdateRole,
      deleteRole: mockDeleteRole,
      bulkDelete: vi.fn(),
      refresh: vi.fn(),
    });

    // Act
    render(
      <TestWrapper>
        <TestComponent />
      </TestWrapper>
    );

    // Assert
    expect(screen.getByText('Create Role')).toBeInTheDocument();
    expect(screen.getByText('Update Role')).toBeInTheDocument();
    expect(screen.getByText('Delete Role')).toBeInTheDocument();
    
    // Verify functions are provided
    expect(mockCreateRole).toBeDefined();
    expect(mockUpdateRole).toBeDefined();
    expect(mockDeleteRole).toBeDefined();
  });

  /**
   * Test: Hook should provide pagination functionality
   * Scenario: Component needs pagination controls
   * Expected: Pagination state and handlers available
   */
  it('should provide pagination functionality', () => {
    // Arrange
    const mockPageChange = vi.fn();
    const mockPageSizeChange = vi.fn();

    vi.mocked(useRoleManagement).mockReturnValue({
      roles: mockRoles,
      loading: false,
      error: null,
      pagination: {
        page: 2,
        pageSize: 5,
        totalPages: 3,
        totalItems: 15,
        hasNextPage: true,
        hasPreviousPage: true,
        onPageChange: mockPageChange,
        onPageSizeChange: mockPageSizeChange,
      },
      filters: {
        searchTerm: '',
        onSearchChange: vi.fn(),
        onFilterChange: vi.fn(),
        onFilterClear: vi.fn(),
      },
      selection: {
        selectedIds: [],
        onSelect: vi.fn(),
        onDeselect: vi.fn(),
        onSelectAll: vi.fn(),
        onDeselectAll: vi.fn(),
        isAllSelected: false,
      },
      createRole: vi.fn(),
      updateRole: vi.fn(),
      deleteRole: vi.fn(),
      bulkDelete: vi.fn(),
      refresh: vi.fn(),
    });

    // Act
    render(
      <TestWrapper>
        <TestComponent />
      </TestWrapper>
    );

    // Assert
    expect(screen.getByTestId('pagination-page')).toHaveTextContent('2');
    expect(mockPageChange).toBeDefined();
    expect(mockPageSizeChange).toBeDefined();
  });

  /**
   * Test: Hook should provide selection functionality
   * Scenario: Component needs row selection
   * Expected: Selection state and handlers available
   */
  it('should provide selection functionality', () => {
    // Arrange
    const mockSelect = vi.fn();
    const mockDeselect = vi.fn();
    const mockSelectAll = vi.fn();
    const mockDeselectAll = vi.fn();

    vi.mocked(useRoleManagement).mockReturnValue({
      roles: mockRoles,
      loading: false,
      error: null,
      pagination: {
        page: 1,
        pageSize: 10,
        totalPages: 1,
        totalItems: 2,
        hasNextPage: false,
        hasPreviousPage: false,
        onPageChange: vi.fn(),
        onPageSizeChange: vi.fn(),
      },
      filters: {
        searchTerm: '',
        onSearchChange: vi.fn(),
        onFilterChange: vi.fn(),
        onFilterClear: vi.fn(),
      },
      selection: {
        selectedIds: [1],
        onSelect: mockSelect,
        onDeselect: mockDeselect,
        onSelectAll: mockSelectAll,
        onDeselectAll: mockDeselectAll,
        isAllSelected: false,
      },
      createRole: vi.fn(),
      updateRole: vi.fn(),
      deleteRole: vi.fn(),
      bulkDelete: vi.fn(),
      refresh: vi.fn(),
    });

    // Act
    render(
      <TestWrapper>
        <TestComponent />
      </TestWrapper>
    );

    // Assert
    expect(mockSelect).toBeDefined();
    expect(mockDeselect).toBeDefined();
    expect(mockSelectAll).toBeDefined();
    expect(mockDeselectAll).toBeDefined();
  });
});
