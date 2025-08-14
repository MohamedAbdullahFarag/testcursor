import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import {
  Table,
  TableHeader,
  TableBody,
  TableRow,
  TableHead,
  TableCell,
  Button,
  Input,
  Checkbox,
  Badge,
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from 'mada-design-system';
import { 
  ChevronLeft, 
  ChevronRight, 
  Search, 
  RefreshCcw, 
  Edit, 
  Trash, 
  Shield, 
  Filter,
  X,
  Settings
} from 'lucide-react';
import { Role, RoleFilterOptions } from '../models/role.types';

interface PaginationProps {
  page: number;
  pageSize: number;
  totalPages: number;
  totalItems: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
  onPageChange: (page: number) => Promise<void>;
  onPageSizeChange: (pageSize: number) => void;
}

interface SelectionProps {
  selectedIds: number[];
  onSelect: (id: number) => void;
  onDeselect: (id: number) => void;
  onSelectAll: () => void;
  onDeselectAll: () => void;
  isAllSelected: boolean;
}

interface FilterProps {
  searchTerm?: string;
  onSearchChange: (term: string) => void;
  onFilterChange: (filters: RoleFilterOptions) => void;
  onFilterClear: () => void;
}

interface RoleListProps {
  roles: Role[];
  loading: boolean;
  pagination: PaginationProps;
  selection: SelectionProps;
  filters: FilterProps;
  onEdit: (role: Role) => void;
  onDelete: (role: Role) => void;
  onManagePermissions: (role: Role) => void;
}

/**
 * RoleList component - Displays roles in a table with pagination, filtering, and selection
 * 
 * Features:
 * - Sortable columns
 * - Search and filtering
 * - Pagination
 * - Row selection
 * - System role indication
 * - Permission count display
 * 
 * @param props - RoleListProps
 */
export const RoleList: React.FC<RoleListProps> = ({
  roles,
  loading,
  pagination,
  selection,
  filters,
  onEdit,
  onDelete,
  onManagePermissions,
}) => {
  const { t } = useTranslation('roleManagement');
  const [showFilters, setShowFilters] = useState(false);
  const [activeFilters, setActiveFilters] = useState<RoleFilterOptions>({});

  // Filter handlers
  const toggleFilters = () => {
    setShowFilters(!showFilters);
  };

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    filters.onSearchChange(e.target.value);
  };

  const applyFilters = () => {
    filters.onFilterChange(activeFilters);
  };

  const clearFilters = () => {
    setActiveFilters({});
    filters.onFilterClear();
  };

  const handleSystemRoleFilterChange = (value: string) => {
    setActiveFilters(prev => ({
      ...prev,
      isSystemRole: value === 'true' ? true : value === 'false' ? false : undefined
    }));
  };

  const handleActiveFilterChange = (value: string) => {
    setActiveFilters(prev => ({
      ...prev,
      isActive: value === 'true' ? true : value === 'false' ? false : undefined
    }));
  };

  // Page size options
  const pageSizeOptions = [5, 10, 25, 50];

  return (
    <div className="role-list">
      {/* Search and filter */}
      <div className="flex flex-wrap justify-between items-center gap-2 mb-4">
        <div className="flex flex-1 items-center gap-2">
          <div className="relative flex-1 max-w-sm">
            <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-gray-500" />
            <Input
              placeholder={t('searchRoles')}
              className="pl-8"
              value={filters.searchTerm || ''}
              onChange={handleSearchChange}
              aria-label={t('searchRoles')}
            />
          </div>
          
          <Button
            variant="outline"
            size="sm"
            onClick={toggleFilters}
            aria-label={t('filters')}
          >
            <Filter className="h-4 w-4 mr-1" />
            {t('filters')}
          </Button>
          
          {Object.keys(activeFilters).length > 0 && (
            <Button
              variant="outline"
              size="sm"
              onClick={clearFilters}
              aria-label={t('clearFilters')}
            >
              <X className="h-4 w-4 mr-1" />
              {t('clearFilters')}
            </Button>
          )}
        </div>
        
        <div className="flex items-center gap-2">
          <Select
            value={pagination.pageSize.toString()}
            onValueChange={(value) => pagination.onPageSizeChange(Number(value))}
          >
            <SelectTrigger className="w-[130px]" aria-label={t('pageSize')}>
              <SelectValue placeholder={t('rowsPerPage')} />
            </SelectTrigger>
            <SelectContent>
              {pageSizeOptions.map(size => (
                <SelectItem key={size} value={size.toString()}>
                  {t('rowsPerPageValue', { count: size })}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
      </div>

      {/* Advanced filters */}
      {showFilters && (
        <div className="bg-gray-50 p-4 rounded-md mb-4">
          <h3 className="font-medium mb-3">{t('advancedFilters')}</h3>
          
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            {/* System role filter */}
            <div>
              <label className="block text-sm font-medium mb-1">
                {t('isSystemRole')}
              </label>
              <Select
                value={activeFilters.isSystemRole?.toString() || ''}
                onValueChange={handleSystemRoleFilterChange}
              >
                <SelectTrigger aria-label={t('isSystemRole')}>
                  <SelectValue placeholder={t('all')} />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="">{t('all')}</SelectItem>
                  <SelectItem value="true">{t('yes')}</SelectItem>
                  <SelectItem value="false">{t('no')}</SelectItem>
                </SelectContent>
              </Select>
            </div>
            
            {/* Active status filter */}
            <div>
              <label className="block text-sm font-medium mb-1">
                {t('isActive')}
              </label>
              <Select
                value={activeFilters.isActive?.toString() || ''}
                onValueChange={handleActiveFilterChange}
              >
                <SelectTrigger aria-label={t('isActive')}>
                  <SelectValue placeholder={t('all')} />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="">{t('all')}</SelectItem>
                  <SelectItem value="true">{t('active')}</SelectItem>
                  <SelectItem value="false">{t('inactive')}</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>
          
          <div className="mt-4 flex justify-end gap-2">
            <Button
              variant="outline"
              onClick={clearFilters}
              aria-label={t('clearFilters')}
            >
              {t('clearFilters')}
            </Button>
            <Button onClick={applyFilters} aria-label={t('applyFilters')}>
              {t('applyFilters')}
            </Button>
          </div>
        </div>
      )}

      {/* Role table */}
      <div className="border rounded-md overflow-hidden">
        <Table>
          <TableHeader>
            <TableRow>
              {/* Selection header */}
              <TableHead className="w-[40px]">
                <Checkbox
                  checked={selection.isAllSelected}
                  onCheckedChange={(checked) => {
                    checked ? selection.onSelectAll() : selection.onDeselectAll();
                  }}
                  aria-label={selection.isAllSelected ? t('deselectAll') : t('selectAll')}
                  disabled={loading || roles.length === 0}
                />
              </TableHead>
              
              {/* Role info headers */}
              <TableHead>{t('name')}</TableHead>
              <TableHead>{t('code')}</TableHead>
              <TableHead>{t('description')}</TableHead>
              <TableHead>{t('status')}</TableHead>
              <TableHead>{t('permissions')}</TableHead>
              <TableHead className="text-right">{t('actions')}</TableHead>
            </TableRow>
          </TableHeader>
          
          <TableBody>
            {loading && (
              <TableRow>
                <TableCell colSpan={7} className="text-center py-8">
                  <div className="flex justify-center items-center">
                    <RefreshCcw className="h-6 w-6 animate-spin mr-2" />
                    <span>{t('loading')}</span>
                  </div>
                </TableCell>
              </TableRow>
            )}
            
            {!loading && roles.length === 0 && (
              <TableRow>
                <TableCell colSpan={7} className="text-center py-8">
                  {t('noRolesFound')}
                </TableCell>
              </TableRow>
            )}
            
            {!loading && roles.map((role) => {
              const isSelected = selection.selectedIds.includes(role.id);
              
              return (
                <TableRow key={role.id} className={isSelected ? 'bg-gray-50' : ''}>
                  {/* Selection cell */}
                  <TableCell>
                    <Checkbox
                      checked={isSelected}
                      onCheckedChange={(checked) => {
                        checked 
                          ? selection.onSelect(role.id)
                          : selection.onDeselect(role.id);
                      }}
                      aria-label={
                        isSelected
                          ? t('deselectRole', { name: role.name })
                          : t('selectRole', { name: role.name })
                      }
                    />
                  </TableCell>
                  
                  {/* Role data cells */}
                  <TableCell>
                    <div className="flex items-center">
                      <span className="font-medium">{role.name}</span>
                      {role.isSystemRole && (
                        <Badge variant="outline" className="ml-2">
                          <Shield className="h-3 w-3 mr-1" />
                          {t('systemRole')}
                        </Badge>
                      )}
                    </div>
                  </TableCell>
                  
                  <TableCell>{role.code}</TableCell>
                  
                  <TableCell>
                    <div className="max-w-xs truncate">
                      {role.description || t('noDescription')}
                    </div>
                  </TableCell>
                  
                  <TableCell>
                    <Badge variant={role.isActive ? "success" : "secondary"}>
                      {role.isActive ? t('active') : t('inactive')}
                    </Badge>
                  </TableCell>
                  
                  <TableCell>
                    {role.permissions?.length > 0 ? (
                      <Badge variant="outline">
                        {t('permissionsCount', { count: role.permissions.length })}
                      </Badge>
                    ) : (
                      <Badge variant="outline" className="text-gray-500">
                        {t('noPermissions')}
                      </Badge>
                    )}
                  </TableCell>
                  
                  {/* Actions cell */}
                  <TableCell className="text-right">
                    <div className="flex justify-end gap-2">
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => onManagePermissions(role)}
                        aria-label={t('managePermissions')}
                      >
                        <Settings className="h-4 w-4" />
                      </Button>
                      
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => onEdit(role)}
                        disabled={role.isSystemRole}
                        aria-label={t('editRole')}
                      >
                        <Edit className="h-4 w-4" />
                      </Button>
                      
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => onDelete(role)}
                        disabled={role.isSystemRole}
                        aria-label={t('deleteRole')}
                      >
                        <Trash className="h-4 w-4" />
                      </Button>
                    </div>
                  </TableCell>
                </TableRow>
              );
            })}
          </TableBody>
        </Table>
      </div>

      {/* Pagination controls */}
      {!loading && roles.length > 0 && (
        <div className="flex justify-between items-center mt-4">
          <div className="text-sm text-gray-600">
            {t('paginationInfo', {
              start: ((pagination.page - 1) * pagination.pageSize) + 1,
              end: Math.min(pagination.page * pagination.pageSize, pagination.totalItems),
              total: pagination.totalItems,
            })}
          </div>
          
          <div className="flex items-center gap-2">
            <Button
              variant="outline"
              size="sm"
              onClick={() => pagination.onPageChange(pagination.page - 1)}
              disabled={!pagination.hasPreviousPage}
              aria-label={t('previousPage')}
            >
              <ChevronLeft className="h-4 w-4" />
            </Button>
            
            <span className="text-sm">
              {t('pageInfo', { current: pagination.page, total: pagination.totalPages })}
            </span>
            
            <Button
              variant="outline"
              size="sm"
              onClick={() => pagination.onPageChange(pagination.page + 1)}
              disabled={!pagination.hasNextPage}
              aria-label={t('nextPage')}
            >
              <ChevronRight className="h-4 w-4" />
            </Button>
          </div>
        </div>
      )}
    </div>
  );
};
