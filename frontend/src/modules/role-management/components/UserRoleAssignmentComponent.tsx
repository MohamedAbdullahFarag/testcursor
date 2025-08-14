/**
 * User Role Assignment Component
 * Allows admins to assign or remove roles from users
 * Features multi-select, search, and batch operations
 */

import React, { useState, useEffect, useCallback, useMemo } from 'react';
import { useTranslation } from 'react-i18next';
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
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
  Alert,
  AlertDescription,
} from 'mada-design-system';
import { Search } from 'lucide-react';
import { roleService } from '../services/roleService';
import type { Role, UserRoleSummary, UpdateUserRolesRequest } from '../models/role.types';

interface UserRoleAssignmentProps {
  /** User ID for single-user mode, omit for multi-user mode */
  userId?: number;
  
  /** Initial selected roles, used in edit mode */
  initialRoles?: number[];
  
  /** Whether the component is in read-only mode */
  readOnly?: boolean;
  
  /** Callback when roles are updated successfully */
  onRolesUpdated?: (roles: number[]) => void;
  
  /** Callback when the operation is cancelled */
  onCancel?: () => void;
}

/**
 * UserRoleAssignment component for managing user role assignments
 */
export const UserRoleAssignmentComponent: React.FC<UserRoleAssignmentProps> = ({
  userId,
  initialRoles = [],
  readOnly = false,
  onRolesUpdated,
  onCancel,
}) => {
  const { t } = useTranslation(['roleManagement', 'common']);
  
  // State for available roles
  const [availableRoles, setAvailableRoles] = useState<Role[]>([]);
  const [selectedRoles, setSelectedRoles] = useState<number[]>(initialRoles);
  const [loading, setLoading] = useState(false);
  const [loadingRoles, setLoadingRoles] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [userSummary, setUserSummary] = useState<UserRoleSummary | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  
  // Load all available roles
  const loadAvailableRoles = useCallback(async () => {
    try {
      setLoadingRoles(true);
      setErrorMessage(null);
      const response = await roleService.getRoles({
        page: 1,
        pageSize: 100, // Load a large number of roles
        filters: {
          isActive: true,
          searchTerm: searchTerm || undefined,
        },
      });
      setAvailableRoles(response.items);
    } catch (error) {
      setErrorMessage(t('roleManagement:error.loadRoles'));
      console.error('Error loading roles:', error);
    } finally {
      setLoadingRoles(false);
    }
  }, [searchTerm, t]);
  
  // Load user's current roles if userId is provided
  const loadUserRoles = useCallback(async () => {
    if (!userId) return;
    
    try {
      setLoading(true);
      setErrorMessage(null);
      const userRoles = await roleService.getUserRoles(userId);
      setUserSummary(userRoles);
      setSelectedRoles(userRoles.roles.map(role => role.id));
    } catch (error) {
      setErrorMessage(t('roleManagement:error.loadUserRoles'));
      console.error('Error loading user roles:', error);
    } finally {
      setLoading(false);
    }
  }, [userId, t]);
  
  // Initial data loading
  useEffect(() => {
    loadAvailableRoles();
    if (userId) {
      loadUserRoles();
    }
  }, [loadAvailableRoles, loadUserRoles, userId]);
  
  // Handle search input change
  const handleSearchChange = useCallback((e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchTerm(e.target.value);
  }, []);
  
  // Toggle role selection
  const toggleRoleSelection = useCallback((roleId: number) => {
    setSelectedRoles(current => {
      if (current.includes(roleId)) {
        return current.filter(id => id !== roleId);
      } else {
        return [...current, roleId];
      }
    });
  }, []);
  
  // Check if all roles are selected
  const allSelected = useMemo(() => {
    return availableRoles.length > 0 && 
           availableRoles.every(role => selectedRoles.includes(role.id));
  }, [availableRoles, selectedRoles]);
  
  // Toggle all roles
  const toggleAllRoles = useCallback(() => {
    if (allSelected) {
      setSelectedRoles([]);
    } else {
      setSelectedRoles(availableRoles.map(role => role.id));
    }
  }, [allSelected, availableRoles]);
  
  // Save role assignments
  const saveRoleAssignments = useCallback(async () => {
    if (!userId) return;
    
    try {
      setLoading(true);
      setErrorMessage(null);
      
      const request: UpdateUserRolesRequest = {
        userId,
        roleIds: selectedRoles,
      };
      
      await roleService.updateUserRoles(request);
      
      // Notify parent component
      onRolesUpdated?.(selectedRoles);
    } catch (error) {
      setErrorMessage(t('roleManagement:error.updateRoles'));
      console.error('Error updating user roles:', error);
    } finally {
      setLoading(false);
    }
  }, [userId, selectedRoles, onRolesUpdated, t]);
  
  // Filter roles by search term
  const filteredRoles = useMemo(() => {
    if (!searchTerm) return availableRoles;
    
    const lowerSearchTerm = searchTerm.toLowerCase();
    return availableRoles.filter(role => 
      role.name.toLowerCase().includes(lowerSearchTerm) || 
      role.code.toLowerCase().includes(lowerSearchTerm) ||
      (role.description?.toLowerCase().includes(lowerSearchTerm))
    );
  }, [availableRoles, searchTerm]);
  
  return (
    <Card className="w-full">
      <CardHeader>
        <CardTitle>
          {userId 
            ? t('roleManagement:userRoleAssignment.assignRolesToUser', { username: userSummary?.username || '' })
            : t('roleManagement:userRoleAssignment.title')}
        </CardTitle>
      </CardHeader>
      
      <CardContent>
        {errorMessage && (
          <Alert className="mb-4">
            <AlertDescription>{errorMessage}</AlertDescription>
          </Alert>
        )}
        
        <div className="flex justify-between items-center mb-4">
          <div className="flex items-center">
            <div className="relative">
              <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
              <Input
                placeholder={t('roleManagement:search')}
                value={searchTerm}
                onChange={handleSearchChange}
                className="pl-8 w-[300px]"
              />
            </div>
          </div>
        </div>
        
        <div className="rounded-md border">
          <Table>
            <TableHeader>
              <TableRow>
                {!readOnly && (
                  <TableHead className="w-12">
                    <Checkbox 
                      checked={allSelected} 
                      onCheckedChange={toggleAllRoles}
                      disabled={loadingRoles || loading}
                    />
                  </TableHead>
                )}
                <TableHead>{t('roleManagement:role.name')}</TableHead>
                <TableHead>{t('roleManagement:role.code')}</TableHead>
                <TableHead>{t('roleManagement:role.description')}</TableHead>
                <TableHead>{t('roleManagement:role.isSystemRole')}</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {loadingRoles ? (
                <TableRow>
                  <TableCell 
                    colSpan={readOnly ? 4 : 5} 
                    className="text-center py-6"
                  >
                    {t('roleManagement:loading')}
                  </TableCell>
                </TableRow>
              ) : filteredRoles.length === 0 ? (
                <TableRow>
                  <TableCell 
                    colSpan={readOnly ? 4 : 5} 
                    className="text-center py-6"
                  >
                    {t('roleManagement:noRolesFound')}
                  </TableCell>
                </TableRow>
              ) : (
                filteredRoles.map((role) => (
                  <TableRow 
                    key={role.id} 
                    className={!readOnly ? "cursor-pointer" : ""}
                    onClick={() => !readOnly && toggleRoleSelection(role.id)}
                  >
                    {!readOnly && (
                      <TableCell className="w-12">
                        <Checkbox
                          checked={selectedRoles.includes(role.id)}
                          disabled={loading}
                          onCheckedChange={() => {}}
                        />
                      </TableCell>
                    )}
                    <TableCell>{role.name}</TableCell>
                    <TableCell>
                      <Badge variant="outline">{role.code}</Badge>
                    </TableCell>
                    <TableCell>{role.description || '-'}</TableCell>
                    <TableCell>
                      {role.isSystemRole ? (
                        <Badge>{t('roleManagement:yes')}</Badge>
                      ) : (
                        t('roleManagement:no')
                      )}
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </div>
        
        {!readOnly && (
          <div className="flex justify-end gap-2 mt-4">
            {onCancel && (
              <Button 
                variant="outline" 
                onClick={onCancel} 
                disabled={loading}
              >
                {t('common:cancel')}
              </Button>
            )}
            
            <Button
              onClick={saveRoleAssignments}
              disabled={loadingRoles || loading}
            >
              {loading ? t('common:saving') : t('common:save')}
            </Button>
          </div>
        )}
      </CardContent>
    </Card>
  );
};

export default UserRoleAssignmentComponent;
