/**
 * UserList Component
 * Displays a table of users with pagination, search, filtering, and actions
 * Supports both English and Arabic layouts with full accessibility
 */

import { memo, useState, useCallback } from 'react';
import { useTranslation } from 'react-i18next';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
  Button,
  Badge,
  Stack,
  Card,
  CardContent,
  CardHeader,
  Input,
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
  Checkbox,
  Skeleton,
  cn,
  useLanguage,
} from 'mada-design-system';
import { NotificationOffGray } from 'streamline-icons';
import { EmptyState } from '../../../shared/components';
import type { User, UserFilterOptions } from '../models/user.types';

/**
 * Props for the UserList component
 */
export interface UserListProps {
  /** Array of users to display */
  users: User[];
  
  /** Total number of users (for pagination) */
  total: number;
  
  /** Current page number */
  page: number;
  
  /** Number of items per page */
  pageSize: number;
  
  /** Total number of pages */
  totalPages: number;
  
  /** Loading state */
  loading: boolean;
  
  /** Error message */
  error: string | null;
  
  /** Current filter options */
  filters: UserFilterOptions;
  
  /** Selected user IDs */
  selectedUsers: number[];
  
  /** Whether the list is refreshing */
  isRefreshing?: boolean;
  
  // Event handlers
  /** Called when page changes */
  onPageChange?: (page: number) => void;
  
  /** Called when page size changes */
  onPageSizeChange?: (pageSize: number) => void;
  
  /** Called when filters change */
  onFiltersChange?: (filters: Partial<UserFilterOptions>) => void;
  
  /** Called when search term changes */
  onSearch?: (searchTerm: string) => void;
  
  /** Called when user selection changes */
  onUserSelect?: (userId: number) => void;
  
  /** Called when select all changes */
  onSelectAll?: (selected: boolean) => void;
  
  /** Called when create user is requested */
  onCreate?: () => void;
  
  /** Called when edit user is requested */
  onEdit?: (user: User) => void;
  
  /** Called when delete user is requested */
  onDelete?: (user: User) => void;
  
  /** Called when bulk delete is requested */
  onBulkDelete?: (userIds: number[]) => void;
  
  /** Called when export is requested */
  onExport?: (format: 'csv' | 'excel' | 'pdf') => void;
  
  /** Called when refresh is requested */
  onRefresh?: () => void;
  
  /** Custom CSS class */
  className?: string;
  
  /** Whether actions are shown */
  showActions?: boolean;
  
  /** Whether bulk actions are enabled */
  enableBulkActions?: boolean;
  
  /** Whether export is enabled */
  enableExport?: boolean;
}

/**
 * Skeleton loader for the user table
 */
const UserTableSkeleton = memo(() => (
  <Card>
    <CardHeader>
      <Stack direction="row" justifyContent="between" alignItems="center">
        <Skeleton className="h-8 w-32" />
        <Stack direction="row" gap={2}>
          <Skeleton className="h-10 w-32" />
          <Skeleton className="h-10 w-24" />
        </Stack>
      </Stack>
    </CardHeader>
    <CardContent>
      <div className="space-y-4">
        {Array.from({ length: 5 }).map((_, index) => (
          <div key={index} className="flex items-center space-x-4">
            <Skeleton className="h-4 w-4" />
            <Skeleton className="h-4 w-24" />
            <Skeleton className="h-4 w-32" />
            <Skeleton className="h-4 w-20" />
            <Skeleton className="h-4 w-16" />
            <Skeleton className="h-4 w-20" />
          </div>
        ))}
      </div>
    </CardContent>
  </Card>
));

UserTableSkeleton.displayName = 'UserTableSkeleton';

/**
 * UserList Component
 * Comprehensive user management table with full functionality
 */
