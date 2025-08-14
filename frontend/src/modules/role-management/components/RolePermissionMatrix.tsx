import React, { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import {
  Button,
  Checkbox,
  Alert,
  AlertDescription,
  Tabs,
  TabsContent,
  TabsList,
  TabsTrigger,
} from 'mada-design-system';
import { roleService } from '../services/roleService';
import { Permission } from '../models/role.types';

interface RolePermissionMatrixProps {
  /** Role ID */
  roleId: number;
  /** Role name for display */
  roleName: string;
  /** Whether this is a system role */
  isSystemRole: boolean;
  /** Available permissions */
  availablePermissions?: Permission[];
  /** Loading state */
  isLoading?: boolean;
  /** Save callback */
  onSave: (roleId: number, permissions: string[]) => Promise<void>;
  /** Cancel callback */
  onCancel: () => void;
}

/**
 * RolePermissionMatrix component - Manages role permissions with a matrix view
 * 
 * Features:
 * - Group permissions by category
 * - Tab-based category navigation
 * - Select/deselect all in category
 * - System role protection
 * - Loading state handling
 * 
 * @param props - RolePermissionMatrixProps
 */
export const RolePermissionMatrix: React.FC<RolePermissionMatrixProps> = ({
  roleId,
  roleName,
  isSystemRole,
  availablePermissions = [],
  isLoading = false,
  onSave,
  onCancel,
}) => {
  const { t } = useTranslation('roleManagement');
  
  // State for selected permissions
  const [selectedPermissions, setSelectedPermissions] = useState<string[]>([]);
  const [initialPermissions, setInitialPermissions] = useState<string[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Group permissions by category for tabs
  const permissionsByCategory = availablePermissions.reduce<Record<string, Permission[]>>(
    (groups, permission) => {
      const category = permission.category || 'Other';
      if (!groups[category]) {
        groups[category] = [];
      }
      groups[category].push(permission);
      return groups;
    },
    {}
  );

  // Get categories for tabs
  const categories = Object.keys(permissionsByCategory).sort();
  
  // Default tab is the first category or "General" if exists
  const defaultTab = categories.includes('General') ? 'General' : categories[0] || '';
  
  // Selected tab state
  const [activeTab, setActiveTab] = useState(defaultTab);

  // Load current permissions for the role
  useEffect(() => {
    const loadRolePermissions = async () => {
      try {
        setLoading(true);
        setError(null);
        
        // Get current permission matrix for role
        const matrix = await roleService.getRolePermissionMatrix(roleId);
        
        // Extract permission codes that are granted (true)
        const grantedPermissions = Object.entries(matrix.permissions)
          .filter(([, isGranted]) => isGranted)
          .map(([code]) => code);
        
        setSelectedPermissions(grantedPermissions);
        setInitialPermissions(grantedPermissions);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to load role permissions');
      } finally {
        setLoading(false);
      }
    };

    loadRolePermissions();
  }, [roleId]);

  // Handle permission toggle
  const handlePermissionChange = (code: string, checked: boolean) => {
    setSelectedPermissions((prev) =>
      checked
        ? [...prev, code]
        : prev.filter((p) => p !== code)
    );
  };

  // Handle toggling all permissions in a category
  const toggleCategoryPermissions = (category: string, checked: boolean) => {
    const categoryPermissionCodes = permissionsByCategory[category].map(p => p.code);
    
    setSelectedPermissions((prev) => {
      if (checked) {
        // Add all permissions from this category
        const newPermissions = [...prev];
        categoryPermissionCodes.forEach(code => {
          if (!newPermissions.includes(code)) {
            newPermissions.push(code);
          }
        });
        return newPermissions;
      } else {
        // Remove all permissions from this category
        return prev.filter(code => !categoryPermissionCodes.includes(code));
      }
    });
  };

  // Check if all permissions in a category are selected
  const isCategoryFullySelected = (category: string): boolean => {
    const categoryPermissionCodes = permissionsByCategory[category].map(p => p.code);
    return categoryPermissionCodes.every(code => selectedPermissions.includes(code));
  };

  // Check if any permission in a category is selected
  const isCategoryPartiallySelected = (category: string): boolean => {
    const categoryPermissionCodes = permissionsByCategory[category].map(p => p.code);
    return categoryPermissionCodes.some(code => selectedPermissions.includes(code)) 
      && !isCategoryFullySelected(category);
  };

  // Handle save action
  const handleSave = async () => {
    try {
      setLoading(true);
      setError(null);
      await onSave(roleId, selectedPermissions);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to save permissions');
      setLoading(false);
    }
  };

  // Check if permissions have been modified
  const hasChanges = () => {
    if (initialPermissions.length !== selectedPermissions.length) return true;
    
    // Check if all initial permissions are still selected
    return !initialPermissions.every(p => selectedPermissions.includes(p));
  };

  // Handle cancel action
  const handleCancel = () => {
    // Reset to initial state
    setSelectedPermissions(initialPermissions);
    onCancel();
  };

  // Loading state from component or from internal state
  const isLoadingState = isLoading || loading;

  return (
    <div className="role-permission-matrix">
      {/* Error alert */}
      {error && (
        <Alert className="mb-4">
          <AlertDescription>{error}</AlertDescription>
        </Alert>
      )}

      {/* System role warning */}
      {isSystemRole && (
        <Alert className="mb-4">
          <AlertDescription>{t('systemRolePermissionWarning')}</AlertDescription>
        </Alert>
      )}

      {/* Permission matrix */}
      <div className="mt-4">
        <Tabs value={activeTab} onValueChange={setActiveTab}>
          <TabsList className="grid grid-flow-col auto-cols-fr">
            {categories.map((category) => (
              <TabsTrigger key={category} value={category}>
                {category}
              </TabsTrigger>
            ))}
          </TabsList>

          {categories.map((category) => (
            <TabsContent key={category} value={category} className="mt-4">
              <div className="space-y-4">
                {/* Category header with select all */}
                <div className="flex items-center justify-between bg-gray-50 p-2 rounded-md">
                  <h3 className="font-medium">{category}</h3>
                  <div className="flex items-center space-x-2">
                    <Checkbox
                      id={`category-${category}`}
                      checked={isCategoryFullySelected(category)}
                      indeterminate={isCategoryPartiallySelected(category)}
                      onCheckedChange={(checked) => 
                        toggleCategoryPermissions(category, checked === true)
                      }
                      disabled={isLoadingState || isSystemRole}
                      aria-label={t('selectAllInCategory', { category })}
                    />
                    <label htmlFor={`category-${category}`} className="text-sm">
                      {t('selectAll')}
                    </label>
                  </div>
                </div>
                
                {/* Permission items */}
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-2">
                  {permissionsByCategory[category].map((permission) => (
                    <div key={permission.code} className="flex items-center space-x-2 p-2 hover:bg-gray-50 rounded-md">
                      <Checkbox
                        id={`permission-${permission.code}`}
                        checked={selectedPermissions.includes(permission.code)}
                        onCheckedChange={(checked) => 
                          handlePermissionChange(permission.code, checked === true)
                        }
                        disabled={isLoadingState || isSystemRole}
                        aria-label={permission.name}
                      />
                      <label
                        htmlFor={`permission-${permission.code}`}
                        className="text-sm leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70"
                      >
                        {permission.name}
                        {permission.description && (
                          <span className="block text-xs text-gray-500">
                            {permission.description}
                          </span>
                        )}
                      </label>
                    </div>
                  ))}
                </div>
              </div>
            </TabsContent>
          ))}
        </Tabs>
      </div>

      {/* Actions */}
      <div className="flex justify-end gap-2 mt-6">
        <Button
          type="button"
          variant="outline"
          onClick={handleCancel}
          disabled={isLoadingState}
          aria-label={t('cancel')}
        >
          {t('cancel')}
        </Button>
        
        <Button
          type="button"
          onClick={handleSave}
          disabled={isLoadingState || isSystemRole || !hasChanges()}
          aria-label={t('savePermissions')}
        >
          {isLoadingState ? t('saving') : t('savePermissions')}
        </Button>
      </div>
    </div>
  );
};