export const UserList = memo<UserListProps>(({
  users,
  total,
  page,
  pageSize,
  totalPages,
  loading,
  error,
  filters,
  selectedUsers,
  isRefreshing = false,
  onPageChange,
  onPageSizeChange,
  onFiltersChange,
  onSearch,
  onUserSelect,
  onSelectAll,
  onCreate,
  onEdit,
  onDelete,
  onBulkDelete,
  onExport,
  onRefresh,
  className,
  showActions = true,
  enableBulkActions = true,
  enableExport = true,
}) => {
  const { t } = useTranslation(['users', 'common']);
  const { dir } = useLanguage();
  
  // Local state for UI interactions
  const [searchTerm, setSearchTerm] = useState(filters.searchTerm || '');
  const [showFilters, setShowFilters] = useState(false);
  
  /**
   * Handle search input change with debouncing
   */
  const handleSearchChange = useCallback((value: string) => {
    setSearchTerm(value);
    if (onSearch) {
      onSearch(value);
    }
  }, [onSearch]);
  
  /**
   * Handle filter changes
   */
  const handleFilterChange = useCallback((key: keyof UserFilterOptions, value: any) => {
    if (onFiltersChange) {
      onFiltersChange({ [key]: value });
    }
  }, [onFiltersChange]);
  
  /**
   * Handle individual user selection
   */
  const handleUserSelect = useCallback((userId: number) => {
    if (onUserSelect) {
      onUserSelect(userId);
    }
  }, [onUserSelect]);
  
  /**
   * Handle select all toggle
   */
  const handleSelectAll = useCallback((checked: boolean) => {
    if (onSelectAll) {
      onSelectAll(checked);
    }
  }, [onSelectAll]);
  
  /**
   * Format user roles for display
   */
  const formatRoles = useCallback((roles: string[]) => {
    if (!roles || roles.length === 0) {
      return <Badge variant="outline">{t('users:noRoles')}</Badge>;
    }
    
    if (roles.length === 1) {
      return <Badge variant="default">{roles[0]}</Badge>;
    }
    
    return (
      <Stack direction="row" gap={1}>
        <Badge variant="default">{roles[0]}</Badge>
        {roles.length > 1 && (
          <Badge variant="outline">+{roles.length - 1}</Badge>
        )}
      </Stack>
    );
  }, [t]);
  
  /**
   * Format date for display
   */
  const formatDate = useCallback((dateString: string | undefined) => {
    if (!dateString) return t('common:never');
    
    const date = new Date(dateString);
    return new Intl.DateTimeFormat(dir === 'rtl' ? 'ar-SA' : 'en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    }).format(date);
  }, [t, dir]);
  
  // Show loading skeleton while initial load
  if (loading && users.length === 0) {
    return <UserTableSkeleton />;
  }
  
  // Show error state
  if (error && users.length === 0) {
    return (
      <Card>
        <CardContent className="py-8">
          <EmptyState
            title={t('users:errorTitle')}
            description={error}
            icon={{ light: NotificationOffGray, dark: NotificationOffGray }}
            buttons={[
              <Button key="retry" onClick={onRefresh} variant="outline">
                🔄 {t('common:retry')}
              </Button>
            ]}
          />
        </CardContent>
      </Card>
    );
  }
  
  // Calculate selection state
  const allUsersSelected = users.length > 0 && selectedUsers.length === users.length;
  
  return (
    <div className={cn('space-y-4', className)}>
      {/* Header with search and actions */}
      <Card>
        <CardHeader>
          <Stack direction="row" justifyContent="between" alignItems="center" className="flex-wrap gap-4">
            <Stack direction="row" alignItems="center" gap={4} className="flex-1 min-w-0">
              <h2 className="text-lg font-semibold">{t('users:title')}</h2>
              {total > 0 && (
                <Badge variant="outline">{t('users:totalCount', { count: total })}</Badge>
              )}
            </Stack>
            
            <Stack direction="row" gap={2} alignItems="center" className="flex-wrap">
              {/* Search */}
              <div className="relative min-w-[200px]">
                <span className="absolute left-3 top-1/2 transform -translate-y-1/2 text-muted-foreground">🔍</span>
                <Input
                  placeholder={t('users:searchPlaceholder')}
                  value={searchTerm}
                  onChange={(e) => handleSearchChange(e.target.value)}
                  className="pl-9"
                />
              </div>
              
              {/* Filter Toggle */}
              <Button
                variant="outline"
                size="sm"
                onClick={() => setShowFilters(!showFilters)}
                className={cn(showFilters && 'bg-accent')}
              >
                🔽 {t('common:filter')}
              </Button>
              
              {/* Refresh */}
              <Button
                variant="outline"
                size="sm"
                onClick={onRefresh}
                disabled={loading || isRefreshing}
              >
                {(loading || isRefreshing) ? '⏳' : '🔄'} {t('common:refresh')}
              </Button>
              
              {/* Export */}
              {enableExport && onExport && (
                <Select onValueChange={(value) => onExport(value as 'csv' | 'excel' | 'pdf')}>
                  <SelectTrigger className="w-32">
                    <SelectValue placeholder="📥 Export" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="csv">CSV</SelectItem>
                    <SelectItem value="excel">Excel</SelectItem>
                    <SelectItem value="pdf">PDF</SelectItem>
                  </SelectContent>
                </Select>
              )}
              
              {/* Create User */}
              {onCreate && (
                <Button onClick={onCreate}>
                  ➕ {t('users:createUser')}
                </Button>
              )}
            </Stack>
          </Stack>
          
          {/* Bulk Actions */}
          {enableBulkActions && selectedUsers.length > 0 && (
            <Stack direction="row" alignItems="center" gap={2} className="pt-4 border-t">
              <span className="text-sm text-muted-foreground">
                {t('users:selectedCount', { count: selectedUsers.length })}
              </span>
              <Button
                variant="outline"
                size="sm"
                onClick={() => onBulkDelete?.(selectedUsers)}
              >
                🗑️ {t('users:deleteSelected')}
              </Button>
            </Stack>
          )}
        </CardHeader>
        
        {/* Filters */}
        {showFilters && (
          <CardContent className="border-t pt-4">
            <Stack direction="row" gap={4} alignItems="end" className="flex-wrap">
              <div className="min-w-[150px]">
                <label className="text-sm font-medium mb-2 block">
                  {t('users:filterByStatus')}
                </label>
                <Select
                  value={filters.isActive?.toString() || ''}
                  onValueChange={(value) => handleFilterChange('isActive', value === '' ? undefined : value === 'true')}
                >
                  <SelectTrigger>
                    <SelectValue placeholder={t('common:all')} />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="">{t('common:all')}</SelectItem>
                    <SelectItem value="true">{t('users:active')}</SelectItem>
                    <SelectItem value="false">{t('users:inactive')}</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              
              <div className="min-w-[150px]">
                <label className="text-sm font-medium mb-2 block">
                  {t('users:filterByLanguage')}
                </label>
                <Select
                  value={filters.preferredLanguage || ''}
                  onValueChange={(value) => handleFilterChange('preferredLanguage', value || undefined)}
                >
                  <SelectTrigger>
                    <SelectValue placeholder={t('common:all')} />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="">{t('common:all')}</SelectItem>
                    <SelectItem value="en">{t('common:english')}</SelectItem>
                    <SelectItem value="ar">{t('common:arabic')}</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              
              <Button
                variant="outline"
                size="sm"
                onClick={() => onFiltersChange?.({})}
              >
                {t('common:clearFilters')}
              </Button>
            </Stack>
          </CardContent>
        )}
      </Card>
      
      {/* Users Table */}
      <Card>
        <CardContent className="p-0">
          {users.length === 0 ? (
            <div className="py-8">
              <EmptyState
                title={t('users:noUsers')}
                description={t('users:noUsersDescription')}
                icon={{ light: NotificationOffGray, dark: NotificationOffGray }}
                buttons={onCreate ? [
                  <Button key="create" onClick={onCreate}>
                    ➕ {t('users:createFirstUser')}
                  </Button>
                ] : undefined}
              />
            </div>
          ) : (
            <>
              <Table>
                <TableHeader>
                  <TableRow>
                    {enableBulkActions && (
                      <TableHead className="w-12">
                        <Checkbox
                          checked={allUsersSelected}
                          onCheckedChange={handleSelectAll}
                          aria-label={t('users:selectAll')}
                        />
                      </TableHead>
                    )}
                    <TableHead>{t('users:user')}</TableHead>
                    <TableHead>{t('users:contact')}</TableHead>
                    <TableHead>{t('users:roles')}</TableHead>
                    <TableHead>{t('users:status')}</TableHead>
                    <TableHead>{t('users:lastLogin')}</TableHead>
                    {showActions && <TableHead className="w-32">{t('common:actions')}</TableHead>}
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {users.map((user) => (
                    <TableRow key={user.id}>
                      {enableBulkActions && (
                        <TableCell>
                          <Checkbox
                            checked={selectedUsers.includes(user.id)}
                            onCheckedChange={() => handleUserSelect(user.id)}
                            aria-label={t('users:selectUser', { name: user.fullName })}
                          />
                        </TableCell>
                      )}
                      <TableCell>
                        <div>
                          <div className="font-medium">{user.fullName}</div>
                          <div className="text-sm text-muted-foreground">@{user.username}</div>
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="space-y-1">
                          <div className="flex items-center gap-1 text-sm">
                            📧 {user.email}
                            {user.emailVerified && <span className="text-green-600">✓</span>}
                          </div>
                          {user.phoneNumber && (
                            <div className="flex items-center gap-1 text-sm text-muted-foreground">
                              📞 {user.phoneNumber}
                              {user.phoneVerified && <span className="text-green-600">✓</span>}
                            </div>
                          )}
                        </div>
                      </TableCell>
                      <TableCell>{formatRoles(user.roles)}</TableCell>
                      <TableCell>
                        <Badge variant={user.isActive ? 'default' : 'outline'}>
                          {user.isActive ? t('users:active') : t('users:inactive')}
                        </Badge>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-1 text-sm">
                          📅 {formatDate(user.lastLoginAt)}
                        </div>
                      </TableCell>
                      {showActions && (
                        <TableCell>
                          <Stack direction="row" gap={1}>
                            <Button
                              variant="ghost"
                              size="sm"
                              onClick={() => onEdit?.(user)}
                              disabled={loading}
                            >
                              ✏️
                            </Button>
                            <Button
                              variant="ghost"
                              size="sm"
                              onClick={() => onDelete?.(user)}
                              disabled={loading}
                            >
                              🗑️
                            </Button>
                          </Stack>
                        </TableCell>
                      )}
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
              
              {/* Simple Pagination */}
              {totalPages > 1 && (
                <div className="border-t p-4">
                  <Stack direction="row" justifyContent="between" alignItems="center" className="flex-wrap gap-4">
                    <div className="text-sm text-muted-foreground">
                      {t('users:showingResults', {
                        start: (page - 1) * pageSize + 1,
                        end: Math.min(page * pageSize, total),
                        total,
                      })}
                    </div>
                    
                    <Stack direction="row" gap={2} alignItems="center">
                      <Select
                        value={pageSize.toString()}
                        onValueChange={(value) => onPageSizeChange?.(parseInt(value))}
                      >
                        <SelectTrigger className="w-20">
                          <SelectValue />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="10">10</SelectItem>
                          <SelectItem value="20">20</SelectItem>
                          <SelectItem value="50">50</SelectItem>
                          <SelectItem value="100">100</SelectItem>
                        </SelectContent>
                      </Select>
                      
                      <Stack direction="row" gap={1}>
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => onPageChange?.(1)}
                          disabled={page <= 1}
                        >
                          ⏮️
                        </Button>
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => onPageChange?.(page - 1)}
                          disabled={page <= 1}
                        >
                          ⬅️
                        </Button>
                        <span className="px-3 py-1 text-sm">
                          {page} / {totalPages}
                        </span>
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => onPageChange?.(page + 1)}
                          disabled={page >= totalPages}
                        >
                          ➡️
                        </Button>
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => onPageChange?.(totalPages)}
                          disabled={page >= totalPages}
                        >
                          ⏭️
                        </Button>
                      </Stack>
                    </Stack>
                  </Stack>
                </div>
              )}
            </>
          )}
        </CardContent>
      </Card>
    </div>
  );
});

UserList.displayName = 'UserList';

export default UserList;
